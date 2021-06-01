using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.Data.Sqlite;
using Telegram.Bot;
using Telegram.Bot.Args;
using Dapper;
using Bot_Dotnet.Models;

namespace Bot_Dotnet
{
    public class Moep
    {
        private TelegramBotClient botClient;
        private string telegramApiToken;
        private List<Triggers> triggers;
        private string databaseFileName;
        public SqliteConnection databaseConnection;

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
            this.databaseConnection = this.openDatabaseConnection(databaseFileName);
            this.triggers = getTriggers();
        }

        public void InitTelegramClient()
        {
            this.botClient = new TelegramBotClient(telegramApiToken);
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
            System.Console.WriteLine($@"{DateTime.Now.ToString()} – Message in {e.Message.Chat.Title} / {e.Message.Chat.Id} from {e.Message.From.Username}: {e.Message.Text} ");

            int triggerId = this.SearchTriggerInMessage(e.Message.Text) ?? 0;
            if (triggerId == 0) return;

            string answer = GetRandomAnswerByTriggerId(triggerId);
            await this.botClient.SendTextMessageAsync(e.Message.Chat, answer);
        }

        private int? SearchTriggerInMessage(string message)
        {
            foreach (var item in this.triggers)
            {
                if (message.ToLower().Contains(item.searchstring))
                {
                    System.Console.WriteLine($"found trigger {item.searchstring}");
                    return item.id;
                }
            }

            return null;
        }

        private string GetRandomAnswerByTriggerId(int triggerId)
        {
            var result = this.databaseConnection.
            Query<Answers>(@"SELECT
                                 answer
                             FROM
                                 Triggers
                             JOIN
                                 Answers
                             ON (Answers.trigger_id=Triggers.id)
                             WHERE
                                 trigger_id = @id
                             ORDER BY random()
                             LIMIT 1"
                            , new { id = triggerId });

            return result.FirstOrDefault().answer;
        }
        private List<Triggers> getTriggers()
        {
            var result = this.databaseConnection.Query<Triggers>("SELECT id, searchstring FROM Triggers").AsList();

            return result;
        }

        private SqliteConnection openDatabaseConnection(string filename)
        {
            var connection = new SqliteConnection($"Data Source={filename}");
            connection.Open();

            return connection;
        }
    }
}