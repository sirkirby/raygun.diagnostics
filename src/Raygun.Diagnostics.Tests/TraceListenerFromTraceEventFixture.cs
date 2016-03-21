using System;
using System.Collections.Generic;
using System.Diagnostics;
using Mindscape.Raygun4Net.Messages;
using NUnit.Framework;
using Raygun.Diagnostics.Models;

namespace Raygun.Diagnostics.Tests
{
  [TestFixture, RaygunDiagnostics("trace-class", "event-class"), RaygunDiagnosticsUser("johnd-class", "1234-class", "johnd@email.com-class", "John D-class", "John-class")]
  public class TraceListenerFromTraceEventFixture
  {
    private RaygunTraceListener _listener;
    private RaygunIdentifierMessage _raygunUser;

    [SetUp]
    public void Init()
    {
      _listener = new RaygunTraceListener();
      _raygunUser = new RaygunIdentifierMessage("johnd") {UUID = "12345", Email = "johnd@email.com", FullName = "John D", FirstName = "John", IsAnonymous = false};
      Settings.MessageTraceLevel = MessageTraceLevel.Error;

    }

    [Test]
    public void RaygunEventTraceExceptionMessageShouldMirrorTraceErrorMessage()
    {
      var context = _listener.MessageFromTraceEvent(null, "nunit", TraceEventType.Error, 0, "nunit test RaygunExceptionMessageShouldMirrorTraceErrorMessage");
      Assert.That(context.Exception, Is.Not.Null);
      Assert.That(context.Exception.Message, Does.Contain("nunit test RaygunExceptionMessageShouldMirrorTraceErrorMessage"));
    }

    [Test]
    public void RaygunEventTraceExceptionMessageShouldMirrorExceptionPassedAsCustomArg()
    {
      var context = _listener.MessageFromTraceEvent(null, "nunit", TraceEventType.Error, 0, "nunit test TraceMessageShouldBeNewExceptionObjectMessage", new Exception("bad things happened"));
      Assert.That(context.Exception, Is.Not.Null);
      Assert.That(context.Exception.Message, Does.Contain("bad things happened"));
    }

    [Test, RaygunDiagnosticsUser("johnd-method", "1234-method", "johnd@email.com-method", "John D-method", "John-method")]
    public void RaygunEventTraceUserPassedViaCustomArgOverridesClassOrMethodAttribute()
    {
      var context = _listener.MessageFromTraceEvent(null, "nunit", TraceEventType.Error, 0, "nunit test GetRaygunUserFromMessageTraceEvent", _raygunUser);

      Assert.That(context.User, Is.Not.Null);
      Assert.That(context.User.Identifier, Does.Contain(_raygunUser.Identifier));
      Assert.That(context.User.Email, Does.Contain(_raygunUser.Email));
      Assert.That(context.User.FullName, Does.Contain(_raygunUser.FullName));
      Assert.That(context.User.FirstName, Does.Contain(_raygunUser.FirstName));
      Assert.That(context.User.IsAnonymous, Is.False);
    }

    [Test, RaygunDiagnostics("trace-method", "event-method")]
    public void RaygunEventTraceHasTagsAssignedViaMethodAndClassAttributes()
    {
      var context = _listener.MessageFromTraceEvent(null, "nunit", TraceEventType.Error, 0, "nunit test GetRaygunTagsFromAttribute", "custom1", "custom2");
      Assert.That(context.Tags, Is.Not.Null);
      Assert.That(context.Tags.ToArray(), Has.Length.EqualTo(4));
      Assert.That(context.Tags, Has.Exactly(1).Contains("trace-method"));
      Assert.That(context.Tags, Has.Exactly(1).Contains("event-method"));
      Assert.That(context.Tags, Has.Exactly(1).Contains("trace-class"));
      Assert.That(context.Tags, Has.Exactly(1).Contains("event-class"));
    }

    [Test]
    public void RaygunEventTraceHasTagsAssignedViaClassAttribute()
    {
      var context = _listener.MessageFromTraceEvent(null, "nunit", TraceEventType.Error, 0, "nunit test RaygunHasTagsAssignedViaClassAttribute", "custom1", "custom2");
      Assert.That(context.Tags, Is.Not.Null);
      Assert.That(context.Tags.ToArray(), Has.Length.EqualTo(2));
      Assert.That(context.Tags, Has.Exactly(1).Contains("trace-class"));
      Assert.That(context.Tags, Has.Exactly(1).Contains("event-class"));
    }

    [Test]
    public void RaygunEventTraceTagsAssignedViaCustomArgAndClassAttribute()
    {
      var context = _listener.MessageFromTraceEvent(null, "nunit", TraceEventType.Error, 0, "nunit test RaygunTagsAssignedViaCustomArgAndClassAttribute", new List<string> { "custom1", "custom2"});
      Assert.That(context.Tags, Is.Not.Null);
      Assert.That(context.Tags.ToArray(), Has.Length.EqualTo(4));
      Assert.That(context.Tags, Has.Exactly(1).Contains("custom1"));
      Assert.That(context.Tags, Has.Exactly(1).Contains("custom2"));
      Assert.That(context.Tags, Has.Exactly(1).Contains("trace-class"));
      Assert.That(context.Tags, Has.Exactly(1).Contains("event-class"));
    }

    [Test, RaygunDiagnosticsUser("johnd-method", "1234-method", "johnd@email.com-method", "John D-method", "John-method")]
    public void RaygunEventTraceUserAssignedViaMethodAttribute()
    {
      var context = _listener.MessageFromTraceEvent(null, "nunit", TraceEventType.Error, 0, "nunit test RaygunUserAssignedViaMethodAttribute");
      Assert.That(context.User, Is.Not.Null);
      Assert.That(context.User.Identifier, Is.EqualTo("johnd-method"));
      Assert.That(context.User.Email, Does.Contain("johnd@email.com-method"));
      Assert.That(context.User.UUID, Does.Contain("1234-method"));
      Assert.That(context.User.FullName, Does.Contain("John D-method"));
      Assert.That(context.User.FirstName, Does.Contain("John-method"));
      Assert.That(context.User.IsAnonymous, Is.False);
    }

    [Test]
    public void RaygunEventTraceUserAssignedViaClassAttribute()
    {
      var context = _listener.MessageFromTraceEvent(null, "nunit", TraceEventType.Error, 0, "nunit test RaygunUserAssignedViaClassAttribute");
      Assert.That(context.User, Is.Not.Null);
      Assert.That(context.User.Identifier, Is.EqualTo("johnd-class"));
      Assert.That(context.User.Email, Does.Contain("johnd@email.com-class"));
      Assert.That(context.User.UUID, Does.Contain("1234-class"));
      Assert.That(context.User.FullName, Does.Contain("John D-class"));
      Assert.That(context.User.FirstName, Does.Contain("John-class"));
      Assert.That(context.User.IsAnonymous, Is.False);
    }

    [Test, RaygunDiagnosticsUser("johnd-method", "1234-method", "johnd@email.com-method", "John D-method", "John-method")]
    public void RaygunEventTraceUserPassedAsArgOverridesClassUserAttribute()
    {
      var context = _listener.MessageFromTraceEvent(null, "nunit", TraceEventType.Error, 0, "nunit test RaygunUserPassedAsArgOverridesClassUserAttribute", _raygunUser);
      Assert.That(context.User, Is.Not.Null);
      Assert.That(context.User.Identifier, Is.EqualTo("johnd"));
      Assert.That(context.User.Email, Does.Contain("johnd@email.com"));
      Assert.That(context.User.UUID, Does.Contain("1234"));
      Assert.That(context.User.FullName, Does.Contain("John D"));
      Assert.That(context.User.FirstName, Does.Contain("John"));
      Assert.That(context.User.IsAnonymous, Is.False);
    }
  }
}
