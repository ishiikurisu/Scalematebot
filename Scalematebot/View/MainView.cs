using Scalematebot.Controller;
using Scalematebot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Creates an instance of the view so a test subject can interact with the bot.
        /// </summary>
        /// <param name="bot">The bot this view will answer for.</param>
        /// <param name="message">A sample message to get the subject identification.</param>
        public MainView(TelegramBotClient bot, Message message)
        {
            Bot = bot;
            // Obtain an unique ID for this view
            Id = "???";
        }

        /// <summary>
        /// Sends a message depending on the test's state.
        /// </summary>
        /// <param name="message">The message sent by the user.</param>
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
                    // TODO Save answers
                    foreach (var answer in Answers)
                    {
                        Console.WriteLine(answer);
                    }
                }
            }
            else if (message.Text.StartsWith("/test"))
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
                var usage = @"Uso:
/test - Começar um novo teste
";
                await Bot.SendTextMessageAsync(message.Chat.Id, usage, replyMarkup: new ReplyKeyboardHide());
            }
        }


        /// <summary>
        /// The message to send to the user containing a question.
        /// </summary>
        /// <param name="message">The original message, the one to be answered.</param>
        private async void SetQuestion(Message message)
        {
            // TODO Implement instructions
            // TODO Implement survey
            var question = Controller.Question;
            var keyboardButtons = Controller.Answers.Select(it => new[] { new KeyboardButton(it) }).ToArray();
            var keyboard = new ReplyKeyboardMarkup(keyboardButtons);
            await Bot.SendTextMessageAsync(message.Chat.Id, question, replyMarkup: keyboard);
            Controller.Next();
        }
    }
}
