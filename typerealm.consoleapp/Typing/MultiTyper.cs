namespace TypeRealm.ConsoleApp.Typing
{
    using System.Collections.Generic;
    using System.Linq;

    internal abstract class MultiTyper : IInputHandler
    {
        private readonly Dictionary<char, Typer> _typers
            = new Dictionary<char, Typer>();

        private Typer _focusedTyper;

        protected MultiTyper(Game game)
        {
            Game = game;
        }

        protected Game Game { get; }

        public void Type(char character)
        {
            if (_focusedTyper == null)
            {
                if (!_typers.ContainsKey(character))
                    return;

                _focusedTyper = _typers[character];
            }

            _focusedTyper.Type(character);

            if (_focusedTyper.IsFinishedTyping)
            {
                ResetUniqueTyper(_focusedTyper);

                OnTyped(_focusedTyper);
                _focusedTyper = null;
            }
        }

        public void Backspace()
        {
            if (_focusedTyper == null)
                return;

            _focusedTyper.Backspace();

            if (!_focusedTyper.IsStartedTyping)
                _focusedTyper = null;
        }

        public void Escape()
        {
            if (_focusedTyper != null)
            {
                _focusedTyper.Reset();
                _focusedTyper = null;
            }
        }

        public virtual void Tab() { }

        protected abstract void OnTyped(Typer typer);

        protected Typer MakeUniqueTyper()
        {
            var word = GetUniqueWord();

            var typer = new Typer(word);
            _typers.Add(word[0], typer);

            return typer;
        }

        private void ResetUniqueTyper(Typer typer)
        {
            var word = GetUniqueWord();

            typer.Reset(word);
            _typers.Remove(_typers.Single(x => x.Value == typer).Key);
            _typers.Add(word[0], typer);
        }

        private string GetUniqueWord()
        {
            var word = WordGenerator.Generate();
            while (_typers.ContainsKey(word[0]))
                word = WordGenerator.Generate();

            return word;
        }
    }
}
