using System.Linq;
using System;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.Security.Cryptography;
using Telegram.Bot.Types;
using System.Text;
using Telegram.Bot.Types.Enums;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Speech.V1;
using Google.LongRunning;
using System.Globalization;

namespace Bot_Dotnet
{
    public class Moep
    {
        private TelegramBotClient botClient;
        private readonly string telegramApiToken;
        public textContext databaseConnection;
        private Message lastSentMessage;

        public Moep(string telegramApiToken)
        {
            this.telegramApiToken = telegramApiToken;
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
                string line = Console.ReadLine();
                if (line == "quit")
                {
                    break;
                }

                // Debug stuff:
                if (line.StartsWith("/append"))
                {
                    System.Console.WriteLine("Appending …");
                    this.AppendToMessage(this.lastSentMessage, "moep!");
                    continue;
                }

                if (line.StartsWith("/processvoice"))
                {

                    //this.ProcessVoiceMessage();
                    continue;
                }
            }

            this.botClient.StopReceiving();
        }

        private async void HandleIncomingMessage(object sender, MessageEventArgs e)
        {
            System.Console.WriteLine($"{DateTime.Now} – Message in {e.Message.Chat.Title} / {e.Message.Chat.Id} from {e.Message.From.Username}: {e.Message.Text} ");

            // convert to switch

            switch (e.Message.Type)
            {
                case MessageType.Text:
                    await this.ProcessTextMessage(e.Message);
                    break;

                case MessageType.Voice:
                    await this.ProcessVoiceMessage(e.Message);
                    break;
                default:
                    System.Console.WriteLine("No text message, aborting …");
                    return;
            }
        }

        private async Task ProcessTextMessage(Message message)
        {
            int triggerId = this.SearchTriggerInMessage(message.Text) ?? 0;
            if (triggerId == 0) return;

            string answer = GetRandomAnswerByTriggerId(triggerId);

            Message lastMessage = await this.botClient.SendTextMessageAsync(message.Chat, answer);
            this.lastSentMessage = lastMessage;
        }

        private async Task ProcessVoiceMessage(Message message)
        {
            /*
                1. get file id: message.Voice.FileId
                2. get temp file name: Path.GetTempFileName
                3. Download file https://github.com/TelegramBots/Telegram.Bot/blob/78e2573ca612270a6f29df8e4db3d10867ba859a/test/Telegram.Bot.Tests.Integ/Other/FileDownloadTests.cs#L79
                4. create google cloud voice api client
                5. upload file
                6. wait for callback
                7. send transcribed text
            */
            var pathForTempFile = Path.GetTempFileName();
            System.Console.WriteLine($"temp file: {pathForTempFile}");
            System.Console.WriteLine($"File name: {message.Voice.FileId}");

            using (FileStream fileStream = System.IO.File.OpenWrite(pathForTempFile))
            {
                Telegram.Bot.Types.File file = await this.botClient.GetInfoAndDownloadFileAsync(
                                fileId: message.Voice.FileId,
                                destination: fileStream
                            );

            }

            this.botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            string transcript = this.CallCloudApi(pathForTempFile);

            await this.botClient.SendTextMessageAsync(message.Chat,
                                                transcript,
                                                parseMode: ParseMode.Markdown,
                                                replyToMessageId: message.MessageId);

            System.IO.File.Delete(pathForTempFile);
        }

        private string CallCloudApi(string path)
        {
            System.Console.WriteLine("Baue Verbindung auf …");

            SpeechClient speechClient = SpeechClient.Create();

            LongRunningRecognizeRequest request = new()
            {
                Config = new RecognitionConfig()
                {
                    SampleRateHertz = 48000,
                    Encoding = RecognitionConfig.Types.AudioEncoding.OggOpus,
                    LanguageCode = "de_DE",
                    Model = "default",
                    UseEnhanced = true,
                    MaxAlternatives = 1,
                    EnableAutomaticPunctuation = true

                },
                Audio = RecognitionAudio.FromFile(path),
            };

            System.Console.WriteLine("Lade Memo hoch …");

            Operation<LongRunningRecognizeResponse, LongRunningRecognizeMetadata> response = speechClient.LongRunningRecognize(request);

            System.Console.WriteLine("Warte auf Texterkennung …");

            Operation<LongRunningRecognizeResponse, LongRunningRecognizeMetadata> completedResponse = response.PollUntilCompleted();
            LongRunningRecognizeResponse result = completedResponse.Result;

            System.Console.WriteLine("Fertig.");

            var transcript = result.Results[0].Alternatives[0];

            string confidence = (transcript.Confidence * 100).ToString("F2");
            string output = @$"_Ich bin mir zu {confidence}% sicher, dass folgendes gesagt wurde:_
                
{transcript.Transcript}";

            return output;
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

        private async void AppendToMessage(Message message, string text)
        {

            var newMessage = new StringBuilder(message.Text);
            newMessage.AppendLine("*moep!* _nech wahr_");

            this.lastSentMessage.Text = newMessage.ToString();

            Message editedMessage = await this.botClient.EditMessageTextAsync(
                chatId: message.Chat.Id,
                messageId: message.MessageId,
                text: newMessage.ToString(),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
            );

        }

    }
}