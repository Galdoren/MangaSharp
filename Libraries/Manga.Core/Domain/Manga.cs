using System;
using System.Collections.Generic;

namespace Manga.Core.Domain
{
    public class Manga : BaseEntity, IEquatable<Manga>
    {
        public Manga()
        {
            Chapters = new List<Chapter>();
        }

        /// <summary>
        /// Gets or sets Name of the manga
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets Link of the manga
        /// </summary>
        public String URL { get; set; }

        /// <summary>
        /// Gets or sets Summary of manga
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Gets or sets the genres of manga
        /// </summary>
        public IList<String> Genres { get; set; }

        /// <summary>
        /// Gets or sets the artist of the manga
        /// </summary>
        public String Artist { get; set; }

        /// <summary>
        /// Gets or sets the author of manga
        /// </summary>
        public String Author { get; set; }

        /// <summary>
        /// Gets or sets the release year of manga
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Gets or sets the alternative name of manga
        /// </summary>
        public String AlternativeName { get; set; }

        /// <summary>
        /// Gets or sets the status of manga
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets if the manga is in favourites list
        /// </summary>
        public bool IsFavourite { get; set; }

        /// <summary>
        /// Gets or sets the number of chapters in manga
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the publisher identifier
        /// </summary>
        public int PublisherId { get; set; }

        /// <summary>
        /// Gets or sets the cover image path
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the creation date
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the last update date
        /// </summary>
        public DateTime LastUpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the chapters
        /// </summary>
        public virtual ICollection<Chapter> Chapters { get; set; }

        /// <summary>
        /// Gets or sets the publisher
        /// </summary>
        public virtual Publisher Publisher { get; set; }

        public bool Equals(Manga other)
        {
            return Name.Equals(other.Name);
        }
    }

    public class MangaNameComparer : IComparer<Manga>
    {
        public int Compare(Manga x, Manga y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }

    public class MangaNameEqualityComparer : IEqualityComparer<Manga>
    {
        public bool Equals(Manga x, Manga y)
        {
            return x.Name.Equals(y.Name);
        }

        public int GetHashCode(Manga obj)
        {
            return obj.Name.GetHashCode();
        }
    }

    public class MangaUrlEqualityComparer : IEqualityComparer<Manga>
    {
        public bool Equals(Manga x, Manga y)
        {
            return x.URL.Equals(y.URL);
        }

        public int GetHashCode(Manga obj)
        {
            return obj.URL.GetHashCode();
        }
    }
}
