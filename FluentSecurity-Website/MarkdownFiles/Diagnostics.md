# Diagnostics
Since the very early versions of Fluent Security it has had the ability to help you get a good overview of your configuration using the WhatDoIHave method on ISecurityConfiguration. Recently we also added a [Glimpse](http://getglimpse.com/) plugin that will give you a super awesome overview of your configuration. Here's how it all works.

## WhatDoIHave
`SecurityConfiguration.Current.WhatDoIHave()` will return a string representing the current configuration. You could write the result to a console or choose to print it as HTML. The choice is yours.

## Glimpse plugin
Fluent Security has a plugin for Glimpse, the very popular, firebug-like framework, that lets you debug your server using a browser. The [FluentSecurity.Glimpse NuGet package](http://nuget.org/List/Packages/FluentSecurity.Glimpse) will provide roughly the same output as WhatDoIHave but in a much more pretty way. Moving forward my guess is that all energy will go into extending this plugin rather then extending the WhatDoIHave method.

**Here's how you get started using the plugin**:

* **Install FluentSecurity.Glimpse** from NuGet in your MVC project
* Compile and run it
* Navigte to glimpse.axd and make sure glimpse is turned on
* Go back to your main paige
* Click the little eye in your bottom right corner
* **Select the Fluent Security tab** and there you go...