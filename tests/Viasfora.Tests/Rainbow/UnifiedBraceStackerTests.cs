using System;
using Winterdom.Viasfora.Rainbow;
using Xunit;

namespace Viasfora.Tests.Rainbow {
  public class UnifiedBraceStackerTests {

    [Fact]
    public void WhenEmptyCountIsZero() {
      var stack = new UnifiedBraceStacker();
      Assert.Equal(0, stack.Count('{'));
    }
    [Fact]
    public void WhenPopGetsSameBracePushed() {
      var stack = new UnifiedBraceStacker();
      stack.Push(new CharPos('{', 0));
      var item = stack.Pop('{');
      Assert.Equal('{', item.Brace);
    }
    [Fact]
    public void WhenPushingMultipleBracesCountIsIncreased() {
      var stack = new UnifiedBraceStacker();
      stack.Push(new CharPos('{', 0));
      stack.Push(new CharPos('(', 0));
      Assert.Equal(2, stack.Count('{'));
    }
    [Fact]
    public void WhenPushingMultipleBracesDepthIsIncreased() {
      var stack = new UnifiedBraceStacker();
      stack.Push(new CharPos('{', 0));
      stack.Push(new CharPos('(', 0));
      Assert.Equal(1, stack.Pop('{').Depth);
      Assert.Equal(0, stack.Pop('{').Depth);
    }
  }
}
