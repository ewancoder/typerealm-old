namespace TypeRealm.ConsoleApp
{
    using System.Collections.Generic;

    internal static class WordGenerator
    {
        // TODO: Use length of words as DIFFICULTY parameter. And adjust the difficulty accordingly based on speed of typing (pass this data to server).
        // Cause slow typer and fast typer should have the same tactical advantage over each other.
        // (slow one = one-letter word, just a letter or something, fast one - multi-word phrase)
        //private static readonly IEnumerator<string> _words = Data.Data.GetPhrases(5, 15).GetEnumerator();
        private static readonly IEnumerator<string> _words = Data.Data.GetPhrases(2, 5).GetEnumerator();

        public static string Generate()
        {
            _words.MoveNext();
            return _words.Current;
        }

        public static IEnumerable<string> Generate(int count)
        {
            for (var i = 1; i <= count; i++)
            {
                yield return Generate();
            }
        }
    }
}
