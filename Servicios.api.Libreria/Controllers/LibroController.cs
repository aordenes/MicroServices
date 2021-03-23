using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Servicios.api.Libreria.Core.Entities;
using Servicios.api.Libreria.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicios.api.Libreria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibroController : ControllerBase
    {
        private readonly IMongoRepository<LibroEntity> _libroRepository;

        public LibroController(IMongoRepository<LibroEntity> libroRepository)
        {
            _libroRepository = libroRepository;
        }

        /// <summary>
        /// Inserta un nuevo Libro
        /// </summary>
        /// <param name="libro"></param>
        /// <returns></returns>
        [HttpPost]        
        public async Task Post(LibroEntity libro)
        {
            await _libroRepository.InsertDocument(libro);
        
        }

        /// <summary>
        /// Obtengo todos los resultados
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public  async Task<ActionResult<IEnumerable<LibroEntity>>> Get()
        {
           return Ok( await _libroRepository.GetAll());
        }


        [HttpPost("pagination")]
        public async Task<ActionResult<PaginationEntity<LibroEntity>>> PostPagination(PaginationEntity<LibroEntity> paginationEntity)
        {
            //var result = await _autorGenercioRepository.PaginationBy(filtro => filtro.Nombre == paginationEntity.Filter,
            //                                                         paginationEntity); Se cambia por el nuevo filtro creado.

            var result = await _libroRepository.PaginationByFilter(paginationEntity);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LibroEntity>> GetById(string id)
        {
            var book = await _libroRepository.GetById(id);
            return Ok(book);
        }


    }
}
