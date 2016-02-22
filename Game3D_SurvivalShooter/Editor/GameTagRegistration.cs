//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using UnityEditor;
using System.Collections;
using DungeonArchitect.Editors;

namespace DAShooter.Editors
{
	public class GameTagRegistration : AssetPostprocessor 
	{
		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			//DungeonEditorHelper.CreateEditorTag(GameTags.Enemy);
		}
	}
}
