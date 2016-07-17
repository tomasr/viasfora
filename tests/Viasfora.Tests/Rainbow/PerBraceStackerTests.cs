using System;
using Winterdom.Viasfora.Rainbow;
using Xunit;

namespace Viasfora.Tests.Rainbow {
  public class PerBraceStackerTests {
    String braceList = "{}()[]";

    [Fact]
    public void WhenEmptyCountIsZero() {
      var stack = new PerBraceStacker(braceList);
      Assert.Equal(0, stack.Count('{'));
    }
    [Fact]
    public void WhenAddingOneOfEachCountIs1() {
      var stack = new PerBraceStacker(braceList);
      stack.Push(new CharPos('{', 0));
      stack.Push(new CharPos('(', 0));
      Assert.Equal(1, stack.Count('{'));
      Assert.Equal(1, stack.Count('('));
    }
    [Fact]
    public void WhenPushingMultipleBracesDepthIsIncreased() {
      var stack = new PerBraceStacker(braceList);
      stack.Push(new CharPos('{', 0));
      stack.Push(new CharPos('{', 0));
      Assert.Equal(1, stack.Pop('{').Depth);
      Assert.Equal(0, stack.Pop('{').Depth);
    }
    [Fact]
    public void WhenRemovingOneTheCountOfTheOtherIsNotReduced() {
      var stack = new PerBraceStacker(braceList);
      stack.Push(new CharPos('{', 0));
      stack.Push(new CharPos('(', 0));
      stack.Pop('{');
      Assert.Equal(0, stack.Count('{'));
      Assert.Equal(1, stack.Count('('));
    }
  }
}
