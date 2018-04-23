using System;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Rainbow {
  // TODO: start using in TextBufferBraces
  public interface ITextLinesSource {
    int Length { get; }
    int GetLineNumberFromPosition(int position);
    ITextChars GetLineFromLineNumber(int lineNumber);
    bool IsSame(ITextLinesSource source);
  }
}
