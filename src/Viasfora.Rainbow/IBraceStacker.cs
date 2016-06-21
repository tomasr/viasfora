using System;

namespace Winterdom.Viasfora.Rainbow {
  public interface IBraceStacker {
    int Count(char brace);
    void Push(CharPos brace);
    BracePos Pop(char brace);
  }
}
