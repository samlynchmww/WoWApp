using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WoWApp.Services;
using System.Text.Json;

namespace WoWApp.Pages
{
	public class CharacterModel : PageModel
	{
		private readonly BlizzardApiService _blizzardApi;

		public CharacterModel(BlizzardApiService blizzardApi)
		{
			_blizzardApi = blizzardApi;
		}

		[BindProperty]
		public string Realm { get; set; } = "";

		[BindProperty]
		public string CharacterName { get; set; } = "";

		public JsonDocument? CharacterData { get; set; }
		public string? ArmoryLink { get; set; }

		public async Task OnPostAsync()
		{
			try
			{
				var (json, region) = await _blizzardApi.GetCharacterAsync(Realm, CharacterName);

				if (json != null)
				{
					CharacterData = JsonDocument.Parse(json);
				}
				else
				{
					// Fallback to Armory link if API has no data
					ArmoryLink = $"https://worldofwarcraft.blizzard.com/en-us/character/{region}/{Slugify(Realm)}/{Slugify(CharacterName)}";
				}
			}
			catch (HttpRequestException ex)
			{
				Console.WriteLine(ex.Message);
				ModelState.AddModelError(string.Empty, "Failed to fetch character.");
			}
		}

		private string Slugify(string input)
		{
			return input.ToLowerInvariant()
						.Replace("'", "")
						.Replace(" ", "-");
		}
	}
}
