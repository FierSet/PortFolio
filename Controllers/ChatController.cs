using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly string fileid;
    private readonly List<string> _apikeys = [];
    private readonly int link = 0, subkey = 1, key = 2;

    public ChatController(IConfiguration config)
    {
        fileid = "file-JhGiDMbGZekZtBu7T3rT84";
        _apikeys.Add("https://api.openai.com/v1/chat/completions");
        _apikeys.Add("Bearer");
        _apikeys.Add(Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "");
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Chat([FromBody] UserMessage input)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_apikeys[subkey], _apikeys[key]);

        //string file = File.ReadAllText(Path);

        var requestBody = new
        {
            instruction = "A document has been provided with information on Portfolio.",
            model = "gpt-4.1",
            tools = new[]
            { 
                new { type = "retrieval"}
            },
           file_id = new[] { fileid },
            messages = new[]
            {
                new { role = "user", content = input.Message }
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = await client.PostAsync(_apikeys[link], content);
        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);

        string? reply = null;

        if (doc.RootElement.TryGetProperty("error", out JsonElement errorElement))
        {
            string errorMsg = errorElement.GetProperty("message").GetString() ?? "Unknown error";
            string errorType = errorElement.GetProperty("type").GetString() ?? "Unknown type";
            reply = $"Error ({errorType}) : {errorMsg}";
        }
        else if (doc.RootElement.TryGetProperty("choices", out JsonElement choices))
            reply = choices[0].GetProperty("message").GetProperty("content").GetString();
        else
            reply = "Unexpected response format.";
            
        return Ok(new { response = reply });
    }
}

public class UserMessage
{
    public string? Message { get; set; }
}