using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Manga.Core;
using Manga.Core.Domain;
using Manga.Core.Infrastructure;
using Manga.Services.Publishers;
using MangaSharp.Infrastructure;
using MangaSharp.Extensions;
using MangaSharp.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MangaSharp.ViewModels
{
    [ViewModelLocation("Main Window")]
    public class CatalogViewModel : Screen, IViewModel
    {
        #region Fields

        private String _searchText;

        private Publisher _selectedPublisher;
        private BindableCollection<Manga.Core.Domain.Manga> _items;
        private CollectionViewSource _mangaCatalog;
        private Manga.Core.Domain.Manga _selectedManga;

        private readonly IWindowManager _windowManager;
        private readonly IPublisherService _publisherService;
        private readonly Func<IPublisherWebService, MangaModel, MangaViewModel> _mangaViewModel;

        #endregion

        #region Ctor

        public CatalogViewModel(IWindowManager windowManager,
            IPublisherService publisherService, Func<IPublisherWebService, MangaModel, MangaViewModel> mangaViewModel)
            : base()
        {
            this.DisplayName = "CATALOG";
            this._items = new BindableCollection<Manga.Core.Domain.Manga>();
            this._mangaCatalog = new CollectionViewSource();
            this._mangaCatalog.Source = Items;

            this._mangaCatalog.Filter += _mangaCatalog_Filter;

            this._publisherService = publisherService;
            this._windowManager = windowManager;
            this._mangaViewModel = mangaViewModel;
        }

        #endregion

        #region Properties

        public int Order
        {
            get { return 1; }
        }

        public String SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                NotifyOfPropertyChange(() => SearchText);
                MangaCatalog.View.Refresh();
            }
        }

        public IEnumerable<Publisher> Publishers
        {
            get { return _publisherService.GetAllPublishers(); }
        }

        public Publisher SelectedPublisher
        {
            get { return _selectedPublisher; }
            set
            {
                if (_selectedPublisher == value)
                    return;
                _selectedPublisher = value;
                PublisherChanged();
                NotifyOfPropertyChange(() => CanRefreshList);
            }
        }

        public BindableCollection<Manga.Core.Domain.Manga> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                NotifyOfPropertyChange(() => Items);
            }
        }

        public CollectionViewSource MangaCatalog
        {
            get
            {
                if (_mangaCatalog.Source != Items)
                    _mangaCatalog.Source = Items;
                return _mangaCatalog;
            }
            set
            {
                _mangaCatalog = value;
                NotifyOfPropertyChange(() => MangaCatalog);
            }
        }

        public Manga.Core.Domain.Manga SelectedManga
        {
            get { return _selectedManga; }
            set
            {
                _selectedManga = value;
                NotifyOfPropertyChange(() => SelectedManga);
            }
        }

        public bool CanRefreshList
        {
            get
            {
                return _selectedPublisher != null;
            }
        }


        #endregion

        #region Methods

        private void _mangaCatalog_Filter(object sender, FilterEventArgs e)
        {
            if (String.IsNullOrEmpty(SearchText))
                e.Accepted = true;
            else
            {
                var manga = e.Item as Manga.Core.Domain.Manga;
                
                if (manga.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    e.Accepted = true;
                else
                    e.Accepted = false;                
            }
        }

        private void PublisherChanged()
        {
            var window = App.Current.MainWindow as MetroWindow;
            var controllerTask = window.ShowProgressAsync("Please Wait...", String.Format("Loading catalog from {0}", _selectedPublisher.Name));

            Task.Run(async () =>
            {
                var service = EngineContext.Current.ContainerManager.Resolve<IPublisherWebService>(_selectedPublisher.Name);
                var list = await service.GetList();

                Items.Clear();
                Items.AddRange(list);
                Items.Refresh();
                var controller = await controllerTask;
                await controller.CloseAsync();
            });
        }

        public void OpenDetails(object obj)
        {
            var publisherWebservice = EngineContext.Current.ContainerManager.Resolve<IPublisherWebService>(_selectedPublisher.Name);
            /*
            var viewModel = EngineContext.Current.ContainerManager.Resolve<MangaViewModel>("", null,
                new NamedParameter("publisherWebService", publisherWebservice),
                new NamedParameter("model", new Models.MangaModel(SelectedManga)));
            */
            var viewModel = _mangaViewModel(publisherWebservice, SelectedManga.ToModel());

            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.Manual;

            _windowManager.ShowWindow(viewModel, null, settings);
        }

        public void RefreshList()
        {
            var window = App.Current.MainWindow as MetroWindow;
            var controllerTask = window.ShowProgressAsync("Please Wait...", String.Format("Updating catalog for {0}", _selectedPublisher.Name));

            Task.Run(async () =>
            {
                var service = EngineContext.Current.ContainerManager.Resolve<IPublisherWebService>(_selectedPublisher.Name);
                var list = await service.Update();

                Items.Clear();
                Items.AddRange(list);
                Items.Refresh();
                var controller = await controllerTask;
                await controller.CloseAsync();
            });
        }

        #endregion
    }
}
