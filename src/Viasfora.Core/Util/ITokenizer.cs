using System;

namespace Winterdom.Viasfora.Util {
  public interface ITokenizer {
    bool AtEnd { get; }
    String Token { get; }
    bool Next();
  }
}
