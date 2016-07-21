using System.Diagnostics;
using Raygun.Diagnostics.Models;

namespace Raygun.Diagnostics.Helpers
{
  public static class ValidationExtensions
  {
    /// <summary>
    /// Validates that the configured message trace level is in line with the trace event level
    /// </summary>
    /// <param name="eventType">Type of the event.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool IsValid(this TraceEventType eventType)
    {
      switch (eventType)
      {
        case TraceEventType.Critical:
        case TraceEventType.Error:
          if (Settings.MessageTraceLevel != MessageTraceLevel.Warning)
            return true;
          break;
        case TraceEventType.Warning:
          if (Settings.MessageTraceLevel != MessageTraceLevel.Error)
            return true;
          break;
      }
      return false;
    }
  }
}
