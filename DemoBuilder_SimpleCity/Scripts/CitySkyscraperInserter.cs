//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect;
using DungeonArchitect.Utils;
using DungeonArchitect.Builders.SimpleCity;

[System.Serializable]
public class SkyscraperRange
{
	public float startDistance;
	public float endDistance;
	public string markerName;
};

public class CitySkyscraperInserter : DungeonEventListener {
	public SkyscraperRange[] skyscraperRanges;

	public override void OnDungeonMarkersEmitted(Dungeon dungeon, DungeonModel model, LevelMarkerList markers) { 
		var cityModel = model as SimpleCityDungeonModel;
		var width = cityModel.CityWidth;
		var height = cityModel.CityHeight;
		var center = Vector3.Scale(new Vector3 (width / 2.0f, 0, height / 2.0f), new Vector3 (cityModel.Config.CellSize.x, 0, cityModel.Config.CellSize.y));
		center += dungeon.transform.position;

		foreach (var marker in markers) {
			if (marker.SocketType == SimpleCityDungeonMarkerNames.House) {
				var distanceFromCenter = (Matrix.GetTranslation (ref marker.Transform) - center).magnitude;
				foreach (var rangeInfo in skyscraperRanges) {
					if (distanceFromCenter >= rangeInfo.startDistance && distanceFromCenter <= rangeInfo.endDistance) {
						marker.SocketType = rangeInfo.markerName;
						break;
					}
				}
			}
		}
	}

}
