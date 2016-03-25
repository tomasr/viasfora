using EditorUtils;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;

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
