using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StardewValley.Audio;

public class ModEntry : Mod
{
    private IAudioEngine? audioEngine;

    public override void Entry(IModHelper helper)
    {
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.Input.ButtonPressed += OnButtonPressed;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        ReplaceSound("cat", "assets/bennie.wav");
    }

    private void ReplaceSound(string cueName, string filePath)
    {
        var existingCueDef = Game1.soundBank.GetCueDefinition(cueName);
        string filePathCombined = Path.Combine(this.Helper.DirectoryPath, filePath);
        using (var stream = new FileStream(filePathCombined, FileMode.Open))
        {
            SoundEffect audio = SoundEffect.FromStream(stream);
            audioEngine = Game1.audioEngine;
            existingCueDef.SetSound(audio, audioEngine.GetCategoryIndex("Sound"), false);
        }
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady || !e.Button.IsActionButton()) return;

        Vector2 tile = e.Cursor.Tile;
        foreach (var character in Game1.currentLocation.characters)
        {
            if (character is Pet pet && pet.petType == "Cat")
            {
                Rectangle petBox = new Rectangle((int)pet.Position.X, (int)pet.Position.Y, pet.Sprite.getWidth(), pet.Sprite.getHeight());
                Rectangle cursorBox = new Rectangle((int)tile.X * 64, (int)tile.Y * 64, 64, 64);
                bool isCursorIntersects = petBox.Intersects(cursorBox);
                bool isPlayerFacingPet = IsPlayerFacingPet(Game1.player, pet);
                bool isPlayerNearPet = IsPlayerNearPet(Game1.player, pet);

                if (isCursorIntersects || (isPlayerNearPet && isPlayerFacingPet))
                {
                    Game1.playSound("cat");
                    break;
                }
            }
        }
    }

    private bool IsPlayerFacingPet(Farmer player, Pet pet)
    {
        Vector2 playerToPet = pet.Position - player.Position;
        Vector2 facingDirection = player.FacingDirection switch
        {
            0 => new Vector2(0, -1), // Up
            1 => new Vector2(1, 0),  // Right
            2 => new Vector2(0, 1),  // Down
            3 => new Vector2(-1, 0), // Left
            _ => Vector2.Zero
        };
        return Vector2.Dot(playerToPet, facingDirection) > 0;
    }

    private bool IsPlayerNearPet(Farmer player, Pet pet)
    {
        float proximityDistance = 128f; // equivalent to 2 tiles (64 pixels each)
        return Vector2.Distance(player.Position, pet.Position) <= proximityDistance;
    }
}