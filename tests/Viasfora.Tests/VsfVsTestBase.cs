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
        var compositionContainer = GetCompositionContainer();
        cachedEditorHost = new VsfEditorHost(compositionContainer);
      }
      return cachedEditorHost;
    }

    private CompositionContainer GetCompositionContainer() {
      var catalogs = new List<ComposablePartCatalog> {
        new AssemblyCatalog(typeof(IUpdatableSettings).Assembly), // Viasfora.Settings
        new AssemblyCatalog(typeof(LanguageFactory).Assembly), // Viasfora.Languages
        new AssemblyCatalog(typeof(Guids).Assembly), // Viasfora.Core
        new AssemblyCatalog(typeof(TextBufferBraces).Assembly), // Viasfora.Rainbow
        new AssemblyCatalog(typeof(XmlTaggerProvider).Assembly) // Viasfora.Xml
      };
      AddEditorAssemblies(catalogs);
      var catalog = new AggregateCatalog(catalogs);
      return new CompositionContainer(catalog);
    }

    private void AddEditorAssemblies(IList<ComposablePartCatalog> catalogs) {
      var editorParts = new String[] {
        "Microsoft.VisualStudio.Platform.VSEditor",
        "Microsoft.VisualStudio.Text.Internal",
        "Microsoft.VisualStudio.Text.Logic",
        "Microsoft.VisualStudio.Text.UI",
        "Microsoft.VisualStudio.Text.UI.Wpf",
        "Microsoft.VisualStudio.Threading",
      };
      foreach ( var part in editorParts ) {
        var asm = GetEditorAssembly(part);
        catalogs.Add(new AssemblyCatalog(asm));
        catalogs.Add(GetExports(asm));
      }
    }

    private TypeCatalog GetExports(Assembly asm) {
      var types = asm.GetTypes().OrderBy(x => x.Name).ToList();
      return new TypeCatalog(types);
    }

    private Assembly GetEditorAssembly(String name) {
      String assemblyName = $"{name}, Version={VSAssemblyResolverFixture.VSASMVERSION}, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
      return Assembly.Load(assemblyName);
    }
  }
}
