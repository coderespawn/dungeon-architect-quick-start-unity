using System.Collections;
using System.Collections.Generic;
using DungeonArchitect;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonArchitect.Samples.SGF.ProceduralRooms
{
    [ExecuteInEditMode]
    public class ProcSgfRoomModule : MonoBehaviour
    {
        public Dungeon moduleDungeon;
        public int localSeed = 0;
        private int masterSeed = 0;
        private IDungeonSceneObjectInstantiator cachedObjectInstantiator = null;
        
        public void BuildRoom(int masterSeed, IDungeonSceneObjectInstantiator objectInstantiator)
        {
            if (objectInstantiator == null)
            {
                objectInstantiator = new RuntimeDungeonSceneObjectInstantiator();
            }
            
            cachedObjectInstantiator = objectInstantiator;
            if (moduleDungeon != null && moduleDungeon.Config != null)
            {
                moduleDungeon.Config.Seed = (uint)masterSeed;
                moduleDungeon.Build(objectInstantiator);
            }
        }

        public void DestroyRoom()
        {
            if (moduleDungeon != null)
            {
                moduleDungeon.DestroyDungeon();
            }
        }
        
        private int GenerateDeterministicSeed()
        {
            var pos = transform.position;
            unchecked // Allow integer overflow
            {
                int hash = 23;
                hash = hash * 31 + Mathf.RoundToInt(pos.x);
                hash = hash * 31 + Mathf.RoundToInt(pos.y);
                hash = hash * 31 + Mathf.RoundToInt(pos.z);
                hash = hash * 31 + Mathf.RoundToInt(transform.rotation.eulerAngles.z * 100);
                hash = hash * 31 + localSeed;
                hash = hash * 31 + masterSeed;
                return hash;
            }
        }
        public void RebuildRoom()
        {
            var dungeonSeed = GenerateDeterministicSeed();
            BuildRoom(dungeonSeed, cachedObjectInstantiator);
        }
        
        public void RandomizeRoom()
        {
            localSeed = Random.Range(0, int.MaxValue);
            RebuildRoom();
        }
    }
}