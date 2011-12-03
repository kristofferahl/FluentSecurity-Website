# Policy violation handlers
When a policy is violated, Fluent Security will throw a PolicyViolationException. The PolicyViolationException will have a Message as well as the type (PolicyType) of the policy that was violated . You can catch the exception in your application and deal with it any way you want. While this will get you started securing your controllers this is probably not what you want do in the long run. Instead you want to create and register an IPolicyViolationHandler for your policy.

## Creating a policy violation handler
Create a class and implement the IPolicyViolationHandler interface. There's really not much you have to do. The only requirement is that you return an ActionResult of some kind. Any class inheriting from ActionResult can be returned so HttpUnauthorizedResult, JsonResult, RedirectResult and ViewResult will all work. More about ActionResult's can be found here: http://msdn.microsoft.com/en-us/library/system.web.mvc.actionresult.aspx

### Naming your handler
Fluent Security uses a naming convention to locate the correct policy violation handler. Name your handler according to this format: **\<PolicyName\>ViolationHandler.**

So if you want to create a handler for your "*LocalOnlyPolicy*" the name of the handler must be "*LocalOnlyPolicyViolationHandler*". If you want a handler for the "*DenyAnonymousAccessPolicy*" you must name it "*DenyAnonymousAccessPolicyViolationHandler*".

If you don't like this convention you can create your very own policy violation handler selector by implementing the *IPolicyViolationHandlerSelector* interface and register it in your IoC-container. 

### Example policy violation handler
```csharp
public class LocalOnlyPolicyViolationHandler : IPolicyViolationHandler
{
	public ActionResult Handle(PolicyViolationException exception)
	{
		return new HttpUnauthorizedResult(exception.Message);
	}
}
```

## Registering your policy violation handlers
To get Fluent Security to use your custom violation handlers you need to register them. At this time, the only way to do this is by using an IoC-container. The following examples use Jeremy Miller's excellent [StructureMap](http://structuremap.net/structuremap/) but you can use any container you want. Here's the steps you need to take:

### 1. Register the violation handlers in your container
Below is an example of registering all violation handlers in the calling assembly using StructureMap.

```csharp
public class WebRegistry : Registry
{
	public WebRegistry()
	{
		Scan(scan =>
		{
			scan.TheCallingAssembly();
			scan.AddAllTypesOf<IPolicyViolationHandler>();
		});
	}
}
```

### 2. Register your IoC-container in Fluent Security
You register your IoC-container by telling Fluent Security where to resolve instances from. Again, below is an example using StructureMap:

```csharp
configuration.ResolveServicesUsing(type => ObjectFactory.GetAllInstances(type).Cast<object>());
```

That's it! Fluent Security will now use the appropriate violation handler when a policy violation occurs.
Read more about [using Fluent Security with an IoC container](IoC-container-integration).

## Gotcha's
* Fluent Security uses a naming convention to locate the correct policy violation handler. See "Naming your handler".
* Don't forget to bootstrap your IoC-container before trying to use it in Fluent Security.