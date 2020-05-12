using System;

namespace ConsoleUtils
{
    public static class ConsoleHelper
    {
        public static bool QuitRequest(int timeout)
        {
            var sleepTime = timeout / 5;

            Console.WriteLine("===========================================================");
            Console.WriteLine("Press ENTER key to exit after receiving all the messages.");
            Console.WriteLine("===========================================================");

            ConsoleKeyInfo k = new ConsoleKeyInfo();
            bool quit = false;
            for (int cnt = 5; cnt > 0; cnt--)
            {
                if (Console.KeyAvailable == true)
                {
                    k = Console.ReadKey();
                    if (k.Key == ConsoleKey.Enter)
                    {
                        quit = true;
                    }
                }
                else
                {
                    Console.WriteLine(".");
                    System.Threading.Thread.Sleep(sleepTime);
                }
            }
            //            Console.WriteLine("The key pressed was " + k.Key);
            return quit;
        }
    }
}
