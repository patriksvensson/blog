---
Title: How to dynamically add attributes to a class with Castle.Core
Slug: how-to-dynamically-add-attributes-to-a-class-with-castle-core
Published: 2013-08-15
Tags:
- .NET
- C#
- Unit testing
---

Yesterday I was working on some unit tests that ensured that some user derived classes passed to a method were decorated with a specific attribute. 

<!--excerpt-->

Since the mocking framework I used ([Moq][1]) did not support dynamically adding of attributes to a mocked type, my first approach was simply to create some private test classes that contained different permutations of the specified attribute, like this: 

    [TheAttribute(5)]
    public class FakeClassWithAnAttributeSetToANumberLessThanTen
    {
    }

    [TheAttribute(15)]
    public class FakeClassWithAnAttributeSetToANumberGreaterThanTen
    {
    }

    [TheAttribute(10, DefaultStuffz=Stuffz.Random)]
    public class FakeClassWithAnAttributeSetWithDefaultStuffz
    {
    }

Alas, as the amount of tests grew, I realized that this wasn't a viable solution.

Finally I found a quite nice solution by using [Castle.DynamicProxy][2] available in the Castle project. I have personally never used the Castle framework before, but the experience using it was quite nice.

    public class MyClass
    {
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class MyClassAttribute : Attribute
    {
    }

To create an instance of `MyClass`, dynamically decorated with a `MyClassAttribute` you simply do as below.

    public static MyClass CreateClassDecoratedWithAttribute()
    {
    	// Get the attribute constructor.
    	Type[] ctorTypes = Type.EmptyTypes;
    	var ctor = typeof(MyClassAttribute).GetConstructor(ctorTypes);
    	Debug.Assert(ctor != null, "Could not get constructor for attribute.");
    
    	// Create an attribute builder.
    	object[] attributeArguments = new object[] { };
    	var builder = new System.Reflection.Emit.CustomAttributeBuilder(ctor, attributeArguments);
    
    	// Create the proxy generation options.
        // This is how we tell Castle.DynamicProxy how to create the attribute.
    	var proxyOptions = new Castle.DynamicProxy.ProxyGenerationOptions();
    	proxyOptions.AdditionalAttributes.Add(builder);
    
    	// Create the proxy generator.
    	var proxyGenerator = new Castle.DynamicProxy.ProxyGenerator();
    
    	// Create the class proxy.
    	var classArguments = new object[] { };
    	return (MyClass)proxyGenerator.CreateClassProxy(typeof(MyClass), proxyOptions, classArguments);
    }

And now you can test that this actually works.

    [Fact]
    public void Created_Class_Should_Be_Decorated_With_Attribute()
    {
    	// Given
    	var myClass = CreateClassDecoratedWithAttribute();
    
    	// When
    	var attribute = myClass.GetType().GetCustomAttributes(typeof(MyClassAttribute), true)[0];
    
    	// Then
    	Assert.NotNull(attribute);
        Assert.IsType<MyClassAttribute>(attribute);
    }


  [1]: https://code.google.com/p/moq/
  [2]: http://www.castleproject.org/projects/dynamicproxy/