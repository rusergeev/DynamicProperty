using NUnit.Framework;

namespace DynamicProperty.Tests
{
    public class Invalidatable : DependencyNode
    {
        protected override void Eval()
        {
            Valid = false;
        }

        public bool Valid { get; set; }
    }

    [TestFixture]
    public class NodeTests
    {
        [Test]
        public void NodeInitializedasFlase()
        {
            var a = new Invalidatable();
            Assert.That(a.Valid, Is.EqualTo(false));
        }
        [Test]
        public void InvalidateDoesNotAffectItself()
        {
            var a = new Invalidatable {Valid = true};
            a.Invalidate();
            Assert.That(a.Valid, Is.EqualTo(true));
        }
        [Test]
        public void InvalidatefWorksOnDependent()
        {
            var a = new Invalidatable {Valid = true};
            var b = new Invalidatable();
            b.AddLink(a);
            b.Invalidate();
            Assert.That(a.Valid, Is.EqualTo(false));
        }
        [Test]
        public void InvalidateDoesNotWorksOnSiblings()
        {
            var a = new Invalidatable {Valid = true};
            var b = new Invalidatable();
            b.AddLink(a);
            var c = new Invalidatable {Valid = true};
            b.Invalidate();
            Assert.That(c.Valid, Is.EqualTo(true));
        }
        [Test]
        public void CutTailDisconnectsFromFurtherDependency()
        {
            var a = new Invalidatable {Valid = true};
            var b = new Invalidatable { Valid = true };
            b.AddLink(a);
            var c = new Invalidatable();
            c.AddLink(b);
            b.CutDependency();
            a.Valid = true;
            b.AddLink(a);
            c.Invalidate();
            Assert.That(b.Valid, Is.EqualTo(true));
        }
        [Test]
        public void InvalidateWorksOnMultipleDependantBroad()
        {
            var a = new Invalidatable {Valid = true};
            var b = new Invalidatable {Valid = true};
            var c = new Invalidatable();
            c.AddLink(a);
            c.AddLink(b);
            c.Invalidate();
            Assert.That(a.Valid, Is.EqualTo(false));
            Assert.That(b.Valid, Is.EqualTo(false));
        }
        [Test]
        public void InvalidateWorksOnMultipleDependencies1()
        {
            var a = new Invalidatable { Valid = true };
            var b = new Invalidatable { Valid = true };
            var c = new Invalidatable { Valid = true };
            c.AddLink(a);
            b.AddLink(a);
            b.Invalidate();
            Assert.That(a.Valid, Is.EqualTo(false));
        }
        [Test]
        public void InvalidateWorksOnMultipleDependencies2()
        {
            var a = new Invalidatable { Valid = true };
            var b = new Invalidatable { Valid = true };
            var c = new Invalidatable { Valid = true };
            c.AddLink(a);
            b.AddLink(a);
            c.Invalidate();
            Assert.That(a.Valid, Is.EqualTo(false));
        }
        [Test]
        public void InvalidateWorksInDepth()
        {
            var a = new Invalidatable {Valid = true};
            var b = new Invalidatable {Valid = true};
            b.AddLink(a);
            var c = new Invalidatable();
            c.AddLink(b);
            c.Invalidate();
            Assert.That(a.Valid, Is.EqualTo(false));
            Assert.That(b.Valid, Is.EqualTo(false));
        }
    }
}