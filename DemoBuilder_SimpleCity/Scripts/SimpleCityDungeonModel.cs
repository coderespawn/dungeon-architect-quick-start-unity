using UnityEngine;
using System.Collections;
using DungeonArchitect;
using DungeonArchitect.Utils;

namespace SimpleCity
{

    public enum SimpleCityCellType
    {
        Road,
        House
    }

    public class SimpleCityCell
    {
        public IntVector Position;
        public SimpleCityCellType CellType;
        public Quaternion Rotation;
    }

    public class SimpleCityDungeonModel : DungeonModel
    {
        [HideInInspector]
        public SimpleCityCell[,] Cells = new SimpleCityCell[0, 0];

        [HideInInspector]
        public SimpleCityDungeonConfig Config;


    }

}