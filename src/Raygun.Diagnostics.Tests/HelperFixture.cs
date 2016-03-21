using System;
using System.Diagnostics;
using NUnit.Framework;
using Raygun.Diagnostics.Helpers;
using Raygun.Diagnostics.Models;
using Raygun.Diagnostics.Tests.Models;

namespace Raygun.Diagnostics.Tests
{
  [TestFixture]
  public class HelperFixture
  {
    private object _testArgument;
    private MockUser _mockUser;

    [SetUp]
    public void Init()
    {
      _testArgument = new {Username = "johnd", Email = "johnd@email.com"};
      _mockUser = new MockUser();
    }

    [Test]
    public void IsValidEventHelperChecksValidEventOnErrorTraceLevel()
    {
      Settings.MessageTraceLevel = MessageTraceLevel.Error;
      Assert.IsTrue(TraceEventType.Critical.IsValid());
      Assert.IsTrue(TraceEventType.Error.IsValid());
    }

    [Test]
    public void IsValidEventHelperChecksValidEventOnWarningTraceLevel()
    {
      Settings.MessageTraceLevel = MessageTraceLevel.Warning;
      Assert.IsTrue(TraceEventType.Warning.IsValid());
    }

    [Test]
    public void IsValidEventHelperChecksInValidEventsOnErrorTraceLevel()
    {
      Settings.MessageTraceLevel = MessageTraceLevel.Error;
      Assert.IsFalse(TraceEventType.Information.IsValid());
      Assert.IsFalse(TraceEventType.Resume.IsValid());
      Assert.IsFalse(TraceEventType.Start.IsValid());
    }

    [Test]
    public void TryGetPropertyValueReturnsExpectedPropertyValue()
    {
      object output;
      _testArgument.TryGetPropertyValue("username", out output);
      Assert.That(output, Is.EqualTo("johnd"));
      _testArgument.TryGetPropertyValue("Username", out output);
      Assert.That(output, Is.EqualTo("johnd"));
      _testArgument.TryGetPropertyValue("email", out output);
      Assert.That(output, Is.EqualTo("johnd@email.com"));
    }

    [Test]
    public void HasPropertyHelperWorksForValidPublicPropertyOnInstance()
    {
      Assert.That(_testArgument.HasProperty("username"), Is.True);
      Assert.That(_testArgument.HasProperty("email"), Is.True);
      Assert.That(new Exception().HasProperty("message"), Is.True);
      Assert.That(new Exception().HasProperty("iPhone"), Is.False);
    }

    [Test]
    public void HasPropertyHelperWorksForValidPublicMethodOnInstance()
    {
      Assert.That(_testArgument.HasMethod("gettype"), Is.True);
      Assert.That(_mockUser.HasMethod("capitalizename"), Is.True);
      Assert.That(new Exception().HasMethod("getbaseexception"), Is.True);
      Assert.That(new Exception().HasMethod("iPhone"), Is.False);
    }
  }
}
