﻿using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace Winterdom.Viasfora.Options {
  public class ColorStorage {
    public IVsUIShell2 Shell { get; private set; }
    public IVsFontAndColorStorage Storage { get; private set; }
    public IVsFontAndColorUtilities Utilities { get; private set; }

    public ColorStorage(IServiceProvider provider) {
      ThreadHelper.ThrowIfNotOnUIThread();
      Shell = (IVsUIShell2)provider.GetService(typeof(SVsUIShell));
      Storage = (IVsFontAndColorStorage)provider.GetService(typeof(SVsFontAndColorStorage));
      Utilities = (IVsFontAndColorUtilities)Storage;
    }

    public uint GetAutomaticColor() {
      ThreadHelper.ThrowIfNotOnUIThread();
      int hr = Utilities.EncodeAutomaticColor(out uint result);
      ErrorHandler.ThrowOnFailure(hr);
      return result;
    }
  }
}
