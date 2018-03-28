using System;

namespace Winterdom.Viasfora.Util {
  public interface ITextChars {
    int Position { get; }
    int AbsolutePosition { get; }
    bool EndOfLine { get; }
    char Char();
    char NChar();
    char NNChar();
    void Next();
    void Skip(int count);
    void SkipRemainder();
    void Mark();
    void ClearMark();
    void BackToMark();

    String PreviousToken();
    String GetRemainder();
  }
}
