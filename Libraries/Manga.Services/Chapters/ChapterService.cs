using Manga.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Manga.Services.Chapters
{
    public partial class ChapterService : IChapterService
    {
        #region Fields

        private readonly IRepository<Manga.Core.Domain.Chapter> _chapterRepository;

        #endregion

        #region Ctor

        public ChapterService(IRepository<Manga.Core.Domain.Chapter> chapterRepository)
        {
            this._chapterRepository = chapterRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all chapters
        /// </summary>
        /// <param name="mangaId">Manga identifier</param>
        /// <returns></returns>
        public IList<Core.Domain.Chapter> GetAllChapters(int mangaId)
        {
            var query = _chapterRepository.Table;

            if (mangaId > 0)
                query = query.Where(c => c.MangaId == mangaId);
            query = query.OrderBy(p => p.Index);
            return query.ToList();
        }

        /// <summary>
        /// Gets a manga by identifier
        /// </summary>
        /// <param name="mangaId">Chapter identifier</param>
        /// <returns>Chapter</returns>
        public Core.Domain.Chapter GetChapterById(int chapterId)
        {
            if (chapterId == 0)
                return null;

            return _chapterRepository.GetById(chapterId);
        }

        /// <summary>
        /// Insert a chapter
        /// </summary>
        /// <param name="chapter">Chapter</param>
        public void InsertChapter(Core.Domain.Chapter chapter)
        {
            if (chapter == null)
                throw new ArgumentNullException("chapter");

            _chapterRepository.Insert(chapter);
        }

        /// <summary>
        /// Updates a chapter
        /// </summary>
        /// <param name="chapter">Chapter</param>
        public void UpdateChapter(Core.Domain.Chapter chapter)
        {
            if (chapter == null)
                throw new ArgumentNullException("chapter");

            _chapterRepository.Update(chapter);
        }

        /// <summary>
        /// Deletes a chapter
        /// </summary>
        /// <param name="chapter">Chapter</param>
        public void DeleteChapter(Core.Domain.Chapter chapter)
        {
            if (chapter == null)
                throw new ArgumentNullException("chapter");

            _chapterRepository.Delete(chapter);
        }

        #endregion
    }
}
