# Raygun.Diagnostics #

[![Join the chat at https://gitter.im/sirkirby/raygun.diagnostics](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/sirkirby/raygun.diagnostics?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) 

## master ##
[![Build Status](https://travis-ci.org/sirkirby/raygun.diagnostics.svg?branch=master)](https://travis-ci.org/sirkirby/raygun.diagnostics)

## develop ##
[![Build Status](https://travis-ci.org/sirkirby/raygun.diagnostics.svg?branch=develop)](https://travis-ci.org/sirkirby/raygun.diagnostics)

Allow you to use Raygun with your existing System.Diagnostics debug and trace code. 

## Supported Platforms

* .NET 4.5+ (Desktop / Server)

## Nuget ##

`PM> install-package Raygun.Diagnostics`

## Usage ##

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
		// the tags defined in the attribute will be automatically applied via reflection
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

Custom Raygun client settings (http://raygun.io/docs/Languages#net)
```c#
// the listener uses this staic instace of the raygun client
var client = Raygun.Diagnostics.Settings.Client;
// example for user tracking
client.User "current@userid.com";
// see other settings in the Raygun documentation
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
```

## Build ##
Using msbuild (v4+) against [Raygun.Diagnostics.csproj](src/Raygun.Diagnostics/Raygun.Diagnostics.csproj).


## Requirements ##
- Visual Studio 2012/2013
- .NET FX 4.5+

## Copyright & License ##

Copyright 2014 Chris Kirby
[MIT License](LICENSE.txt)

