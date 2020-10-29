using System.Collections.Generic;

namespace Manga.Services.Chapters
{
    public interface IChapterService
    {
        /// <summary>
        /// Gets all chapters
        /// </summary>
        /// <param name="mangaId">Manga identifier</param>
        /// <returns></returns>
        IList<Manga.Core.Domain.Chapter> GetAllChapters(int mangaId);

        /// <summary>
        /// Gets a manga by identifier
        /// </summary>
        /// <param name="mangaId">Chapter identifier</param>
        /// <returns>Chapter</returns>
        Manga.Core.Domain.Chapter GetChapterById(int chapterId);

        /// <summary>
        /// Insert a chapter
        /// </summary>
        /// <param name="chapter">Chapter</param>
        void InsertChapter(Manga.Core.Domain.Chapter chapter);

        /// <summary>
        /// Updates a chapter
        /// </summary>
        /// <param name="chapter">Chapter</param>
        void UpdateChapter(Manga.Core.Domain.Chapter chapter);

        /// <summary>
        /// Deletes a chapter
        /// </summary>
        /// <param name="chapter">Chapter</param>
        void DeleteChapter(Manga.Core.Domain.Chapter chapter);
    }
}
