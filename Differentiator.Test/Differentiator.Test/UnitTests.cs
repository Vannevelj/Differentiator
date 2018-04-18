using System;
using System.Linq;
using Xunit;

namespace Differentiator.Test
{
    public class UnitTests
    {
        [Theory]
        [InlineData(500, 10.0, 1)]
        [InlineData(500, 10.0, 2)]
        [InlineData(500, 10.0, 3)]
        [InlineData(500, 10.0, 4)]
        [InlineData(500, 10.0, 100)]
        [InlineData(500, 10.0, 333)]
        [InlineData(500, 10.0, 999)]
        [InlineData(1000, 7.5, 1)]
        [InlineData(1000, 7.5, 2)]
        [InlineData(1000, 7.5, 3)]
        [InlineData(1000, 7.5, 4)]
        [InlineData(1000, 7.5, 100)]
        [InlineData(1000, 7.5, 333)]
        [InlineData(1000, 7.5, 999)]
        [InlineData(10000, 2.5, 1)]
        [InlineData(10000, 2.5, 2)]
        [InlineData(10000, 2.5, 3)]
        [InlineData(10000, 2.5, 4)]
        [InlineData(10000, 2.5, 100)]
        [InlineData(10000, 2.5, 333)]
        [InlineData(10000, 2.5, 999)]
        [InlineData(100000, 1.0, 1)]
        [InlineData(100000, 1.0, 2)]
        [InlineData(100000, 1.0, 3)]
        [InlineData(100000, 1.0, 4)]
        [InlineData(100000, 1.0, 100)]
        [InlineData(100000, 1.0, 333)]
        [InlineData(100000, 1.0, 999)]
        [InlineData(1000000, 0.75, 1)]
        [InlineData(1000000, 0.75, 2)]
        [InlineData(1000000, 0.75, 3)]
        [InlineData(1000000, 0.75, 4)]
        [InlineData(1000000, 0.75, 100)]
        [InlineData(1000000, 0.75, 333)]
        [InlineData(1000000, 0.75, 999)]
        public void IsInVariant_IsEquallyDistributed(int numberOfUsers, double percentageDifferenceAllowed, int experimentId)
        {
            var service = new ExperimentService();

            var userIds = Enumerable.Range(0, numberOfUsers).Select(x => x.ToString());
            int control = 0;
            int variant = 0;

            foreach (var userId in userIds)
            {
                var isInVariant = service.IsInVariant(experimentId, userId);
                if (isInVariant)
                {
                    variant++;
                }
                else
                {
                    control++;
                }
            }

            var diff = (double)variant - control;
            var percentage = Math.Abs((diff / control) * 100);

            Assert.True(percentage < percentageDifferenceAllowed, $"Control: {control}\tVariant:{variant}\tPercentage{percentage}\tExperimentId:{experimentId}");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(250)]
        [InlineData(333)]
        [InlineData(999)]
        public void IsInVariant_ReturnsSameValueForEachInvocation(int experimentId)
        {
            var iterations = 1000;
            var service = new ExperimentService();

            for (var userId = 1; userId < 100; userId++)
            {
                var control = 0;
                var variant = 0;

                for (var i = 0; i < iterations; i++)
                {
                    var isInVariant = service.IsInVariant(experimentId, userId.ToString());
                    if (isInVariant)
                    {
                        variant++;
                    }
                    else
                    {
                        control++;
                    }
                }

                Assert.False(control > 0 && variant > 0, $"Control: {control}\tVariant: {variant}\tExperimentID: {experimentId}\tUserID: {userId}");
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(100000)]
        [InlineData(56789)]
        [InlineData(32)]
        [InlineData(9812345)]
        [InlineData(5000000)]
        public void IsInVariant_GivesUsersBothVariants(int userId)
        {
            var iterations = 100;
            var service = new ExperimentService();

            var control = 0;
            var variant = 0;

            for (var experimentId = 1; experimentId < iterations + 1; experimentId++)
            {
                var isInVariant = service.IsInVariant(experimentId, userId.ToString());
                if (isInVariant)
                {
                    variant++;
                }
                else
                {
                    control++;
                }
            }

            Assert.True(control > 30 && variant > 30, $"Control: {control}\tVariant: {variant}\tUserID: {userId}");
        }
    }
}
