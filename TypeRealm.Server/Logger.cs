/// <summary>
/// The network messaging engine was written at the night of new year 2019: from 8PM till 5AM.
/// </summary>
namespace TypeRealm.Server
{
    using System;

    internal sealed class Logger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
