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
    public class LibreriaAutorController : ControllerBase
    {
        private readonly IMongoRepository<AutorEntity> _autorGenercioRepository;

        public LibreriaAutorController(IMongoRepository<AutorEntity> autorGenercioRepository)
        {
            _autorGenercioRepository = autorGenercioRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AutorEntity>>> Get()
        {
            var autor = await _autorGenercioRepository.GetAll();
            return Ok(autor);        
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<IEnumerable<AutorEntity>>> GetById(string Id)
        {
            var autor = await _autorGenercioRepository.GetById(Id);
            return Ok(autor);
        }

        [HttpPost]
        public async Task Post(AutorEntity autor)
        {
           await _autorGenercioRepository.InsertDocument(autor);
        }

        [HttpPut("{Id}")]
        public async Task Put(string Id, AutorEntity autor)
        {
            autor.Id = Id;
            await _autorGenercioRepository.UpdateDocument(autor);           
        }

        [HttpDelete]
        public async Task Delete(string Id)
        {           
            await _autorGenercioRepository.DeleteById(Id);
        }
    }
}
