using Mindscape.Raygun4Net;

namespace Raygun.Diagnostics
{
  public static class Settings
  {
    /// <summary>
    /// Gets or sets the API key.
    /// </summary>
    /// <value>The API key.</value>
    public static string ApiKey { get; set; }
    
    private static RaygunClient _client;

    /// <summary>
    /// Gets the client.
    /// </summary>
    /// <value>The client.</value>
    public static RaygunClient Client
    {
      get { return _client ?? (_client = ApiKey == null ? new RaygunClient() : new RaygunClient(ApiKey)); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [enable automatic tag].
    /// </summary>
    /// <value><c>true</c> if [enable automatic tag]; otherwise, <c>false</c>.</value>
    public static bool EnableAutoTag { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="Settings"/> is debug.
    /// </summary>
    /// <value><c>true</c> if debug; otherwise, <c>false</c>.</value>
    public static bool Debug { get; set; }
  }
}
