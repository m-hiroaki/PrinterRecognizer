using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.CommandLineUtils;

namespace PrinterRecognizer
{
    enum FileSource
    {
        URL,
        LocalImage,
    }

    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication(throwOnUnexpectedArg: false);
            app.Name = nameof(PrinterRecognizer);
            app.HelpOption("-?|-h|--help");

            var urlOption = app.Option("-u|--url",
                        "URL to an image to be predicted",
                        CommandOptionType.SingleValue);

            var localOption = app.Option("-l|--local",
                        "Path to an image to be predicted",
                        CommandOptionType.SingleValue);

            string file = string.Empty;
            string key = string.Empty;
            try
            {
                FileSource type;
                app.OnExecute(() =>
                {

                    if (localOption.HasValue())
                    {
                        file = localOption.Value();
                        type = FileSource.LocalImage;
                    }
                    else if (urlOption.HasValue())
                    {
                        file = urlOption.Value();
                        type = FileSource.URL;
                    }
                    else
                    {
                        app.ShowHelp();
                        return 0;
                    }

                    // run recognition process
                    JArray result = RunPrinterRecognition(file, type).Result;

                    // show recognition result on console
                    showRecognitionResult(result);

                    return 0;
                });

                app.Execute(args);

            }
            catch (Exception e)
            {
                Console.WriteLine("Oops. Failed to run recognition process...");
                Console.WriteLine("Make sure you set correct option.");
                app.ShowHelp();

                Console.WriteLine(e.ToString());
            }
        }

        public static async Task<JArray> RunPrinterRecognition(string file, FileSource type)
        {
            // create recognizer
            IRecognizer recognizer = RecongnizerFactory.CreateRecognizer(file, type);

            return await recognizer.RunPrediction();
        }

        private static void showRecognitionResult(JArray result)
        {
            if (result is null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            Console.WriteLine("[Recognition Result]");
            foreach (JObject val in result)
            {
                var prob = Double.Parse((string)val["probability"]);
                Console.WriteLine("{0, -8} : {1, 3}%", (string)val["tagName"], new JValue(Math.Truncate(prob * 100.0)));
            }
        }
    }
}
