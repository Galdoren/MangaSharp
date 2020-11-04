using System.Collections.Generic;

namespace Manga.Services.Mangas
{
    public interface IMangaService
    {
        /// <summary>
        /// Gets all manga
        /// </summary>
        /// <param name="publisherId">Publisher identifier</param>
        /// <returns></returns>
        IList<Manga.Core.Domain.Manga> GetAllManga(int publisherId);

        /// <summary>
        /// Gets a manga by identifier
        /// </summary>
        /// <param name="mangaId">Manga identifier</param>
        /// <returns>Manga</returns>
        Manga.Core.Domain.Manga GetMangaById(int mangaId);


        void InsertManga(IList<Manga.Core.Domain.Manga> list);

        /// <summary>
        /// Insert a manga
        /// </summary>
        /// <param name="manga">Manga</param>
        void InsertManga(Manga.Core.Domain.Manga manga);

        /// <summary>
        /// Updates a manga
        /// </summary>
        /// <param name="manga">Manga</param>
        void UpdateManga(Manga.Core.Domain.Manga manga);

        /// <summary>
        /// Deletes a manga
        /// </summary>
        /// <param name="manga">Manga</param>
        void DeleteManga(Manga.Core.Domain.Manga manga);
    }
}
