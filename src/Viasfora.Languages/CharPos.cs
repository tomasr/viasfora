using System;

namespace Winterdom.Viasfora.Rainbow {
  public struct CharPos {
    private readonly char ch;
    private readonly int state;
    private readonly int position;
    public static CharPos Empty = new CharPos('\0', 0);

    public char Char => this.ch;
    public int State => this.state;
    public int Position => this.position;

    public CharPos(char ch, int pos) : this(ch, pos, 0) {
    }

    public CharPos(char ch, int pos, int state) {
      this.ch = ch;
      this.position = pos;
      this.state = state;
    }

    public static implicit operator char(CharPos cp) {
      return cp.Char;
    }

    public override string ToString() {
      return String.Format("'{0}' ({1})", Char, Position);
    }
  }
}
