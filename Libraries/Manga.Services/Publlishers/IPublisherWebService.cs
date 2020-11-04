using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Manga.Services.Publishers
{
    public interface IPublisherWebService
    {
        /// <summary>
        /// Fetches the manga list from publisher website
        /// </summary>
        /// <returns></returns>
        Task<IList<Manga.Core.Domain.Manga>> GetList();

        /// <summary>
        /// Fetches the manga list from publisher website
        /// </summary>
        /// <returns></returns>
        Task<IList<Manga.Core.Domain.Manga>> Update();

        /// <summary>
        /// Fetches the manga details from publisher website
        /// </summary>
        /// <param name="manga">Manga</param>
        /// <param name="detailsLevel">Manga details level</param>
        Task GetDetails(Manga.Core.Domain.Manga manga, MangaDetailsLevel detailsLevel);

        /// <summary>
        /// Downloads the chapter from publisher website
        /// </summary>
        /// <param name="chapters">Chapters</param>
        /// <param name="options">Download Options</param>
        Task Download(IList<Manga.Core.Domain.Chapter> chapters, DownloadOptions options);
    }

    public enum MangaDetailsLevel
    {
        None,
        Minimal,
        Extended
    };

    public struct DownloadOptions
    {
        public Guid DownloadGuid { get; set; }
        public bool OverwriteIfExists { get; set; }
        public string Path { get; set; }
        //public DownloadProgress Progress { get; set; }
    }
}
