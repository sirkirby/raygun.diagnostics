using System;
using System.Collections;
using System.Collections.Generic;
using Mindscape.Raygun4Net.Messages;

namespace Raygun.Diagnostics.Models
{
  [Serializable]
  public class MessageContext
  {
    public Exception Exception { get; set; }
    public List<string> Tags { get; set; }
    public IDictionary Data { get; set; }
    public RaygunIdentifierMessage User { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageContext"/> class.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <param name="tags">The tags.</param>
    /// <param name="data">The data.</param>
    public MessageContext(Exception exception = null, List<string> tags = null, IDictionary data = null, RaygunIdentifierMessage user = null)
    {
      Exception = exception;
      Tags = tags;
      Data = data;
      User = user;
    }
  }
}
