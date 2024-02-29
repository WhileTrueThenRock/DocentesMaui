using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDocenteMAUI.Utils
{
    public static class PDFUtils
    {

        internal static async Task<Dictionary<string, object>> OpenPDF()
        {

            var result = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Pdf, PickerTitle = "Selecione un pdf"
            });
            if (result != null)
            {
                var pdfSources = new Dictionary<string, object>();

                // Abre un flujo de lectura desde el pdf seleccionada
                var stream = await result.OpenReadAsync();
                // Abre otro flujo de lectura para convertir el pdf a Base64
                var streamForPDFBase64 = await result.OpenReadAsync();

                // Almacena el pdf como fuente de pdf en el diccionario
                pdfSources["pdfFromStream"] = ImageSource.FromStream(() => stream);

                // Crea un MemoryStream para copiar el contenido del flujo de lectura
                var msstream = new MemoryStream();
                streamForPDFBase64.CopyTo(msstream);

                // Convierte el contenido del MemoryStream a una cadena Base64
                string convert = Convert.ToBase64String(msstream.ToArray());

                // Almacena la cadena Base64 en el diccionario, incluyendo el tipo de contenido (MIME type)
                pdfSources["pdfBase64"] = "data:" + result.ContentType + ";base64," + convert;

                // Retorna el diccionario que contiene las fuentes de pdf
                return pdfSources;
            }

            return null;
        }
         
    }
}
