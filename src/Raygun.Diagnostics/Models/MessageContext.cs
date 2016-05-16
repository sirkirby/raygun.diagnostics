using System;
using System.Collections;
using System.Collections.Generic;
using Mindscape.Raygun4Net.Messages;

namespace Raygun.Diagnostics.Models
{
  [Serializable]
  public class MessageContext
  {
    /// <summary>
    /// Standard exception object
    /// </summary>
    /// <value>The exception.</value>
    public Exception Exception { get; set; }
    /// <summary>
    /// List of tags passed through to Raygun
    /// </summary>
    /// <value>The tags.</value>
    public List<string> Tags { get; set; }
    /// <summary>
    /// Key/Value custom data passed to Raygun.
    /// </summary>
    /// <value>The data.</value>
    public IDictionary Data { get; set; }
    /// <summary>
    /// Basic user information passed to Raygun
    /// </summary>
    /// <value>The user.</value>
    public IUserInfo User { get; set; }
    
    /// <summary>
    /// Message used for custom hash grouping when sending the error to Raygun.  Raygun does their own examination of the data sent to group errors in sometimes unintended ways.
    /// This allows the calling application to control the grouping.
    /// </summary>
    public IMessageGroup Grouping { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageContext" /> class.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <param name="tags">The tags.</param>
    /// <param name="data">The data.</param>
    /// <param name="user">The user.</param>
    public MessageContext(Exception exception = null, List<string> tags = null, IDictionary data = null, IUserInfo user = null)
    {
      Exception = exception;
      Tags = tags;
      Data = data;
      User = user;
    }

    /// <summary>
    /// Gets the Raygun user from the IUserInfo
    /// </summary>
    /// <returns>RaygunIdentifierMessage.</returns>
    public RaygunIdentifierMessage GetRaygunUser()
    {
      if (User == null)
        return null;
      return new RaygunIdentifierMessage(User.Username)
      {
        UUID = User.Id,
        Email = User.Email,
        FullName = User.FullName,
        FirstName = User.FirstName,
        IsAnonymous = User.IsAnonymous
      };
    }       
  }
}
