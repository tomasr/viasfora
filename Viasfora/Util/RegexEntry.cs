using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Winterdom.Viasfora.Util {
  public class RegexEntry {
    private String name;
    private String regex;
    private ExpressionKind kind;

    public String Name {
      get { return this.name; }
      set { this.name = value; }
    }
    public String RegularExpression {
      get { return this.regex; }
      set {
        if ( String.IsNullOrEmpty(value) ) {
          throw new ArgumentException("value", "Regular expression must be provided");
        }
        this.regex = value;
      }
    }
    public ExpressionKind Kind {
      get { return this.kind; }
      set { this.kind = value; }
    }

    public Regex GetRegex() {
      return new Regex(this.RegularExpression, RegexOptions.Compiled);
    }
    public bool IsValid() {
      try {
        var temp = new Regex(this.RegularExpression);
        return true;
      } catch {
        return false;
      }
    }
  }

  // added for extensibility later on
  public enum ExpressionKind {
    RegularExpression = 0
  }
}
