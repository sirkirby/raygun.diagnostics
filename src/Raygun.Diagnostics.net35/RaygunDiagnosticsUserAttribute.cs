using System;
using Mindscape.Raygun4Net.Messages;
using Raygun.Diagnostics.Models;

namespace Raygun.Diagnostics
{
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
  public class RaygunDiagnosticsUserAttribute : Attribute
  {
    /// <summary>
    /// Raygun user object
    /// </summary>
    public IUserInfo User { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RaygunDiagnosticsUserAttribute"/> class.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="email">The email.</param>
    /// <param name="fullName">The full name.</param>
    /// <param name="firstName">The first name.</param>
    /// <param name="isAnonymous">if set to <c>true</c> [is anonymous].</param>
    public RaygunDiagnosticsUserAttribute(string username, string id = null, string email = null, string fullName = null, string firstName = null, bool isAnonymous = false)
    {
      User = new UserInfo(username)
      {
        Id = id,
        Email = email,
        FullName = fullName,
        FirstName = firstName,
        IsAnonymous = isAnonymous
      };
    }
  }
}
