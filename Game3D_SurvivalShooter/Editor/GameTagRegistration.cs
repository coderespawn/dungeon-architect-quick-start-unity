

using UnityEditor;

namespace DungeonArchitect.Samples.ShooterGame.Editors
{
	public class GameTagRegistration : AssetPostprocessor 
	{
		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			//DungeonEditorHelper.CreateEditorTag(GameTags.Enemy);
		}
	}
}
