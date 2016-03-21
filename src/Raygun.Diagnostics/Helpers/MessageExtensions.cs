using System;
using System.Reflection;
using Mindscape.Raygun4Net.Messages;

namespace Raygun.Diagnostics.Helpers
{
  public static class MessageExtensions
  {
    /// <summary>
    /// Checks for a public property on the specified type
    /// </summary>
    /// <param name="objectToCheck">object in context</param>
    /// <param name="propertyName">case insensitve name of property</param>
    /// <param name="value">value of the property if available</param>
    /// <returns></returns>
    public static bool TryGetPropertyValue(this object objectToCheck, string propertyName, out object value)
    {
      var property = objectToCheck.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
      if (property != null)
      {
        value = property.GetValue(objectToCheck);
        return true;
      }
      value = null;
      return false;
    }

    /// <summary>
    /// Checks for a public method on the specified type
    /// </summary>
    /// <param name="objectToCheck">object in context</param>
    /// <param name="propertyName">case insensitve name of method</param>
    /// <returns></returns>
    public static bool HasProperty(this object objectToCheck, string propertyName)
    {
      return objectToCheck.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null;
    }

    /// <summary>
    /// Checks for a public method on the specified type
    /// </summary>
    /// <param name="objectToCheck">object in context</param>
    /// <param name="methodName">case insensitve name of method</param>
    /// <returns></returns>
    public static bool HasMethod(this object objectToCheck, string methodName)
    {
      return objectToCheck.GetType().GetMethod(methodName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null;
    }
  }
}
