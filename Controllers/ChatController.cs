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
    private readonly GptConfig GptConfig = new GptConfig();

    public ChatController(IConfiguration config)
    {
        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
        GptConfig = builder.GetSection("Env:GPT").Get<GptConfig>() ?? new GptConfig();

        fileid = GptConfig.FileId ?? ""; //pdf //"file-JhGiDMbGZekZtBu7T3rT84"; //txt
        _apikeys.Add(GptConfig.Url ?? "");
        _apikeys.Add(GptConfig.AuthType ?? "");
        _apikeys.Add(Environment.GetEnvironmentVariable(GptConfig.Key ?? "") ?? "");
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Chat([FromBody] UserMessage input)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_apikeys[subkey], _apikeys[key]);

        //string file = File.ReadAllText(Path);

        var requestBody = new
        {
            model = GptConfig.Model,
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
                            text = "You are an assistant that analyzes and explains the information in the provided portfolio file, be clear and professional."
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

public class GptConfig
{
    public string? FileId { get; set; }
    public string? Url { get; set; }
    public string? AuthType { get; set; }
    public string? Key { get; set; }
    public string? Model { get; set; }
}