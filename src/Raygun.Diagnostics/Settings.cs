using Mindscape.Raygun4Net;
using Raygun.Diagnostics.Models;

namespace Raygun.Diagnostics
{
  public static class Settings
  {
    private static RaygunClient _client;
    private static MessageTraceLevel _messageTraceLevel = MessageTraceLevel.Error;

    /// <summary>
    /// The Raygun api key. This value will be used in place of a the app.config.
    /// </summary>
    /// <value>The API key.</value>
    public static string ApiKey { get; set; }

    /// <summary>
    /// Gets the Raygun client. If ApiKey is specified, that will be used in place of the app.config key.
    /// </summary>
    /// <value>The client.</value>
    public static RaygunClient Client
    {
      get { return _client ?? (_client = ApiKey == null ? new RaygunClient() : new RaygunClient(ApiKey)); }
    }

    /// <summary>
    /// Toggle debug mode.
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

    /// <summary>
    /// Default tags that will be attached to very request.
    /// </summary>
    /// <value>The default tags.</value>
    public static string[] DefaultTags { get; set; }
  }
}
