using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EFDocenteMAUI.Models
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
        public int Telefono { get; set; }
        public string FechaNacimiento { get; set; }
        public bool RolProfesor { get; set; }
        public string Curso { get; set; }
        public DireccionModel Direccion { get; set; }
        public string Avatar { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsNotificationEnabled { get; set; }

        public class DireccionModel
        {
            public string Calle { get; set; }
            public string Numero { get; set; }
            public string Poblacion { get; set; }
            public int Cp { get; set; }
        }


        public UserModel() { 
            Id=ObjectId.GenerateNewId().ToString();
            FechaNacimiento = DateTime.Now.ToString();
            RolProfesor = false;
            Avatar = APIService.ImagenesServerUrl + "/avatars/defaultimage";
            Direccion = new DireccionModel();
        }

    }
}
