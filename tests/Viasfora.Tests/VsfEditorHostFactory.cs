using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Winterdom.Viasfora;
using Winterdom.Viasfora.Languages;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Xml;

namespace Viasfora.Tests {
  internal class VsfEditorHostFactory {
    private readonly List<ComposablePartCatalog> composablePartCatalogList = new List<ComposablePartCatalog>();
    private readonly List<ExportProvider> exportProviderList = new List<ExportProvider>();

    public VsfEditorHostFactory() {
      BuildCatalog();
    }

    public VsfEditorHost CreateEditorHost() {
      return new VsfEditorHost(GetCompositionContainer());
    }

    public void BuildCatalog() {
      AddEditorAssemblies(this.composablePartCatalogList);
      this.exportProviderList.Add(new JoinableTaskContextExportProvider());

      this.composablePartCatalogList.AddRange(new List<ComposablePartCatalog> {
        new AssemblyCatalog(typeof(IUpdatableSettings).Assembly), // Viasfora.Settings
        new AssemblyCatalog(typeof(LanguageFactory).Assembly), // Viasfora.Languages
        new AssemblyCatalog(typeof(Guids).Assembly), // Viasfora.Core
        new AssemblyCatalog(typeof(TextBufferBraces).Assembly), // Viasfora.Rainbow
        new AssemblyCatalog(typeof(XmlTaggerProvider).Assembly), // Viasfora.Xml
        new AssemblyCatalog(typeof(VsfEditorHostFactory).Assembly)
      });
    }

    private CompositionContainer GetCompositionContainer() {
      var catalog = new AggregateCatalog(this.composablePartCatalogList.ToArray());
      return new CompositionContainer(catalog, this.exportProviderList.ToArray());
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
      }
    }

    public void AddAssembly(Assembly asm) {
      this.composablePartCatalogList.Add(new AssemblyCatalog(asm));
    }

    private TypeCatalog GetExports(Assembly asm) {
      var types = asm.GetTypes().OrderBy(x => x.Name).ToList();
      return new TypeCatalog(types);
    }

    private Assembly GetEditorAssembly(String name) {
      String assemblyName = $"{name}, Version={VSAssemblyResolverFixture.VSASMVERSION}, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL";
      return Assembly.Load(assemblyName);
    }
  }
  sealed class JoinableTaskContextExportProvider : ExportProvider {
    internal static string TypeFullName => typeof(JoinableTaskContext).FullName;
    private readonly Export _export;
    private readonly JoinableTaskContext _context;

    internal JoinableTaskContextExportProvider() {
      _export = new Export(TypeFullName, GetValue);
#pragma warning disable VSSDK005
      _context = new JoinableTaskContext(Thread.CurrentThread, new DispatcherSynchronizationContext());
    }

    protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition) {
      if ( definition.ContractName == TypeFullName ) {
        yield return _export;
      }
    }

    private object GetValue() => _context;
  }
}
