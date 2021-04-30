//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections;
using DungeonArchitect;


public class RegenerateDungeonLayout : MonoBehaviour {
    public Dungeon dungeon;

    /// <summary>
    /// If we have static geometry already in the level created during design time, then the pooled scene
    /// provider cannot re-use it because the editor would have performed optimizations on it and might not be able to move it
    /// This flag clears out any design time static geometry before rebuilding to avoid movement issues of static objects
    /// </summary>
    bool performCleanRebuild = true;

	// Use this for initialization
	void Start () {
        StartCoroutine(RebuildDungeon());
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(RebuildDungeon());
        }
	}

    IEnumerator RebuildDungeon()
    {
        if (dungeon != null)
        {
            if (performCleanRebuild)
            {
                // We want to remove design time data with a clean destroy since editor would allow modification of optimized static game objects
                // We want to do this only for the first time
                dungeon.DestroyDungeon();
                performCleanRebuild = false;

                // Wait for 1 frame to make sure our design time objects were destroyed
                yield return 0;
            }

            // Build the dungeon
            var config = dungeon.Config;
            if (config != null)
            {
                config.Seed = (uint)(Random.value * uint.MaxValue);
                dungeon.Build();
            }
        }
    }
}
