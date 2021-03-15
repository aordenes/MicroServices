using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicios.api.Libreria.Core
{
    public class MongoSettings
    {
        /// <summary>
        /// cadena de conexion
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// Nombre de BD
        /// </summary>
        public string Database { get; set; }
    }
}
