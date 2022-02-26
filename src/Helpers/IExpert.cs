using System.Collections.Generic;
using PunchedCards.BitVectors;

namespace PunchedCards.Helpers
{
    internal interface IExpert
    {
        public double CalculateLoss(IBitVector bitVector, IBitVector label);

        public IReadOnlyDictionary<IBitVector, double> CalculateLosses(IBitVector bitVector);
    }
}