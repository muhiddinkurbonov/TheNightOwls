using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Fadebook.DTOs;
using Fadebook.Repositories;
using System.Web;

namespace Fadebook.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GoogleCalendarController(
    IHttpClientFactory _httpClientFactory,
    IAppointmentRepository _appointmentRepository,
    IConfiguration _configuration
    ) : ControllerBase
{
    [HttpGet("google-auth", Name = "GoogleAuth")]
    // /google-auth?apptId=1
    public IActionResult GoogleAuth([FromQuery] int apptId)
    {
        var clientId = _configuration["Google:ClientId"];
        var redirectUri = Url.Link("GoogleCallback", new { apptId });
        var scope = HttpUtility.UrlEncode("https://www.googleapis.com/auth/calendar.events");

        var url = $"https://accounts.google.com/o/oauth2/v2/auth" +
                  $"?client_id={clientId}" +
                  $"&redirect_uri={redirectUri}" +
                  $"&response_type=code" +
                  $"&scope={scope}" +
                  $"&access_type=offline" +
                  $"&prompt=consent";

        return Redirect(url);
    }

    [HttpGet("google-callback", Name = "GoogleCallback")]
    public async Task<IActionResult> Callback([FromQuery] int apptId, [FromQuery] string code)
    {
        var client = _httpClientFactory.CreateClient();
        var clientId = _config["Google:ClientId"];
        var clientSecret = _config["Google:ClientSecret"];
        // var redirectUri = _config["Google:RedirectUri"];
        var redirectUri = "localhost:3000/book/";

        AddEventAsync(apptId, accessToken);

        RevokeTokenAsync(refreshToken);
        RevokeTokenAsync(accessToken);

        return Redirect($"localhost:3000/book/confirmation/{apptId}");
    }

    public async Task<IActionResult> AddEventAsync(string apptId, int accessToken)
    {
        //  Adding Calendar Event (requires valid token)
        var eventData = new 
        {
           
        }
    }

    private async Task RevokeTokenAsync(string token)
    {
        // revoke tokens: https://oauth2.googleapis.com/revoke?token={token}
    }

}
