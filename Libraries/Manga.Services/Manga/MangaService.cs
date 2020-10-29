using Manga.Core.Data;
using Manga.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Manga.Services.Mangas
{
    public partial class MangaService : IMangaService
    {
        #region Fields

        private readonly IRepository<Manga.Core.Domain.Manga> _mangaRepository;
        private readonly IDbContext _dbContext;
        private readonly IDataProvider _dataProvider;

        #endregion

        #region Ctor

        public MangaService(IRepository<Manga.Core.Domain.Manga> mangaRepository,
            IDataProvider dataProvider, IDbContext dbContext)
        {
            this._mangaRepository = mangaRepository;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all manga
        /// </summary>
        /// <param name="publisherId">Publisher identifier</param>
        public IList<Core.Domain.Manga> GetAllManga(int publisherId = 0)
        {
            var query = _mangaRepository.Table;

            if(publisherId > 0)
                query = query.Where(m => m.PublisherId == publisherId);
            query = query.OrderBy(p => p.Name);
            return query.ToList();
        }

        /// <summary>
        /// Gets a manga by identifier
        /// </summary>
        /// <param name="mangaId">Manga identifier</param>
        public Core.Domain.Manga GetMangaById(int mangaId)
        {
            if (mangaId == 0)
                return null;

            return _mangaRepository.GetById(mangaId);
        }

        /// <summary>
        /// Insert a collection of manga
        /// </summary>
        /// <param name="list">List</param>
        public void InsertManga(IList<Core.Domain.Manga> list)
        {
            if(list == null)
                throw new ArgumentNullException("list");
            
            using (var transaction = _dbContext.BeginTransaction())
            {
                try
                {
                    using (var command = _dataProvider.GetCommand())
                    {
                        String sql = @"INSERT INTO [Manga] ([Name], [Url], [PublisherId], [Status], [Year], [IsFavourite], [Size], [CreatedDate], [LastUpdatedDate]) 
                                        VALUES (@name, @url, @publisherId, @status, @year, @favourite, @size, @createdAt, @updatedAt);";

                        command.Transaction = transaction.UnderlyingTransaction;
                        command.Connection = transaction.UnderlyingTransaction.Connection;
                        command.CommandText = sql;

                        // prepare parameters
                        var pName = _dataProvider.GetParameter();
                        pName.ParameterName = "name";
                        pName.DbType = System.Data.DbType.String;

                        var pUrl = _dataProvider.GetParameter();
                        pUrl.ParameterName = "url";
                        pUrl.DbType = System.Data.DbType.String;

                        var pPublisherId = _dataProvider.GetParameter();
                        pPublisherId.ParameterName = "publisherId";
                        pPublisherId.DbType = System.Data.DbType.Int32;

                        var pStatus = _dataProvider.GetParameter();
                        pStatus.ParameterName = "status";
                        pStatus.DbType = System.Data.DbType.Int32;

                        var pYear = _dataProvider.GetParameter();
                        pYear.ParameterName = "year";
                        pYear.DbType = System.Data.DbType.Int32;

                        var pIsFavourite = _dataProvider.GetParameter();
                        pIsFavourite.ParameterName = "favourite";
                        pIsFavourite.DbType = System.Data.DbType.Boolean;

                        var pSize = _dataProvider.GetParameter();
                        pSize.ParameterName = "size";
                        pSize.DbType = System.Data.DbType.Int32;

                        var pCreatedDate = _dataProvider.GetParameter();
                        pCreatedDate.ParameterName = "createdAt";
                        pCreatedDate.DbType = System.Data.DbType.DateTime;

                        var pUpdatedDate = _dataProvider.GetParameter();
                        pUpdatedDate.ParameterName = "updatedAt";
                        pUpdatedDate.DbType = System.Data.DbType.DateTime;

                        command.Parameters.Add(pName);
                        command.Parameters.Add(pUrl);
                        command.Parameters.Add(pPublisherId);
                        command.Parameters.Add(pYear);
                        command.Parameters.Add(pIsFavourite);
                        command.Parameters.Add(pSize);
                        command.Parameters.Add(pStatus);
                        command.Parameters.Add(pCreatedDate);
                        command.Parameters.Add(pUpdatedDate);

                        for (int i = 0; i < list.Count; i++ )
                        {
                            pName.Value = list[i].Name;
                            pUrl.Value = list[i].URL;
                            pPublisherId.Value = list[i].PublisherId;
                            pYear.Value = list[i].Year;
                            pIsFavourite.Value = list[i].IsFavourite;
                            pSize.Value = list[i].Size;
                            pStatus.Value = list[i].Status;
                            pCreatedDate.Value = list[i].CreatedDate;
                            pUpdatedDate.Value = list[i].LastUpdatedDate;

                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }
                catch
                {
                    transaction.Rollback();
                }
            }
            
            //_mangaRepository.Insert(list);
            return;
        }

        /// <summary>
        /// Insert a manga
        /// </summary>
        /// <param name="manga">Manga</param>
        public void InsertManga(Core.Domain.Manga manga)
        {
            if (manga == null)
                throw new ArgumentNullException("manga");

            _mangaRepository.Insert(manga);
        }

        /// <summary>
        /// Updates a manga
        /// </summary>
        /// <param name="manga">Manga</param>
        public void UpdateManga(Core.Domain.Manga manga)
        {
            if (manga == null)
                throw new ArgumentNullException("manga");

            _mangaRepository.Update(manga);
        }

        /// <summary>
        /// Deletes a manga
        /// </summary>
        /// <param name="manga">Manga</param>
        public void DeleteManga(Core.Domain.Manga manga)
        {
            if (manga == null)
                throw new ArgumentNullException("manga");

            _mangaRepository.Delete(manga);
        }

        #endregion
    }
}
