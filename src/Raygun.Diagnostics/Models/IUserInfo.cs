namespace Raygun.Diagnostics.Models
{
  /// <summary>
  /// Describes basic end user information
  /// </summary>
  public interface IUserInfo
  {
    /// <summary>
    /// Unique identifier for the user that could identify them across applications and sessions
    /// </summary>
    /// <value>The identifier.</value>
    string Id { get; set; }
    /// <summary>
    /// Username credential
    /// </summary>
    /// <value>The username.</value>
    string Username { get; set; }
    /// <summary>
    /// User email address
    /// </summary>
    /// <value>The email.</value>
    string Email { get; set; }
    /// <summary>
    /// The full name of the user, including middle and suffix
    /// </summary>
    /// <value>The fullname.</value>
    string FullName { get; set; }
    /// <summary>
    /// User first name
    /// </summary>
    /// <value>The firstname.</value>
    string FirstName { get; set; }
    /// <summary>
    /// Indicates whether the user is anonymous.
    /// </summary>
    /// <value><c>true</c> if this instance is anonymous; otherwise, <c>false</c>.</value>
    bool IsAnonymous { get; set; }

  }
}
