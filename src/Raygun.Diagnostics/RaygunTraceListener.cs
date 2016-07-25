using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Mindscape.Raygun4Net.Messages;
using Mindscape.Raygun4Net;
using Raygun.Diagnostics.Helpers;
using Raygun.Diagnostics.Models;

namespace Raygun.Diagnostics
{
  public delegate void Grouped(object sender, RaygunCustomGroupingKeyEventArgs e, IMessageGroup group);

  public class RaygunTraceListener : TraceListener
  {

    public event Grouped OnGrouping;

    /// <summary>
    /// Checks for other trace listeners
    /// </summary>
    private static bool NotAlone()
    {
      // see if others are listening
      return (Trace.Listeners != null && Trace.Listeners["RaygunTraceListener"] != null && Trace.Listeners.Count > 1);
    }

    /// <summary>
    /// Emits an error message to Raygun.
    /// </summary>
    /// <param name="message">A message to emit.</param>
    public override void Fail(string message)
    {
      WriteMessage(MessageFromString(message, null, TraceEventType.Error));
    }

    /// <summary>
    /// Emits an error message and a detailed error message to Raygun.
    /// </summary>
    /// <param name="message">A message to emit.</param>
    /// <param name="detailMessage">A detailed message to emit.</param>
    public override void Fail(string message, string detailMessage)
    {
      WriteMessage(MessageFromString(message, detailMessage, TraceEventType.Error));
    }

    /// <summary>
    /// Writes trace information, a data object and event information to the Raygun.
    /// </summary>
    /// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache" /> object that contains the current process ID, thread ID, and stack trace information.</param>
    /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
    /// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType" /> values specifying the type of event that has caused the trace.</param>
    /// <param name="id">A numeric identifier for the event.</param>
    /// <param name="data">The trace data to emit.</param>
    /// <PermissionSet>
    ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
    /// </PermissionSet>
    public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id,
      object data)
    {
      TraceData(eventCache, source, eventType, id, new[] {data});
    }

    /// <summary>
    /// Writes trace information, an array of data objects and event information to Raygun.
    /// </summary>
    /// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache" /> object that contains the current process ID, thread ID, and stack trace information.</param>
    /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
    /// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType" /> values specifying the type of event that has caused the trace.</param>
    /// <param name="id">A numeric identifier for the event.</param>
    /// <param name="data">An array of objects to emit as data.</param>
    /// <PermissionSet>
    ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
    /// </PermissionSet>
    public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id,
      params object[] data)
    {
      if ((Filter != null) && !Filter.ShouldTrace(eventCache, source, eventType, id, null, null, null, data)) return;
      var message = string.Format("{0} trace event", eventType);
      WriteMessage(MessageFromTraceEvent(eventCache, source, eventType, id, message, data));
    }

    /// <summary>
    /// Writes trace and event information to Raygun.
    /// </summary>
    /// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache" /> object that contains the current process ID, thread ID, and stack trace information.</param>
    /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
    /// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType" /> values specifying the type of event that has caused the trace.</param>
    /// <param name="id">A numeric identifier for the event.</param>
    /// <PermissionSet>
    ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
    /// </PermissionSet>
    public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
    {
      var message = string.Format("{0} trace event", eventType);
      TraceEvent(eventCache, source, eventType, id, message);
    }

    /// <summary>
    /// Writes trace information, a message, and event information to Raygun.
    /// </summary>
    /// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache" /> object that contains the current process ID, thread ID, and stack trace information.</param>
    /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
    /// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType" /> values specifying the type of event that has caused the trace.</param>
    /// <param name="id">A numeric identifier for the event.</param>
    /// <param name="message">A message to write.</param>
    /// <PermissionSet>
    ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
    /// </PermissionSet>
    public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id,
      string message)
    {
      if ((Filter != null) && !Filter.ShouldTrace(eventCache, source, eventType, id, message, null, null, null)) return;
      WriteMessage(MessageFromTraceEvent(eventCache, source, eventType, id, message, null));
    }

    /// <summary>
    /// Writes trace information, a formatted array of objects and event information to Raygun.
    /// </summary>
    /// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache" /> object that contains the current process ID, thread ID, and stack trace information.</param>
    /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
    /// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType" /> values specifying the type of event that has caused the trace.</param>
    /// <param name="id">A numeric identifier for the event.</param>
    /// <param name="format">A format string that contains zero or more format items, which correspond to objects in the <paramref name="args" /> array.</param>
    /// <param name="args">An object array containing zero or more objects to format.</param>
    /// <PermissionSet>
    ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
    /// </PermissionSet>
    public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id,
      string format, params object[] args)
    {
      if ((Filter != null) && !Filter.ShouldTrace(eventCache, source, eventType, id, format, args, null, null)) return;
      // look at the format and decide how to handle the message
      // allows for an easy work around for using the Tace.TraceError method for passing custom argument data
      WriteMessage(Regex.IsMatch(format, @"\{[0-9]+\}")
        ? MessageFromTraceEvent(eventCache, source, eventType, id, String.Format(format, args), null)
        // just formatted text
        : MessageFromTraceEvent(eventCache, source, eventType, id, format, args));
        // treat args as additional custom information
    }

    /// <summary>
    /// When overridden in a derived class, writes the specified message to the listener you create in the derived class.
    /// </summary>
    /// <param name="message">A message to write.</param>
    public override void Write(string message)
    {
      WriteMessage(MessageFromString(message));
    }

    /// <summary>
    /// When overridden in a derived class, writes a message to the listener you create in the derived class, followed by a line terminator.
    /// </summary>
    /// <param name="message">A message to write.</param>
    public override void WriteLine(string message)
    {
      WriteMessage(MessageFromString(message));
    }

    /// <summary>
    /// Sends the message to Raygun
    /// </summary>
    /// <param name="message">The message.</param>
    public virtual void WriteMessage(MessageContext message)
    {
      if (message == null || Settings.Debug) return;
      try
      {
        // add default tags to every send if specified
        if (Settings.DefaultTags != null)
          message.Tags.AddRange(Settings.DefaultTags);

        //Set the event handler to automatically handle custom grouping
        if (message.Group != null)
        {
          Settings.Client.CustomGroupingKey += (sender, e) => { HandleGrouping(sender, e, message.Group); };
        }

        Settings.Client.Send(message.Exception, message.Tags, message.Data, message.GetRaygunUser());
      }
      catch (Exception e)
      {
        if (NotAlone()) // if someone else is listening, then trace the error
          Trace.TraceError("Error on Raygun Send() : {0}", e.Message);
      }
    }

    /// <summary>
    /// Method used to bind to the CustomGroupingKey event of the RaygunClient object
    /// </summary>
    /// <param name="sender">the Raygun client object firing the event</param>
    /// <param name="e">The custom group arguments of the RaygunClient message</param>
    /// <param name="group">The message group that contains the data to tell the Raygun client how to group the message</param>
    private void HandleGrouping(object sender, RaygunCustomGroupingKeyEventArgs e, IMessageGroup group)
    {
      //Only override the grouping if specified
      if (!String.IsNullOrEmpty(group.GroupKey))
      {
        e.CustomGroupingKey = group.GroupKey;
      }

      OnGrouping?.Invoke(sender, e, group);
    }

    /// <summary>
    /// Generates a message context from a trace event message
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="detail">The detail.</param>
    /// <param name="eventType">Type of the event.</param>
    /// <returns>MessageContext.</returns>
    public virtual MessageContext MessageFromString(string message, string detail = null,
      TraceEventType eventType = TraceEventType.Information)
    {
      try
      {
        if (!eventType.IsValid())
          return null;

        //Create default message
        if (string.IsNullOrEmpty(message)) message = DefaultMessage();

        var tags = new List<string>();
        // get tags from the stack trace
        tags.AddRange(GetAttributeTags());
        // get user from stack trace
        var attributeUser = GetAttributeUser();

        return new MessageContext(new Exception($"{message}. {detail}"), tags, user: attributeUser);
      }
      catch (Exception e)
      {
        if (NotAlone()) // if someone else is listening, then trace the error
          Trace.TraceError("Error on MessageFromString in RaygunTraceListener : {0}", e.Message);
        return new MessageContext(e);
      }
    }

    /// <summary>
    /// Generates a message context from a trace event.
    /// </summary>
    /// <param name="eventCache">The event cache.</param>
    /// <param name="source">The source.</param>
    /// <param name="eventType">Type of the event.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="message">The message.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>MessageContext.</returns>
    public virtual MessageContext MessageFromTraceEvent(TraceEventCache eventCache, string source,
      TraceEventType eventType, int id, string message, params object[] args)
    {
      try
      {
        if (!eventType.IsValid())
          return null;

        //Create default message
        if (string.IsNullOrEmpty(message)) message = DefaultMessage();
        
        var context = new MessageContext(new Exception(message), new List<string>(), new Dictionary<object, object>(),
        new UserInfo(AppDomain.CurrentDomain.FriendlyName) {IsAnonymous = true});

        // get tags from the stack trace
        context.Tags.AddRange(GetAttributeTags());
        // get user from the stack trace
        var attributeUser = GetAttributeUser();
        if (attributeUser != null)
          context.User = attributeUser;

        if (args != null)
        {
          var localArgs = args.Where(a => a != null).ToList();
          // check the args for custom data
          var custom = localArgs.FirstOrDefault(a => a is IDictionary);
          if (custom != null)
          {
            context.Data = (IDictionary) custom;
            localArgs.Remove(custom);
          }

          // check the args for tags
          var tags = localArgs.FirstOrDefault(a => a is IList<string>);
          if (tags != null)
          {
            context.Tags.AddRange((IList<string>) tags);
            localArgs.Remove(tags);
          }

          // check the args for a custom exception
          var error = localArgs.FirstOrDefault(a => a is Exception);
          if (error != null)
          {
            // use the arg exception for raygun and pass the message as custom data
            context.Exception = (Exception) error;
            context.Data.Add("Message", message);
            localArgs.Remove(error);
          }
          else
          {
            // wrap the trace message as the exception
            context.Exception = new Exception(message);
          }

          // check for user information
          var user = localArgs.FirstOrDefault(a => a.HasProperty("username"));
          if (user != null)
          {
            object username;
            user.TryGetPropertyValue("username", out username);
            object userId;
            user.TryGetPropertyValue("id", out userId);
            object userEmail;
            user.TryGetPropertyValue("email", out userEmail);
            object userFullName;
            user.TryGetPropertyValue("fullname", out userFullName);
            object userFirstName;
            user.TryGetPropertyValue("firstname", out userFirstName);
            object userIsAnonymous;
            user.TryGetPropertyValue("isAnonymous", out userIsAnonymous);

            context.User = new UserInfo(username?.ToString())
            {
              Id = userId?.ToString(),
              Email = userEmail?.ToString(),
              FullName = userFullName?.ToString(),
              FirstName = userFirstName?.ToString()
            };
          }
          else
          {
            user = localArgs.FirstOrDefault(a => a is IUserInfo);
            if (user != null)
            {
              context.User = (IUserInfo) user;
            }
          }

          var grouping = localArgs.FirstOrDefault(a => a.HasProperty("groupkey"));
          if (grouping != null)
          {
            context.Group = GetGrouping(grouping);
            localArgs.Remove(grouping);
          }

          // add the rest
          var count = 0;
          foreach (var leftover in localArgs)
            context.Data.Add($"arg-{count++}", leftover);
        }

        return context;
      }
      catch (Exception e)
      {
        if (NotAlone()) // if someone else is listening, then trace the error
          Trace.TraceError("Error on MessageFromTraceEvent in RaygunTraceListener : {0}", e.Message);
        return new MessageContext(e);
      }
    }

    /// <summary>
    /// Gets the user info from the custom attribute
    /// </summary>
    /// <returns>IUserInfo.</returns>
    public static IUserInfo GetAttributeUser()
    {
      // walk up the stack and check for custom attribute tags
      var st = new StackTrace();
      var stackFrames = st.GetFrames();
      if (stackFrames == null) return null;

      foreach (
        var method in (from frame in stackFrames where frame != null select frame.GetMethod() into method select method)
        )
      {
        var m = method;
#if NET46
        var classAttr = m.ReflectedType?.GetCustomAttribute(typeof(RaygunDiagnosticsUserAttribute));
        var methodAttr = m.GetCustomAttribute(typeof(RaygunDiagnosticsUserAttribute));
#else
        var classAttr = m.ReflectedType != null ? m.ReflectedType.GetCustomAttributes(typeof(RaygunDiagnosticsUserAttribute), false).FirstOrDefault() : null;
        var methodAttr = m.GetCustomAttributes(typeof(RaygunDiagnosticsUserAttribute), false).FirstOrDefault();
#endif

        // take the first instance of the attribute, starting with methods
        if (methodAttr != null)
          return ((RaygunDiagnosticsUserAttribute) methodAttr).User;
        if (classAttr != null)
          return ((RaygunDiagnosticsUserAttribute) classAttr).User;

      }
      return null;
    }

    public static IMessageGroup GetGrouping(object group)
    {
      object groupKey = new object();
      var grouping = new MessageGroup();

      if (group.TryGetPropertyValue("groupkey", out groupKey))
      {
        grouping.GroupKey = groupKey.ToString();
      }

      return grouping;
    }

    /// <summary>
    /// Gets all tags defined using the custom attribute on the curent stack trace
    /// </summary>
    /// <returns>IEnumerable&lt;System.String&gt;.</returns>
    public static IEnumerable<string> GetAttributeTags()
    {
      // walk up the stack and check for custom attribute tags
      var st = new StackTrace();
      var stackFrames = st.GetFrames();
      if (stackFrames == null) yield break;

      foreach (
        var method in (from frame in stackFrames where frame != null select frame.GetMethod() into method select method)
        )
      {
        var m = method;
#if NET46
        var classAttr = m.ReflectedType?.GetCustomAttribute(typeof(RaygunDiagnosticsAttribute));
        var methodAttr = m.GetCustomAttribute(typeof(RaygunDiagnosticsAttribute));
#else
        var classAttr = m.ReflectedType != null ? m.ReflectedType.GetCustomAttributes(typeof(RaygunDiagnosticsAttribute), false).FirstOrDefault() : null;
        var methodAttr = m.GetCustomAttributes(typeof(RaygunDiagnosticsAttribute), false).FirstOrDefault();
#endif
        // build a list of tags from all method and class attributes in the stack trace
        var tags = new List<string>();
        if (classAttr != null)
          tags.AddRange(((RaygunDiagnosticsAttribute) classAttr).Tags);
        if (methodAttr != null)
          tags.AddRange(((RaygunDiagnosticsAttribute) methodAttr).Tags);
        foreach (var tag in tags)
          yield return tag;
      }
    }

    public static IDictionary<string, string> GetStackTraceDefaultMessageInfo()
    {
      var stackInfo = new Dictionary<string, string>();
      var st = new StackTrace(true);
      var stackFrames = st.GetFrames();
      if (stackFrames == null) return new Dictionary<string, string>();

      var stack = stackFrames.ToList().FirstOrDefault(f => !f.GetMethod().ReflectedType.Module.Name.Contains("Raygun.Diagnostics.dll") 
                                                            && !f.GetMethod().ReflectedType.Module.Name.Contains("System"));
      var method = stack.GetMethod();

      stackInfo.Add("MethodName", method.Name);
      stackInfo.Add("ClassName", method.ReflectedType?.Name);
      stackInfo.Add("LineNumber", stack.GetFileLineNumber().ToString());

      return stackInfo;
    }

    private static string DefaultMessage()
    {
      //Retrieve stack trace info for default message
      var stackInfo = GetStackTraceDefaultMessageInfo();
      return $"An unexpected error occurred while calling {stackInfo["MethodName"]} in {stackInfo["ClassName"]} at line {stackInfo["LineNumber"]}.";
    }

  }
}
