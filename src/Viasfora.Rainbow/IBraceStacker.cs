using System;

namespace Winterdom.Viasfora.Rainbow {
  public interface IBraceStacker {
    int Count(char brace);
    BracePos Push(CharPos brace);
    BracePos Pop(char brace);
    BracePos Peek(char brace);
  }
}
