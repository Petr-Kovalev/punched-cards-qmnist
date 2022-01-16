using System.Collections.Generic;

namespace PunchedCards.Helpers
{
    internal interface IExpert
    {
        public double CalculateLoss(IEnumerable<uint> activeBitIndices);
    }
}