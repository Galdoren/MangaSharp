using Caliburn.Micro;
using Manga.Services.Net;
using MangaSharp.Infrastructure;
using MangaSharp.Models;
using MangaSharp.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MangaSharp.ViewModels
{
    [ViewModelLocation("Main Window")]
    public class DownloadViewModel : Screen, IViewModel
    {
        #region Fields

        private readonly IDownloadManager _downloadManager;
        AsyncDownloadMonitor _monitor;
        IDisposable _unsubscriber;

        #endregion

        #region Ctor

        public DownloadViewModel(IDownloadManager downloadManager)
            : base()
        {
            this.DisplayName = "DOWNLOAD";
            this.Items = new ThreadSafeObservableCollection<IDownloadProgress>();
            this._downloadManager = downloadManager;
            
            _monitor = new AsyncDownloadMonitor(this.Items);
            
        }

        #endregion

        #region Properties

        public int Order
        {
            get { return 2; }
        }

        public ThreadSafeObservableCollection<IDownloadProgress> Items
        { get; set; }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            _unsubscriber = _downloadManager.Subscribe(_monitor);
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            if(_unsubscriber != null)
            {
                _unsubscriber.Dispose();
                _unsubscriber = null;
            }            
            base.OnDeactivate(close);
        }

        public void AddJob()
        {
            var random = new Random();
            Task.Run(() =>
            {
                for (var j = 0; j < 10; j++)
                {
                    var jobs = new ChapterQueue(5);
                    jobs.Manga = String.Format("Naruto #{0}", j + 1);
                    jobs.Progress = new DownloadProgress()
                    {
                        Progress = 0.0d,
                        Status = DownloadStatus.Created,
                        StatusText = "Waiting...",
                        Title = String.Format("Job #{0}", j + 1)
                    };

                    _downloadManager.Add(jobs);
                    
                    Task.Run(() =>
                    {
                        for (var i = 0; i < jobs.Size; i++)
                        {
                            jobs.Enqueue(new AsyncChapterJob());
                            Thread.Sleep(200 + random.Next(800));
                        }
                    });
                    Thread.Sleep(200);
                }
            });
        }

        #endregion
    }
}
