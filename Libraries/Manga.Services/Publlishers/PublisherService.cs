using Manga.Core.Data;
using Manga.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manga.Services.Publishers
{
    public class PublisherService : IPublisherService
    {
        #region Fields

        private readonly IRepository<Publisher> _publisherRepository;

        #endregion

        #region Ctor

        public PublisherService(IRepository<Publisher> publisherRepository)
        {
            this._publisherRepository = publisherRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all publishers
        /// </summary>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public virtual IList<Publisher> GetAllPublishers(bool loadOnlyActive = true)
        {
            var query = _publisherRepository.Table;

            if (loadOnlyActive)
                query = query.Where(p => p.IsActive == true);
            
            query = query.OrderBy(p => p.Name);
            return query.ToList();            
        }

        /// <summary>
        /// Gets a publisher by identifier
        /// </summary>
        /// <param name="publisherId">Publisher identifier</param>
        /// <returns>Publisher</returns>
        public virtual Publisher GetPublisherById(int publisherId)
        {
            if (publisherId == 0)
                return null;

            return _publisherRepository.GetById(publisherId);
        }

        public virtual Publisher GetPublisherByName(string publisherName)
        {
            if (publisherName == null)
                throw new ArgumentNullException("publisherName");

            var query = _publisherRepository.Table;
            return query.Where(p => p.Name.Equals(publisherName)).FirstOrDefault();
        }

        /// <summary>
        /// Insert a publisher
        /// </summary>
        /// <param name="publisher">Publisher</param>
        public virtual void InsertPublisher(Publisher publisher)
        {
            if (publisher == null)
                throw new ArgumentNullException("publisher");

            _publisherRepository.Insert(publisher);            
        }

        /// <summary>
        /// Updates a publisher
        /// </summary>
        /// <param name="publisher">Publisher</param>
        public virtual void UpdatePublisher(Publisher publisher)
        {
            if (publisher == null)
                throw new ArgumentNullException("publisher");

            _publisherRepository.Update(publisher);
        }

        /// <summary>
        /// Deletes a publisher
        /// </summary>
        /// <param name="publisher">Publisher</param>
        public virtual void DeletePublisher(Publisher publisher)
        {
            if (publisher == null)
                throw new ArgumentNullException("publisher");

            _publisherRepository.Delete(publisher);
        }

        #endregion
    }
}
