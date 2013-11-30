using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;

namespace Winterdom.Viasfora.Text {
  class BraceExtractor {
    private ITextSnapshot snapshot;
    private String braceChars;
    private ITextSnapshotLine currentLine;
    private int pos;
    private int status;
    private char quoteChar;
    private LanguageInfo language;
    private const int ST_NORMAL = 0x00;
    private const int ST_COMMENT = 0x01;
    private const int ST_STRING = 0x02;
    private const int ST_MULTILINE = 0x80;

    public BraceExtractor(SnapshotPoint startPoint, LanguageInfo language) {
      this.snapshot = startPoint.Snapshot;
      this.currentLine = this.snapshot.GetLineFromPosition(startPoint.Position);
      this.language = language;
      this.braceChars = language.BraceList;
      this.pos = 0;
      this.status = ST_NORMAL;
    }

    public IEnumerable<SnapshotPoint> All() {
      while ( true ) {
        SnapshotPoint? pt = NextBrace();
        if ( pt.HasValue ) {
          yield return pt.Value;
        } else {
          break;
        }
      }
    }
    public SnapshotPoint? NextBrace() {
      while ( currentLine != null ) {
        SnapshotPoint? pt = this.NextBraceInLine();
        if ( pt != null ) {
          return pt;
        }
        if ( currentLine.LineNumber < snapshot.LineCount - 1 ) {
          currentLine = this.snapshot.GetLineFromLineNumber(currentLine.LineNumber+1);
          pos = 0;
        } else {
          currentLine = null;
        }
      }
      return null;
    }

    private SnapshotPoint? NextBraceInLine() {
      String text = currentLine.GetText();
      for ( ; pos < currentLine.Length; pos++ ) {
        switch ( this.status ) {
          case ST_NORMAL:
            if ( IsBrace(text[pos]) ) {
              var ret = new SnapshotPoint(this.snapshot, currentLine.Start + pos);
              pos++;
              return ret;
            }
            if ( language.IsSingleLineCommentStart(text, pos) ) {
              // single line comment, ignore the rest
              pos = currentLine.Length;
            } else if ( language.IsMultiLineCommentStart(text, pos) ) {
              // multiline comment
              this.status = ST_COMMENT | ST_MULTILINE;
            } else if ( language.IsSingleLineStringStart(text, pos, out quoteChar) ) {
              this.status = ST_STRING;
            } else if ( language.IsMultiLineStringStart(text, pos, out quoteChar) ) {
              this.status = ST_STRING | ST_MULTILINE;
            } 
            break;
          case ST_COMMENT | ST_MULTILINE:
            if ( language.IsMultiLineCommentEnd(text, pos) ) {
              this.status = ST_NORMAL;
            }
            break;
          case ST_STRING:
          case ST_STRING | ST_MULTILINE:
            if ( language.IsStringEnd(text, pos, quoteChar) ) {
              this.status = ST_NORMAL;
            }
            break;
        }
      }
      return null;
    }

    private bool IsBrace(char ch) {
      return this.braceChars.IndexOf(ch) >= 0;
    }

  }
}
