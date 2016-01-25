using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Options {
  public partial class TextObfuscationDialog : UserControl {
    public ObservableCollection<RegexEntry> Entries { get; private set; }
    public RegexEntry SelectedEntry { get; private set; }
    public ICommand AddEntryCmd { get; private set; }
    public ICommand RemoveEntryCmd { get; private set; }

    public TextObfuscationDialog() {
      this.Entries = new ObservableCollection<RegexEntry>();
      this.AddEntryCmd = new RelayCommand<RegexEntry>(AddEntry, AddEntryCmdEnabled);
      this.RemoveEntryCmd = new RelayCommand<RegexEntry>(RemoveEntry, RemoveEntryCmdEnabled);
      InitializeComponent();
    }

    private bool AddEntryCmdEnabled(RegexEntry selected) {
      return true;
    }

    private bool RemoveEntryCmdEnabled(RegexEntry selected) {
      return this.SelectedEntry != null;
    }

    private void AddEntry(RegexEntry entry) {
      Entries.Add(new RegexEntry());
    }

    private void RemoveEntry(RegexEntry entry) {
      Entries.Remove(entry);
    }
  }
}
