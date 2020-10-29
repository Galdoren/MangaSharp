using MangaSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                for (int i = 0; i < entityCollection.Count; i++ )
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
