using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Manga.Core.Infrastructure;
using Manga.Services.Publishers;
using MangaSharp.Infrastructure;
using MangaSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaSharp.ViewModels
{
    [ViewModelLocation("Manga Window")]
    public class DetailsViewModel : Screen, IViewModel
    {
        #region Fields

        private readonly IPublisherService _publisherService;
        private readonly IPublisherWebService _publisherWebService;
        private readonly MangaModel _model;

        #endregion

        #region Ctor

        public DetailsViewModel(IPublisherService publisherService, IPublisherWebService publisherWebService, MangaModel model)
        {
            this._publisherService = publisherService;
            this._publisherWebService = publisherWebService;
            this._model = model;

            this.DisplayName = "DETAILS";
        }

        #endregion

        #region Properties

        public int Order
        {
            get { return 0; }
        }

        public MangaModel Model
        {
            get { return _model; }
        }

        #endregion        

        #region Methods

        public bool CanDownload()
        {
            return Model != null && Model.Chapters != null && Model.Chapters.Count > 0;
        }

        public void Download()
        {

        }

        #endregion
    }
}
