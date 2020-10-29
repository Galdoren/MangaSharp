using Caliburn.Micro;
using Manga.Core.Domain;
using Manga.Services.Net;
using Manga.Services.Publishers;
using MangaSharp.Infrastructure;
using MangaSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MangaSharp.ViewModels
{
    [ViewModelLocation("Manga Window")]
    public class ChaptersViewModel : Screen, IViewModel
    {
        #region Fields

        private readonly IPublisherService _publisherService;
        private readonly IPublisherWebService _publisherWebService;
        private readonly IDownloadManager _downloadManager;
        private readonly MangaModel _model;

        private CollectionViewSource _itemsSource;

        #endregion

        #region Ctor

        public ChaptersViewModel(IPublisherService publisherService, IPublisherWebService publisherWebService, IDownloadManager downloadManager, MangaModel model)
        {
            this._publisherService = publisherService;
            this._publisherWebService = publisherWebService;
            this._downloadManager = downloadManager;
            this._model = model;

            this._itemsSource = new CollectionViewSource();
            this._itemsSource.Source = _model.Chapters;

            this.DisplayName = "CHAPTERS";
        }

        #endregion

        #region Properties

        public int Order
        {
            get { return 1; }
        }

        public MangaModel Model
        {
            get { return _model; }
        }

        public CollectionViewSource ItemsSource
        {
            get
            {
                if (_itemsSource.Source != _model.Chapters)
                    _itemsSource.Source = _model.Chapters;
                return _itemsSource;
            }
            set
            {
                _itemsSource = value;
                NotifyOfPropertyChange(() => ItemsSource);
            }
        }

        #endregion

        #region Methods

        public bool CanDownload()
        {
            return _model != null && _model.Chapters != null && _model.Chapters.Count > 0;
        }

        public void Download()
        {
            IEnumerable<Chapter> selectedChapters = from c in _model.Chapters
                                                         where c.IsSelected
                                                         orderby c.Index ascending
                                                         select c.Chapter;

            ChapterQueue queue = new ChapterQueue(selectedChapters.Count());
            queue.Manga = Model.Name;
            queue.Progress = new DownloadProgress()
            {
                Progress = 0.0d,
                Status = DownloadStatus.Created,
                StatusText = "Waiting...",
                Title = String.Format("Job #{0}", j + 1)
            };

            
        }

        #endregion

        
    }
}
