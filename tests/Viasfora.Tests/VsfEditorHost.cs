using EditorUtils;
using Microsoft.VisualStudio.Text.Tagging;
using System.ComponentModel.Composition.Hosting;

namespace Viasfora.Tests {
  public class VsfEditorHost : EditorHost {
    public VsfEditorHost(CompositionContainer compositionContainer)
      : base(compositionContainer) {
    }

    public IBufferTagAggregatorFactoryService BufferTagAggregatorFactory {
      get { return this.CompositionContainer.GetExportedValue<IBufferTagAggregatorFactoryService>(); }
    }
  }
}
