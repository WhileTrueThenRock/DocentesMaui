using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDocenteMAUI.Models
{
    internal class ResourceModel
    {
        public string Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion {  get; set; }
        public string Contenido { get; set; }

        public ResourceModel() 
        {
            Id = ObjectId.GenerateNewId().ToString();
        }
    }
}
