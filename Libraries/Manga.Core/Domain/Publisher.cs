namespace Manga.Core.Domain
{
    public class Publisher : BaseEntity
    {

        /// <summary>
        /// Gets or sets the publisher name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the base address
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the active flag
        /// </summary>
        public bool IsActive { get; set; }
    }
}
