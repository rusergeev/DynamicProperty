using NUnit.Framework;
using DynamicProperty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicProperty.Tests
{
    public class Invalidatable : DependencyNode
    {

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
        public void LinkItselfValidates()
        {
            var a = new Invalidatable();
            a.AddLink(a);
            Assert.That(a.Valid, Is.EqualTo(true));
        }
        [Test]
        public void NodeIsValidWhenLinked()
        {
            var a = new Invalidatable();
            var b = new Invalidatable();
            b.AddLink(b);
            a.AddLink(b);
            Assert.That(a.Valid, Is.EqualTo(true));
        }
        [Test]
        public void InvalidateItselfWorks()
        {
            var a = new Invalidatable();
            a.AddLink(a);
            a.Invalidate();
            Assert.That(a.Valid, Is.EqualTo(false));
        }
        [Test]
        public void InvalidatefWorksOnDependent()
        {
            var a = new Invalidatable();
            a.AddLink(a);
            var b = new Invalidatable();
            b.AddLink(a);
            b.Invalidate();
            Assert.That(a.Valid, Is.EqualTo(false));
        }
        [Test]
        public void InvalidateDoesNotWorksOnSiblings()
        {
            var a = new Invalidatable();
            a.AddLink(a);
            var b = new Invalidatable();
            b.AddLink(a);
            var c = new Invalidatable();
            c.AddLink(c);
            b.Invalidate();
            Assert.That(c.Valid, Is.EqualTo(true));
        }
        [Test]
        public void InvalidateDisconnectsFromFurtherDependency()
        {
            var a = new Invalidatable();
            a.AddLink(a);
            var b = new Invalidatable();
            b.AddLink(a);
            var c = new Invalidatable();
            c.AddLink(b);
            b.Invalidate();
            a.AddLink(a);
            b.AddLink(a);
            c.Invalidate();
            Assert.That(b.Valid, Is.EqualTo(true));
        }
        [Test]
        public void InvalidateWorksOnMultipleDependantBroad()
        {
            var a = new Invalidatable();
            a.AddLink(a);
            var b = new Invalidatable();
            b.AddLink(b);
            var c = new Invalidatable();
            c.AddLink(a);
            c.AddLink(b);
            c.Invalidate();
            Assert.That(a.Valid, Is.EqualTo(false));
            Assert.That(b.Valid, Is.EqualTo(false));
        }
        [Test]
        public void InvalidateWorksInDepth()
        {
            var a = new Invalidatable();
            a.AddLink(a);
            var b = new Invalidatable();
            b.AddLink(a);
            var c = new Invalidatable();
            c.AddLink(b);
            c.Invalidate();
            Assert.That(a.Valid, Is.EqualTo(false));
            Assert.That(b.Valid, Is.EqualTo(false));
        }
    }
}