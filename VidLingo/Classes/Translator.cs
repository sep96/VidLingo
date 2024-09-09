using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VidLingo.Classes
{
    class Translator
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task<string> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage)
        {
            var url = "https://libretranslate.com/translate";

            // Create the JSON payload
            var payload = new
            {
                q = text,
                source = sourceLanguage,
                target = targetLanguage
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                // Send the POST request
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode(); // Throw if not a success code

                // Read and return the response content
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
    }
}
