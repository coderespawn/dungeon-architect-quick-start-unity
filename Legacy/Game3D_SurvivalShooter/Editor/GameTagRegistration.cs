//$ Copyright 2015-25, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
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