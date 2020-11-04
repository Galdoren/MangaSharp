using MangaSharp.Models;
using System.Collections.Generic;

namespace MangaSharp.Extensions
{
    public static class MappingExtensions
    {
        public static MangaModel ToModel(this Manga.Core.Domain.Manga entity)
        {
            if (entity == null)
                return null;

            var model = new MangaModel(entity);
            return model;
        }

        public static IList<MangaModel> ToModel(this IList<Manga.Core.Domain.Manga> entityCollection)
        {
            IList<MangaModel> list = new List<MangaModel>();
            if (entityCollection != null)
            {
                for (var i = 0; i < entityCollection.Count; i++ )
                {
                    list.Add(new MangaModel(entityCollection[i]));
                }
            }
            return list;
        }

        public static IList<ChapterModel> ToModel(this ICollection<Manga.Core.Domain.Chapter> entityCollection, MangaModel manga)
        {
            IList<ChapterModel> list = new List<ChapterModel>();
            if(entityCollection != null)
            {
                foreach (var item in entityCollection)
                {
                    list.Add(new ChapterModel(manga, item));
                }
            }
            return list;
        }
    }
}
