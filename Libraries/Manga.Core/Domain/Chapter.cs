using System;
using System.Collections.Generic;

namespace Manga.Core.Domain
{
    /// <summary>
    /// Chapter is a class which holds the chapter information, It can be compared with other Chapters.
    /// </summary>
    public class Chapter : BaseEntity
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the Chapter
        /// </summary>
        public String Name { get; set; }
        
        /// <summary>
        /// Gets or sets the address of the Chapter
        /// </summary>
        public String Url { get; set; }

        /// <summary>
        /// Gets or sets the index of chapter in manga
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Size of the chapter
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the flag which indicated the chapter is downloaded
        /// </summary>
        public bool IsDownloaded { get; set; }

        /// <summary>
        /// Gets or sets the download path
        /// </summary>
        public String DownloadPath { get; set; }

        /// <summary>
        /// Gets or sets the downloaded image count in the download path folder
        /// </summary>
        public int DownloadedImageCount { get; set; }

        /// <summary>
        /// Release date of the chapter
        /// </summary>
        public DateTime Date { get; set; }        

        /// <summary>
        /// Gets or sets the manga foreign identifier
        /// </summary>
        public int MangaId { get; set; }

        /// <summary>
        /// Gets or sets the manga this chapter belongs to
        /// </summary>
        public virtual Manga Manga { get; set; }

        #endregion
    }

    /// <summary>
    /// Compares chapters based on their names
    /// </summary>
    public class ChapterNameComparer : IComparer<Chapter>
    {
        public int Compare(Chapter x, Chapter y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }

    /// <summary>
    /// Compares chapters based on their index
    /// </summary>
    public class ChapterIndexComparer : IComparer<Chapter>
    {
        public int Compare(Chapter x, Chapter y)
        {
            return x.Index.CompareTo(y.Index);
        }
    }

    public class ChapterUrlComparer : IComparer<Chapter>
    {
        public int Compare(Chapter x, Chapter y)
        {
            return x.Url.CompareTo(y.Url);
        }
    }

    public class ChapterUrlEqualityComparer : IEqualityComparer<Chapter>
    {
        public bool Equals(Chapter x, Chapter y)
        {
            return x.Url.Equals(y.Url);
        }

        public int GetHashCode(Chapter obj)
        {
            return obj.Url.GetHashCode();
        }
    }
}
