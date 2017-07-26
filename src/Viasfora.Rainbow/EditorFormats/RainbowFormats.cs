using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Rainbow.EditorFormats {
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Rainbows.Rainbow1)]
  [Name(Rainbows.Rainbow1)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class Rainbow1Format : ClassificationFormatDefinition {
    public Rainbow1Format() {
      this.DisplayName = "Viasfora Rainbow Brace 1";
      this.ForegroundColor = Color.FromArgb(0xff, 0xff, 0x99, 0x00);
    }
  }
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Rainbows.Rainbow2)]
  [Name(Rainbows.Rainbow2)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class Rainbow2Format : ClassificationFormatDefinition {
    public Rainbow2Format() {
      this.DisplayName = "Viasfora Rainbow Brace 2";
      this.ForegroundColor = Colors.DeepPink; //Colors.MediumVioletRed;
    }
  }
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Rainbows.Rainbow3)]
  [Name(Rainbows.Rainbow3)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class Rainbow3Format : ClassificationFormatDefinition {
    public Rainbow3Format() {
      this.DisplayName = "Viasfora Rainbow Brace 3";
      this.ForegroundColor = Colors.YellowGreen; //MediumSeaGreen;
    }
  }
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Rainbows.Rainbow4)]
  [Name(Rainbows.Rainbow4)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class Rainbow4Format : ClassificationFormatDefinition {
    public Rainbow4Format() {
      this.DisplayName = "Viasfora Rainbow Brace 4";
      this.ForegroundColor = Colors.DarkViolet;
    }
  }
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Rainbows.Rainbow5)]
  [Name(Rainbows.Rainbow5)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class Rainbow5Format : ClassificationFormatDefinition {
    public Rainbow5Format() {
      this.DisplayName = "Viasfora Rainbow Brace 5";
      this.ForegroundColor = Colors.DimGray;
    }
  }
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Rainbows.Rainbow6)]
  [Name(Rainbows.Rainbow6)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class Rainbow6Format : ClassificationFormatDefinition {
    public Rainbow6Format() {
      this.DisplayName = "Viasfora Rainbow Brace 6";
      this.ForegroundColor = Colors.RoyalBlue;
    }
  }
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Rainbows.Rainbow7)]
  [Name(Rainbows.Rainbow7)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class Rainbow7Format : ClassificationFormatDefinition {
    public Rainbow7Format() {
      this.DisplayName = "Viasfora Rainbow Brace 7";
      this.ForegroundColor = Colors.Crimson;
    }
  }
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Rainbows.Rainbow8)]
  [Name(Rainbows.Rainbow8)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class Rainbow8Format : ClassificationFormatDefinition {
    public Rainbow8Format() {
      this.DisplayName = "Viasfora Rainbow Brace 8";
      this.ForegroundColor = Colors.DarkTurquoise;
    }
  }
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Rainbows.Rainbow9)]
  [Name(Rainbows.Rainbow9)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class Rainbow9Format : ClassificationFormatDefinition {
    public Rainbow9Format() {
      this.DisplayName = "Viasfora Rainbow Brace 9";
      this.ForegroundColor = Colors.Green;
    }
  }

  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Rainbows.TipHilight)]
  [Name(Rainbows.TipHilight)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class RainbowTipHighlightFormat : EditorFormatDefinition {
    public RainbowTipHighlightFormat() {
      this.DisplayName = "Viasfora Rainbow Tip Highlight";
      this.BackgroundColor = Colors.Turquoise;
      this.ForegroundCustomizable = false;
    }
  }

  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Rainbows.RainbowError)]
  [Name(Rainbows.RainbowError)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class RainbowErrorFormat : ClassificationFormatDefinition {
    public RainbowErrorFormat() {
      this.DisplayName = "Viasfora Rainbow Brace Error";
      this.BackgroundColor = Colors.LightCoral;
      this.ForegroundCustomizable = false;
    }
  }
}
