using Scalematebot.View;
using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace Scalematebot
{
    class Program
    {
        static TelegramBotClient Bot;
        static Dictionary<string, MainView> Conversations = new Dictionary<string, MainView>();

        /// <summary>
        /// Entry point for the application
        /// </summary>
        static void Main(string[] args)
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

        /// <summary>
        /// Creates a new Telegram bot.
        /// </summary>
        /// <param name="token">The Telegram API for this bot.</param>
        /// <returns>The bot with the appropriate callbacks.</returns>
        static TelegramBotClient CreateBot(string token)
        {
            var bot = new TelegramBotClient(token);

            // Registering callbacks
            bot.OnMessage += BotOnMessageReceived;

            return bot;
        }

        /// <summary>
        /// Called when the bot receives a message. Will relate each user to a different
        /// test application.
        /// </summary>
        static void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            // Checking message is valid
            var message = messageEventArgs.Message;
            if (message == null || message.Type != MessageType.TextMessage) return;

            // Answering according to message
            var id = message.Chat.Id.ToString();

            if (message.Chat.Username != null)
            {
                id = message.Chat.Username;
            }

            if (!Conversations.ContainsKey(id))
            {
                Conversations[id] = new MainView(Bot, message);
            }

            Conversations[id].OnMessageReceived(message);
        }
    }
}
