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
        }

        public async Task<JArray> RunPrediction()
        {
            JObject reqobj = new JObject();
            reqobj.Add("Url", new JValue(_url));

            var content = new StringContent(reqobj.ToString(), Encoding.UTF8, "application/json");

            var cv = new CVHelper(FileSource.URL, content);

            // Post prediction request
            JArray resJson = await cv.Prediction();

            return resJson;
        }
    }

    class RecognizerFromLocal : IRecognizer
    {
        // URL to an image to be predicted
        private string _filepath;

        public RecognizerFromLocal(string url)
        {
            _filepath = url;
        }

        public async Task<JArray> RunPrediction()
        {
            // Request body. Try this sample with a locally stored image.
            byte[] byteData = GetImageAsByteArray(_filepath);

            JArray resJson;
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                var cv = new CVHelper(FileSource.LocalImage, content);

                // Post prediction request
                resJson = await cv.Prediction();
            }

            return resJson;
        }

        private byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
    }
}