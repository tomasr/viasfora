using Microsoft.VisualStudio.Text;
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
    private ExpressionOptions options;
    private Regex compiledExpression;

    public String Name {
      get { return this.name; }
      set { this.name = value; }
    }
    public String RegularExpression {
      get { return this.regex; }
      set {
        this.regex = value;
      }
    }
    public ExpressionKind Kind {
      get { return this.kind; }
      set { this.kind = value; }
    }
    public ExpressionOptions Options {
      get { return this.options; }
      set { this.options = value; }
    }

    public Regex GetRegex() {
      if ( String.IsNullOrEmpty(this.RegularExpression) ) {
        return null;
      }
      if ( compiledExpression == null ) {
        compiledExpression = new Regex(this.RegularExpression, RegexOptions.Compiled);
      }
      return compiledExpression;
    }

    public IEnumerable<SnapshotSpan> Match(ITextSnapshotLine line) {
      Regex regex = GetRegex();
      if ( line.Length == 0 || regex == null ) {
        yield break;
      }
      ITextSnapshot snapshot = line.Snapshot;
      var matches = GetRegex().Matches(line.GetText());
      foreach ( Match m in matches ) {
        switch ( Options ) {
          case ExpressionOptions.HideMatch:
            yield return new SnapshotSpan(snapshot, 
              line.Start + m.Index, m.Length);
            break;
          case Util.ExpressionOptions.HideGroups:
            for ( int g = 1; g < m.Groups.Count; g++ ) {
              yield return new SnapshotSpan(snapshot, 
                line.Start + m.Groups[g].Index, m.Groups[g].Length);
            }
            break;
        }
      }
    }

    public bool IsValid() {
      if ( String.IsNullOrEmpty(this.RegularExpression) ) {
        return false;
      }
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
  public enum ExpressionOptions {
    // Hide the entire match
    HideMatch = 0,
    // Hide individual capture groups
    HideGroups = 1
  }
}
