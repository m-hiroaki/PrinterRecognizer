using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PrinterRecognizer
{
    class CVHelper
    {
        private string _endpoint;
        private string _predictionkey;
        private HttpContent _content;

        public CVHelper(FileSource type, HttpContent content)
        {
            EnvHelper.LoadEnv();
            if (FileSource.URL == type)
                _endpoint = EnvHelper.Endpoint + "/url";
            else if (FileSource.LocalImage == type)
                _endpoint = EnvHelper.Endpoint + "/image";
            _predictionkey = EnvHelper.Predictionkey;
            _content = content;
        }

        public async Task<JArray> Prediction()
        {
            var client = new HttpClient();

            // Set the prediction key
            client.DefaultRequestHeaders.Add("Prediction-Key", _predictionkey);

            HttpResponseMessage response;
            response = await client.PostAsync(_endpoint, _content);

            // read prediction result from the http response
            var jsonres = await response.Content.ReadAsStringAsync();

            // convert into json object
            JObject resobj = JObject.Parse(jsonres);

            return (JArray)resobj["predictions"];
        }
    }
}
