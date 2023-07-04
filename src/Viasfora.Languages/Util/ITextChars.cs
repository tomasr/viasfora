using System;

namespace Winterdom.Viasfora.Util {
  public interface ITextChars {
    int Position { get; }
    int AbsolutePosition { get; }
    int End { get; }
    bool AtEnd { get; }
    char Char();
    char NChar();
    char NNChar();
    char NNNChar();
    char NNNNChar();
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
