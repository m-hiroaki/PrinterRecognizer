using System;

namespace PrinterRecognizer
{
    static class EnvHelper
    {
        public static void LoadEnv()
        {
            // Load environment variables from .env
            DotNetEnv.Env.Load();
        }

        public static string Predictionkey
        {
            get
            {
                return DotNetEnv.Env.GetString("PREDICTION_KEY");
            }
        }

        public static string Endpoint
        {
            get
            {
                return DotNetEnv.Env.GetString("ENDPOINT");
            }
        }
    }
}