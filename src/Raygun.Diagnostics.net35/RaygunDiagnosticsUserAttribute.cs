using System;
using Mindscape.Raygun4Net.Messages;

namespace Raygun.Diagnostics
{
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
  public class RaygunDiagnosticsUserAttribute : Attribute
  {
    /// <summary>
    /// Raygun user object
    /// </summary>
    public RaygunIdentifierMessage User { get; set; }

    public RaygunDiagnosticsUserAttribute(string username, string id = null, string email = null, string fullname = null, string firstname = null, bool isAnonymous = false)
    {
      User = new RaygunIdentifierMessage(username)
      {
        UUID = id,
        Email = email,
        FullName = fullname,
        FirstName = firstname,
        IsAnonymous = isAnonymous
      };
    }
  }
}
