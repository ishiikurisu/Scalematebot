using System;
using Telegram.Bot;

namespace Scalematebot
{
    class Program
    {
        static private bool Done = false;

        public static void Main(string[] args)
        {
            Console.WriteLine("# Hello from Scalematebot!");
            string token = Console.ReadLine();
            var bot = new TelegramBotClient(token);
            var me = bot.GetMeAsync().Result;
            Console.WriteLine("Hello my name is " + me.FirstName);
            Console.ReadLine();
        }
    }
}
