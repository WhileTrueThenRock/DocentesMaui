using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace EFDocenteMAUI.Models
{
    internal class APIService
    {
        // Constantes que representan las URL de los servidores
        public const string GestionServerUrl = "http://192.168.20.10:5000";
        public const string ImagenesServerUrl = "http://192.168.20.10:12061";

        // Método para ejecutar una solicitud HTTP y obtener una respuesta asincrónica
        public static async Task<ResponseModel> ExecuteRequest(RequestModel requestModel)
        {
            // Se crea una instancia de la clase ResponseModel para almacenar la respuesta
            ResponseModel responseModel = new ResponseModel();
            // Se establece configuración para que se conviertan todas la claves a minúsculas
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new LowercaseContractResolver();
            // Se serializa el objeto RequestModel a formato JSON
            var data = JsonConvert.SerializeObject(requestModel.Data, settings);
            Debug.WriteLine(data);

            // Se utilizan bloques 'using' para asegurar que los recursos se liberen correctamente
            using (var handler = new StandardSocketsHttpHandler())
            using (var client = new HttpClient(handler))
            {

                // Se crea una nueva solicitud HTTP con el método y la ruta especificados en el objeto RequestModel
                var request = new HttpRequestMessage(new HttpMethod(requestModel.Method),
                    requestModel.Server + requestModel.Route);

                // Se establece el encabezado 'Accept' para indicar que se acepta JSON como tipo de respuesta
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string token = await SecureStorage.Default.GetAsync("token");
                if(token != null)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                }
                // Se incluye el contenido JSON en la solicitud
                request.Content = new StringContent(data, Encoding.UTF8, "application/json");

                try
                {
                    // Se envía la solicitud al servidor de manera asíncrona y se espera una respuesta
                    using (HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                    {
                        // Si la respuesta es exitosa
                        if (response.IsSuccessStatusCode)
                        {
                            // Se lee la respuesta como una cadena JSON
                            var stringResponse = await response.Content.ReadAsStringAsync();
                            if (stringResponse != null)
                            {
                                // Se deserializa la cadena JSON en un objeto ResponseModel
                                responseModel = JsonConvert.DeserializeObject<ResponseModel>(stringResponse) ?? new ResponseModel();

                                // Se imprime la respuesta en la consola de depuración
                                Debug.Write("Respuesta desde la API: ");
                                Debug.WriteLine(stringResponse);
                            }

                        }
                        else
                        {
                            // Si la respuesta no es exitosa, se imprime el código de estado en la consola de depuración
                            Debug.WriteLine(response.StatusCode);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Si se produce una excepción durante la solicitud, se imprime un mensaje de error en la consola de depuración
                    Debug.WriteLine($"Error al enviar la solicitud a la API: {ex.Message}");

                    // Se decide relanzar la excepción, pero esto puede personalizarse según los requisitos del usuario

                }
            }

            // Se devuelve el objeto ResponseModel, que contiene la respuesta de la API
            return responseModel;
        }
    }

    //Covierte todas las claves a minúsculas
    public class LowercaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }
}
