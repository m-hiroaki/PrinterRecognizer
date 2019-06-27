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
        public static IRecognizer CreateRecognizer(string filepath, FileSource type)
        {
            if (FileSource.URL == type)
                return (IRecognizer)(new RecognizerFromUrl(filepath));
            else if (FileSource.LocalImage == type)
                return (IRecognizer)(new RecognizerFromLocal(filepath));

            return null;
        }
    }

    class RecognizerFromUrl : IRecognizer
    {
        // URL to an image to be predicted
        private string _url;

        public RecognizerFromUrl(string url)
        {
            _url = url;
            EnvHelper.LoadEnv();
        }

        public async Task<JArray> RunPrediction()
        {
            var client = new HttpClient();

            // Get prediction key
            var predictionkey = EnvHelper.Predictionkey;
            // Set the prediction key
            client.DefaultRequestHeaders.Add("Prediction-Key", predictionkey);

            JObject reqobj = new JObject();
            reqobj.Add("Url", new JValue(_url));

            var content = new StringContent(reqobj.ToString(), Encoding.UTF8, "application/json");

            // Get endpoint url from .env
            string endpoint = EnvHelper.Endpoint;
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

        public RecognizerFromLocal(string url)
        {
            _filepath = url;
            EnvHelper.LoadEnv();
        }

        public async Task<JArray> RunPrediction()
        {
            var client = new HttpClient();

            // Get prediction key
            var predictionkey = EnvHelper.Predictionkey;
            // Set the prediction key
            client.DefaultRequestHeaders.Add("Prediction-Key", predictionkey);

            // Request body. Try this sample with a locally stored image.
            byte[] byteData = GetImageAsByteArray(_filepath);

            HttpResponseMessage response;

            using (var content = new ByteArrayContent(byteData))
            {
                // Get endpoint url from .env
                string endpoint = EnvHelper.Endpoint;
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