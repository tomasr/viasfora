using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Options {
  public partial class TextObfuscationDialog : UserControl, INotifyPropertyChanged {
    public ObservableCollection<RegexEntry> Entries { get; private set; }
    public ICommand AddEntryCmd { get; private set; }
    public ICommand RemoveEntryCmd { get; private set; }

    private RegexEntry selectedEntry;
    public RegexEntry SelectedEntry {
      get { return this.selectedEntry; }
      set { this.selectedEntry = value; RaiseChanged(nameof(SelectedEntry)); RaiseChanged(nameof(IsEntrySelected)); }
    }
    public bool IsEntrySelected => SelectedEntry != null;

    public TextObfuscationDialog() {
      this.Entries = new ObservableCollection<RegexEntry>();
      this.AddEntryCmd = new RelayCommand<RegexEntry>(AddEntry, AddEntryCmdEnabled);
      this.RemoveEntryCmd = new RelayCommand<RegexEntry>(RemoveEntry, RemoveEntryCmdEnabled);
      InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private bool AddEntryCmdEnabled(RegexEntry selected) {
      return true;
    }

    private bool RemoveEntryCmdEnabled(RegexEntry selected) {
      return this.SelectedEntry != null;
    }

    private void AddEntry(RegexEntry entry) {
      var newEntry = new RegexEntry { Name = "New Entry" };
      Entries.Add(newEntry);
      SelectedEntry = newEntry;
    }

    private void RemoveEntry(RegexEntry entry) {
      Entries.Remove(SelectedEntry);
      SelectedEntry = null;
    }

    private void RaiseChanged(String property) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
  }
}
