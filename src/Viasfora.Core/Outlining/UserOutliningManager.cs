using System;
using Microsoft.VisualStudio.Text;
using Winterdom.Viasfora.Settings;

namespace Winterdom.Viasfora.Outlining {
  public class UserOutliningManager : BaseOutliningManager {

    protected UserOutliningManager(ITextBuffer buffer) : base(buffer) {
      LoadRegions(buffer);
    }

    public static IUserOutlining Get(ITextBuffer buffer) {
      return buffer.Properties.GetOrCreateSingletonProperty(() => {
        return new UserOutliningManager(buffer);
      });
    }

    public static IOutliningManager GetManager(ITextBuffer buffer) {
      return Get(buffer) as IOutliningManager;
    }

    protected override void OnSpanAdded(SnapshotSpan span) {
      UpdateUserSettings(span.Snapshot.TextBuffer, span.Snapshot);
    }
    protected override void OnRegionRemoved(SnapshotPoint point) {
      UpdateUserSettings(point.Snapshot.TextBuffer, point.Snapshot);
    }
    protected override void OnAllRegionsRemoved(ITextSnapshot snapshot) {
      UpdateUserSettings(snapshot.TextBuffer, snapshot);
    }

    private void LoadRegions(ITextBuffer buffer) {
      var sus = VsSolution.GetUserSettings();
      if ( sus == null ) {
        return;
      }
      String filename = TextEditor.GetFileName(buffer);
      if ( String.IsNullOrEmpty(filename) ) {
        return;
      }
      filename = VsSolution.MakeRelativePath(filename);
      OutlineSettings settings = sus.Load<OutlineSettings>(filename);
      if ( settings != null ) {
        this.Regions.LoadStoredData(buffer.CurrentSnapshot, settings);
      }
    }

    private void UpdateUserSettings(ITextBuffer buffer, ITextSnapshot snapshot) {
      var sus = VsSolution.GetUserSettings();
      if ( sus == null ) {
        return;
      }
      String filename = TextEditor.GetFileName(buffer);
      if ( String.IsNullOrEmpty(filename) ) {
        return;
      }
      filename = VsSolution.MakeRelativePath(filename);
      sus.Store(filename, Regions.GetStorableData(snapshot));
    }
  }
}
