using System;
using System.Collections.Generic;

namespace Elenor {
    /// <summary>
    /// Deterministic RNG for floor generation.
    /// Thanks to: https://stackoverflow.com/questions/767999/random-number-generator-only-generating-one-random-number
    /// </summary>
    public sealed class SeededRng {
        readonly Random _random;

        public SeededRng(int seed) {
            _random = new Random(seed);
        }

        public int NextInt(int minInclusive, int maxExclusive) {
            return _random.Next(minInclusive, maxExclusive);
        }

        public void Shuffle<T>(IList<T> list) {
            for (int i = list.Count - 1; i > 0; i--) {
                int j = NextInt(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}