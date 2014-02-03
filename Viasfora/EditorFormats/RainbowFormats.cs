using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Media;
using System.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.EditorFormats {
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.RAINBOW_1)]
  [Name(Constants.RAINBOW_1)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class Rainbow1Format : ClassificationFormatDefinition {
    public Rainbow1Format() {
      this.DisplayName = Constants.RAINBOW_1;
      this.ForegroundColor = Colors.Crimson;
    }
  }
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.RAINBOW_2)]
  [Name(Constants.RAINBOW_2)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class Rainbow2Format : ClassificationFormatDefinition {
    public Rainbow2Format() {
      this.DisplayName = Constants.RAINBOW_2;
      this.ForegroundColor = Colors.MediumOrchid;
    }
  }
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.RAINBOW_3)]
  [Name(Constants.RAINBOW_3)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class Rainbow3Format : ClassificationFormatDefinition {
    public Rainbow3Format() {
      this.DisplayName = Constants.RAINBOW_3;
      this.ForegroundColor = Colors.LimeGreen;
    }
  }
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.RAINBOW_4)]
  [Name(Constants.RAINBOW_4)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class Rainbow4Format : ClassificationFormatDefinition {
    public Rainbow4Format() {
      this.DisplayName = Constants.RAINBOW_4;
      this.ForegroundColor = Colors.RoyalBlue;
    }
  }
}
