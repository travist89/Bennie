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

                    // Check if the cursor box intersects the pet box (cursor interaction)
                    bool isCursorIntersects = petBox.Intersects(cursorBox);

                    // Check player facing direction and proximity
                    bool isPlayerFacingPet = IsPlayerFacingPet(Game1.player, pet);
                    bool isPlayerNearPet = IsPlayerNearPet(Game1.player, pet);

                    // If the player is near and facing the pet, or if the cursor intersects with the pet, trigger the interaction
                    if (isCursorIntersects || (isPlayerNearPet && isPlayerFacingPet))
                    {
                        // Play the custom sound when the pet is petted
                        Game1.playSound("cat");

                        //this.Monitor.Log($"Played custom sound for petting the pet at tile {cursorTile}.", LogLevel.Debug);

                        break;
                    }
                }
            }
        }
    }

    // Method to check if the player is facing the pet
    private bool IsPlayerFacingPet(Farmer player, Pet pet)
    {
        // Calculate the difference in position between the player and the pet
        Vector2 playerToPet = pet.Position - player.Position;
        Vector2 facingDirection;

        if (player.FacingDirection == 0)
        {
            facingDirection = new Vector2(0, -1); // Up
        }
        else if (player.FacingDirection == 1)
        {
            facingDirection = new Vector2(1, 0); // Right
        }
        else if (player.FacingDirection == 2)
        {
            facingDirection = new Vector2(0, 1); // Down
        }
        else if (player.FacingDirection == 3)
        {
            facingDirection = new Vector2(-1, 0); // Left
        }
        else
        {
            facingDirection = Vector2.Zero;
        }


        // Check if the player is facing towards the pet
        return Vector2.Dot(playerToPet, facingDirection) > 0;
    }

    // Method to check if the player is near the pet (within a certain distance)
    private bool IsPlayerNearPet(Farmer player, Pet pet)
    {
        // Define the proximity distance (adjust as needed, e.g., 2 tiles or 128 pixels)
        float proximityDistance = 128f; // equivalent to 2 tiles (64 pixels each)
        return Vector2.Distance(player.Position, pet.Position) <= proximityDistance;
    }
}