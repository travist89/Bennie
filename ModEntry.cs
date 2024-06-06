using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using System;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
    }

    private void OnGameLaunched(object sender, EventArgs e)
    {
        // Load and replace the cat sound assets
        this.Helper.Content.AssetEditors.Add(new CustomAudioLoader(this.Helper));
    }
}

public class CustomAudioLoader : IAssetEditor
{
    private IModHelper Helper;

    public CustomAudioLoader(IModHelper helper)
    {
        this.Helper = helper;
    }

    public bool CanEdit<T>(IAssetInfo asset)
    {
        return asset.AssetNameEquals("Animals/Cat") || asset.AssetNameEquals("Animals/Dog");
    }

    public void Edit<T>(IAssetData asset)
    {
        if (asset.AssetNameEquals("Animals/Cat"))
        {
            var sound = this.Helper.Content.Load<SoundEffect>("assets/custom_cat1.wav", ContentSource.ModFolder);
            asset.ReplaceWith(sound);
        }
        else if (asset.AssetNameEquals("Animals/Dog"))
        {
            var sound = this.Helper.Content.Load<SoundEffect>("assets/custom_cat2.wav", ContentSource.ModFolder);
            asset.ReplaceWith(sound);
        }
    }
}
