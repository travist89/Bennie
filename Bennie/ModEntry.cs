using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using Microsoft.Xna.Framework;

namespace Bennie
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // Check if the world is ready
            if (!Context.IsWorldReady)
                return;

            // Check if the button pressed is an action button (left-click or controller button)
            if (e.Button.IsActionButton())
            {
                // Get the tile location where the cursor is pointing
                Vector2 tile = e.Cursor.Tile;

                // Convert the tile information to a string and print to console
                string tileString = tile.ToString();
                this.Monitor.Log(tileString, LogLevel.Debug);

                // Iterate through all characters in the current location
                foreach (var character in Game1.currentLocation.characters)
                {
                    // Check if the character is a pet
                    if (character is Pet pet)
                    {
                        // Get the tile location of the pet using the Position property
                        Vector2 petTile = pet.Position / 64f;

                        // Convert the tile information to a string and print to console
                        string petTileString = petTile.ToString();
                        this.Monitor.Log(petTileString, LogLevel.Debug);

                        // Check if the pet's tile location matches the cursor's tile location
                        if (petTile.Equals(tile))
                        {
                            // Play the custom sound when the pet is petted
                            Game1.playSound("rooster");

                            // Log to SMAPI console for debugging
                            this.Monitor.Log($"Played custom sound for petting the pet at tile {tile}.", LogLevel.Debug);

                            break;
                        }
                    }
                }
            }
        }
    }
}
