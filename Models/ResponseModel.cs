using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDocenteMAUI.Models
{
    // Definición de la clase ResponseModel, utilizada para representar un modelo de respuesta.
    internal class ResponseModel
    {
        // Propiedad que indica el estado de éxito de la operación (puede ser un código o bandera).
        public int Success { get; set; }

        // Propiedad que almacena un mensaje asociado a la respuesta (puede ser descriptivo o informativo).
        public string Message { get; set; }

        // Propiedad que contiene los datos asociados a la respuesta (puede ser cualquier tipo de datos).
        public object Data { get; set; }

        // Constructor que permite inicializar las propiedades de la clase con valores específicos.
        public ResponseModel(int success, string message, object data)
        {
            Success = success;  // Inicializa el estado de éxito con el valor proporcionado.
            Message = message;  // Inicializa el mensaje con la cadena proporcionada.
            Data = data;        // Inicializa los datos con el objeto proporcionado.
        }

        // Constructor predeterminado de la clase ResponseModel.
        public ResponseModel()
        {
            Success = 1;
            // Constructor vacío, no realiza ninguna inicialización específica.
            // Las propiedades pueden quedar con sus valores predeterminados (por ejemplo, 0 para int, null para string, object, etc.).
        }
    }

}
