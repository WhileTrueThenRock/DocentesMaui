using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDocenteMAUI.Utils
{
    public static class PDFUtils
    {
        // Método asincrónico que permite al usuario seleccionar un archivo PDF
        public static async Task<MemoryStream> OpenPdf()
        {
            // Utiliza MediaPicker para seleccionar un archivo PDF
            var result = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Pdf,
                PickerTitle = "Selecciona un archivo PDF"
            });

            // Verifica si el usuario seleccionó un archivo PDF
            // Verifica si el usuario seleccionó un archivo PDF
            if (result != null)
            {
                // Abre un flujo de lectura desde el archivo PDF seleccionado
                var pdfStream = await result.OpenReadAsync();

                // Lee el contenido del archivo PDF en un MemoryStream
                var memoryStream = new MemoryStream();
                await pdfStream.CopyToAsync(memoryStream);

                // Coloca el puntero al principio del MemoryStream
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Retorna el MemoryStream que contiene el archivo PDF
                return memoryStream;
            }

            // Retorna nulo si el usuario no seleccionó un archivo PDF
            return null;
        }
    }
}
