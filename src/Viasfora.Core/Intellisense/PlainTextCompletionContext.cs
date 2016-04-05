using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;

namespace Winterdom.Viasfora.Intellisense {
  // This is just a way for our completion 
  // command handler to signal the completion
  // source that it should return data.
  // it's used so that we don't pollute the
  // language provider completion set
  public class PlainTextCompletionContext {
    public static object Key {
      get { return typeof(PlainTextCompletionContext); }
    }
    public static void Add(ICompletionSession session) {
      if ( !session.Properties.ContainsProperty(Key) ) {
        session.Properties.AddProperty(Key, new PlainTextCompletionContext());
      }
    }
    public static bool IsSet(ICompletionSession session) {
      PlainTextCompletionContext context = null;
      return session.Properties.TryGetProperty(Key, out context)
             && context != null;
    }
  }
}
