using Scalematebot.Controller;
using Scalematebot.Model;
using System;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Scalematebot.View
{
    public class MainView
    {
        public TelegramBotClient Bot;
        public MainController Controller;
        public bool TestMode = false;
        public string CurrentStep;
        public string Id;

        /// <summary>
        /// Creates an instance of the view so a test subject can interact with the bot.
        /// </summary>
        /// <param name="bot">The bot this view will answer for.</param>
        /// <param name="message">A sample message to get the subject identification.</param>
        public MainView(TelegramBotClient bot, Message message)
        {
            Bot = bot;
            Id = message.Chat.Username;
        }

        /// <summary>
        /// Sends a message depending on the test's state.
        /// </summary>
        /// <param name="message">The message sent by the user.</param>
        public async void OnMessageReceived(Message message)
        {
            if (TestMode)
            {
                Evaluate(message);
            }
            else if (message.Text.StartsWith("/test"))
            {
                TestMode = true;
                Controller = new MainController(new MainModel(), this);
                Controller.Start();
                CurrentStep = Controller.CurrentStep;
                Evaluate(message);
            }
            else if (message.Text.StartsWith("/about"))
            {
                // TODO Add /about message to explain this research.
                var help = "???";
                await Bot.SendTextMessageAsync(message.Chat.Id, help, replyMarkup: new ReplyKeyboardHide());
            }
            else
            {
                var usage = @"Uso:
/test - Começar um novo teste.
/about - Mostra informações sobre o Bot e sobre a pesquisa sendo realizada.
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
            // Setting options
            int i = 0;
            var keyboardButtons = Controller.Options
                                            .Select(it => $"{++i}. {it}")
                                            .Select(it => new[] { new KeyboardButton(it) })
                                            .ToArray();
            var keyboard = new ReplyKeyboardMarkup(keyboardButtons);

            // Setting question
            // TODO Move this question logic to controller
            var question = Controller.Question;
            if (question[0] == '*') question = question.Substring(1);

            // Drawing
            await Bot.SendTextMessageAsync(message.Chat.Id, question, replyMarkup: keyboard);
            Controller.NextQuestion();
        }

        /// <summary>
        /// Displays the instructions on the screen.
        /// </summary>
        private async void DisplayInstructions(Message message)
        {
            var instructions = Controller.GetInstructions();
            await Bot.SendTextMessageAsync(message.Chat.Id, instructions, replyMarkup: new ReplyKeyboardHide());
            CurrentStep = Controller.NextStep();
            Evaluate(message);
        }

        /// <summary>
        /// Collects data for the survey part.
        /// </summary>
        private async void ConductSurvey(Message message)
        {
            var question = Controller.NextSurveyQuestion();
            if (question != null)
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, question);
            }
            else
            {
                CurrentStep = Controller.NextStep();
                Evaluate(message);
            }
        }

        /// <summary>
        /// Evaluates the message content according to the test's state.
        /// </summary>
        /// <param name="message">The message sent by the user.</param>
        private async void Evaluate(Message message)
        {
            Controller.Set(CurrentStep, message.Text);
            if (!Controller.Ended())
            {
                switch (CurrentStep)
                {
                    case "instructions":
                        DisplayInstructions(message);
                        break;
                    case "survey":
                        ConductSurvey(message);
                        break;
                    case "test":
                        SetQuestion(message);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
            else
            {
                TestMode = false;
                var thanks = Controller.GetThanks();
                await Bot.SendTextMessageAsync(message.Chat.Id, thanks, replyMarkup: new ReplyKeyboardHide());
                Controller.Save();
            }
        }
    }
}
