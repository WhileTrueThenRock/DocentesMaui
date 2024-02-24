using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDocenteMAUI.Utils
{
    internal static class VideoUtils
    {
        // Método asincrónico que permite al usuario seleccionar un video de la galería
        internal static async Task<Dictionary<string, object>> OpenVideo()
        {
            // Utiliza MediaPicker para seleccionar una foto con una ventana de selección de imagen
            var result = await MediaPicker.PickVideoAsync(new MediaPickerOptions
            {
                Title = "Selecciona un video"
            });

            // Verifica si el usuario seleccionó una imagen
            if (result != null)
            {
                // Inicializa un diccionario para almacenar las fuentes de imagen
                var videoSources = new Dictionary<string, object>();

                // Abre un flujo de lectura desde la imagen seleccionada
                var stream = await result.OpenReadAsync();
                // Abre otro flujo de lectura para convertir la imagen a Base64
                var streamForVideoBase64 = await result.OpenReadAsync();

                // Almacena la imagen como fuente de imagen en el diccionario
                videoSources["imageFromStream"] = ImageSource.FromStream(() => stream);

                // Crea un MemoryStream para copiar el contenido del flujo de lectura
                var msstream = new MemoryStream();
                streamForVideoBase64.CopyTo(msstream);

                // Convierte el contenido del MemoryStream a una cadena Base64
                string convert = Convert.ToBase64String(msstream.ToArray());

                // Almacena la cadena Base64 en el diccionario, incluyendo el tipo de contenido (MIME type)
                videoSources["videoBase64"] = "data:" + result.ContentType + ";base64," + convert;

                // Retorna el diccionario que contiene las fuentes de imagen
                return videoSources;
            }

            // Retorna nulo si el usuario no seleccionó una imagen
            return null;
        }
    }
}
