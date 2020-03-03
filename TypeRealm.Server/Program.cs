/// <summary>
/// The network messaging engine was written at the night of new year 2019: from 8PM till 5AM.
/// </summary>
namespace TypeRealm.Server
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    static class Program
    {
        static readonly NotificationService _notificationService = new NotificationService();
        static readonly TimeSpan _saveInterval = TimeSpan.FromSeconds(1);
        static bool _isExiting;

        static void Main()
        {
            Console.WriteLine("=== TypeRealm server ===");
            //var playerRepository = new PlayerRepository();
            var playerRepository = new CachedPlayerRepository(
                new DiskPlayerRepository("players.data"));
            var battleRepository = new BattleRepository();

            var persistence = Task.Run(() =>
            {
                // TODO: NOT THREAD SAFE. Maybe make repositories thread-safe.
                while (!_isExiting)
                {
                    Thread.Sleep(_saveInterval);
                    playerRepository.PersistAllPending();
                    Console.WriteLine("Persisted all pending players.");
                }

                Thread.Sleep(_saveInterval);
                playerRepository.PersistAllPending();
            });

            using (var server = new Server(playerRepository, battleRepository))
            {
                while (Console.ReadLine() != "exit")
                {
                    Console.WriteLine("Type 'exit' to gracefully exit the server.");
                }

                // Stops saving players after saving all of them one last time.
                _isExiting = true;

                while (!persistence.IsCompleted && !persistence.IsCanceled && !persistence.IsFaulted)
                {
                    Console.WriteLine("Saving data to disk...");
                    Console.ReadLine();
                }

                // TODO: Gracefully exit the server, giving time for all players to quit.
            }
        }
    }
}
