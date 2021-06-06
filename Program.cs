using System;
using System.Collections.Generic;


namespace Bot_Dotnet
{
    class Program
    {
        static void Main(string[] args)
        {
            // dotnet publish -r linux-arm -c Release -p:PublishSingleFile=true --self-contained true -p:PublishTrimmed=true -p:InvariantGlobalization=true
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

            Moep moep = new(token, "text.sqlite");

            moep.Init();
            moep.InitTelegramClient();

            moep.StartBot();
        }
    }
}
