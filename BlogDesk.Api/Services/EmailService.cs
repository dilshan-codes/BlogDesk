using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BlogDesk.Api.Services;

public class EmailService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public EmailService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
        _http.BaseAddress = new Uri("https://api.resend.com/");
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _config["Resend:ApiKey"]);
    }

    public async Task SendOtpAsync(string toEmail, string code)
    {
        var payload = new
        {
            from = _config["Resend:FromAddress"],
            to = new[] { toEmail },
            subject = "Your BlogDesk admin login code",
            html = $"<p>Your one-time code is:</p><h2 style=\"letter-spacing:4px\">{code}</h2><p>This code expires in 10 minutes. If you didn't request this, ignore this email.</p>"
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var res = await _http.PostAsync("emails", content);
        res.EnsureSuccessStatusCode();
    }
}