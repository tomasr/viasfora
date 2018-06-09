using EditorUtils;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
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

    public String[] ReadResource(String name) {
      Assembly asm = this.GetType().Assembly;
      var stream = asm.GetManifestResourceStream(name);
      IList<String> lines = new List<String>();
      using ( var reader = new StreamReader(stream) ) {
        String line = null;
        while ( (line = reader.ReadLine()) != null ) {
          lines.Add(line);
        }
      }
      return lines.ToArray();
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
        var editorHostFactory = new EditorHostFactory();
        var catalog = new AggregateCatalog(
          new AssemblyCatalog(typeof(IUpdatableSettings).Assembly), // Viasfora.Settings
          new AssemblyCatalog(typeof(LanguageFactory).Assembly), // Viasfora.Languages
          new AssemblyCatalog(typeof(Guids).Assembly), // Viasfora.Core
          new AssemblyCatalog(typeof(TextBufferBraces).Assembly), // Viasfora.Rainbow
          new AssemblyCatalog(typeof(XmlTaggerProvider).Assembly) // Viasfora.Xml
          );
        editorHostFactory.Add(catalog);
        var compositionContainer = editorHostFactory.CreateCompositionContainer();
        cachedEditorHost = new VsfEditorHost(compositionContainer);
      }
      return cachedEditorHost;
    }
  }
}
