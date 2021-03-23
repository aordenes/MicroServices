using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Servicios.api.Libreria.Core;
using Servicios.api.Libreria.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Servicios.api.Libreria.Repository
{
    public class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : IDocument
    {
        private readonly IMongoCollection<TDocument> _collection;

        public MongoRepository(IOptions<MongoSettings> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            var db = client.GetDatabase(options.Value.Database);

            _collection = db.GetCollection<TDocument>(GetcollectionName(typeof(TDocument)));
        }

        /// <summary>
        ///  retorna el nombre de la coleecion solo pasandole el documenttype
        /// </summary>
        /// <param name="documentType">nombre de la collection en formato string</param>
        /// <returns></returns>
        private protected string GetcollectionName(Type documentType)
        {
            return ((BsonCollectionAtribute)documentType.GetCustomAttributes(typeof(BsonCollectionAtribute), true).FirstOrDefault()).CollectionName; 
        }

        /// <summary>
        /// todos los registros de la coleccion
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<TDocument>> GetAll()
        {
            return await _collection.Find(a=> true).ToListAsync();
        }

        #region CRUD

        public async Task<TDocument> GetById(string Id)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, Id);
            return await _collection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task InsertDocument(TDocument document)
        {
            await _collection.InsertOneAsync(document);
        }

        public async Task UpdateDocument(TDocument document)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
            await _collection.FindOneAndReplaceAsync(filter, document);
        }

        public async Task DeleteById(string Id)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, Id);
            await _collection.FindOneAndDeleteAsync(filter);
        }

        /// <summary>
        /// Metodo encargado de la paginacion respecto a la data obtenida.
        /// </summary>
        /// <param name="filterExpression"> Este metodo trabjara con Expresion, esto me permite definir una funcionalidad, de un metodo que ha futuro sera implementado.</param>
        /// <param name="pagination">entidad de la paginacion</param>
        /// <returns></returns>
        public async Task<PaginationEntity<TDocument>> PaginationBy(Expression<Func<TDocument, bool>> filterExpression, PaginationEntity<TDocument> pagination)
        {
            var sort = Builders<TDocument>.Sort.Ascending(pagination.Sort);

            if (pagination.SortDirection.Equals("desc"))
            {
                sort = Builders<TDocument>.Sort.Descending(pagination.Sort);
            }
            // si no tengo filtros devulevo la data
            if (string.IsNullOrEmpty(pagination.Filter))
            {
                pagination.Data = await _collection.Find(a => true)
                                  .Sort(sort)
                                  .Skip((pagination.Page - 1) * pagination.PageSize)//desde que psosicion quiero contar  
                                  .Limit(pagination.PageSize)//cuanto elementos quiero extraer
                                  .ToListAsync();
            }
            else
            {
                pagination.Data = await _collection.Find(filterExpression)
                                   .Sort(sort)
                                   .Skip((pagination.Page - 1) * pagination.PageSize)//desde que psosicion quiero contar  
                                   .Limit(pagination.PageSize)//cuanto elementos quiero extraer
                                   .ToListAsync();
            }

            long totalDocument = await _collection.CountDocumentsAsync(FilterDefinition<TDocument>.Empty);//obtener todos los records de esta coleccion
            var totalPages = (int)Math.Ceiling(Convert.ToDecimal(totalDocument / pagination.PageSize));

            pagination.PageQuantity = totalPages;

            return pagination;
               
        }

        /// <summary>
        /// Metodo encargado de la paginacion respecto a la data obtenida. con expresion regular, para cualquier tipo de filtro.
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public async Task<PaginationEntity<TDocument>> PaginationByFilter(PaginationEntity<TDocument> pagination)
        {
            var sort = Builders<TDocument>.Sort.Ascending(pagination.Sort);

            if (pagination.SortDirection.Equals("desc"))
            {
                sort = Builders<TDocument>.Sort.Descending(pagination.Sort);
            }
            // si no tengo filtros devulevo la data
            var totalDocument = 0;
            if (pagination.FilterValue == null)//(pagination.FilterValue.Equals(null))
            {
                pagination.Data = await _collection.Find(a => true)
                                  .Sort(sort)
                                  .Skip((pagination.Page - 1) * pagination.PageSize)//desde que psosicion quiero contar  
                                  .Limit(pagination.PageSize)//cuanto elementos quiero extraer
                                  .ToListAsync();

                totalDocument = (await _collection.Find(a => true).ToListAsync()).Count;
            }
            else
            {
                //se crea un regular expresion busca todos los valores que coincidan con una parte de busqueda del parametro
                var filterValue = ".*" + pagination.FilterValue.Valor + ".*";
                // recordar que la expresion regular del filtro debe ser agragada a la busqueda en MongoDb, no quiero que se key sensitive (letra i en l asobrecarga del metoso)
                var filter = Builders<TDocument>.Filter.Regex(pagination.FilterValue.Propiedad,  new BsonRegularExpression(filterValue, "i"));
                pagination.Data = await _collection.Find(filter)
                                   .Sort(sort)
                                   .Skip((pagination.Page - 1) * pagination.PageSize)//desde que psosicion quiero contar  
                                   .Limit(pagination.PageSize)//cuanto elementos quiero extraer
                                   .ToListAsync();

                totalDocument = (await _collection.Find(filter).ToListAsync()).Count;
            }

            //long totalDocument = await _collection.CountDocumentsAsync(FilterDefinition<TDocument>.Empty);//obtener todos los records de esta coleccion

            var redondeo = Math.Ceiling(totalDocument / Convert.ToDecimal(pagination.PageSize));
            

            var totalPages = (int)(redondeo); //Convert.ToInt32(redondeo);

            pagination.PageQuantity = totalPages;
            pagination.TotalRows = (int)totalDocument;

            return pagination;
        }

        #endregion
    }
}
