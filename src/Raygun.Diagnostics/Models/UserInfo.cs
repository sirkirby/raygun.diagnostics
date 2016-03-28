namespace Raygun.Diagnostics.Models
{
  /// <summary>
  /// Internal UserInfo class
  /// </summary>
  /// <seealso cref="Raygun.Diagnostics.Models.IUserInfo" />
  internal class UserInfo : IUserInfo
  {
    /// <summary>
    /// Unique identifier for the user that could identify them across applications and sessions
    /// </summary>
    /// <value>The identifier.</value>
    public string Id { get; set; }
    /// <summary>
    /// Username credential
    /// </summary>
    /// <value>The username.</value>
    public string Username { get; set; }
    /// <summary>
    /// User email address
    /// </summary>
    /// <value>The email.</value>
    public string Email { get; set; }
    /// <summary>
    /// The full name of the user, including middle and suffix
    /// </summary>
    /// <value>The fullname.</value>
    public string FullName { get; set; }
    /// <summary>
    /// User first name
    /// </summary>
    /// <value>The firstname.</value>
    public string FirstName { get; set; }
    /// <summary>
    /// Indicates whether the user is anonymous.
    /// </summary>
    /// <value><c>true</c> if this instance is anonymous; otherwise, <c>false</c>.</value>
    public bool IsAnonymous { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserInfo" /> class.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="email">The email.</param>
    /// <param name="fullName">The full name.</param>
    /// <param name="firstName">The first name.</param>
    /// <param name="isAnonymous">if set to <c>true</c> [is anonymous].</param>
    public UserInfo(string username , string id = null, string email = null, string fullName = null, string firstName = null, bool isAnonymous = false)
    {
      Id = id;
      Username = username;
      Email = email;
      FullName = fullName;
      FirstName = firstName;
      IsAnonymous = isAnonymous;
    }
  }
}
