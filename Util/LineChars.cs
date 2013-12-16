using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;

namespace Winterdom.Viasfora.Util {
  public class LineChars : ITextChars {
    private ITextSnapshotLine line;
    private int position;
    private String text;
    const char EOT = '\0';

    public LineChars(ITextSnapshotLine line, int start=0) {
      this.line = line;
      this.text = line.GetText();
      this.position = start;
    }

    public int Position {
      get { return this.position;  }
    }

    public int AbsolutePosition {
      get { return this.line.Start + this.position;  }
    }

    public bool EndOfLine {
      get { return this.position >= line.Length; }
    }

    public char Char() {
      return Available(1) ? text[position] : EOT;
    }

    public char NChar() {
      return Available(2) ? text[position+1] : EOT;
    }

    public char NNChar() {
      return Available(3) ? text[position+2] : EOT;
    }

    public void Next() {
      Skip(1);
    }
    public void Skip(int count) {
      this.position += count;
    }

    public void SkipRemainder() {
      this.position = line.Length;
    }

    public String PreviousToken() {
      int startPos = this.Position-1;
      // skip any whitespace
      for ( ; startPos > 0; startPos-- ) {
        if ( !System.Char.IsWhiteSpace(text[startPos]) )
          break;
      }
      int end = startPos;
      for ( ; startPos > 0; startPos-- ) {
        if ( System.Char.IsWhiteSpace(text[startPos]) )
          break;
      }
      return text.Substring(startPos, end - startPos + 1);
    }

    private bool Available(int count) {
      return this.position + count - 1 < line.Length;
    }
  }
}
