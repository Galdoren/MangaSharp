using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manga.Core.Infrastructure
{
    public abstract class Observable<T> : IObservable<T>
    {
        #region Fields

        private IList<IObserver<T>> _observers;

        #endregion

        #region Ctor

        public Observable()
        {
            _observers = new List<IObserver<T>>();
        }

        #endregion

        public void Notify(T value)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(value);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
            return new Unsubscriber(_observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            #region Fields

            private IList<IObserver<T>> _observers;
            private IObserver<T> _observer;

            #endregion

            #region Ctor

            public Unsubscriber(IList<IObserver<T>> observers, IObserver<T> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            #endregion

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}
