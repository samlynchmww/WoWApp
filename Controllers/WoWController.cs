using Microsoft.AspNetCore.Mvc;
using WoWApp.Services;

[Route("api/[controller]")]
[ApiController]
public class WoWController : ControllerBase
{
    private readonly BlizzardApiService _blizzardApi;

    public WoWController(BlizzardApiService blizzardApi)
    {
        _blizzardApi = blizzardApi;
    }

    [HttpGet("character/{realm}/{name}")]
    public async Task<IActionResult> GetCharacter(string realm, string name)
    {
        var data = await _blizzardApi.GetCharacterAsync(realm, name);
        return Ok(data);
    }
}
