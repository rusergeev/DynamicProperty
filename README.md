# DynamicProperty

Basic Information

The goal of this quiz is two-fold:

1.	Implement dynamic properties which send out notifications when they are modified.
2.	Implement dynamic properties which are composed of other dynamic properties.

Included in this archive are 5 source files:

•	IDYNAMICPROPERTY{T}.CS – Defines the interface that must be implemented.

•	DYNAMICPROPERTY.CS – Static factory methods for creating dynamic property instances.  You need to provide implementations for the two factory methods.

•	TESTSET1BASIC.CS – NUnit unit tests which test the core functionality required of your dynamic property implementation.  The tests are ordered in increasing difficulty.  These tests are all single-threaded and it is possible to pass all of these tests without the use of any multi-threading concepts or synchronization primitives.

•	TESTSET2INTERMEDIATE.CS – NUnit unit tests that are more difficult to pass.  The first test is an efficiency test.  The second test tests that your implementation is Conditionally thread safe and can be used in a multi-threaded application as long as the threads do not concurrently access the same dynamic properties.  Passing this test requires minimal knowledge of multi-threaded programming concepts.

•	TESTSET3ADVANCED.CS – Nunit unit tests that are significantly more difficult to pass.  These tests test that your implementation is Thread safe and multiple threads can concurrently access the same dynamic properties with deterministic results.  Passing these tests require significant knowledge of multi-threaded programming concepts.
 
## What are Dynamic Properties?

At their most basic, they are just a value that can be observed for changes:

```C#
 var p = DynamicProperty.Create(42);
 p.Value; // 42
 p.Subscribe(v => Console.WriteLine(v)); // callback will be called whenever p is modified
 p.Value = 99; // prints 99
 p.Value = -1; // prints -1
 p.Value; // -1
```

 The real power is in the calculated dynamic properties, which allow you to define dynamic properties which are composed of other dynamic properties:
 ```C#
 var fraction = DynamicProperty.Create(0.5);
 var percent = DynamicProperty.Create(() => fraction.Value * 100, p => fraction.Value = p / 100);
 var displayUnits = DynamicProperty.Create("fraction");
 var displayValue = DynamicProperty.Create(
     () => displayUnits.Value == "fraction" ? fraction.Value : percent.Value,
     v =>
         {
             if (displayUnits.Value == "fraction") fraction.Value = v;
             else percent.Value = v;
         });

 var subscription = displayValue.Subscribe(Console.WriteLine); // prints out the display value whenever it changes
 fraction.Value = 0.25; // prints 0.25
 percent.Value = 30; // prints 0.3
 displayUnits.Value = "percent"; // prints 30
 displayValue.Value = 100; // prints 100
 fraction.Value; // 1
 percent.Value; // 100

 subscription.Dispose(); // stop listening
 percent.Value = 10; // nothing prints
 ```
The above code samples show the basic usage of dynamic properties and how they can be composed.  Notice that the composition is two-way.  When we create a calculated dynamic property, we provide a method that calculates its value and we also supply a method which is used to update its value (by updating the dynamic properties it depends upon).  The code sample also shows that the values are kept up to date automatically.  Whenever any of the dynamic properties used in the calculation change, the calculation is automatically run again.
