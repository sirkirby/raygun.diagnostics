using System;
using System.Collections.Generic;
using System.Diagnostics;
using Moq;
using NUnit.Framework;
using Raygun.Diagnostics.Models;
using Mindscape.Raygun4Net;
using Raygun.Diagnostics.Tests.Models;

namespace Raygun.Diagnostics.Tests
{
  [TestFixture, RaygunDiagnostics("trace-class", "event-class"), RaygunDiagnosticsUser("johnd-class", "1234-class", "johnd@email.com-class", "John D-class", "John-class")]
  public class TraceListenerFromTraceEventFixture
  {
    private RaygunTraceListener _listener;
    private Mock<IUserInfo> _userInfo;
    private object _anonUserInfo;

    [SetUp]
    public void Init()
    {
      _listener = new RaygunTraceListener();
      _userInfo = new Mock<IUserInfo>();
      _userInfo.SetupProperty(u => u.Username, "johnd");
      _userInfo.SetupProperty(u => u.Email, "johnd@email.com");
      _userInfo.SetupProperty(u => u.Id, "1234");
      _userInfo.SetupProperty(u => u.FullName, "John D");
      _userInfo.SetupProperty(u => u.FirstName, "John");
      _userInfo.SetupProperty(u => u.IsAnonymous, false);
      _anonUserInfo = new
      {
        Username = "johnd",
        Id = "1234",
        Email = "johnd@email.com",
        FullName = "John D",
        FirstName = "John",
        IsAnonymous = false
      };
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
    public void RaygunEventTraceTypedUserInfoPassedViaCustomArgOverridesClassOrMethodAttribute()
    {
      var context = _listener.MessageFromTraceEvent(null, "nunit", TraceEventType.Error, 0, "nunit test GetRaygunUserFromMessageTraceEvent", _userInfo.Object);

      Assert.That(context.User, Is.Not.Null);
      Assert.That(context.User.Username, Does.Contain(_userInfo.Object.Username));
      Assert.That(context.User.Email, Does.Contain(_userInfo.Object.Email));
      Assert.That(context.User.FullName, Does.Contain(_userInfo.Object.FullName));
      Assert.That(context.User.FirstName, Does.Contain(_userInfo.Object.FirstName));
      Assert.That(context.User.Id, Does.Contain(_userInfo.Object.Id));
      Assert.That(context.User.IsAnonymous, Is.False);
    }

    [Test, RaygunDiagnosticsUser("johnd-method", "1234-method", "johnd@email.com-method", "John D-method", "John-method")]
    public void RaygunEventTraceAnonymousUserInfoPassedViaCustomArgOverridesClassOrMethodAttribute()
    {
      var context = _listener.MessageFromTraceEvent(null, "nunit", TraceEventType.Error, 0, "nunit test GetRaygunUserFromMessageTraceEvent", _anonUserInfo);

      Assert.That(context.User, Is.Not.Null);
      Assert.That(context.User.Username, Does.Contain("johnd"));
      Assert.That(context.User.Email, Does.Contain("johnd@email.com"));
      Assert.That(context.User.FullName, Does.Contain("John D"));
      Assert.That(context.User.FirstName, Does.Contain("John"));
      Assert.That(context.User.Id, Does.Contain("1234"));
      Assert.That(context.User.IsAnonymous, Is.False);
    }

    [Test]
    public void RaygunEventTraceTypedUserInfoPassedAsArgOverridesClassUserAttribute()
    {
      var context = _listener.MessageFromTraceEvent(null, "nunit", TraceEventType.Error, 0, "nunit test RaygunUserPassedAsArgOverridesClassUserAttribute", _userInfo.Object);
      Assert.That(context.User, Is.Not.Null);
      Assert.That(context.User.Username, Does.Contain(_userInfo.Object.Username));
      Assert.That(context.User.Email, Does.Contain(_userInfo.Object.Email));
      Assert.That(context.User.FullName, Does.Contain(_userInfo.Object.FullName));
      Assert.That(context.User.FirstName, Does.Contain(_userInfo.Object.FirstName));
      Assert.That(context.User.Id, Does.Contain(_userInfo.Object.Id));
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
      Assert.That(context.User.Username, Is.EqualTo("johnd-method"));
      Assert.That(context.User.Email, Does.Contain("johnd@email.com-method"));
      Assert.That(context.User.Id, Does.Contain("1234-method"));
      Assert.That(context.User.FullName, Does.Contain("John D-method"));
      Assert.That(context.User.FirstName, Does.Contain("John-method"));
      Assert.That(context.User.IsAnonymous, Is.False);
    }

    [Test]
    public void RaygunEventTraceUserAssignedViaClassAttribute()
    {
      var context = _listener.MessageFromTraceEvent(null, "nunit", TraceEventType.Error, 0, "nunit test RaygunUserAssignedViaClassAttribute");
      Assert.That(context.User, Is.Not.Null);
      Assert.That(context.User.Username, Is.EqualTo("johnd-class"));
      Assert.That(context.User.Email, Does.Contain("johnd@email.com-class"));
      Assert.That(context.User.Id, Does.Contain("1234-class"));
      Assert.That(context.User.FullName, Does.Contain("John D-class"));
      Assert.That(context.User.FirstName, Does.Contain("John-class"));
      Assert.That(context.User.IsAnonymous, Is.False);
    }

    [Test]
    public void RaygunStringTraceHasGroupKey()
    {
        var groupKey = "unitTestGroup";
        var context = _listener.MessageFromTraceEvent(new TraceEventCache(), "nunit test RaygunHasGroupKey", TraceEventType.Error, 1, "string exception", new { GroupKey = groupKey });
        Assert.That(context.Group.GroupKey, Is.EqualTo(groupKey));
    }

    [Test]
    public void RaygunStringTraceWithGroupKeyFiresEvent()
    {
        var groupKey = "unitTestGroupEvent";
        var raygunCustomGrouping = string.Empty;

        Settings.Client = new MockRaygunClient();

        //subscribe to the event to detect if it fired correctly
        _listener.OnGrouping += (obj, e, group) =>
        {
            raygunCustomGrouping = e.CustomGroupingKey;
        };
        var context = _listener.MessageFromTraceEvent(new TraceEventCache(), "nunit test RaygunFiresGroupEvent", TraceEventType.Error, 1, "string exception", new { GroupKey = groupKey });

        //todo: somehow need to mock the method so it doesn't actually send a message
        _listener.WriteMessage(context);

        Assert.That(raygunCustomGrouping == groupKey, Is.True);
    }

    [Test]
    public void RaygunEventTraceUseDefaultMessageFormatWithNullInput()
    {
      var context = _listener.MessageFromTraceEvent(new TraceEventCache(), "RaygunEventTraceUseDefaultMessageFormatWithNullInput", TraceEventType.Error, 1, null, _userInfo);

      Assert.That(context.Exception.Message == "An unexpected error occurred while calling RaygunEventTraceUseDefaultMessageFormatWithNullInput in TraceListenerFromTraceEventFixture at line 192.", Is.True);
    }

    [Test]
    public void RaygunEventTraceUseDefaultMessageFormatWithEmptyStringInput()
    {
      var context = _listener.MessageFromTraceEvent(new TraceEventCache(), "RaygunEventTraceUseDefaultMessageFormatWithEmptyStringInput", TraceEventType.Error, 1, string.Empty, _userInfo);

      Assert.That(context.Exception.Message == "An unexpected error occurred while calling RaygunEventTraceUseDefaultMessageFormatWithEmptyStringInput in TraceListenerFromTraceEventFixture at line 200.", Is.True);
    }
  }
}
