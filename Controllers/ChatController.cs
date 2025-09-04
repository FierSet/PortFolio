using System.Net.Http.Headers;
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
        fileid = "file-AGAuHkyoHmGsRTeZfqHvKJ"; //pdf //"file-JhGiDMbGZekZtBu7T3rT84"; //txt
        _apikeys.Add("https://api.openai.com/v1/responses");
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
            model = "gpt-5",
            input = new[]
            {
                new
                {
                    role = "system",
                    content = new object[]
                    {
                        new
                        {
                            type = "input_text",
                            text = "You are an assistant that analyzes and explains the information in the provided portfolio file in a clear and professional way."
                        }
                    }
                },
                new
                {
                    role = "user",
                    content = new object[]
                    {
                        new
                        {
                            type = "input_file",
                            file_id = fileid
                        },
                        new
                        {
                            type= "input_text",
                            text = input.Message
                        }
                    },
                }
            }
        };


        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = await client.PostAsync(_apikeys[link], content);
        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);

        string? reply = null;
        var root = doc.RootElement;

        if (root.TryGetProperty("error", out JsonElement errorElement) && errorElement.ValueKind != JsonValueKind.Null)
        {
            reply = errorElement.GetProperty("message").GetString() ?? "Unknown error";
        }
        if (root.TryGetProperty("output", out JsonElement outputElement) && outputElement.GetArrayLength() > 0)
        {
            foreach (var item in outputElement.EnumerateArray())
                if (item.GetProperty("type").GetString() == "message" && 
                    item.TryGetProperty("content", out JsonElement contentArray)) // Only look for assistant messages
                    foreach (var contents in contentArray.EnumerateArray())
                    {
                        if (contents.GetProperty("type").GetString() == "output_text")
                        {
                            string text = contents.GetProperty("text").GetString() ?? "";
                            reply = text;
                        }
                    }
        }
        else
            reply = "Unexpected response format.";
            
        return Ok(new { response = reply });
    }
}

public class UserMessage
{
    public string? Message { get; set; }
}