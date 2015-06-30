using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Viasfora.Rainbow {
  public class CharList<T> where T : IPosition {
    private List<T> items = new List<T>();

    public int Count {
      get {
        return items.Count;
      }
    }

    public T this[int index] {
      get { return items[index]; }
    }

    public void Add(T item) {
      items.Add(item);
    }

    public void ClearFrom(int index) {
      if ( index < 0 || index >= Count )
        throw new ArgumentOutOfRangeException("index");
      items.RemoveRange(index, Count - index);
    }

    public int FindBefore(int position) {
      int first = 0;
      int last = this.Count - 1;
      int candidate = -1;
      while ( first <= last ) {
        int mid = (first + last) / 2;
        T midPos = this[mid];
        if ( midPos.Position < position ) {
          // keep looking in second half
          candidate = mid;
          first = mid + 1;
        } else if ( midPos.Position > position ) {
          // keep looking in first half
          last = mid - 1;
        } else {
          // we've got an exact match
          // but we're interested on an strict
          // order, so return the item before this one
          candidate = mid - 1;
          break;
        }
      }
      return candidate;
    }

    public int FindAtOrAfter(int position) {
      int first = 0;
      int last = this.Count - 1;
      int candidate = -1;
      while ( first <= last ) {
        int mid = (first + last) / 2;
        T midPos = this[mid];
        if ( midPos.Position < position ) {
          // keep looking in second half
          first = mid + 1;
        } else if ( midPos.Position > position ) {
          // keep looking in first half
          candidate = mid;
          last = mid - 1;
        } else {
          // we've got an exact match
          candidate = mid;
          break;
        }
      }
      return candidate;
    }

    public IEnumerable<T> FindInRange(int start, int end) {
      int startIndex = this.FindAtOrAfter(start);
      if ( startIndex >= 0 ) {
        for ( ; startIndex < this.Count; startIndex++ ) {
          T pos = this.items[startIndex];
          if ( pos.Position > end ) break;
          yield return pos;
        }
      }
    }
  }

  public interface IPosition {
    int Position { get; }
  }
}
