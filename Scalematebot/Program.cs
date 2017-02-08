using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Scalematebot
{
    class Program
    {
        public static TelegramBotClient Bot;

        public static void Main(string[] args)
        {
            Console.WriteLine("# Hello from Scalematebot!");
            var token = Console.ReadLine();
            Bot = CreateBot(token);
            var me = Bot.GetMeAsync().Result;

            // Putting the bot to work
            Console.Title = me.FirstName;
            Console.WriteLine("- Hello my name is " + me.FirstName + "!");
            Console.WriteLine("- Press <Enter> to leave");
            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        public static TelegramBotClient CreateBot(string token)
        {
            var bot = new TelegramBotClient(token);

            // Registering callbacks
            // TODO Create callbacks
            bot.OnMessage += BotOnMessageReceived;

            return bot;
        }

        /// <summary>
        /// Copied from https://github.com/MrRoundRobin/telegram.bot.examples/blob/master/Telegram.Bot.Examples.Echo/Program.cs
        /// </summary>
        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.TextMessage) return;

            if (message.Text.StartsWith("/inline")) // send inline keyboard
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                var keyboard = new InlineKeyboardMarkup(new[]
                {
                    new[] // first row
                    {
                        new InlineKeyboardButton("1.1"),
                        new InlineKeyboardButton("1.2"),
                    },
                    new[] // second row
                    {
                        new InlineKeyboardButton("2.1"),
                        new InlineKeyboardButton("2.2"),
                    }
                });

                await Task.Delay(500); // simulate longer running task

                await Bot.SendTextMessageAsync(message.Chat.Id, "Choose",
                    replyMarkup: keyboard);
            }
            else if (message.Text.StartsWith("/keyboard")) // send custom keyboard
            {
                var keyboard = new ReplyKeyboardMarkup(new[]
                {
                    new [] // first row
                    {
                        new KeyboardButton("1.1"),
                        new KeyboardButton("1.2"),
                    },
                    new [] // last row
                    {
                        new KeyboardButton("2.1"),
                        new KeyboardButton("2.2"),
                    }
                });

                await Bot.SendTextMessageAsync(message.Chat.Id, "Choose",
                    replyMarkup: keyboard);
            }
            else if (message.Text.StartsWith("/photo")) // send a photo
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                const string file = @"<FilePath>";

                var fileName = file.Split('\\').Last();

                using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var fts = new FileToSend(fileName, fileStream);

                    await Bot.SendPhotoAsync(message.Chat.Id, fts, "Nice Picture");
                }
            }
            else if (message.Text.StartsWith("/request")) // request location or contact
            {
                var keyboard = new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton("Location")
                    {
                        RequestLocation = true
                    },
                    new KeyboardButton("Contact")
                    {
                        RequestContact = true
                    },
                });

                await Bot.SendTextMessageAsync(message.Chat.Id, "Who or Where are you?", replyMarkup: keyboard);
            }
            else
            {
                var usage = @"Usage:
/inline   - send inline keyboard
/keyboard - send custom keyboard
/photo    - send a photo
/request  - request location or contact
";

                await Bot.SendTextMessageAsync(message.Chat.Id, usage,
                    replyMarkup: new ReplyKeyboardHide());
            }
        }
    }
}
