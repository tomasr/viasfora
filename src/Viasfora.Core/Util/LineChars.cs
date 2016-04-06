using System;
using Microsoft.VisualStudio.Text;

namespace Winterdom.Viasfora.Util {
  public class LineChars : StringChars {
    private ITextSnapshotLine line;
    const char EOT = '\0';

    public LineChars(ITextSnapshotLine line, int start=0)
        : base(line.GetText(), start) {
      this.line = line;
    }

    public override int AbsolutePosition {
      get { return this.line.Start + this.Position;  }
    }
  }
}
