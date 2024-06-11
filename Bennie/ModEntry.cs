using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using StardewValley.Audio;

public class ModEntry : Mod
{
    private IAudioEngine? audioEngine;

    public override void Entry(IModHelper helper)
    {
        // Event handler for game launch to load the custom sound.
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        // Load and replace the existing sound cue.
        ReplaceSound("cat", "assets/bennie.wav");
    }

    private void ReplaceSound(string cueName, string filePath)
    {
        // Get the cue to manipulate.
        var existingCueDef = Game1.soundBank.GetCueDefinition(cueName);

        // Get the audio file and add it to a new SoundEffect, to replace the old one.
        SoundEffect audio;
        string filePathCombined = Path.Combine(this.Helper.DirectoryPath, filePath);
        using (var stream = new FileStream(filePathCombined, FileMode.Open))
        {
            audio = SoundEffect.FromStream(stream);
        }

        // Assign the new audio to this cue.
        audioEngine = Game1.audioEngine;
        existingCueDef.SetSound(audio, audioEngine.GetCategoryIndex("Sound"), false);
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
                if (character is Pet pet && pet.petType == "Cat")
                {
                    // Create a bounding box for the pet
                    Rectangle petBox = new Rectangle((int)pet.Position.X, (int)pet.Position.Y, pet.Sprite.getWidth(), pet.Sprite.getHeight());

                    // Create a bounding box for the tile under the cursor
                    Rectangle cursorBox = new Rectangle((int)tile.X * 64, (int)tile.Y * 64, 64, 64);

                    // Check if the boxes intersect
                    if (petBox.Intersects(cursorBox))
                    {
                        // Play the custom sound when the pet is petted
                        Game1.playSound("cat");

                        // Log to SMAPI console for debugging
                        this.Monitor.Log($"Played custom sound for petting the pet at tile {tile}.", LogLevel.Debug);

                        break;
                    }
                }
            }
        }
    }
}