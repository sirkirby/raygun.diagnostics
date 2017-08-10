# Raygun.Diagnostics #

Enables fully featured Raygun crash reporting using your existing System.Diagnostics debug and trace code.

[![Join the chat at https://gitter.im/sirkirby/raygun.diagnostics](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/sirkirby/raygun.diagnostics?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## master ##
[![Build status](https://ci.appveyor.com/api/projects/status/7bk54tsjxp2cakr5/branch/master?svg=true)](https://ci.appveyor.com/project/Authenticom/raygun-diagnostics/branch/master)

## develop ##
[![Build status](https://ci.appveyor.com/api/projects/status/7bk54tsjxp2cakr5/branch/master?svg=true)](https://ci.appveyor.com/project/Authenticom/raygun-diagnostics/branch/develop)

## Supported Platforms

* .NET 3.5
* .NET 4.0
* [.NET Standard 2.0](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md)

## Nuget ##

`PM> install-package Raygun.Diagnostics`

## Raygun Configuration ##
Add the following to your `<configSections>`

```xml
<section name="RaygunSettings" type="Mindscape.Raygun4Net.RaygunSettings, Mindscape.Raygun4Net" />
```
Add anywhere under `<configuration>`
``` xml
<RaygunSettings apikey="myAppKey" />
```

### OR set your key in code ###

```c#
Settings.ApiKey = "myAppKey"
```

## Trace Listener Configuration ##
```xml
<!-- package install will add the listener to your debug settings -->
<system.diagnostics>
<trace>
  <listeners>
    <add name="RaygunTraceListener" type="Raygun.Diagnostics.RaygunTraceListener, Raygun.Diagnostics" />
  </listeners>
</trace>
</system.diagnostics>
```

## Usage ##

```c#
try {}
catch (Exception e)
{
	// any arg of type IList<string> will be treated as tags and any type of IDictionary will be treated as custom data
	// any arg of type Exception will be wrapped in the primary raygun exception object
	var customData = new Dictionary<string, object> {{"myType", someParameter}};
	var tags = new List<string> { "tag1", "tag1" };
	Trace.TraceError("Something bad happened", e, tags, customData);
	// or inline with arg just added to a default Dictionary<object, object> object
	Trace.TraceError("something bad happened", e, new List<string> {"tag1"}, someParameter, someObject);
}
```
Tag via [RaygunDiagnosticsAttribute](src/Raygun.Diagnostics/RaygunDiagnosticsAttribute.cs) on any class or method:

```c#
[RaygunDiagnostics("tag1", "tag2")]
public void MyMethod()
{
	try{}
	catch(Exception e)
	{
		// the tags defined in the attribute will pass through to Raygun
		Trace.TraceError("something bad happened", e, someParameter, someObject);
	}
}
```

```c#
// just send a standard formatted error messages with no custom data or tags (unless auto tag is enabled)
Trace.TraceError("Some formatted error message on {0} for {1}", something, otherThing);
Trace.TraceWarning("Some warning information on {0} for {1}", something, otherThing);
// this message will be ignored
Trace.TraceInformation("Ignored by the raygun listener");
```

Set default [User](https://raygun.com/blog/2014/01/unique-user-tracking-for-exceptions-and-crashes/) via [RaygunDiagnosticsUserAttribute](src/Raygun.Diagnostics/RaygunDiagnosticsUserAttribute.cs) on any class or method or pass via custom typed or anonymous argument:

```c#
[RaygunDiagnosticsUser("username", "userId")]
public void MyMethod()
{
	try{}
	catch(Exception e)
	{
		// the user defined in the attribute will pass through to Raygun
		Trace.TraceError("something bad happened", e, someParameter, someObject);
		// pass the user as an arg via the built in MessageInfo object
		Trace.TraceError("something bad happened", e, new UserInfo(user.Username) { Fullname = user.FullName, Email = user.Email, Id = user.Id) });
        // pass user as an anonymous type to avoid taking dependency on the library. anon type must contain a property called Username and optionally contain Email, FullName, FirstName, Id, and IsAnonymous
        Trace.TraceError("something bad happened", e, new { user.Username, user.FullName, user.Email, user.Id) });
	}
}
```
If using an anonymous type for user info (recommended), use these specific property names. Only `Username` (case insensitive) is required for type identification. See unit tests for more details.
```c#
new {
  user.Id  
  user.Username, 
  user.FullName,
  user.FirstName,
  user.Email,  
  user.IsAnonymous
}
```

Custom Raygun client settings (http://raygun.io/docs/Languages#net)
```c#
// the listener uses this staic instace of the raygun client
var client = Raygun.Diagnostics.Settings.Client;
```

Custom error grouping is available by using an anonymous type with a "GroupKey" property
```c#
public void MyMethod()
{
	try{}
	catch(Exception e)
	{
		// aggregate all errors of type MyCustomGroupKey together
        Trace.TraceError("something bad happened", e, new { GroupKey = "MyCustomGroupKey" });
	}
}

```

Other settings
```c#
// enable debug specific options for tracing
Raygun.Diagnostics.Settings.Debug = true;
// set the trace level to Warning and higher (default:Error)
Raygun.Diagnostics.Settings.MessageTraceLevel = MessageTraceLevel.Warning;

// in app example configuration
Raygun.Diagnostics.Settings.Client.AddWrapperExceptions(new List<Type> { typeof(MyCustomWrapperException) }.ToArray());
#if DEBUG
	Raygun.Diagnostics.Settings.Debug = true;
#endif

// add default tags to every message generated by app domain
Raygun.Diagnostics.Settings.DefaultTags = new[] {"debug", "someDynamicString"};
```
## Build ##
`dotnet build Raygun.Diagnostics.sln`

## Requirements ##

- Visual Studio 2017+
- .NET FX 3.5+

## Copyright & License ##

Copyright 2016 Chris Kirby
[MIT License](LICENSE.txt)
