using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Winterdom.Viasfora;
using Winterdom.Viasfora.Languages;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Xml;
using Xunit;

namespace Viasfora.Tests {
  [Collection("DependsOnVS")]
  public class VsfVsTestBase {
    private readonly VsfEditorHost editorHost;
    private static VsfEditorHost cachedEditorHost;
    public const String CSharpContentType = "CSharp";

    public VsfEditorHost EditorHost => this.editorHost;

    public VsfVsTestBase() {
      if ( Application.Current == null ) {
        new Application();
      }
      this.editorHost = GetOrCreateEditorHost();
    }

    public String ReadResource(String name) {
      Assembly asm = this.GetType().Assembly;
      var stream = asm.GetManifestResourceStream(name);
      using ( var reader = new StreamReader(stream) ) {
        return reader.ReadToEnd();
      }
    }

    public ILanguage GetLang(ITextBuffer buffer) {
      var factory = EditorHost.CompositionContainer.GetExportedValue<ILanguageFactory>();
      return factory.TryCreateLanguage(buffer);
    }

    public ITextBuffer GetCSharpTextBuffer(String file) {
      var contentType = this.EditorHost.GetOrCreateContentType(CSharpContentType, "text");
      return this.EditorHost.CreateTextBuffer(
        contentType,
        ReadResource(GetType().Namespace + "." + file)
        );
    }

    public ITextBuffer GetPlainTextBuffer(String file) {
      var contentType = this.EditorHost.GetOrCreateContentType("text", "any");
      return this.EditorHost.CreateTextBuffer(
        contentType,
        ReadResource(GetType().Namespace + "." + file)
        );
    }

    private VsfEditorHost GetOrCreateEditorHost() {
      if ( cachedEditorHost == null ) {
        cachedEditorHost = new VsfEditorHostFactory().CreateEditorHost();
      }
      return cachedEditorHost;
    }

  }
}
