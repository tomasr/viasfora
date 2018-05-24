using System;
using System.Collections.Generic;

namespace Winterdom.Viasfora.Util {
  public interface IModeLineParser {
    IDictionary<String, String> Parse(String text);
  }
}
