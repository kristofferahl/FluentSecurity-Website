# Using Fluent Security with an IoC container
Fluent Security supports dependency injection through an [Inversion of Control](http://martinfowler.com/articles/injection.html) (IoC)-container. By hooking in an IoC-container you get access to even more useful features in Fluent Security. Some of the features include:

* Use of custom policy violation handlers.
* Overriding the naming convention used when locating policy violation handlers.
* Creating custom diagnostics output.

More features will be added over time, including support for dependency injection into your custom policies.

## Setting up IoC container integration
There's two options for registering your IoC-container with Fluent Security. You can choose to provide an instance that implements the ISecurityServiceLocator interface or simply pass in a `System.Func<Type,IEnumerable<object>>` to *ResolveServicesUsing*.

### Option 1 - Func\<Type,IEnumerable\<object>>
The easiest way to get started is telling Fluent Security to use a Func to resolve instances from.
ResolveServicesUsing takes a Func\<Type,IEnumerable\<object>> in and an optional parameter of Func\<Type,object>.
Here's what it would look like using the [Common Service Locator](http://commonservicelocator.codeplex.com/).

```csharp
SecurityConfigurator.Configure(configuration =>
{
	configuration.ResolveServicesUsing(ServiceLocator.Current.GetAllInstances);
});
```

In this case, whenever Fluent Security needs an instance it will ask your container for that instance. If your container returns no instances, Fluent Security will fall back to using the default instances registered in the internal container. If your container returns multiple instances for a requested type, Fluent Security will select the first one. There's two ways you can ensure that Fluent Security gets the correct instance. 1) Make sure your instances are returned in the correct order. 2) Use the following overload of ResolveServicesUsing.

```csharp
SecurityConfigurator.Configure(configuration =>
{
	configuration.ResolveServicesUsing(
		ServiceLocator.Current.GetAllInstances,
		ServiceLocator.Current.GetInstance
		);
});
```

Here's where it gets a bit tricky. If you are fine with registering everything Fluent Security needs in your container, this should work well. But if you want to rely on the defaults of the framework your IoC-container must not throw an exception when you request an instance that isn't registered. Most IoC-containers have a method that is intended for this. In StructureMap it is called TryGetInstance and using it would look something like this:

```csharp
SecurityConfigurator.Configure(configuration =>
{
	configuration.ResolveServicesUsing(
		type => ObjectFactory.GetAllInstances(type).Cast<object>(),
		type => ObjectFactory.TryGetInstance(type)
		);
});
```

### Option 2 - ISecurityServiceLocator
If you don't like the feel of using Func or simple want more control, you can create your own ISecurityServiceLocator and pass that instance to ResolveServicesUsing. `ResolveServicesUsing(ISecurityServiceLocator servicesLocator)`

Here's an example of a service locator using StructureMap:

```csharp
public class FluentSecurityServiceLocator : ISecurityServiceLocator
{
	public object Resolve(Type typeToResolve)
	{
		return ObjectFactory.TryGetInstance(typeToResolve);
	}

	public IEnumerable<object> ResolveAll(Type typeToResolve)
	{
		return ObjectFactory.GetAllInstances(typeToResolve).Cast<object>();
	}
}
```

Here's what it would look like using the above service locator class:

```csharp
SecurityConfigurator.Configure(configuration =>
{
	configuration.ResolveServicesUsing(new FluentSecurityServiceLocator());
});
```

This approach can be a good way for you to explore what Fluent Security does under the cover and what types you can replace with your own implementations.

## Gotcha's
* If you implement ISecurityServiceLocator, you must implement both methods fully!