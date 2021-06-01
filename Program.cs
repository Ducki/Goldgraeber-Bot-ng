using System;
using System.Collections.Generic;

namespace Bot_Dotnet
{
    class Program
    {
        static void Main(string[] args)
        {
            // dotnet publish -r linux-arm -c Release -p:PublishSingleFile=true --self-contained true -p:PublishTrimmed=true -p:InvariantGlobalization=true
            Moep moep = new Moep(args[0], "text.sqlite");

            moep.Init();
            moep.InitTelegramClient();

            moep.StartBot();
        }
    }
}
