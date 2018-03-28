using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using Winterdom.Viasfora.Languages;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Text {
  public class ModeLineProvider {
    private IWpfTextView theView;
    private static Dictionary<String, Action<IWpfTextView, String>> optionMap;
    private ILanguageFactory langFactory;

    static ModeLineProvider() {
      optionMap = new Dictionary<string, Action<IWpfTextView, String>>();
      InitializeOptionMap();
    }
    public ModeLineProvider(IWpfTextView view, ModelineFactory factory) {
      this.theView = view;
      this.langFactory = factory.LanguageFactory;
    }

    public void ParseModeline(int numLine) {
      ITextBuffer buffer = this.theView.TextBuffer;
      var snapshot = buffer.CurrentSnapshot;
      if ( snapshot.LineCount <= numLine ) {
        return;
      }
      ILanguage language = langFactory.TryCreateLanguage(snapshot);
      if ( language == null ) return;

      var firstLine = snapshot.GetLineFromLineNumber(numLine);

      ITextChars tc = new LineChars(firstLine);
      var svc = language.GetService<IFirstLineCommentParser>();
      String commentText = svc.Parse(tc);
      if ( String.IsNullOrEmpty(commentText) ) {
        return;
      }

      var modelineParser = new ModeLineParser();
      var options = modelineParser.Parse(commentText);
      ApplyModelines(options);
    }

    private void ApplyModelines(IDictionary<String, String> options) {
      foreach ( String key in options.Keys ) {
        ApplyModeLine(key, options[key]);
      }
    }

    private void ApplyModeLine(String key, String value) {
      if ( String.IsNullOrEmpty(value) ) {
        // assume this is a boolean option
        value = key.StartsWith("no") ? "false" : "true";
      }
      Action<IWpfTextView, String> option;
      if ( optionMap.TryGetValue(key, out option) ) {
        option(theView, value);
      }
    }

    private static void SetShiftWidth(IWpfTextView view, String value) {
      int intValue;
      if ( Int32.TryParse(value, out intValue) ) {
        view.Options.SetOptionValue(
          DefaultOptions.IndentSizeOptionId, intValue
        );
      }
    }

    private static void SetTabStop(IWpfTextView view, String value) {
      int intValue;
      if ( Int32.TryParse(value, out intValue) ) {
        view.Options.SetOptionValue(
          DefaultOptions.TabSizeOptionId, intValue
        );
      }
    }

    private static void SetExpandTab(IWpfTextView view, String value) {
      bool boolVal;
      if ( bool.TryParse(value, out boolVal) ) {
        view.Options.SetOptionValue(
          DefaultOptions.ConvertTabsToSpacesOptionId, boolVal
        );
      }
    }
    private static void SetFileFormat(IWpfTextView view, String value) {
      String eol = null;
      switch ( value ) {
        case "dos": eol = "\r\n"; break;
        case "unix": eol = "\n"; break;
        case "mac": eol = "\r"; break;
      }
      if ( !String.IsNullOrEmpty(eol) ) {
        view.Options.SetOptionValue(
          DefaultOptions.NewLineCharacterOptionId, eol
        );
      }
    }

    private static void InitializeOptionMap() {
      optionMap["et"] = SetExpandTab;
      optionMap["expandtab"] = SetExpandTab;
      optionMap["noet"] = SetExpandTab;
      optionMap["noexpandtab"] = SetExpandTab;
      optionMap["ts"] = SetTabStop;
      optionMap["tabstop"] = SetTabStop;
      optionMap["sw"] = SetShiftWidth;
      optionMap["shiftwidth"] = SetShiftWidth;
      optionMap["ff"] = SetFileFormat;
      optionMap["fileformat"] = SetFileFormat;
    }

  }
}
