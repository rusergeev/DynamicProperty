
namespace Developer.Test
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using NUnit.Framework;

    /// <summary>
    /// Intermediate difficulty tests.
    /// The first test is a single-threaded test that just ensures the read function is not called needlessly
    /// The second test determines if different threads can work with their own dynamic properties without interfering with each other.
    /// </summary>
    [	TestFixture]
    public class TestSet2Intermediate
    {
        [Test]
        public void ReleasesDependenciesNotUsedInSubsequentReads()
        {
            var values = new[] { DynamicProperty.Create(42), DynamicProperty.Create(99), DynamicProperty.Create(2012) };
            var readCounts = new[] { 0, 0, 0 };
            var which = DynamicProperty.Create(0);
            var p = DynamicProperty.Create(
                () =>
                    {
                        readCounts[which.Value]++;
                        return values[which.Value].Value;
                    },
                newValue => { });

            Assert.That(readCounts, Is.EquivalentTo(new[] { 1, 0, 0 }));

            values[1].Value = 100;
            Assert.That(readCounts, Is.EquivalentTo(new[] { 1, 0, 0 }));

            which.Value = 2;
            Assert.That(readCounts, Is.EquivalentTo(new[] { 1, 0, 1 }));

            values[0].Value = 10;
            Assert.That(readCounts, Is.EquivalentTo(new[] { 1, 0, 1 }));
        }

        [Test]
        public void CalculatedsEvaluatedOnDifferentThreadsShouldNotCaptureEachOthersDependencies()
        {
            var o1 = DynamicProperty.Create(100);
            var o2 = DynamicProperty.Create(1000);
            var waitTime = TimeSpan.FromSeconds(2);
            var waitForBarrier = true;
            using (var barrier = new Barrier(2))
            {
                IDynamicProperty<int> a = null, b = null;

                var t1 = Task.Run(() =>
                    {
                        a = DynamicProperty.Create(
                            () =>
                                {
                                    if (waitForBarrier)
                                    {
                                        Assert.IsTrue(barrier.SignalAndWait(waitTime), "deadlock occurred");
                                    }

                                    var result = o1.Value;
                                    if (waitForBarrier)
                                    {
                                        Assert.IsTrue(barrier.SignalAndWait(waitTime), "deadlock occurred");
                                    }

                                    return result;
                                },
                            v => { });
                    });

                var t2 = Task.Run(() =>
                    {
                        b = DynamicProperty.Create(
                            () =>
                                {
                                    Assert.IsTrue(waitForBarrier, "this calculated should not be evaluated after initialization");
                                    Assert.IsTrue(barrier.SignalAndWait(waitTime), "deadlock occurred");
                                    var result = o2.Value;
                                    Assert.IsTrue(barrier.SignalAndWait(waitTime), "deadlock occurred");
                                    return result;
                                },
                            v => { });
                    });

                Task.WaitAll(t1, t2);
                Assert.That(a.Value, Is.EqualTo(100));
                Assert.That(b.Value, Is.EqualTo(1000));

                waitForBarrier = false;
                o1.Value = -100;
                Assert.That(a.Value, Is.EqualTo(-100));
                Assert.That(b.Value, Is.EqualTo(1000));
            }
        }

    }
}
