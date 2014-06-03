using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Raygun.Diagnostics.Models;

namespace Raygun.Diagnostics
{
  public class RaygunTraceListener : TraceListener
  {
    public override void Fail(string message)
    {
      WriteMessage(MessageFromString(message, null, TraceEventType.Error));
    }

    public override void Fail(string message, string detailMessage)
    {
      WriteMessage(MessageFromString(message, detailMessage, TraceEventType.Error));
    }

    public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
    {
      TraceData(eventCache, source, eventType, id, new[] { data });
    }

    public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
    {
      if ((Filter != null) && !Filter.ShouldTrace(eventCache, source, eventType, id, null, null, null, data)) return;
      var message = string.Format("{0} trace event", eventType);
      WriteMessage(MessageFromTraceEvent(eventCache, source, eventType, id, message, data));
    }

    public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
    {
      var message = string.Format("{0} trace event", eventType);
      TraceEvent(eventCache, source, eventType, id, message);
    }

    public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
    {
      if ((Filter != null) && !Filter.ShouldTrace(eventCache, source, eventType, id, message, null, null, null)) return;
      WriteMessage(MessageFromTraceEvent(eventCache, source, eventType, id, message, null));
    }

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
    /// Writes the message.
    /// </summary>
    /// <param name="message">The message.</param>
    protected virtual void WriteMessage(MessageContext message)
    {
      if (message != null)
        Settings.Client.Send(message.Exception, message.Tags, message.Data);
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
      // only proceed if we need to log an exception. Warning or higher.
      if (eventType != TraceEventType.Error && eventType != TraceEventType.Critical && eventType != TraceEventType.Warning)
        return null;

      var tags = new List<string>();
      if (Settings.EnableAutoTag)
      {
        // walk up the stack to automatically tag the trace message from method names
        var st = new StackTrace();
        var frame = st.GetFrame(st.FrameCount - 1);
        if (frame != null)
        {
          var method = frame.GetMethod();
          tags.Add(method.Name);
          tags.AddRange(method.GetParameters().Select(p => p.Name));
        }
        if (Settings.Debug)
          tags.Add("debug");
      }

      return new MessageContext(new Exception(string.Format("{0}. {1}", message, detail)), tags);
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
      // only proceed if we need to log an exception. Warning or higher.
      if (eventType != TraceEventType.Error && eventType != TraceEventType.Critical && eventType != TraceEventType.Warning)
        return null;

      var context = new MessageContext(new Exception(message), new List<string>(), new Dictionary<object, object>());

      if (Settings.EnableAutoTag)
      {
        // walk up the stack to automatically tag the trace message from method names
        var st = new StackTrace();
        var frame = st.GetFrame(st.FrameCount - 1);
        if (frame != null)
        {
          var method = frame.GetMethod();
          context.Tags.Add(method.Name);
          context.Tags.AddRange(method.GetParameters().Select(p => p.Name));
        }
        if (Settings.Debug)
          context.Tags.Add("debug");
      }

      if (args != null)
      {
        // check the args for custom data
        var custom = args.FirstOrDefault(a => a is IDictionary);
        if (custom != null)
          context.Data = (IDictionary) custom;

        // check the args for tags
        var tags = args.Where(a => a is IList);
        foreach (var tag in tags)
          context.Tags.AddRange((IList<string>)tag);

        // check the args for a custom exception
        var error = args.FirstOrDefault(a => a is Exception);
        if (error != null)
          context.Exception = new Exception(message, (Exception)error);

        // add the rest
        var leftovers = args.Where(a => !(a is IDictionary) && !(a is IList) && !(a is Exception));
        var count = 1;
        foreach (var leftover in leftovers)
        {
          context.Data.Add(String.Format("arg-{0}", count), leftover);
          count++;
        }
      }

      return context;
    }
  }
}
