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

    public void Push(CharPos brace) {
      pairs.Push(brace.AsBrace(pairs.Count));
    }
  }
}
