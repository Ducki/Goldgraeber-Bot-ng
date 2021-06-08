using System;
using System.Globalization;

namespace Bot_Dotnet
{
    class Program
    {
        static void Main(string[] args)
        {
            // dotnet publish -r linux-arm -c Release -p:PublishSingleFile=true --self-contained true -p:PublishTrimmed=true -p:InvariantGlobalization=true
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("de-DE");

            string token;

            if (args.Length == 0)
            {
                System.Console.WriteLine("No token supplied.");
                return;
            }
            else
            {
                token = args[0];
            }

            Moep moep = new(token);

            moep.Init();
            moep.InitTelegramClient();

            moep.StartBot();
        }
    }
}
