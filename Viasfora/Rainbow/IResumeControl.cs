using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Rainbow {
  public interface IResumeControl {
    bool CanResume(CharPos brace);
  }
}
