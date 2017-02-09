using Scalematebot.View;
using System;
using System.Collections.Generic;
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
        public static MainView View;
        public static bool TestMode = false;
        public static List<string> Answers;

        public static void Main(string[] args)
        {
            // Preparing bot
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

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.TextMessage) return;

            if (TestMode)
            {
                Answers.Add(message.Text);
                View.Set(message.Text);
                if (!View.Ended())
                {
                    SetQuestion(message);
                }
                else
                {
                    TestMode = false;
                    var thanks = "Thank you for helping us!";
                    await Bot.SendTextMessageAsync(message.Chat.Id, thanks, replyMarkup: new ReplyKeyboardHide());
                    foreach (var answer in Answers)
                    {
                        Console.WriteLine(answer);
                    }
                }
            }
            else if (message.Text.StartsWith("/start"))
            {
                var userId = message.Chat.Id.ToString();
                TestMode = true;
                Answers = new List<string>();
                View = new MainView(userId);
                View.Start();
                SetQuestion(message);
            }
            else
            {
                var usage = @"Usage:
/start - Starts a new test
";
                await Bot.SendTextMessageAsync(message.Chat.Id, usage, replyMarkup: new ReplyKeyboardHide());
            }
            
        }

        private static void SetQuestion(Message message)
        {
            var question = View.Question;
            var keyboardButtons = View.Answers.Select(it => new[] { new KeyboardButton(it) }).ToArray();
            var keyboard = new ReplyKeyboardMarkup(keyboardButtons);
            var msg = Bot.SendTextMessageAsync(message.Chat.Id, question, replyMarkup: keyboard).Result;
            View.Next();
        }
    }
}
