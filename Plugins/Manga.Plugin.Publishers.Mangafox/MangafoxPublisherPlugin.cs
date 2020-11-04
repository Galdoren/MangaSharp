using Manga.Core.Domain;
using Manga.Core.Plugins;
using Manga.Services.Chapters;
using Manga.Services.Mangas;
using Manga.Services.Publishers;

namespace Manga.Plugin.Publishers.Mangafox
{
    /// <summary>
    /// Mangafox publisher plugin
    /// </summary>
    public class MangafoxPublisherPlugin : BasePlugin
    {
        #region Constants

        private const string GETLIST_URL = "http://mangafox.me/manga/";

        #endregion

        #region Fields

        private readonly IPublisherService _publisherService;
        private readonly IMangaService _mangaService;
        private readonly IChapterService _chapterService;

        #endregion

        #region Ctor

        public MangafoxPublisherPlugin(IPublisherService publisherService,
            IMangaService mangaService,
            IChapterService chapterService)
        {
            _publisherService = publisherService;
            _mangaService = mangaService;
            _chapterService = chapterService;
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
                Name = "Mangafox",
                BaseUrl = "http://mangafox.me/",
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
            var publisher = _publisherService.GetPublisherByName("Mangafox");
            _publisherService.DeletePublisher(publisher);

            base.Uninstall();
        }

        #endregion
    }
}
