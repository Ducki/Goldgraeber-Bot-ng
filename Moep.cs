using System.Linq;
using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.Security.Cryptography;

namespace Bot_Dotnet
{
    public class Moep
    {
        private TelegramBotClient botClient;
        private readonly string telegramApiToken;
        private readonly string databaseFileName;
        public textContext databaseConnection;

        public Moep(string telegramApiToken, string databaseFileName)
        {
            this.telegramApiToken = telegramApiToken;
            this.databaseFileName = databaseFileName;
        }

        ~Moep()
        {
            if (this.databaseConnection != null)
            {
                this.databaseConnection.Dispose();
            }
        }

        public void Init()
        {
            this.databaseConnection = new textContext();
        }

        public void InitTelegramClient()
        {
            this.botClient = new TelegramBotClient(this.telegramApiToken);
        }

        public void StartBot()
        {
            this.botClient.OnMessage += HandleIncomingMessage;
            this.botClient.StartReceiving();

            Console.WriteLine("Listening to messages. Type quit to exit.");

            while (true)
            {
                if (Console.ReadLine() == "quit")
                {
                    break;
                }
            }

            this.botClient.StopReceiving();
        }

        private async void HandleIncomingMessage(object sender, MessageEventArgs e)
        {
            System.Console.WriteLine($"{DateTime.Now} – Message in {e.Message.Chat.Title} / {e.Message.Chat.Id} from {e.Message.From.Username}: {e.Message.Text} ");

            if (e.Message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
            {
                System.Console.WriteLine("No text message, aborting …");
                return;
            }

            int triggerId = this.SearchTriggerInMessage(e.Message.Text) ?? 0;
            if (triggerId == 0) return;

            string answer = GetRandomAnswerByTriggerId(triggerId);
            await this.botClient.SendTextMessageAsync(e.Message.Chat, answer);
        }

        private int? SearchTriggerInMessage(string message)
        {
            foreach (var item in this.databaseConnection.Triggers)
            {
                if (message.ToLower().Contains(item.Searchstring))
                {
                    System.Console.WriteLine($"Found trigger {item.Searchstring}");
                    return ((int)item.Id);
                }
            }

            return null;
        }

        private string GetRandomAnswerByTriggerId(int triggerId)
        {
            /*
             Quick&dirty solution:
             dump all the rows with the matching trigger id
             in a list and shuffle client side.

             TODO: Better solution: create a view in the database
             in order to use the database native random function
             for returning the response
             */

            var responsesRaw = this.databaseConnection.Responses.Where(r => r.TriggerId == triggerId).ToList();
            var randomResultId = (new Random()).Next(responsesRaw.Count);
            var result = responsesRaw[randomResultId];

            System.Console.WriteLine($"Got trigger id {triggerId}, returning response id {result.Id}");

            return result.ResponseText;
        }

    }
}