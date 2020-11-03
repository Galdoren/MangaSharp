using Autofac.Features.AttributeFilters;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Manga.Core.Infrastructure;
using Manga.Services.Publishers;
using MangaSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MangaSharp.ViewModels
{
    public class MangaViewModel : Conductor<IViewModel>.Collection.OneActive
    {
        #region Fields

        private readonly IPublisherService _publisherService;
        private readonly IPublisherWebService _publisherWebService;

        private MangaModel _model;

        #endregion

        #region Ctor

        public MangaViewModel(IPublisherService publisherService, IPublisherWebService publisherWebService, 
             [MetadataFilter("Location", "Manga Window")] IEnumerable<Func<MangaModel, IPublisherWebService, IViewModel>> tabs,
            MangaModel model)
        {
            this._publisherService = publisherService;
            this._publisherWebService = publisherWebService;
            this._model = model;

            this.DisplayName = String.Format("{0} - {1}", Model.Manga.Publisher.Name, Model.Name);

            foreach (var item in tabs)
	        {
		        this.Items.Add(item(model, publisherWebService));
	        }
            this.Items.OrderBy(a => a.Order);
        }

        #endregion

        #region Properties


        public MangaModel Model
        {
            get { return _model; }
            set
            {
                _model = value;
            }
        }

        #endregion

        #region Methods

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            WindowOpened();
        }

        private void WindowOpened()
        {
            var window = GetView() as MetroWindow;
            var controllerTask = window.ShowProgressAsync("Please Wait...", String.Format("Loading details for {0}", Model.Name));

            Task.Run(async () =>
            {
                var publisherWebService = EngineContext.Current.ContainerManager.Resolve<IPublisherWebService>(_model.Manga.Publisher.Name);
                await publisherWebService.GetDetails(_model.Manga, MangaDetailsLevel.Extended);
                Model.Update();
                var controller = await controllerTask;
                await controller.CloseAsync();
            });
        }

        public void WindowSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            // TODO add normal layout and big layout
        }

        #endregion
    }
}
