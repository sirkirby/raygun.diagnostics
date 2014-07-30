using Mindscape.Raygun4Net;
using Raygun.Diagnostics.Models;

namespace Raygun.Diagnostics
{
  public static class Settings
  {
    private static RaygunClient _client;
    private static MessageTraceLevel _messageTraceLevel = MessageTraceLevel.Error;

    /// <summary>
    /// Gets or sets the API key.
    /// </summary>
    /// <value>The API key.</value>
    public static string ApiKey { get; set; }

    /// <summary>
    /// Gets the client.
    /// </summary>
    /// <value>The client.</value>
    public static RaygunClient Client
    {
      get { return _client ?? (_client = ApiKey == null ? new RaygunClient() : new RaygunClient(ApiKey)); }
    }

    /// <summary>
    /// Enables or disables the auto tag feature which will walk the stack trace and add method names as tags
    /// </summary>
    /// <value><c>true</c> if [enable automatic tag]; otherwise, <c>false</c>.</value>
    public static bool EnableAutoTag { get; set; }

    /// <summary>
    /// Disable the message send to raygun when debug is enabled
    /// </summary>
    /// <value><c>true</c> if debug; otherwise, <c>false</c>.</value>
    public static bool Debug { get; set; }

    /// <summary>
    /// Gets or sets the message trace level.
    /// </summary>
    /// <value>The message trace level.</value>
    public static MessageTraceLevel MessageTraceLevel 
    {
      get { return _messageTraceLevel; }
      set { _messageTraceLevel = value; }
    }
  }
}
