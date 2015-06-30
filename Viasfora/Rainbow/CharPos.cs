using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Rainbow {
  public struct CharPos : IPosition {
    private char ch;
    private int position;

    public char Char {
      get { return ch; }
    }
    public int Position {
      get { return position; }
    }

    public CharPos(char ch, int pos) {
      this.ch = ch;
      this.position = pos;
    }

    public BracePos AsBrace(int depth) {
      return new BracePos(this, depth);
    }

    public static implicit operator char(CharPos cp) {
      return cp.Char;
    }

    public override string ToString() {
      return String.Format("'{0}' ({1})", Char, Position);
    }
  }
}
