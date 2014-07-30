using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Raygun.Diagnostics.Helpers;
using Raygun.Diagnostics.Models;

namespace Raygun.Diagnostics
{
  public class RaygunTraceListener : TraceListener
  {
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
    public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
    {
      TraceData(eventCache, source, eventType, id, new[] { data });
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
    public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
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
    public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
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
    public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
    {
      if ((Filter != null) && !Filter.ShouldTrace(eventCache, source, eventType, id, format, args, null, null)) return;
      // look at the format and decide how to handle the message
      // allows for an easy work around for using the Tace.TraceError method for passing custom argument data
      WriteMessage(Regex.IsMatch(format, @"\{[0-9]+\}") 
        ? MessageFromTraceEvent(eventCache, source, eventType, id, String.Format(format, args), null)  // just formatted text
        : MessageFromTraceEvent(eventCache, source, eventType, id, format, args)); // treat args as additional custom information
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
    protected virtual void WriteMessage(MessageContext message)
    {
      if (message == null || Settings.Debug) return;
      try
      {
        Settings.Client.Send(message.Exception, message.Tags, message.Data);
      }
      catch (Exception e)
      {
        if (NotAlone()) // if someone else is listening, then trace the error
          Trace.TraceError("Error on Raygun Send() : {0}", e.Message);
      }
    }

    /// <summary>
    /// Generates a message context from a trace event message
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="detail">The detail.</param>
    /// <param name="eventType">Type of the event.</param>
    /// <returns>MessageContext.</returns>
    protected virtual MessageContext MessageFromString(string message, string detail = null, TraceEventType eventType = TraceEventType.Information)
    {
      try
      {
        if (!eventType.IsValid())
          return null;

        var tags = new List<string>();
        if (Settings.EnableAutoTag)
        {
          // walk up the stack to automatically tag the trace message from method names
          var st = new StackTrace();
          for (var f = 0; f < st.FrameCount; f++)
          {
            var frame = st.GetFrame(f);
            if (frame == null) continue;
            var method = frame.GetMethod();
            tags.Add(method.Name);
          }
        }

        return new MessageContext(new Exception(string.Format("{0}. {1}", message, detail)), tags);
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
    protected virtual MessageContext MessageFromTraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message, params object[] args)
    {
      try
      {
        if (!eventType.IsValid())
          return null;

        var context = new MessageContext(new Exception(message), new List<string>(), new Dictionary<object, object>());

        if (Settings.EnableAutoTag)
        {
          // walk up the stack to automatically tag the trace message from method names
          var st = new StackTrace();
          for (var f = 0; f < st.FrameCount; f++)
          {
            var frame = st.GetFrame(f);
            if (frame == null) continue;
            var method = frame.GetMethod();
            context.Tags.Add(method.Name);
          }
        }

        if (args != null)
        {
          var localArgs = args.ToList();
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

          // add the rest
          var count = 0;
          foreach (var leftover in localArgs)
            context.Data.Add(String.Format("arg-{0}", count++), leftover);
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
  }
}
