using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

public class RoleAccessTests : IClassFixture<WebApplicationFactory<SafeVault.Program>>
{
    private readonly WebApplicationFactory<SafeVault.Program> _factory;

    public RoleAccessTests(WebApplicationFactory<SafeVault.Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Replace authentication with a test scheme
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
            });
        });
    }

    [Theory]
    [InlineData("/Admin/Index", "Admin", HttpStatusCode.OK)]
    [InlineData("/Admin/Index", "User", HttpStatusCode.Forbidden)]
    [InlineData("/User/Profile", "User", HttpStatusCode.OK)]
    [InlineData("/User/Profile", "Guest", HttpStatusCode.Forbidden)]
    public async Task Endpoint_Access_By_Role(string url, string role, HttpStatusCode expectedStatus)
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // Set the test user role via a custom header
        client.DefaultRequestHeaders.Add("Test-Role", role);

        var response = await client.GetAsync(url);

        Assert.Equal(expectedStatus, response.StatusCode);
    }
}

// Custom authentication handler for testing
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Read the role from the request header
        var role = Request.Headers["Test-Role"].ToString() ?? "User";
        var claims = new[] { new Claim(ClaimTypes.Name, "testuser"), new Claim(ClaimTypes.Role, role) };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
