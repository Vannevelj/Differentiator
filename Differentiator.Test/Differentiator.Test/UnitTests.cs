using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Differentiator.Test
{
    public class UnitTests
    {
        [Theory]
        [ClassData(typeof(DistributionTestData))]
        public void IsInVariant_IsEquallyDistributed(int numberOfUsers, int experimentId)
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
            
            Assert.True(percentage < 0.5, $"Control: {control}\tVariant:{variant}\tPercentage{percentage}\tExperimentId:{experimentId}");
        }

        [Theory]
        [ClassData(typeof(InvocationTestData))]
        public void IsInVariant_ReturnsSameValueForEachInvocation(int experimentId, int userId)
        {
            var iterations = 1000;
            var service = new ExperimentService();

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

        [Theory]
        [ClassData(typeof(VariantTestData))]
        public void IsInVariant_GivesUsersBothVariants(int userId, List<int> experimentIds)
        {
            var service = new ExperimentService();

            var control = 0;
            var variant = 0;

            foreach(var id in experimentIds)
            {
                var isInVariant = service.IsInVariant(id, userId.ToString());
                if (isInVariant)
                {
                    variant++;
                }
                else
                {
                    control++;
                }
            }

            var minimumParticipations = (int) (experimentIds.Count * 0.45);

            Assert.True(control > minimumParticipations && variant > minimumParticipations, $"Control: {control}\tVariant: {variant}\tUserID: {userId}");
        }
    }

    public class DistributionTestData : IEnumerable<object[]>
    {
        private Random _random = new Random(32);

        public IEnumerator GetEnumerator() => GetEnumerator();

        IEnumerator<object[]> IEnumerable<object[]>.GetEnumerator()
        {
            for (var experimentId = 1; experimentId < 100; experimentId++)
            {
                var experiment = _random.Next(1, 1000);
                for (var numberOfUsers = 1000; numberOfUsers < 100_000; numberOfUsers += 1000)
                {
                    yield return new object[] { numberOfUsers, experiment };
                }                
            }
        }
    }

    public class VariantTestData : IEnumerable<object[]>
    {
        private Random _random = new Random(32);

        public IEnumerator GetEnumerator() => GetEnumerator();

        IEnumerator<object[]> IEnumerable<object[]>.GetEnumerator()
        {
            for (var i = 1; i < 100; i++)
            {
                var userId = _random.Next(1, 200000);
                var experimentIds = new List<int>();

                for (var j = 1; j < 1000; j++)
                {
                    experimentIds.Add(_random.Next(1, 500));
                }

                yield return new object[] { userId, experimentIds };
            }
        }
    }

    public class InvocationTestData : IEnumerable<object[]>
    {
        private Random _random = new Random(32);

        public IEnumerator GetEnumerator() => GetEnumerator();

        IEnumerator<object[]> IEnumerable<object[]>.GetEnumerator()
        {
            for (var i = 1; i < 100; i++)
            {
                for (var experimentId = 1; experimentId < 100; experimentId++)
                {
                    yield return new object[] { experimentId, _random.Next(1, 100_000) };
                }                
            }
        }
    }
}
