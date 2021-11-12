using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition.Hosting;
using System.IO;

namespace Viasfora.Tests {
  public class VsfEditorHost  {
    public CompositionContainer CompositionContainer { get; }
    public ITextBufferFactoryService TextBufferFactoryService { get; }
    public IContentTypeRegistryService ContentTypeRegistryService { get; }
    public IBufferTagAggregatorFactoryService BufferTagAggregatorFactory { get; }

    public VsfEditorHost(CompositionContainer compositionContainer) {
      this.CompositionContainer = compositionContainer;
      this.TextBufferFactoryService = compositionContainer.GetExportedValue<ITextBufferFactoryService>();
      this.ContentTypeRegistryService = compositionContainer.GetExportedValue<IContentTypeRegistryService>();
      this.BufferTagAggregatorFactory = compositionContainer.GetExportedValue<IBufferTagAggregatorFactoryService>();
    }

    public IContentType GetOrCreateContentType(String name, params String[] baseTypes) {
      var ct = this.ContentTypeRegistryService.GetContentType(name);
      if ( ct != null ) {
        return ct;
      }
      return this.ContentTypeRegistryService.AddContentType(name, baseTypes);
    }

    public ITextBuffer CreateTextBuffer(IContentType ct, String content) {
      var reader = new StringReader(content);
      return TextBufferFactoryService.CreateTextBuffer(content, ct);
    }
  }
}
