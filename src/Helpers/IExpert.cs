using System.Collections.Generic;
using PunchedCards.BitVectors;

namespace PunchedCards.Helpers
{
    internal interface IExpert
    {
        public IReadOnlyDictionary<IBitVector, double> CalculateMatchingScores(IBitVector bitVector);

        public void Finetune(IBitVector bitVector, IBitVector label);
    }
}