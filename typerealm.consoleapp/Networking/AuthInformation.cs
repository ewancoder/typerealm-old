namespace TypeRealm.ConsoleApp.Networking
{
    public sealed class AuthInformation
    {
        public AuthInformation(string playerName)
        {
            PlayerName = playerName;
        }

        public string PlayerName { get; }
    }
}
