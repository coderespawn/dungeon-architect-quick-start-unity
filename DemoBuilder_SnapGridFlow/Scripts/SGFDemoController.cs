using DungeonArchitect;
using UnityEngine;

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
