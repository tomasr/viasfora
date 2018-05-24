using System;
using System.Collections.Generic;

namespace Winterdom.Viasfora.Rainbow {
  public class PerBraceStacker : IBraceStacker {
    private String braceList;
    private Dictionary<char, Stack<BracePos>> stack;

    public PerBraceStacker(String braceList) {
      this.braceList = braceList;
      this.stack = new Dictionary<char, Stack<BracePos>>();
      for ( int i=0; i < braceList.Length; i += 2 ) {
        var pairs = new Stack<BracePos>();
        this.stack[braceList[i]] = pairs;
        this.stack[braceList[i + 1]] = pairs;
      }
    }

    public int Count(char brace) => this.stack[brace].Count;
    public BracePos Pop(char brace) => this.stack[brace].Pop();
    public BracePos Peek(char brace) => this.stack[brace].Peek();

    public BracePos Push(CharPos brace) {
      var pairs = this.stack[brace.Char];
      var bp = new BracePos(brace, pairs.Count);
      pairs.Push(bp);
      return bp;
    }
  }
}
