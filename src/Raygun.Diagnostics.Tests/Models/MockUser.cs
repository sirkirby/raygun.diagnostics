namespace Raygun.Diagnostics.Tests.Models
{
  [RaygunDiagnosticsUser("johnd"), RaygunDiagnostics("mock", "user")]
  public class MockUser
  {
    public string Name { get; set; }

    public void CapitalizeName() {}
  }
}
