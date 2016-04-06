using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Util {
  public struct StringPart {
    public Span Span { get; set; }
    public StringPartType Type { get; set; }

    public StringPart(int start, int length)
      : this(new Span(start, length), StringPartType.EscapeSequence) {
    }
    public StringPart(int start, int length, StringPartType type)
      : this(new Span(start, length), type) {
    }
    public StringPart(Span span)
      : this(span, StringPartType.EscapeSequence) {
    }
    public StringPart(Span span, StringPartType type) : this() {
      this.Span = span;
      this.Type = type;
    }

    public override bool Equals(object obj) {
      if ( obj == null ) return false;
      StringPart part = (StringPart)obj;
      return part.Span == this.Span
          && part.Type == part.Type;
    }
    public override string ToString() {
      return String.Format(
        "{0} {1}",
        GetTypeChar(), Span
        );
    }
    public override int GetHashCode() {
      return Span.GetHashCode() ^ Type.GetHashCode();
    }
    public static bool operator ==(StringPart left, StringPart right) {
      return left.Equals(right);
    }
    public static bool operator !=(StringPart left, StringPart right) {
      return !left.Equals(right);
    }


    private char GetTypeChar() {
      switch ( Type ) {
        case StringPartType.EscapeSequence:
          return 'E';
        case StringPartType.FormatSpecifier:
          return 'F';
        default:
          return 'U';
      }
    }
  }

  public enum StringPartType {
    EscapeSequence = 0,
    FormatSpecifier = 1
  }
}
