using Manga.Core.Domain;
using Manga.Core.Plugins;
using Manga.Services.Chapters;
using Manga.Services.Mangas;
using Manga.Services.Publishers;

namespace Manga.Plugin.Publishers.Manganelo
{
    /// <summary>
    /// Mangafox publisher plugin
    /// </summary>
    public class ManganeloPublisherPlugin : BasePlugin
    {
        #region Constants

        private const string GETLIST_URL = "https://manganelo.com/";

        #endregion

        #region Fields

        private readonly IPublisherService _publisherService;
        private readonly IMangaService _mangaService;
        private readonly IChapterService _chapterService;

        #endregion

        #region Ctor

        public ManganeloPublisherPlugin(IPublisherService publisherService,
            IMangaService mangaService,
            IChapterService chapterService)
        {
            this._publisherService = publisherService;
            this._mangaService = mangaService;
            this._chapterService = chapterService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            var publisher = new Publisher()
            {
                Name = "Manganelo",
                BaseUrl = "https://manganelo.com/",
                IsActive = true
            };
            _publisherService.InsertPublisher(publisher);

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            var publisher = _publisherService.GetPublisherByName("Manganelo");
            _publisherService.DeletePublisher(publisher);

            base.Uninstall();
        }

        #endregion
    }
}
