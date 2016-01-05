using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Rainbow {
  public struct CharPos {
    private char ch;
    private int state;
    private int position;
    public static CharPos Empty = new CharPos('\0', 0);

    public char Char {
      get { return ch; }
    }
    public int State {
      get { return state; }
    }
    public int Position {
      get { return position; }
    }

    public CharPos(char ch, int pos) : this(ch, pos, 0) {
    }

    public CharPos(char ch, int pos, int state) {
      this.ch = ch;
      this.position = pos;
      this.state = state;
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
