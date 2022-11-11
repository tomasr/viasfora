using Microsoft.VisualStudio.Shell;
using System;
using System.Reflection;

namespace Winterdom.Viasfora.Compatibility {

  // using reflection here to avoid
  // a dependency on Microsoft.VisualStudio.ComponentModelHost
  // which is not versioned.
  public class SComponentModel {
    public const String SComponentModelHost = "FD57C398-FDE3-42c2-A358-660F269CBE43";
    private object sComponentModel;

    public SComponentModel() {
      ThreadHelper.ThrowIfNotOnUIThread();
      this.sComponentModel = 
        ServiceProvider.GlobalProvider.GetService(new Guid(SComponentModelHost));
      if ( this.sComponentModel == null ) {
        throw new InvalidOperationException("SComponentModelHost not available");
      }
    }

    public T GetService<T>() {
      MethodInfo generic = this.sComponentModel.GetType().GetMethod("GetService");
      MethodInfo concrete = generic.MakeGenericMethod(typeof(T));
      return (T)concrete.Invoke(this.sComponentModel, null);
    }
  }
}
