# Release notes
See the commit history for the full release notes of each release.

## Fluent Security 1.4.0
 * Added RemovePolicy\<TSecurityPolicy\>(predicate)
 * Marked RemovePoliciesFor as obsolete
 * Added support for changing the default PolicyExecutionMode to "stop on first violation"
 * Added missing .Clear() in DefaultPolicyAppender for RequireAllRolesPolicy

_Released 2011-12-02_. Thanks to [Vish](http://twitter.com/vishcious/)

## Fluent Security 1.3.0
 * Added new policy (RequireAllRolesPolicy)
 * Added support for using delegate policies (DelegatePolicy)
 * Added Has\<T\>(predicate) and DoesNotHave\<T\>(predicate) expectation helpers
 * Added support for scanning for all controllers in namespace of a type  ForAllControllersInNamespaceContainingType
 * Marked PolicyViolationException\<T\> as depricated

_Released 2011-11-09_. Thanks to [Vish](http://twitter.com/vishcious/) and [Marius Schulz](http://twitter.com/MariusSchulz)

## Fluent Security 1.2.0
 * Added support Areas (Thanks to [Bas ter Vrugt](http://twitter.com/bastervrugt))
 * Fix for VB.NET compatibility (Thanks to [Christiaan Baes](http://twitter.com/chrissie1))

_Released 2011-08-09_.

## Fluent Security 1.1.0
+ Added support for scanning for all controllers in:
 * The calling assembly (ForAllControllers)
 * A specified assembly (ForAllControllersInAssembly)
 * Assembly containing type (ForAllControllersInAssemblyContainingType)

_Released 2011-07-04_.

## Fluent Security 1.0
_Released 2011-06-19_. 