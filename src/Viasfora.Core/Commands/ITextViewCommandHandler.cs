using Microsoft.VisualStudio.Text.Editor;
using System;

namespace Winterdom.Viasfora.Commands {
  public interface ITextViewCommandHandler {
    Guid CommandGroup { get; }
    int CommandId { get; }
    bool IsEnabled(ITextView view, ref String commandText);
    bool Handle(ITextView view);
  }
}
