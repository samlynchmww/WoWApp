# WoWApp - World of Warcraft Character Viewer

WoWApp is a simple ASP.NET Core Razor Pages application that allows you to view World of Warcraft characters.  

## Features

- Search by **realm** and **character name**.
- Attempts to fetch character data from the **Blizzard Profile API**.
- If the API does not return data (e.g., character not synced), provides a **direct Armory link** instead.

## Setup

1. Clone the repository:
   ```bash
   git clone <your-repo-url>
Navigate to the project directory and open it in Visual Studio 2022.

Configure Blizzard API credentials using dotnet user-secrets:

bash
Copy code
dotnet user-secrets init
dotnet user-secrets set "BlizzardApi:ClientId" "<your-client-id>"
dotnet user-secrets set "BlizzardApi:ClientSecret" "<your-client-secret>"
dotnet user-secrets set "BlizzardApi:Region" "us"
Run the project in Visual Studio (F5).

Usage
Enter the realm and character name in the form.

If the character is available via the API, JSON data will be displayed.

If not, a link to the official WoW Armory will be provided.

Notes
Only real, logged-in characters will return data from the API. NPCs or lore characters like "Jaina" or "Thrall" will not appear.

The app normalizes input (lowercase, removes spaces/apostrophes) for API requests.
