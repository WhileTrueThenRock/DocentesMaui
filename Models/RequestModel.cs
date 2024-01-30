using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorChat.Models
{

    // Definición de la clase RequestModel, utilizada para representar un modelo de solicitud.
    internal class RequestModel
    {
        // Propiedad que representa el método HTTP de la solicitud (GET, POST, etc.).
        public string Method { get; set; }

        // Propiedad que almacena la ruta de la solicitud dentro del servidor.
        public string Route { get; set; }

        // Propiedad que indica el servidor al que se enviará la solicitud.
        public string Server { get; set; }

        // Propiedad que contiene los datos asociados a la solicitud (puede ser cualquier tipo de datos).
        public object Data { get; set; }

        // Constructor parametrizado de la clase RequestModel.
        public RequestModel(string method, string route, string server, object data)
        {
            Method = method;
            Route = route;
            Server = server;
            Data = data;
        }
    }

}
