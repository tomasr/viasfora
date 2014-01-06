using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Languages;

namespace Winterdom.Viasfora.Util {
  public class ModeLineParser : IModeLineParser {

    public IDictionary<String, String> Parse(ITextChars tc) {
      Dictionary<String, String> result = new Dictionary<String, String>();
      ParseModeLine(tc, result);
      return result;
    }

    private void ParseModeLine(ITextChars tc, Dictionary<String, String> result) {
      // we expect it to look something like this:
      // vim: noai:ts=4:sw=4
      // vim: set noai ts=4 sw=4: 
    }
  }
}
