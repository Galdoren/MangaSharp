using Caliburn.Micro;
using Manga.Services.Publishers;
using MangaSharp.Infrastructure;
using MangaSharp.Models;

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
