using System;
using System.Collections;
using System.Collections.Generic;

namespace Raygun.Diagnostics.Models
{
  [Serializable]
  public class MessageContext
  {
    public Exception Exception { get; set; }
    public List<string> Tags { get; set; }
    public IDictionary Data { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageContext"/> class.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <param name="tags">The tags.</param>
    /// <param name="data">The data.</param>
    public MessageContext(Exception exception = null, List<string> tags = null, IDictionary data = null)
    {
      Exception = exception;
      Tags = tags;
      Data = data;
    }
  }
}
