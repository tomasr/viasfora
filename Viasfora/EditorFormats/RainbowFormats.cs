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
      this.ForegroundColor = Color.FromArgb(0xff, 0xff, 0x99, 0x00);
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
      this.ForegroundColor = Colors.DeepPink; //Colors.MediumVioletRed;
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
      this.ForegroundColor = Colors.YellowGreen; //MediumSeaGreen;
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
      this.ForegroundColor = Colors.DarkViolet;
    }
  }
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.RAINBOW_5)]
  [Name(Constants.RAINBOW_5)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class Rainbow5Format : ClassificationFormatDefinition {
    public Rainbow5Format() {
      this.DisplayName = Constants.RAINBOW_5;
      this.ForegroundColor = Colors.DimGray;
    }
  }
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.RAINBOW_6)]
  [Name(Constants.RAINBOW_6)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class Rainbow6Format : ClassificationFormatDefinition {
    public Rainbow6Format() {
      this.DisplayName = Constants.RAINBOW_6;
      this.ForegroundColor = Colors.RoyalBlue;
    }
  }
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.RAINBOW_7)]
  [Name(Constants.RAINBOW_7)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class Rainbow7Format : ClassificationFormatDefinition {
    public Rainbow7Format() {
      this.DisplayName = Constants.RAINBOW_7;
      this.ForegroundColor = Colors.Crimson;
    }
  }
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.RAINBOW_8)]
  [Name(Constants.RAINBOW_8)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class Rainbow8Format : ClassificationFormatDefinition {
    public Rainbow8Format() {
      this.DisplayName = Constants.RAINBOW_8;
      this.ForegroundColor = Colors.DarkTurquoise;
    }
  }
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.RAINBOW_9)]
  [Name(Constants.RAINBOW_9)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class Rainbow9Format : ClassificationFormatDefinition {
    public Rainbow9Format() {
      this.DisplayName = Constants.RAINBOW_9;
      this.ForegroundColor = Colors.Green;
    }
  }

  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.RAINBOW_TIP_HIGHLIGHT)]
  [Name(Constants.RAINBOW_TIP_HIGHLIGHT)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class RainbowTipHighlightFormat : EditorFormatDefinition {
    public RainbowTipHighlightFormat() {
      this.DisplayName = "Viasfora Rainbow Tip Highlight";
      this.BackgroundColor = Colors.Turquoise;
      this.ForegroundCustomizable = false;
    }
  }
}
