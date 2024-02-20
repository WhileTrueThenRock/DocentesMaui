using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDocenteMAUI.Utils
{
    public static class ImageUtils
    {
        // Método asincrónico que permite al usuario seleccionar una imagen de la galería
        internal static async Task<Dictionary<string, object>> OpenImage()
        {
            // Utiliza MediaPicker para seleccionar una foto con una ventana de selección de imagen
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Selecciona una imagen"
            });

            // Verifica si el usuario seleccionó una imagen
            if (result != null)
            {
                // Inicializa un diccionario para almacenar las fuentes de imagen
                var imageSources = new Dictionary<string, object>();

                // Abre un flujo de lectura desde la imagen seleccionada
                var stream = await result.OpenReadAsync();
                // Abre otro flujo de lectura para convertir la imagen a Base64
                var streamForImageBase64 = await result.OpenReadAsync();

                // Almacena la imagen como fuente de imagen en el diccionario
                imageSources["imageFromStream"] = ImageSource.FromStream(() => stream);

                // Crea un MemoryStream para copiar el contenido del flujo de lectura
                var msstream = new MemoryStream();
                streamForImageBase64.CopyTo(msstream);

                // Convierte el contenido del MemoryStream a una cadena Base64
                string convert = Convert.ToBase64String(msstream.ToArray());

                // Almacena la cadena Base64 en el diccionario, incluyendo el tipo de contenido (MIME type)
                imageSources["imageBase64"] = "data:" + result.ContentType + ";base64," + convert;

                // Retorna el diccionario que contiene las fuentes de imagen
                return imageSources;
            }

            // Retorna nulo si el usuario no seleccionó una imagen
            return null;
        }
    }
}
