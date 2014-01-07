using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Languages;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Text {
  public class ModeLineProvider {
    private IWpfTextView theView;
    public ModeLineProvider(IWpfTextView view) {
      this.theView = view;
    }

    public void ParseModeline() {
      ITextBuffer buffer = this.theView.TextBuffer;
      var snapshot = buffer.CurrentSnapshot;
      if ( snapshot.LineCount == 0 ) {
        return;
      }
      LanguageInfo language = VsfPackage.LookupLanguage(snapshot.ContentType);
      if ( language == null ) return;

      var firstLine = snapshot.GetLineFromLineNumber(0);

      ITextChars tc = new LineChars(firstLine);
      String commentText = language.NewFirstLineCommentParser().Parse(tc);
      if ( String.IsNullOrEmpty(commentText) ) {
        return;
      }

      var modelineParser = new ModeLineParser();
      var options = modelineParser.Parse(commentText);
      ApplyModelines(options);
    }

    private void ApplyModelines(IDictionary<String, String> options) {
      foreach ( String key in options.Keys ) {
        System.Diagnostics.Trace.WriteLine(String.Format("Modeline: {0}={1}", key, options[key]));
      }
    }
  }
}
