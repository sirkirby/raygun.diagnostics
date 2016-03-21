using System;
using Mindscape.Raygun4Net.Messages;

namespace Raygun.Diagnostics
{
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
  public class RaygunDiagnosticsAttribute : Attribute
  {
    /// <summary>
    /// Gets or sets the tags to be used when this method is found in the stack trace
    /// </summary>
    /// <value>The tags.</value>
    public string[] Tags { get; set; }

    /// <summary>
    /// Raygun user object
    /// </summary>
    public RaygunIdentifierMessage User { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RaygunDiagnosticsAttribute"/> class.
    /// </summary>
    /// <param name="tags">The tags.</param>
    public RaygunDiagnosticsAttribute(params string[] tags)
    {
      Tags = tags;
    }
  }
}
