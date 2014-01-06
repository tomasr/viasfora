using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Languages;

namespace Winterdom.Viasfora.Util {
  public class ModeLineParser : IModeLineParser {
    private int status;

    public IDictionary<String, String> Parse(String text) {
      Dictionary<String, String> result = new Dictionary<String, String>();
      ITokenizer tokenizer = new SimpleTokenizer(text);
      ParseModeLine(tokenizer, result);
      return result;
    }

    private void ParseModeLine(ITokenizer tokenizer, Dictionary<string, string> result) {
      // we expect it to look something like this:
      // vim: noai:ts=4:sw=4
      // vim: set noai ts=4 sw=4: 
      if ( !tokenizer.Next() ) return;
      String tool = tokenizer.Token;
      if ( !tokenizer.Next() ) return;

      if ( tokenizer.Token != ":" ) return; // not what we expect
      if ( !tokenizer.Next() ) return;

      if ( tokenizer.Token == "Set" ) {
        tokenizer.Next();
        ParseListNoDelimiter(tokenizer, result);
      } else {
        tokenizer.Next();
        ParseList(tokenizer, result, ":");
      }
    }

    private void ParseList(ITokenizer tokenizer, Dictionary<string, string> result, string delimiter) {
      while ( !tokenizer.AtEnd ) {
        String p1 = tokenizer.Token;
        tokenizer.Next();
        String p2 = tokenizer.Token;
        if ( p2 == "=" ) {
          tokenizer.Next();
          String p3 = tokenizer.Token;
          result[p1] = p3;
          tokenizer.Next();
        } else {
          result[p1] = "";
        }
        if ( tokenizer.Token != delimiter ) break;
      }
    }
    private void ParseListNoDelimiter(ITokenizer tokenizer, Dictionary<string, string> result) {
      while ( !tokenizer.AtEnd ) {
        String p1 = tokenizer.Token;
        tokenizer.Next();
        String p2 = tokenizer.Token;
        if ( p2 == "=" ) {
          tokenizer.Next();
          String p3 = tokenizer.Token;
          result[p1] = p3;
          tokenizer.Next();
        } else {
          result[p1] = "";
        }
      }
    }
  }
}
