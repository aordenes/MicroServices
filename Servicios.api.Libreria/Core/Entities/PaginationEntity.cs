using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicios.api.Libreria.Core.Entities
{
    public class PaginationEntity<TDocument>
    {

        public int PageSize { get; set; }

        public int Page { get; set; }

        public string Sort { get; set; }

        public string SortDirection { get; set; }

        public string Filter { get; set; }

        public int PageQuantity { get; set; }

        public IEnumerable<TDocument> Data { get; set; }
    }
}
