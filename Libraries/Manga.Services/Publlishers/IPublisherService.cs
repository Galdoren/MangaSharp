using Manga.Core.Domain;
using System.Collections.Generic;

namespace Manga.Services.Publishers
{
    public interface IPublisherService
    {
        /// <summary>
        /// Gets all publishers
        /// </summary>
        /// <param name="isActive"></param>
        /// <returns></returns>
        IList<Publisher> GetAllPublishers(bool loadOnlyActive = true);

        /// <summary>
        /// Gets a publisher by identifier
        /// </summary>
        /// <param name="publisherId">Publisher identifier</param>
        /// <returns>Publisher</returns>
        Publisher GetPublisherById(int publisherId);

        /// <summary>
        /// Gets a publisher by it's name
        /// </summary>
        /// <param name="publisherName">Publisher name</param>
        /// <returns>Publisher</returns>
        Publisher GetPublisherByName(string publisherName);

        /// <summary>
        /// Insert a publisher
        /// </summary>
        /// <param name="publisher">Publisher</param>
        void InsertPublisher(Publisher publisher);

        /// <summary>
        /// Updates a publisher
        /// </summary>
        /// <param name="publisher">Publisher</param>
        void UpdatePublisher(Publisher publisher);

        /// <summary>
        /// Deletes a publisher
        /// </summary>
        /// <param name="publisher">Publisher</param>
        void DeletePublisher(Publisher publisher);
    }
}
