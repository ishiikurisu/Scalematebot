using Scalematebot.Controller;
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
        public static Dictionary<long, MainView> Conversations;

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

        // TODO TEST THIS!!!
        public static TelegramBotClient CreateBot(string token)
        {
            var bot = new TelegramBotClient(token);

            // Registering callbacks
            bot.OnMessage += BotOnMessageReceived;

            return bot;
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.TextMessage) return;

            var id = message.Chat.Id;
            if (Conversations.ContainsKey(id))
            {
                MainView view = new MainView(Bot, message);
                Conversations.TryGetValue(id, out view);
                view.OnMessageReceived(message);
            }
        }
    }
}
