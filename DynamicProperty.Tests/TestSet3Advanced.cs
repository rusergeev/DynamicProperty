using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DynamicProperties.Tests
{
    /// <summary>
    /// Advanced difficulty tests.
    /// These test whether the dynamic properties are thread-safe and can be used concurrently by multiple threads.
    /// </summary>
    [TestFixture]
    public class TestSet3Advanced
    {
        [Test]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Gets GCed when the test ends")]
        public void IfDependencyIsUpdatedDuringEvaluationThenCalculatedIsEvaluatedAgainButNotConcurrently()
        {
            // One should never actually write code like this
            var o = DynamicProperty.Create(100);
            var doUpdate = true;
            var evaluating = false;
            var count = 0;
            var c = DynamicProperty.Create(
                () =>
                    {
                        Assert.IsFalse(evaluating);
                        evaluating = true;
                        ++count;

                        var result = o.Value;

                        if (doUpdate)
                        {
                            doUpdate = false;
                            ++o.Value; // read AND update the value.  Should trigger another evaluation when this one finishes.
                        }

                        evaluating = false;
                        return result;
                    },
                v => { });

            Assert.That(count, Is.EqualTo(2));
            Assert.That(c.Value, Is.EqualTo(101));

            o.Value = 0;
            Assert.That(count, Is.EqualTo(3));
            Assert.That(c.Value, Is.EqualTo(0));

            doUpdate = true;
            o.Value = -100;
            Assert.That(count, Is.EqualTo(5));
            Assert.That(c.Value, Is.EqualTo(-99));
        }

        [Test]
        public void IfDependencyIsUpdatedByOtherThreadWhileEvaluatingThenCalculatedIsEvaluatedAgainButNotConcurrently()
        {
            var o = DynamicProperty.Create(100);
            var waitTime = TimeSpan.FromSeconds(2);
            var waitForBarrier = true;
            var evaluating = false;
            var count = 0;
            IDynamicProperty<int> c = null;

            using (var barrier = new Barrier(2))
            {
                var t1 = Task.Run(() =>
                    {
                        c = DynamicProperty.Create(
                            () =>
                                {
                                    Assert.IsFalse(evaluating);
                                    evaluating = true;
                                    ++count;
                                    var result = o.Value;

                                    if (waitForBarrier)
                                    {
                                        waitForBarrier = false;
                                        Assert.IsTrue(barrier.SignalAndWait(waitTime));
                                        Thread.Sleep(TimeSpan.FromMilliseconds(50)); // give the other thread time to do its work
                                    }

                                    evaluating = false;
                                    return result;
                                },
                            v => { });
                    });

                Action changeValue = () =>
                {
                    // wait for the computed to start evaluating
                    Assert.IsTrue(barrier.SignalAndWait(waitTime));

                    // Now make our change
                    ++o.Value;
                };

                var t2 = Task.Run(changeValue);
                Task.WaitAll(t1, t2);
                Assert.That(count, Is.EqualTo(2));
                Assert.That(o.Value, Is.EqualTo(101));
                Assert.That(c.Value, Is.EqualTo(101));

                o.Value = 0;
                Assert.That(count, Is.EqualTo(3));
                Assert.That(c.Value, Is.EqualTo(0));

                waitForBarrier = true;
                var t3 = Task.Run(changeValue);
                o.Value = -100;
                t3.Wait();
                Assert.That(count, Is.EqualTo(5));
                Assert.That(c.Value, Is.EqualTo(-99));
            }
        }
    }
}
