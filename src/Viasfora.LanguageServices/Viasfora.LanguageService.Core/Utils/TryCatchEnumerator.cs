using System;
using System.Collections;
using System.Collections.Generic;

namespace Winterdom.Viasfora.LanguageService.Core.Utils {
  class TryCatchEnumerator<T> : IEnumerator<T> {

    public delegate bool OnExceptionDelegate(Exception ex);

    private IEnumerator<T> _inner;
    private OnExceptionDelegate _onException;

    public TryCatchEnumerator(IEnumerator<T> inner, OnExceptionDelegate onException) {
      this._inner = inner ?? throw new ArgumentNullException(nameof(inner));
      this._onException = onException ?? throw new ArgumentNullException(nameof(onException));
    }

    private T _current;
    public T Current => this._current;
    object IEnumerator.Current => this.Current;

    public void Dispose() {
      this._inner.Dispose();
    }

    public bool MoveNext() {
      var isFaulted = false;
      for (; ; ) {
        try {
          var ok = this._inner.MoveNext();
          if ( ok )
            this._current = this._inner.Current;
          else
            this._current = default(T);

          isFaulted = false;
          return ok;
        } catch ( Exception ex ) {
          if ( isFaulted || !this._onException(ex) ) {
            this._current = default(T);
            return false;
          } 
        }
      }
    }

    public void Reset() {
      this._inner.Reset();
    }
  }
}
