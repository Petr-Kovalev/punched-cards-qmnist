using PunchedCards.BitVectors;

namespace PunchedCards.Helpers
{
    internal interface IExpert
    {
        public double CalculateLoss(IBitVector bitVector, IBitVector label);
    }
}