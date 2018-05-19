using System;
using System.Collections.Generic;

namespace Winterdom.Viasfora.Rainbow {
  public class UnifiedBraceStacker : IBraceStacker {
    private Stack<BracePos> pairs = new Stack<BracePos>();

    public int Count(char brace) => this.pairs.Count;
    public BracePos Pop(char brace) => this.pairs.Pop();
    public BracePos Peek(char brace) => this.pairs.Peek();

    public BracePos Push(CharPos brace) {
      var bp = new BracePos(brace, this.pairs.Count);
      this.pairs.Push(bp);
      return bp;
    }
  }
}
