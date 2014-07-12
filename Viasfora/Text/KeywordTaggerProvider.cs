using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Languages;
using Winterdom.Viasfora.Tags;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora.Text {

  [Export(typeof(IViewTaggerProvider))]
  [ContentType(CSharp.ContentType)]
  [ContentType(Cpp.ContentType)]
  [ContentType(VB.ContentType)]
  [ContentType(VB.VBScriptContentType)]
  [ContentType(JScript.ContentType)]
  [ContentType(JScript.ContentTypeVS2012)]
  [ContentType(JSON.ContentType)]
  [ContentType(FSharp.ContentType)]
  [ContentType(Sql.ContentType)]
  [ContentType(Sql.ContentTypeAlt)]
  [ContentType(TypeScript.ContentType)]
  [ContentType(Python.ContentType)]
  [ContentType(PowerShell.ContentType)]
  [ContentType(Css.ContentType)]
  [ContentType(Css.SassContentType)]
  [ContentType(Css.LessContentType)]
  [TagType(typeof(KeywordTag))]
  public class KeywordTaggerProvider : IViewTaggerProvider {
    [Import]
    public IClassificationTypeRegistryService ClassificationRegistry { get; set; }
    [Import]
    public IBufferTagAggregatorFactoryService Aggregator { get; set; }
    [Import]
    public ILanguageFactory LanguageFactory { get; set; }

    public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag {
      return new KeywordTagger(buffer, this) as ITagger<T>;
    }
  }

}
