using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Viasfora.LanguageService.Core {
  static class SyntaxTreeExtensions {

    public static TextSpan ToTextSpan(this SnapshotSpan span) {
      return new TextSpan(span.Start.Position, span.Length);
    }

    public static SnapshotSpan ToSnapshotSpan(this TextSpan span, ITextSnapshot snapshot) {
      return new SnapshotSpan(snapshot, span.Start, span.Length);
    }
  }
}
