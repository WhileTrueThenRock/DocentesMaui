using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorChat.Models
{
   public class UserModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Dni { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string FechaNacimiento { get; set; }
        public string Direccion { get; set; }
        public string Rol { get; set; }
        public string Curso { get; set; }
        public string Avatar { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        

        public UserModel() { 
            Id=ObjectId.GenerateNewId().ToString();
            Rol = "Estudiante";
        }

    }
}
