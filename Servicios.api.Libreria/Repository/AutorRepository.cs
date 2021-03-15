using MongoDB.Driver;
using Servicios.api.Libreria.Core.ContextMongoDb;
using Servicios.api.Libreria.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicios.api.Libreria.Repository
{
    public class AutorRepository : IAutorRepository
    {
        private readonly IAutorContext _autorContext;
        public AutorRepository(IAutorContext autorContext)
        {
            _autorContext = autorContext;
        }

        /// <summary>
        /// Se crea con el fin para que estas operaciones sean independeintes de otros proceso que sean idenpendeintes de estos proceso, no detienen otras operacioes de otros programas.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Autor>> GetAutores()
        {
            return await _autorContext.Autores.Find(a => true).ToListAsync();
        }
    }
}
