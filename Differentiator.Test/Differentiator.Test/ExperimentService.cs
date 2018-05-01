using System;
using System.Collections.Generic;
using System.Text;

namespace Differentiator.Test
{
    public class ExperimentService
    {
        public bool IsInVariant(int experimentId, string userId)
        {
            const int numberOfVariants = 2;

            var hash = GetParticipationHash(experimentId, userId);
            var variant = (int)(hash % numberOfVariants);

            return variant > 0;
        }

        private long GetParticipationHash(int experimentId, string differentiator)
        {
            var realDifferentiator = int.Parse(differentiator);

            var hash = experimentId ^ realDifferentiator;

            return hash;
        }
    }
}
