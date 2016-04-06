using Microsoft.VisualStudio.Text;
using System;
using System.Linq;
using Winterdom.Viasfora;
using Winterdom.Viasfora.Outlining;
using Xunit;

namespace Viasfora.Tests.Outlining {
  public class SelectionOutliningManagerTests : VsfVsTestBase {

    [Fact]
    public void GetTagsReturnsRegionsAdded() {
      var textBuffer = GetCSharpTextBuffer("Outlining1.txt");
      var snapshot = textBuffer.CurrentSnapshot;
      var manager = SelectionOutliningManager.Get(textBuffer);

      var span1 = GetSpanFromLines(snapshot, 10, 15);
      var span2 = GetSpanFromLines(snapshot, 25, 30);

      manager.Add(span1);
      manager.Add(span2);

      // validate GetTags() gives us both spans
      var allDocument = new NormalizedSnapshotSpanCollection(snapshot.GetSpan());
      var tags = manager.GetTags(allDocument).ToList();
      Assert.Equal(span1, tags.First());
      Assert.Equal(span2, tags.Last());
    }

    [Fact]
    public void RaisesTagsChangedEvent() {
      var textBuffer = GetCSharpTextBuffer("Outlining1.txt");
      var snapshot = textBuffer.CurrentSnapshot;
      var outlining = SelectionOutliningManager.Get(textBuffer);
      var manager = SelectionOutliningManager.GetManager(textBuffer);

      var tagger = manager.GetOutliningTagger();
      SnapshotSpan? spanRaisedByEvent = null;

      tagger.TagsChanged += (sender, e) => {
        spanRaisedByEvent = e.Span;
      };

      var span1 = GetSpanFromLines(snapshot, 10, 15);
      outlining.Add(span1);
      Assert.True(spanRaisedByEvent.HasValue, "The TagsChanged event was not raised");
      Assert.Equal(span1, spanRaisedByEvent.Value);
    }

    // test that it creates the regions correctly when the
    // selection spans complete text lines
    [Fact]
    public void CreateRegionsAround_FullLines() {
      var textBuffer = GetCSharpTextBuffer("Outlining1.txt");
      var snapshot = textBuffer.CurrentSnapshot;
      var outlining = SelectionOutliningManager.Get(textBuffer);

      var selectionSpan = GetSpanFromLines(snapshot, 9, 15);
      outlining.CreateRegionsAround(selectionSpan);

      // validate GetTags() gives us two spans
      var allDocument = new NormalizedSnapshotSpanCollection(snapshot.GetSpan());
      var tags = outlining.GetTags(allDocument).ToList();

      var span1 = GetSpanFromLines(snapshot, 0, 8);
      Assert.Equal(span1, tags.First());

      var span2 = GetSpanFromLines(snapshot, 16, snapshot.LineCount-1);
      Assert.Equal(span2, tags.Last());
    }

    // test that it creates the regions correctly when the
    // selection spans partial text lines
    [Fact]
    public void CreateRegionsAround_PartialLines() {
      var textBuffer = GetCSharpTextBuffer("Outlining1.txt");
      var snapshot = textBuffer.CurrentSnapshot;
      var outlining = SelectionOutliningManager.Get(textBuffer);

      var selectionSpan = GetSpanFromLines(snapshot, 9, 15);
      selectionSpan = new SnapshotSpan(selectionSpan.Start + 10, selectionSpan.End - 2);
      outlining.CreateRegionsAround(selectionSpan);

      // validate GetTags() gives us two spans
      var allDocument = new NormalizedSnapshotSpanCollection(snapshot.GetSpan());
      var tags = outlining.GetTags(allDocument).ToList();

      var span1 = GetSpanFromLines(snapshot, 0, 8);
      Assert.Equal(span1, tags.First());

      var span2 = GetSpanFromLines(snapshot, 16, snapshot.LineCount-1);
      Assert.Equal(span2, tags.Last());
    }

    // test that it creates the regions correctly when the
    // selection starts at the beginning of the document
    [Fact]
    public void CreateRegionsAround_SelectionAtStartOfDocument() {
      var textBuffer = GetCSharpTextBuffer("Outlining1.txt");
      var snapshot = textBuffer.CurrentSnapshot;
      var outlining = SelectionOutliningManager.Get(textBuffer);

      var selectionSpan = GetSpanFromLines(snapshot, 0, 15);
      outlining.CreateRegionsAround(selectionSpan);

      // validate GetTags() gives us two spans
      var allDocument = new NormalizedSnapshotSpanCollection(snapshot.GetSpan());
      var tags = outlining.GetTags(allDocument).ToList();

      var span1 = GetSpanFromLines(snapshot, 16, snapshot.LineCount-1);
      Assert.Equal(span1, tags.First());
    }

    // test that it creates the regions correctly when the
    // selection ends at the end of the document
    [Fact]
    public void CreateRegionsAround_SelectionAtEndOfDocument() {
      var textBuffer = GetCSharpTextBuffer("Outlining1.txt");
      var snapshot = textBuffer.CurrentSnapshot;
      var outlining = SelectionOutliningManager.Get(textBuffer);

      int lineCount = snapshot.LineCount;
      var selectionSpan = GetSpanFromLines(snapshot, lineCount-10, lineCount-1);
      outlining.CreateRegionsAround(selectionSpan);

      // validate GetTags() gives us two spans
      var allDocument = new NormalizedSnapshotSpanCollection(snapshot.GetSpan());
      var tags = outlining.GetTags(allDocument).ToList();

      var span1 = GetSpanFromLines(snapshot, 0, (lineCount - 10)-1);
      Assert.Equal(span1, tags.First());
    }

    private SnapshotSpan GetSpanFromLines(ITextSnapshot snapshot, int start, int end) {
      var startLine = snapshot.GetLineFromLineNumber(start);
      var endLine = snapshot.GetLineFromLineNumber(end);
      return new SnapshotSpan(startLine.Start, endLine.End);
    }
  }
}
