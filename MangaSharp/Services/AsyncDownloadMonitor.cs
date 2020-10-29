using Manga.Services.Net;
using MangaSharp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaSharp.Services
{
    public class AsyncDownloadMonitor : IObserver<IDownloadProgress>
    {
        ThreadSafeObservableCollection<IDownloadProgress> _items;

        public AsyncDownloadMonitor(ThreadSafeObservableCollection<IDownloadProgress> items)
        {
            this._items = items;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(IDownloadProgress value)
        {
            if (_items.Contains(value))
            {
                if (value.Status == DownloadStatus.Completed)
                    _items.Remove(value);
            }
            else
                _items.Add(value);
        }
    }
}
