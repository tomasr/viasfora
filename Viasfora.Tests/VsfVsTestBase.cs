using EditorUtils;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using Winterdom.Viasfora;
using Winterdom.Viasfora.Contracts;

namespace Viasfora.Tests {
  public class VsfVsTestBase {
    private readonly VsfEditorHost editorHost;
    private static VsfEditorHost cachedEditorHost;
    public const String CSharpContentType = "CSharp"; 

    public VsfEditorHost EditorHost {
      get { return editorHost; }
    }

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

    private VsfEditorHost GetOrCreateEditorHost() {
      if ( cachedEditorHost == null ) {
        var editorHostFactory = new EditorHostFactory();
        editorHostFactory.Add(new AssemblyCatalog(typeof(IVsfSettings).Assembly));
        var compositionContainer = editorHostFactory.CreateCompositionContainer();
        cachedEditorHost = new VsfEditorHost(compositionContainer);
      }
      return cachedEditorHost;
    }
  }
}
