using NUnit.Framework;

namespace DynamicProperty.Tests
{
    //public class Invalidatable : DependencyNode
    //{
    //    protected override void Recalc()
    //    {
    //        Valid = false;
    //    }

    //    public bool Valid { get; set; }
    //}

    //[TestFixture]
    //public class NodeTests
    //{
    //    [Test]
    //    public void NodeInitializedasFlase()
    //    {
    //        var a = new Invalidatable();
    //        Assert.That(a.Valid, Is.EqualTo(false));
    //    }
    //    [Test]
    //    public void InvalidateDoesNotAffectItself()
    //    {
    //        var a = new Invalidatable {Valid = true};
    //        a.Invalidate();
    //        Assert.That(a.Valid, Is.EqualTo(true));
    //    }
    //    [Test]
    //    public void InvalidatefWorksOnDependent()
    //    {
    //        var a = new Invalidatable {Valid = true};
    //        var b = new Invalidatable();
    //        a.DependsOn(b);
    //        b.Invalidate();
    //        Assert.That(a.Valid, Is.EqualTo(false));
    //    }
    //    [Test]
    //    public void InvalidateDoesNotWorksOnSiblings()
    //    {
    //        var a = new Invalidatable {Valid = true};
    //        var b = new Invalidatable();
    //        a.DependsOn(b);
    //        var c = new Invalidatable {Valid = true};
    //        b.Invalidate();
    //        Assert.That(c.Valid, Is.EqualTo(true));
    //    }
    //    [Test]
    //    public void CutTailDisconnectsFromFurtherDependency()
    //    {
    //        var a = new Invalidatable {Valid = true};
    //        var b = new Invalidatable { Valid = true };
    //        a.DependsOn(b);
    //        var c = new Invalidatable();
    //        b.DependsOn(c);
    //        b.CutDependency();
    //        a.Valid = true;
    //        a.DependsOn(b);
    //        c.Invalidate();
    //        Assert.That(b.Valid, Is.EqualTo(true));
    //    }
    //    [Test]
    //    public void InvalidateWorksOnMultipleDependantBroad()
    //    {
    //        var a = new Invalidatable {Valid = true};
    //        var b = new Invalidatable {Valid = true};
    //        var c = new Invalidatable();
    //        a.DependsOn(c);
    //        b.DependsOn(c);
    //        c.Invalidate();
    //        Assert.That(a.Valid, Is.EqualTo(false));
    //        Assert.That(b.Valid, Is.EqualTo(false));
    //    }
    //    [Test]
    //    public void InvalidateWorksOnMultipleDependencies1()
    //    {
    //        var a = new Invalidatable { Valid = true };
    //        var b = new Invalidatable { Valid = true };
    //        var c = new Invalidatable { Valid = true };
    //        a.DependsOn(c);
    //        a.DependsOn(b);
    //        b.Invalidate();
    //        Assert.That(a.Valid, Is.EqualTo(false));
    //    }
    //    [Test]
    //    public void InvalidateWorksOnMultipleDependencies2()
    //    {
    //        var a = new Invalidatable { Valid = true };
    //        var b = new Invalidatable { Valid = true };
    //        var c = new Invalidatable { Valid = true };
    //        a.DependsOn(c);
    //        a.DependsOn(b);
    //        c.Invalidate();
    //        Assert.That(a.Valid, Is.EqualTo(false));
    //    }
    //    [Test]
    //    public void InvalidateWorksInDepth()
    //    {
    //        var a = new Invalidatable {Valid = true};
    //        var b = new Invalidatable {Valid = true};
    //        a.DependsOn(b);
    //        var c = new Invalidatable();
    //        b.DependsOn(c);
    //        c.Invalidate();
    //        Assert.That(a.Valid, Is.EqualTo(false));
    //        Assert.That(b.Valid, Is.EqualTo(false));
    //    }
    //}
}