using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora;
using Winterdom.Viasfora.Outlining;
using Xunit;

namespace Viasfora.Tests.Outlining {
  public class SelectionOutliningManagerTests : VsfVsTestBase {

    [Fact]
    public void GetTagsReturnsRegionsAdded() {
      var textBuffer = GetCSharpTextBuffer("Outlining1.txt");
      var snapshot = textBuffer.CurrentSnapshot;
      var manager = SelectionOutliningManager.Get(textBuffer);

      var span1 = GetSpanFromLines(snapshot, 10, 15);
      var span2 = GetSpanFromLines(snapshot, 25, 30);

      manager.Add(span1);
      manager.Add(span2);

      // validate GetTags() gives us both spans
      var allDocument = new NormalizedSnapshotSpanCollection(snapshot.GetSpan());
      var tags = manager.GetTags(allDocument).ToList();
      Assert.Equal(span1, tags.First());
      Assert.Equal(span2, tags.Last());
    }

    private SnapshotSpan GetSpanFromLines(ITextSnapshot snapshot, int start, int end) {
      var startLine = snapshot.GetLineFromLineNumber(start);
      var endLine = snapshot.GetLineFromLineNumber(end);
      return new SnapshotSpan(startLine.Start, endLine.End);
    }
  }
}
