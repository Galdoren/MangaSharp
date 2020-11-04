using Manga.Core;
using Manga.Core.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;

namespace Manga.Data
{
    /// <summary>
    /// Entity Framework Repository
    /// </summary>
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        #region Fields

        private readonly IDbContext _context;
        private IDbSet<T> _entities;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public EfRepository(IDbContext context)
        {
            this._context = context;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IQueryable<T> Table
        {
            get { return this.Entities; }
        }


        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        public virtual IQueryable<T> TableNoTracking
        {
            get { return this.Entities.AsNoTracking(); }
        }

        /// <summary>
        /// Entities
        /// </summary>
        protected virtual IDbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<T>();
                return _entities;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public T GetById(object id)
        {
            //see some suggested performance optimization (not tested)
            //http://stackoverflow.com/questions/11686225/dbset-find-method-ridiculously-slow-compared-to-singleordefault-on-id/11688189#comment34876113_11688189
            return this.Entities.Find(id);
        }

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public void Insert(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this.Entities.Add(entity);
                this._context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                //use stringbuilder to prevent memory leak
                var builder = new StringBuilder();

                foreach (var validationErrors in e.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        builder.AppendFormat("Property: {0} Error: {1}{2}", validationError.PropertyName, validationError.ErrorMessage, Environment.NewLine);

                var fail = new Exception(builder.ToString(), e);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public void Update(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                //use stringbuilder to prevent memory leak
                var builder = new StringBuilder();

                foreach (var validationErrors in e.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        builder.AppendFormat("{0}Property: {1} Error: {2}", Environment.NewLine, validationError.PropertyName, validationError.ErrorMessage);

                var fail = new Exception(builder.ToString(), e);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public void Delete(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this.Entities.Remove(entity);
            }
            catch (DbEntityValidationException e)
            {
                //use stringbuilder to prevent memory leak
                var builder = new StringBuilder();

                foreach (var validationErrors in e.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        builder.AppendFormat("{0}Property: {1} Error: {2}", Environment.NewLine, validationError.PropertyName, validationError.ErrorMessage);

                var fail = new Exception(builder.ToString(), e);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        public void Insert(IEnumerable<T> entities)
        {
            var acd = ((MObjectContext)_context).Configuration.AutoDetectChangesEnabled;
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                // speed hack
                ((MObjectContext)_context).Configuration.AutoDetectChangesEnabled = false;
                var i = 0;
                foreach (var entity in entities)
                {
                    this.Entities.Add(entity);
                    if (i % 50 == 0)
                        this._context.SaveChanges();
                    i++;
                }
                this._context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                //use stringbuilder to prevent memory leak
                var builder = new StringBuilder();

                foreach (var validationErrors in e.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        builder.AppendFormat("Property: {0} Error: {1}{2}", validationError.PropertyName, validationError.ErrorMessage, Environment.NewLine);

                var fail = new Exception(builder.ToString(), e);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
            finally
            {
                ((MObjectContext)_context).Configuration.AutoDetectChangesEnabled = acd;
            }
        }

        #endregion
    }
}
