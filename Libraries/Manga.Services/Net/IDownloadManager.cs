using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using Manga.Core.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Manga.Services.Net
{
    public interface IDownloadManager : IObservable<IDownloadProgress>
    {
        void Add(ChapterQueue queue);
        void Cancel(ChapterQueue queue);
    }

    public interface IChapterJob
    {
        String Name { get; set; }
        String ChapterUrl { get; set; }
        String LocalPath { get; set; }
        List<ImageFile> DownloadList { get; set; }
    }

    public struct ImageFile
    {
        public String Url;
        public String Filename;
        public int Index;

        public ImageFile(String url, String filename, int index)
        {
            this.Url = url;
            this.Filename = filename;
            this.Index = index;
        }
    }
    
    public class ChapterQueue : ConcurrentQueue<IChapterJob>
    {
        #region Ctor

        public ChapterQueue(int size)
            : base()
        {
            Size = size;
            Manga = String.Empty;
            Progress = null;
            Directory = String.Empty;
        }

        #endregion

        #region Properties

        public int Size { get; private set; }
        public String Manga { get; set; }
        public IDownloadProgress Progress { get; set; }
        public String Directory { get; set; }

        #endregion

        #region Methods

        #endregion
    }

    public class ConsumerProducerDoubleQueue<T> : IEnumerable<T>, ICollection, IEnumerable, IDisposable
    {
        #region Fields

        private int _outCapacity;

        private Queue<T> _inQueue;
        private Queue<T> _outQueue;
        private volatile object _syncRoot;
        private volatile object _syncRoot2;
        private Semaphore _semaphore;

        #endregion

        #region Ctor

        public ConsumerProducerDoubleQueue(int outCapacity)
        {
            _outCapacity = outCapacity;

            Initialize();
        }

        #endregion

        #region Utilities

        private void Initialize()
        {
            _syncRoot = new object();
            _syncRoot2 = new object();
            _semaphore = new Semaphore(0, int.MaxValue);

            _inQueue = new Queue<T>();
            _outQueue = new Queue<T>(_outCapacity);
        }

        #endregion

        #region Methods

        public void Enqueue(T item)
        {
            lock(_syncRoot)
            {
                // if right is not full
                if(_outQueue.Count < _outCapacity)
                {
                    // if right is not full and left is empty, add directly to right
                    if(_inQueue.Count == 0)
                        _outQueue.Enqueue(item);
                    // if right is not full and left is not empty, add to left
                    else
                    {
                        _inQueue.Enqueue(item);
                        // while right is not full and left is not empty, add from left to right
                        while(_outQueue.Count < _outCapacity && _inQueue.Count > 0)
                        {                            
                            _outQueue.Enqueue(_inQueue.Dequeue());
                        }                        
                    }
                }
                // if right is full, add to left
                else
                {
                    _inQueue.Enqueue(item);
                }                
            }
            _semaphore.Release();
        }

        public T Dequeue()
        {
            _semaphore.WaitOne();
            T item = default(T);
            if (Count > 0)
            {
                lock (_syncRoot)
                {
                    // fill right from left
                    while (_outQueue.Count < _outCapacity && _inQueue.Count > 0)
                    {
                        _outQueue.Enqueue(_inQueue.Dequeue());
                    }
                    // if right is not empty return item
                    if (_outQueue.Count > 0)
                    {
                        item = _outQueue.Dequeue();
                    }
                }
            }
            return item;
        }

        public bool TryDequeue(out T item)
        {
            item = default(T);
            bool result = false;
            if(Count > 0)
            {
                lock(_syncRoot)
                {
                    // fill right from left
                    while (_outQueue.Count < _outCapacity && _inQueue.Count > 0)
                    {
                        _outQueue.Enqueue(_inQueue.Dequeue());
                    }
                    // if right is not empty return item
                    if (_outQueue.Count > 0)
                    {
                        item = _outQueue.Dequeue();
                        result = true;
                    }
                }
            }
            return result;
        }

        public void Clear()
        {
            lock(_syncRoot)
            {
                _inQueue.Clear();
                _outQueue.Clear();
            }
        }

        public bool Contains(T item)
        {
            bool contains = false;
            lock(_syncRoot)
            {
                contains = _inQueue.Contains(item) || _outQueue.Contains(item);
            }

            return contains;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (arrayIndex > array.Length)
                return;
            lock(_syncRoot)
            {
                var e = _inQueue.Concat(_outQueue).GetEnumerator();
                e.Reset();
                
                for(int i = arrayIndex; i < array.Length; i++)
                {
                    if (!e.MoveNext())
                        break;
                    array[i] = e.Current;
                }
            }
        }

        public void CopyTo(Array array, int index)
        {
            if (index > array.Length)
                return;
            lock(_syncRoot)
            {
                var e = _inQueue.Concat(_outQueue).GetEnumerator();
                e.Reset();

                for (int i = index; i < array.Length; i++)
                {
                    if (!e.MoveNext())
                        break;
                    array.SetValue(e.Current, i);
                }
            }
        }

        public int Count
        {
            get { return _inQueue.Count + _outQueue.Count; }
        }

        public int InCapacity
        {
            get { return _inQueue.Count; }
        }

        public int OutCount
        {
            get { return _outQueue.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }     

        public bool IsSynchronized
        {
            get { return true; }
        }

        public object SyncRoot
        {
            get { return _syncRoot; }
        }

        public void Dispose()
        {
            lock(_syncRoot)
            {
                Clear();
                _semaphore.Dispose();
                _semaphore = null;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            while (true) yield return Dequeue();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }

        #endregion

        #region Hidden

        public void Add(T item)
        { throw new NotImplementedException(); }

        public bool Remove(T item)
        { throw new NotImplementedException(); }

        #endregion
    }
}
