using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicios.api.Libreria.Core.Entities
{
    /// <summary>
    /// estamos trabajando documnetos genericos, no exiswte un metodo que me permita ingresarle el nombre de la collecion por erso es necesario crearla la clase
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class BsonCollectionAtribute : Attribute
    {
        public string CollectionName { get; }

        public BsonCollectionAtribute(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}
