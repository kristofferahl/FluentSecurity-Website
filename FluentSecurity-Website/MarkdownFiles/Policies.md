# Policies

Fluent Security provides a small set of policies that should be enough for the basic scenarios.

## Built in policies

### DenyAnonymousAccessPolicy
The user must be authenticated. Requires no specific role.

### DenyAuthenticatedAccessPolicy
The user must be anonymous.

### RequireRolePolicy
The user must be authenticated with one or more of the specified roles.

### RequireAllRolesPolicy _(v. 1.3.0+)_
The user must be authenticated with all of the specified roles.

### IgnorePolicy
All users are allowed.

### DelegatePolicy _(v. 1.3.0+)_
The specified delegate must return true or a success result (PolicyResult).

The DelegatePolicy gives you a quick and easy way of adding custom policies to your configuration. However, **it is not the recommended way to apply policies** even if you don't intend to reuse the same policy elsewhere. In most cases you should still create custom policies by implementing the ISecurityPolicy interface (see below documentation). **Don't be lazy!**

The DelegatePolicy has a few overloads taking a number of different arguments:

* A *unique* name. This is the name you setup expectations for in your tests. (required)
* A policy delegate that returns a boolean value or a PolicyResult. (required)
* A violation handler delegate that returns an ActionResult. (optional).
* A failure message (optional)

If no violation handler delegate is provided, the default convention is used to try and identify a IPolicyViolationHandler by using the name given for the DelegatePolicy. [Read more about policy violation handlers](Policy-violation-handlers).

Here's a simple example of how you could add a DelegatePolicy to SystemMonitorController that only allows local requests.

```csharp
configuration.For<SystemMonitorController>().DelegatePolicy("LocalOnlyPolicy",
	context => HttpContext.Current.Request.IsLocal
);
```

## Custom policies
If needed, you can create your own policies by implementing the interface ISecurityPolicy. You can then add the policy to your configuration like this:
`configuration.For<SomeController>().AddPolicy(new MyCustomPolicy());`

### LocalOnlyPolicy example
Here's an example of a policy that could be used to limit access to a controller to local requests.

```csharp
public class LocalOnlyPolicy : ISecurityPolicy
{
	public PolicyResult Enforce(ISecurityContext context)
	{
		return HttpContext.Current.Request.IsLocal ?
			PolicyResult.CreateSuccessResult(this) :
			PolicyResult.CreateFailureResult(this, "Access denied!");
	}
}
```

## Gotcha's
At this point in time we won't cache policy results, meaning that you should be careful what you put in your policies. It is probably not a good idea to do lots of database calls in here. In the standard scenario you can expect a single call per request. Caching of results will be added in an upcoming release of Fluent Security.