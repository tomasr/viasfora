using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Windows.Media;
using Sgml;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Xml {

  public class XmlTagMatchingTagger : ITagger<TextMarkerTag>, IDisposable {
    private ITextView theView;
    private ITextBuffer theBuffer;
    private SnapshotSpan? currentSpan;
    private ITagAggregator<IClassificationTag> aggregator;
    private IMarkupLanguage language;

    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

    public XmlTagMatchingTagger(ITextView textView, ITextBuffer buffer, ITagAggregator<IClassificationTag> aggregator) {
      this.theView = textView;
      this.theBuffer = buffer;
      this.aggregator = aggregator;
      this.currentSpan = null;

      this.language = new XmlMarkup();

      this.theView.Caret.PositionChanged += CaretPositionChanged;
      this.theView.LayoutChanged += ViewLayoutChanged;
      VsfSettings.SettingsUpdated += OnSettingsUpdated;
    }

    public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
      if ( !VsfSettings.XmlMatchTagsEnabled ) yield break;
      if ( spans.Count == 0 ) yield break;
      if ( !currentSpan.HasValue ) yield break;

      SnapshotSpan current = currentSpan.Value;

      if ( current.Snapshot != spans[0].Snapshot ) {
        current = current.TranslateTo(spans[0].Snapshot, SpanTrackingMode.EdgePositive);
      }

      SnapshotSpan currentTag = CompleteTag(current);
      String text = currentTag.GetText();
      // avoid processing statements or xml declarations
      if ( text.Contains('?') ) yield break;

      SnapshotSpan? complementTag = null;
      if ( text.StartsWith("</") ) {
        complementTag = FindOpeningTag(current.Snapshot, currentTag.End, current.GetText());
        if ( complementTag != null ) {
          complementTag = ExtendOpeningTag(complementTag.Value);
        }
      } else {
        String searchFor = "</" + current.GetText() + ">";
        currentTag = ExtendOpeningTag(currentTag);
        complementTag = FindClosingTag(current.Snapshot, currentTag.Start, searchFor);
      }

      yield return new TagSpan<TextMarkerTag>(currentTag, new TextMarkerTag("bracehighlight"));
      if ( complementTag.HasValue ) {
        yield return new TagSpan<TextMarkerTag>(complementTag.Value, new TextMarkerTag("bracehighlight"));
      }
    }

    // Extend the opening tag all the way to the >, even if
    // it has multiple attributes and what not.
    private SnapshotSpan ExtendOpeningTag(SnapshotSpan currentTag) {
      var snapshot = currentTag.Snapshot;
      int end = -1;
      String currentQuote = null;
      for ( int i = currentTag.Start; i < snapshot.Length; i++ ) {
        String ch = snapshot.GetText(i, 1);
        if ( currentQuote == null ) {
          if ( ch == "\"" || ch == "'" ) {
            currentQuote = ch;
          } else if ( ch == ">" ) {
            end = i;
            break;
          }
        } else if ( ch == currentQuote ) {
          currentQuote = null;
        }
      }
      if ( end > currentTag.Start ) {
        return new SnapshotSpan(snapshot, currentTag.Start, end - currentTag.Start + 1);
      }
      return currentTag;
    }

    // Parse the document from the current position until we find the
    // matching closing tag
    private SnapshotSpan? FindClosingTag(ITextSnapshot snapshot, int searchStart, string searchFor) {
      String textToSearch = snapshot.GetText(searchStart, snapshot.Length - searchStart);

      using ( SgmlReader reader = new SgmlReader() ) {
        reader.InputStream = new StringReader(textToSearch);
        reader.WhitespaceHandling = WhitespaceHandling.All;
        try {
          reader.Read();
          if ( !reader.IsEmptyElement ) {
            // skip all the internal nodes, until the end
            while ( reader.Read() ) {
              if ( reader.NodeType == XmlNodeType.EndElement && reader.Depth == 1 )
                break;
            }
            // calculate the new position based on the number of lines
            // read in the SgmlReader + the position within that line.
            // Note that if there is whitespace after the closing tag
            // we'll be positioned on it, so we need to keep track of that.
            var origLine = snapshot.GetLineFromPosition(searchStart);
            int startOffset = searchStart - origLine.Start.Position;
            int newStart = 0;
            // tag is on same position as the opening one
            if ( reader.LineNumber == 1 ) {
              var line = snapshot.GetLineFromPosition(searchStart);
              newStart = line.Start.Position + startOffset + reader.LinePosition - 2;
            } else {
              int newLineNum = origLine.LineNumber + reader.LineNumber - 1;
              var newLine = snapshot.GetLineFromLineNumber(newLineNum);
              newStart = newLine.Start.Position + reader.LinePosition - 1;
            }
            newStart -= reader.Name.Length + 3; // </ + element + >

            SnapshotSpan? newSpan = new SnapshotSpan(snapshot, newStart, searchFor.Length);
            if ( newSpan.Value.GetText() != searchFor ) {
              Trace.WriteLine(String.Format("Searching for '{0}', but found '{1}'.", searchFor, newSpan.Value.GetText()));
              newSpan = null;
            }
            return newSpan;
          }
        } catch ( Exception ex ) {
          Trace.WriteLine(String.Format("Exception while parsing document: {0}.", ex.ToString()));
        }
      }
      return null;
    }

    // parse the document from the start, and try to
    // figure out where the opening tag matching our closing tag starts
    private SnapshotSpan? FindOpeningTag(ITextSnapshot snapshot, int searchEnd, string searchFor) {
      String textToSearch = snapshot.GetText(0, searchEnd);
      int origLineNum = snapshot.GetLineNumberFromPosition(searchEnd);

      using ( SgmlReader reader = new SgmlReader() ) {
        reader.InputStream = new StringReader(textToSearch);
        reader.WhitespaceHandling = WhitespaceHandling.All;
        try {
          Stack<int> openingPositions = new Stack<int>();
          while ( reader.Read() ) {
            if ( reader.LocalName != searchFor ) {
              continue;
            }
            if ( reader.NodeType == XmlNodeType.Element && !reader.IsEmptyElement ) {
              // find close to where the tag starts
              int lineNum = reader.LineNumber - 1;
              var line = snapshot.GetLineFromLineNumber(lineNum);
              int position = line.Start.Position + reader.LinePosition - searchFor.Length;
              position = BacktrackToLessThan(snapshot, position);
              String textFound = snapshot.GetText(position, 10);
              openingPositions.Push(position);
            } else if ( reader.NodeType == XmlNodeType.EndElement ) {
              if ( openingPositions.Count <= 0 ) {
                // document is malformed, so just get the heck out
                return null;
              }
              var line = snapshot.GetLineFromLineNumber(reader.LineNumber - 1);
              int position = line.Start.Position + reader.LinePosition;
              if ( position >= searchEnd ) break;
              openingPositions.Pop();
            }
          }
          // done, last
          if ( openingPositions.Count > 0 ) {
            int position = openingPositions.Pop();
            return new SnapshotSpan(snapshot, position, searchFor.Length + 2);
          }
        } catch ( Exception ex ) {
          Trace.WriteLine(String.Format("Exception while parsing document: {0}.", ex.ToString()));
        }
      }
      return null;
    }

    private int BacktrackToLessThan(ITextSnapshot snapshot, int start) {
      int rs = start - 1;
      while ( snapshot.GetText(rs, 1) != "<" ) {
        rs--;
      }
      return rs;
    }

    private SnapshotSpan CompleteTag(SnapshotSpan current) {
      var snapshot = current.Snapshot;
      int end = current.End + 1;
      int start = BacktrackToLessThan(snapshot, current.Start);

      return new SnapshotSpan(snapshot, start, end - start);
    }

    public void Dispose() {
      if ( theBuffer != null ) {
        this.theView.Caret.PositionChanged -= CaretPositionChanged;
        this.theView.LayoutChanged -= ViewLayoutChanged;
        VsfSettings.SettingsUpdated -= OnSettingsUpdated;
        theBuffer = null;
      }
    }

    void OnSettingsUpdated(object sender, EventArgs e) {
      UpdateAtCaretPosition(theView.Caret.Position);
    }

    private void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) {
      if ( e.NewSnapshot != e.OldSnapshot ) {
        UpdateAtCaretPosition(theView.Caret.Position);
      }
    }

    private void CaretPositionChanged(object sender, CaretPositionChangedEventArgs e) {
      UpdateAtCaretPosition(e.NewPosition);
    }

    private void UpdateAtCaretPosition(CaretPosition caretPosition) {
      var point = caretPosition.Point.GetPoint(theBuffer, caretPosition.Affinity);
      if ( !point.HasValue )
        return;

      // get the tag beneath our position:
      this.currentSpan = GetTagAtPoint(point.Value);

      var tempEvent = TagsChanged;
      if ( tempEvent != null ) {
        tempEvent(this, new SnapshotSpanEventArgs(new SnapshotSpan(theBuffer.CurrentSnapshot, 0,
            theBuffer.CurrentSnapshot.Length)));
      }
    }

    private SnapshotSpan? GetTagAtPoint(SnapshotPoint point) {
      int pos = point.Position >= 1 ? point.Position - 1 : 0;
      SnapshotSpan testSpan = new SnapshotSpan(point.Snapshot, new Span(pos, 1));

      foreach ( var tagSpan in aggregator.GetTags(testSpan) ) {
        String tagName = tagSpan.Tag.ClassificationType.Classification;
        if ( !language.IsName(tagName) ) continue;
        foreach ( var span in tagSpan.Span.GetSpans(point.Snapshot.TextBuffer) ) {
          if ( span.Contains(point.Position) ) {
            return span;
          }
        }
      }
      return null;
    }
  }
}
