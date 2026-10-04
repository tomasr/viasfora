using System;

namespace Winterdom.Viasfora.Rainbow {
  public struct CharPos {
    private readonly char ch;
    private readonly int state;
    private readonly int position;
    private readonly int length;
    public static CharPos Empty = new CharPos('\0', 0);

    public char Char => this.ch;
    public int State => this.state;
    public int Position => this.position;
    // number of characters in the brace, e.g. {{ in C# raw strings
    public int Length => this.length;

    public CharPos(char ch, int pos) : this(ch, pos, 0) {
    }

    public CharPos(char ch, int pos, int state, int length = 1) {
      this.ch = ch;
      this.position = pos;
      this.state = state;
      this.length = length;
    }

    public static implicit operator char(CharPos cp) {
      return cp.Char;
    }

    public override string ToString() {
      return String.Format("'{0}' ({1})", Char, Position);
    }
  }
}
