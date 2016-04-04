using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Winterdom.Viasfora.Compatibility {

  // using reflection here to avoid
  // a dependency on Microsoft.VisualStudio.ComponentModelHost
  // which is not versioned.
  public class SComponentModel {
    public const String SComponentModelHost = "FD57C398-FDE3-42c2-A358-660F269CBE43";
    private object sComponentModel;

    public SComponentModel() {
      sComponentModel = 
        ServiceProvider.GlobalProvider.GetService(new Guid(SComponentModelHost));
    }

    public T GetService<T>() {
      MethodInfo generic = sComponentModel.GetType().GetMethod("GetService");
      MethodInfo concrete = generic.MakeGenericMethod(typeof(T));
      return (T)concrete.Invoke(sComponentModel, null);
    }
  }
}
