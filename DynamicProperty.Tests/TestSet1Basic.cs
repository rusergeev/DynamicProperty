
using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;

namespace DynamicProperty.Tests
{
    /// <summary>
    /// Tests the minimum required features for this coding quiz
    /// Tests that your dynamic properties function correctly in a single-threaded application.
    /// </summary>
    public static class TestSet1Basic
    {
        [TestFixture]
        public class DynamicProperties
        {
            [Test]
            public void CanBeConstructedWithInitialValue()
            {
                var p = DynamicProperty.Create(42);
                Assert.That(p.Value, Is.EqualTo(42));
            }

            [Test]
            public void ValueCanBeChanged()
            {
                var p = DynamicProperty.Create(42);
                p.Value = 100;
                Assert.That(p.Value, Is.EqualTo(100));
            }

            [Test]
            public void NothingImmediatelyHappensWhenSubscribed()
            {
                var p = DynamicProperty.Create("Forty Two");
                p.Subscribe(value => Assert.Fail());
            }

            [Test]
            public void SubscriptionReceivesNotificationsWhenValueChanges()
            {
                var p = DynamicProperty.Create(42);
                var notifications = new List<int>();

                // add each notification to our notifications list.
                p.Subscribe(notifications.Add);

                p.Value = 100;
                Assert.That(notifications, Is.EquivalentTo(new[] { 100 }));

                p.Value = 200;
                Assert.That(notifications, Is.EquivalentTo(new[] { 100, 200 }));

                p.Value = 0;
                Assert.That(notifications, Is.EquivalentTo(new[] { 100, 200, 0 }));
            }

            [Test]
            public void SubscriptionNotNotifiedAfterItIsDisposed()
            {
                var p = DynamicProperty.Create(42);
                var notifications = new List<int>();

                // add each notification to our notifications list.
                using (p.Subscribe(notifications.Add))
                {
                    p.Value = 100;
                    Assert.That(notifications, Is.EquivalentTo(new[] { 100 }));

                    p.Value = 200;
                    Assert.That(notifications, Is.EquivalentTo(new[] { 100, 200 }));

                    p.Value = 0;
                    Assert.That(notifications, Is.EquivalentTo(new[] { 100, 200, 0 }));
                }

                // No more notifications
                p.Value = 1;
                p.Value = 2;
                p.Value = 3;
                Assert.That(notifications, Is.EquivalentTo(new[] { 100, 200, 0 }));
            }

            [Test]
            public void MultipleSubscribersAreNotified()
            {
                var p = DynamicProperty.Create(0);
                var notifications = new[] { new List<int>(), new List<int>(), new List<int>() };
                var subscriptions = new IDisposable[3];
                var expected = new[] { new List<int>(), new List<int>(), new List<int>() };

                subscriptions[0] = p.Subscribe(notifications[0].Add);
                p.Value = 1;
                expected[0].Add(1);

                subscriptions[1] = p.Subscribe(notifications[1].Add);
                p.Value = 2;
                expected[0].Add(2);
                expected[1].Add(2);

                subscriptions[2] = p.Subscribe(notifications[2].Add);
                p.Value = 3;
                expected[0].Add(3);
                expected[1].Add(3);
                expected[2].Add(3);

                subscriptions[1].Dispose();
                p.Value = 4;
                expected[0].Add(4);
                expected[2].Add(4);

                subscriptions[2].Dispose();
                p.Value = 5;
                expected[0].Add(5);

                subscriptions[0].Dispose();
                p.Value = 6;

                for (var i = 0; i < 3; ++i)
                {
                    Assert.That(notifications[i], Is.EquivalentTo(expected[i]));
                }
            }
        }

        [TestFixture]
        public class CalculatedProperties
        {
            [Test]
            public void ReadFunctionIsEvaluatedExactlyOnceDuringConstruction()
            {
                var evalCount = 0;
                var p = DynamicProperty.Create(() => ++evalCount, newValue => Assert.Fail("write method should not be called"));
                Assert.That(evalCount, Is.EqualTo(1));
            }

            [Test]
            public void InitialValueIsResultOfReadFunction()
            {
                var evalCount = 0;
                var p = DynamicProperty.Create(() => ++evalCount, newValue => Assert.Fail("write method should not be called"));
                Assert.That(p.Value, Is.EqualTo(1));
            }

            [Test]
            public void ReadingValueShouldNotTriggerAnotherEvaulation()
            {
                var evalCount = 0;
                var p = DynamicProperty.Create(() => ++evalCount, newValue => Assert.Fail("write method should not be called"));
                Assert.That(p.Value, Is.EqualTo(1));
                Assert.That(evalCount, Is.EqualTo(1));
            }

            [Test]
            public void WhenWriteMethodDoesNothingReadIsNotTriggered()
            {
                var evalCount = 0;
                var p = DynamicProperty.Create(() => ++evalCount, newValue => { });
                p.Value = 10;
                Assert.That(p.Value, Is.EqualTo(1));
                Assert.That(evalCount, Is.EqualTo(1));
            }

            [Test]
            public void WhenWriteMethodDoesNotModifyDynamicPropertyReadIsNotTriggered()
            {
                var value = 0;
                var p = DynamicProperty.Create(() => ++value, newValue => value = newValue);
                p.Value = 10;
                Assert.That(value, Is.EqualTo(10)); // write method updated our value variable
                Assert.That(p.Value, Is.EqualTo(1)); // but read method is not re-evaluated so it is still 1.
            }

            [Test]
            public void WhenWriteMethodUpdatesDependencyReadIsTriggered()
            {
                var value = DynamicProperty.Create(42);
                var p = DynamicProperty.Create(() => value.Value, newValue => value.Value = newValue);

                Assert.That(p.Value, Is.EqualTo(42));

                p.Value = 100;
                Assert.That(value.Value, Is.EqualTo(100));
                Assert.That(p.Value, Is.EqualTo(100));
            }

            [Test]
            public void WhenWriteMethodUpdatesNonDependentValueReadIsNotTriggered()
            {
                var value = DynamicProperty.Create(42);
                var otherValue = DynamicProperty.Create(99);
                var p = DynamicProperty.Create(() => value.Value, newValue => otherValue.Value = newValue);

                Assert.That(p.Value, Is.EqualTo(42));

                p.Value = 100;
                Assert.That(otherValue.Value, Is.EqualTo(100));
                Assert.That(value.Value, Is.EqualTo(42));
                Assert.That(p.Value, Is.EqualTo(42));
            }

            [Test]
            public void WhenDependencyIsModifiedCalculatedPropertyIsReevaluated()
            {
                var value = DynamicProperty.Create(42);
                var p = DynamicProperty.Create(() => 10 * value.Value, newValue => value.Value = newValue / 10);

                Assert.That(p.Value, Is.EqualTo(420));

                value.Value = 55;
                Assert.That(p.Value, Is.EqualTo(550));
            }

            [Test]
            public void WhenMultipleDependenciesExistPropertyIsReevaluatedWhenAnyChange()
            {
                var a = DynamicProperty.Create("forty-two");
                var b = DynamicProperty.Create(42);
                var c = DynamicProperty.Create(99);
                var p = DynamicProperty.Create(() => b.Value + c.Value + a.Value.Length, newValue => { });

                Assert.That(p.Value, Is.EqualTo(42 + 99 + "forty-two".Length));

                b.Value = 100;
                Assert.That(p.Value, Is.EqualTo(100 + 99 + "forty-two".Length));

                a.Value = "";
                Assert.That(p.Value, Is.EqualTo(100 + 99));
            }

            [Test]
            public void CanBeDependentOnOtherCalculatedProperties()
            {
                var a = DynamicProperty.Create(42);
                var b = DynamicProperty.Create(() => a.Value * 10, newValue => a.Value = newValue / 10);
                var c = DynamicProperty.Create(() => b.Value.ToString(CultureInfo.InvariantCulture), newValue => b.Value = int.Parse(newValue, CultureInfo.InvariantCulture));

                Assert.That(c.Value, Is.EqualTo("420"));

                a.Value = 31;
                Assert.That(c.Value, Is.EqualTo("310"));

                c.Value = "990";
                Assert.That(b.Value, Is.EqualTo(990));
                Assert.That(a.Value, Is.EqualTo(99));
            }

            [Test]
            public void CapturesNewDependenciesOnSubsequentReads()
            {
                var values = new[] { DynamicProperty.Create(42), DynamicProperty.Create(99), DynamicProperty.Create(2012) };
                var which = DynamicProperty.Create(0);
                var p = DynamicProperty.Create(() => values[which.Value].Value, newValue => values[which.Value].Value = newValue);

                Assert.That(p.Value, Is.EqualTo(42));

                p.Value = 24;
                Assert.That(p.Value, Is.EqualTo(24));
                Assert.That(values[0].Value, Is.EqualTo(24));

                which.Value = 2;
                Assert.That(p.Value, Is.EqualTo(2012));
                Assert.That(values[0].Value, Is.EqualTo(24));

                values[2].Value = 3012;
                Assert.That(p.Value, Is.EqualTo(3012));
                Assert.That(values[2].Value, Is.EqualTo(3012));

                values[1].Value = 1999;
                Assert.That(p.Value, Is.EqualTo(3012));
                Assert.That(values[2].Value, Is.EqualTo(3012));

                which.Value = 1;
                Assert.That(p.Value, Is.EqualTo(1999));
            }
            [Test]
            public void CanIncrementTheValue()
            {
                var o = DynamicProperty.Create(100);
                ++o.Value;
                Assert.That(o.Value, Is.EqualTo(101));
                o.Value++;
                Assert.That(o.Value, Is.EqualTo(102));
            }
       }
    }
}
