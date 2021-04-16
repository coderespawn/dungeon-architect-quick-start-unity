using DungeonArchitect;
using UnityEngine;

namespace DungeonArchitect.Samples.SnapGridFlow
{
    public class SGFDemoController : MonoBehaviour
    {
        public Dungeon dungeon;

        void Start()
        {
            if (dungeon != null)
            {
                dungeon.Build();
            }
        }

    }
}
