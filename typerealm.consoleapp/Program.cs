namespace TypeRealm.ConsoleApp
{
    using System;
    using Networking;

    static class Program
    {
        static int Port = 30100;
        static void Main()
        {
            Console.WriteLine("=== TypeRealm ===");
            Console.Write("Server: ");
            var server = Console.ReadLine();

            Console.Write("Your name: ");
            var playerName = Console.ReadLine();

            using (var game = new Game(server, Port, new AuthInformation(playerName)))
            {
                //game.Input(new ConsoleKeyInfo());
                game.UpdateStatus(null); // Better HACK to update status to LOADING screen.

                Console.CursorVisible = false;
                while (true)
                {
                    var key = Console.ReadKey(true);
                    game.Input(key);
                }
            }
        }
    }
}
