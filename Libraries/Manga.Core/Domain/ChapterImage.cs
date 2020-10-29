using System;

namespace Manga.Core.Domain
{
    /// <summary>
    /// Represents the image of a chapter for storage in database
    /// </summary>
    public class ChapterImage : BaseEntity, IComparable<ChapterImage>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the address of the chapter image
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the index of the image in chapter
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Get os sets the link type identifer
        /// </summary>
        public int LinkTypeId { get; set; }

        /// <summary>
        /// Gets or sets the link type
        /// </summary>
        public LinkType LinkType
        {
            get { return (LinkType)LinkTypeId; }
            set { this.LinkTypeId = (int)value; }
        }

        /// <summary>
        /// Gets or sets the chapter foreign identifier
        /// </summary>
        public int ChapterId { get; set; }

        /// <summary>
        /// Gets or sets the chapter
        /// </summary>
        public virtual Chapter Chapter { get; set; }

        #endregion

        #region Comparer

        public int CompareTo(ChapterImage other)
        {
            if (other == null)
                throw new ArgumentNullException("other");
            
            return Index.CompareTo(other.Index);
        }

        #endregion
    }

    public enum LinkType : int
    {
        External = 0,
        Local = 1,
    };
}
