using Scalemate;
using Scalematebot.Controller;
using Scalematebot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Scalematebot.View
{
    public class MainView
    {
        public static TelegramBotClient Bot;
        public MainController Controller;
        public bool TestMode = false;
        public List<string> Answers;
        public string Id;

        public MainView(TelegramBotClient bot, Message message)
        {
            // TODO Implement constructor
            Bot = bot;
            Id = "???";
        }

        public async void OnMessageReceived(Message message)
        {
            if (TestMode)
            {
                Answers.Add(message.Text);
                Controller.Set(message.Text);
                if (!Controller.Ended())
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
                Controller = new MainController(new MainModel(), this);
                Controller.Start();
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


        private void SetQuestion(Message message)
        {
            var question = Controller.Question;
            var keyboardButtons = Controller.Answers.Select(it => new[] { new KeyboardButton(it) }).ToArray();
            var keyboard = new ReplyKeyboardMarkup(keyboardButtons);
            var msg = Bot.SendTextMessageAsync(message.Chat.Id, question, replyMarkup: keyboard).Result;
            Controller.Next();
        }
    }
}
