using System;
using System.Collections.Generic;

namespace Winterdom.Viasfora.Rainbow {
  public class UnifiedBraceStacker : IBraceStacker {
    private Stack<BracePos> pairs = new Stack<BracePos>();

    public int Count(char brace) {
      return pairs.Count;
    }

    public BracePos Pop(char brace) {
      return pairs.Pop();
    }

    public BracePos Peek(char brace) {
      return pairs.Peek();
    }

    public BracePos Push(CharPos brace) {
      var bp = new BracePos(brace, pairs.Count);
      pairs.Push(bp);
      return bp;
    }
  }
}
