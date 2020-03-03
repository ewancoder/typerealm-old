using System;

namespace TypeRealm.Domain.Battling
{
    public sealed class PlayerHp
    {
        private int _current;

        public PlayerHp(int current, int maximum)
        {
            if (maximum <= 0)
                throw new ArgumentException("Maximum HP should be positive.", nameof(maximum));

            if (current > maximum || current < 0)
                throw new ArgumentException("Current HP should be less than maximum and positive or zero.", nameof(current));

            Maximum = maximum;
            _current = current;
        }

        public int Maximum { get; }

        public int Current
        {
            get => _current;
            set
            {
                if (value < 0)
                {
                    _current = 0;
                    return;
                }

                if (value > Maximum)
                {
                    _current = Maximum;
                    return;
                }

                _current = value;
            }
        }
    }
}
