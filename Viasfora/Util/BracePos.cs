using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Winterdom.Viasfora.Tags;

namespace Winterdom.Viasfora.Util {
  public struct BracePos {
    private char brace;
    private int depth;
    private int position;

    public char Brace {
      get { return brace; }
    }
    public int Depth {
      get { return depth; }
    }
    public int Position {
      get { return position; }
    }

    public BracePos(char ch, int pos, int depth) {
      this.brace = ch;
      this.position = pos;
      this.depth = depth;
    }

    public ITagSpan<RainbowTag> ToSpan(ITextSnapshot snapshot, IClassificationType type) {
      var span = new SnapshotSpan(snapshot, Position, 1);
      return new TagSpan<RainbowTag>(span, new RainbowTag(type));
    }

    public override string ToString() {
      return String.Format("'{0}' ({1}) {3}", Brace, Position, Depth);
    }
  }
}
