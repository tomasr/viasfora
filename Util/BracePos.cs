using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text;

namespace Winterdom.Viasfora.Util {
  public struct BracePos {
    private char brace;
    private int depth;
    private int position;
    private int lineNum;

    public char Brace {
      get { return brace; }
    }
    public int Depth {
      get { return depth; }
    }
    public int Position {
      get { return position; }
    }
    public int LineNumber {
      get { return lineNum; }
    }

    public BracePos(char ch, int pos, int lineNum, int depth) {
      this.brace = ch;
      this.position = pos;
      this.depth = depth;
      this.lineNum = lineNum;
    }

    public ClassificationSpan ToSpan(ITextSnapshot snapshot, IClassificationType type) {
      var span = new SnapshotSpan(snapshot, Position, 1);
      return new ClassificationSpan(span, type);
    }
  }
}
