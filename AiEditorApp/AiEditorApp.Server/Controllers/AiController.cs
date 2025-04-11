using Microsoft.AspNetCore.Mvc;
using AiEditorApp.Shared;
using System.Net.Http.Headers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly HttpClient _http;

    public AiController(IConfiguration config)
    {
        _config = config;
        _http = new HttpClient();
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _config["OpenAI:ApiKey"]);
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] PromptInput input)
    {
        var body = new
        {
            model = "gpt-3.5-turbo",
            messages = new[] {
                new { role = "user", content = input.Prompt }
            }
        };

        var res = await _http.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", body);
        var json = await res.Content.ReadAsStringAsync();
        return Ok(json);
    }

    [HttpPost("summarize")]
    public async Task<IActionResult> Summarize([FromBody] PromptInput input)
    {
        input.Prompt = "Summarize this:\n" + input.Prompt;
        return await Generate(input);
    }

    [HttpPost("fix")]
    public async Task<IActionResult> Fix([FromBody] PromptInput input)
    {
        input.Prompt = "Correct grammar and punctuation:\n" + input.Prompt;
        return await Generate(input);
    }
}