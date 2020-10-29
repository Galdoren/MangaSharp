using Manga.Core.Domain;
using Manga.Core.Infrastructure;
using Manga.Services.Net;
using MangaSharp.Models;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MangaSharp.Services
{
    public class AsyncDownloadManager : Observable<IDownloadProgress>, IDownloadManager, IDisposable
    {
        #region Fields

        private int _concurrencyLimit;

        private BlockingCollection<ChapterQueue> _downloadQueue;
        private BlockingCollection<ChapterQueue> _runningQueue;

        //private ConsumerProducerDoubleQueue<ChapterQueue> _queue;

        private SemaphoreSlim _semaphore;
        private Mutex _mutex;
        private WaitHandle _waitHandle;
        private CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region Ctor

        public AsyncDownloadManager()
        {
            _concurrencyLimit = 5;

            _downloadQueue = new BlockingCollection<ChapterQueue>();
            _runningQueue = new BlockingCollection<ChapterQueue>();

            _semaphore = new SemaphoreSlim(ConcurrencyLimit, ConcurrencyLimit);
            _mutex = new Mutex();
            
            _cancellationTokenSource = new CancellationTokenSource();

            TransferLoop();
            DownloadLoop();
        }

        #endregion

        #region Properties

        public int ConcurrencyLimit { get { return _concurrencyLimit; } }

        #endregion

        #region Methods

        public void Dispose()
        {
            if (_semaphore != null)
                _semaphore.Dispose();
        }

        public void Add(ChapterQueue queue)
        {
            _downloadQueue.Add(queue);

            var progress = queue.Progress;

            progress.Status = DownloadStatus.Downloading;
            progress.Progress = 0.0d;

            Notify(progress);
        }

        public void Cancel(ChapterQueue queue)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Transfers loop
        /// </summary>        
        protected void TransferLoop()
        {
            CancellationToken token = _cancellationTokenSource.Token;
            Task.Run(() =>
            {
                try
                {
                    foreach (var item in _downloadQueue.GetConsumingEnumerable(token))
                    {
                        Console.WriteLine("TransferLoop: {0}", item.Manga);
                        _semaphore.Wait();
                        _runningQueue.Add(item, token);
                    }
                }
                catch (OperationCanceledException e)
                {
                    _semaphore.Release();
                }
                catch(Exception e)
                {
                    _semaphore.Release();
                }
            }, token);
        }
        

        protected void DownloadLoop()
        {
            CancellationToken token = _cancellationTokenSource.Token;
            Task.Run(() =>
            {
                try
                {
                    foreach (var item in _runningQueue.GetConsumingEnumerable())
                    {
                        Console.WriteLine("DownloadLoop: {0}", item.Manga);
                        Task.Run(() =>
                        {
                            DownloadChapterQueue(item);
                            _semaphore.Release();
                        });
                    }
                }
                catch(OperationCanceledException e)
                {
                    _semaphore.Release();
                }
                catch (Exception e)
                {
                    _semaphore.Release();
                }
            }, token);
        }

        protected void DownloadChapterQueue(ChapterQueue queue)
        {
            var progress = queue.Progress;

            progress.StatusText = "Waiting...";

            progress.Status = DownloadStatus.Downloading;
            progress.Progress = 0.0d;
            
            int i = 0, j = queue.Size;

            while (j > 0)
            {
                IChapterJob job;
                if (!queue.TryDequeue(out job))
                    Thread.Sleep(20);
                else
                {
                    progress.Progress += 100 / queue.Size;
                    progress.StatusText = String.Format("{0} / {1}", i + 1, queue.Size);
                    Thread.Sleep(100);

                    j--;
                    i++;
                }
            }
            progress.Status = DownloadStatus.Completed;
            Notify(progress);
        }

        /// <summary>
        /// Downloads a chapter
        /// </summary>
        /// <param name="job"></param>
        protected void DownloadChapter(IChapterJob job, IDownloadProgress progress)
        {
            
        }

        /// <summary>
        /// Downloads a chapter image
        /// </summary>
        /// <param name="file"></param>
        /// <param name="progress"></param>
        protected void DownloadRemoteImageFile(ImageFile file, IDownloadProgress progress)
        {            
            int tryCount = 0;
            bool done = false;

            while (tryCount < 5)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(file.Url);
                    request.ServicePoint.Expect100Continue = false;
                    request.Proxy = null;
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        // Check that the remote file was found. The ContentType
                        // check is performed since a request for a non-existent
                        // image file might be redirected to a 404-page, which would
                        // yield the StatusCode "OK", even though the image was not
                        // found.
                        if ((response.StatusCode == HttpStatusCode.OK ||
                            response.StatusCode == HttpStatusCode.Moved ||
                            response.StatusCode == HttpStatusCode.Redirect) &&
                            response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
                        {
                            // if the remote file was found, download oit
                            using (Stream inputStream = response.GetResponseStream())
                            using (Stream outputStream = File.OpenWrite(file.Filename))
                            {
                                byte[] buffer = new byte[4096];
                                int bytesRead;
                                do
                                {
                                    bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                                    outputStream.Write(buffer, 0, bytesRead);
                                } while (bytesRead != 0);
                                outputStream.Close();
                                inputStream.Close();
                            }
                        }
                        response.Close();
                        done = true;
                    }
                }
                catch (WebException e)
                {
                    done = false;
                    if(tryCount == 4)
                        throw e;
                }
                if (done)
                    break;

                tryCount++;
            }
        }

        #endregion
    }

    public class AsyncChapterJob : IChapterJob
    {
        public string Name { get; set; }
        public string ChapterUrl { get; set; }

        public string LocalPath { get; set; }

        public List<ImageFile> DownloadList { get; set; }

        public AsyncChapterJob()
        {
            DownloadList = new List<ImageFile>();
        }

        #region Methods

        #endregion
    }
}
