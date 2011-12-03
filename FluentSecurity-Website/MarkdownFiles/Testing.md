# Testing
Having a security configuration is not of much use unless you can verify that it is correct. That is what FluentSecurity's  test helpers are for. It enables you to **testdrive your applicaiton** like you would with any other part of your system. It can be used with any of the major testing frameworks (like **NUnit**, **MbUnit**, **xUnit.net** and **MS Test**). **"FluentSecurity.TestHelper"** is available as a separate nuget package.

## Verifying your configuration
Start by installing FluentSecurity.TestHelper in your test project. Make sure you have reference your favourite unit testing framework. Next you'll want to add a test fixture of some kind that can hold your expectations.

Before you can get to the current configuration you need to **make sure you have configured Fluent Security**. When that is out the way we can now get to the current configuration using **SecurityConfiguration.Current**.

Next we use the **Verify(expectations => {})** extensions method placing our expectations in the nested closure. Here's a short snippet of what that could look like using the Arrange, Act, Assert style:

```csharp
// Arrange
Bootstrapper.ConfigureSecurity();

// Act
var results = SecurityConfiguration.Current.Verify(expectations =>
{
	expectations.Expect<HomeController>().Has<IgnorePolicy>();
	expectations.Expect<AccountController>().Has<DenyAuthenticatedAccessPolicy>();
	expectations.Expect<AccountController>(x => x.LogOff())
		.Has<DenyAnonymousAccessPolicy>()
		.DoesNotHave<DenyAuthenticatedAccessPolicy>();
});

// Assert
... Put your assertions here ...
```

The Verify extension returns an IEnumerable<ExpectationResult> with an expectation result for each expectation. To find out if our expectations are met we use the extension method Valid(). The extension method ErrorMessages() will give you a string representation of any expectations that has not been met.

**Using your testing framework of choice you should assert that results.Valid() is true** and provide results.ErrorMessages() as the failure message for that assertion. You can find a full [NUnit example](http://www.fluentsecurity.net/getting-started) in the getting started guid.

## Recommendations
When testing your security configuration **you should be very specific about your configuration expectations**. That way you can with confidence apply your policies using methods like ForAllControllersInAssembly in your configuration without the risk of doing something stupid. So instead of using the `Expect<TController>()` extension **you should in most cases be using the overload that lets you specify a controller action `Expect<TController>(x => x.SomeAction())`**.