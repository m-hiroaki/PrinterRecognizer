using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PrinterRecognizer
{
    interface IRecognizer
    {
        Task<JArray> RunPrediction();
    }

    class RecongnizerFactory
    {
        public static IRecognizer CreateRecognizer(string filepath, string key, FileSource type)
        {
            if( FileSource.URL == type ) 
                return (IRecognizer)(new RecognizerFromUrl(filepath, key));
            else if( FileSource.LocalImage == type ) 
                return (IRecognizer)(new RecognizerFromLocal(filepath, key));

            return null;
        }
    }

    class RecognizerFromUrl : IRecognizer
    {
        // URL to an image to be predicted
        private string _url;
        // Prediction-Key
        private string _key;

        public RecognizerFromUrl(string url, string key)
        {
            _url = url;
            _key = key;
        }

        public async Task<JArray> RunPrediction()
        {
            var client = new HttpClient();

            // Set the prediction key
            client.DefaultRequestHeaders.Add("Prediction-Key", _key);

            JObject reqobj = new JObject();
            reqobj.Add("Url", new JValue(_url));

            var content = new StringContent(reqobj.ToString(), Encoding.UTF8, "application/json");

            // Get endpoint url from .env
            string endpoint = DotNetEnv.Env.GetString("ENDPOINT");
            HttpResponseMessage response;
            response = await client.PostAsync(endpoint + "/url", content);

            // read prediction result from the http response
            var jsonres = await response.Content.ReadAsStringAsync();

            // convert into json object
            JObject resobj = JObject.Parse(jsonres);
          
            return (JArray)resobj["predictions"];
        }
    }

    class RecognizerFromLocal : IRecognizer
    {
        // URL to an image to be predicted
        private string _filepath;
        // Prediction-Key
        private string _key;

        public RecognizerFromLocal(string url, string key)
        {
            _filepath = url;
            _key      = key;
        }

        public async Task<JArray> RunPrediction()
        {
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add("Prediction-Key", _key);

            // Request body. Try this sample with a locally stored image.
            byte[] byteData = GetImageAsByteArray(_filepath);

            HttpResponseMessage response;

            using (var content = new ByteArrayContent(byteData))
            {
                // Get endpoint url from .env
                string endpoint = DotNetEnv.Env.GetString("ENDPOINT");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(endpoint + "/image", content);
            }

            // read prediction result from the http response
            var jsonres = await response.Content.ReadAsStringAsync();

            // convert into json object
            JObject resobj = JObject.Parse(jsonres);
          
            return (JArray)resobj["predictions"];
        }

        private byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
    }
}