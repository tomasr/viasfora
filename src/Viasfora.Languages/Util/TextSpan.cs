using System;

namespace Winterdom.Viasfora.Util {
  public struct TextSpan {
    private int start;
    private int length;

    public int Start => start;
    public int Length => length;
    public int End => Start + Length;

    public TextSpan(int start, int length) {
      this.start = start;
      this.length = length;
    }

    public override bool Equals(object obj) {
      var other = (TextSpan)obj;
      return Start == other.Start && Length == other.Length;
    }

    public override int GetHashCode() {
      return Start.GetHashCode() ^ Length.GetHashCode();
    }

    public override string ToString() {
      return $"({Start}, {End})";
    }

    public static bool operator ==(TextSpan left, TextSpan right) {
      return left.Start == right.Start && left.Length == right.Length;
    }

    public static bool operator !=(TextSpan left, TextSpan right) {
      return !(left == right);
    }
  }
}
