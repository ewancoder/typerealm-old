﻿namespace TypeRealm.ConsoleApp.Typing
{
    using System;

    internal sealed class Typer
    {
        private const char NewLine = '\n';
        private const char WhiteSpace = ' ';
        private string _text;

        public Typer(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text should not be empty.", nameof(text));

            Reset(text);
        }

        public Typer(string text, int progress) : this(text)
        {
            if (progress > _text.Length)
                throw new ArgumentException("Progress should be less than text length.", nameof(progress));

            Typed = _text.Substring(0, progress);
        }

        public string Typed { get; private set; }
        public string Error { get; private set; }
        public string NotTyped => _text.Substring(Typed.Length, _text.Length - Typed.Length);

        public bool IsStartedTyping => Typed.Length > 0;
        public bool IsFinishedTyping => Typed == _text;

        public void Type(char character)
        {
            if (IsFinishedTyping || Error != string.Empty)
                return;

            if (character == NotTyped[0])
            {
                Typed += character;
                return;
            }

            if (NotTyped[0] == NewLine && character == WhiteSpace)
            {
                Typed += NewLine;
                return;
            }

            Error += character;
        }

        public void Backspace()
        {
            if (Error.Length > 0)
            {
                Error = Error.Substring(0, Error.Length - 1);
                return;
            }

            if (Typed.Length > 0)
                Typed = Typed.Substring(0, Typed.Length - 1);
        }

        public void Reset(string text)
        {
            _text = text;
            Reset();
        }

        public void Reset()
        {
            Typed = string.Empty;
            Error = string.Empty;
        }
    }
}
