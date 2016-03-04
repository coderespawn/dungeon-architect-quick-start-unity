using UnityEngine;
using System.Collections;
using DungeonArchitect;
using DungeonArchitect.Utils;

namespace SimpleCity
{
    public class SimpleCityDungeonBuilder : DungeonBuilder
    {
        SimpleCityDungeonConfig demoConfig;
        SimpleCityDungeonModel demoModel;

        new System.Random random;
        /// <summary>
        /// Builds the dungeon layout.  In this method, you should build your dungeon layout and save it in your model file
        /// No markers should be emitted here.   (EmitMarkers function will be called later by the engine to do that)
        /// </summary>
        /// <param name="config">The builder configuration</param>
        /// <param name="model">The dungeon model that the builder will populate</param>
        public override void BuildDungeon(DungeonConfig config, DungeonModel model)
        {
            base.BuildDungeon(config, model);

            random = new System.Random((int)config.Seed);

            // We know that the dungeon prefab would have the appropriate config and models attached to it
            // Cast and save it for future reference
            demoConfig = config as SimpleCityDungeonConfig;
            demoModel = model as SimpleCityDungeonModel;
            demoModel.Config = demoConfig;

            // Generate the city layout and save it in a model.   No markers are emitted here. 
            GenerateCityLayout();
        }

        /// <summary>
        /// Override the builder's emit marker function to emit our own markers based on the layout that we built
        /// You should emit your markers based on the layout you have saved in the model generated previously
        /// When the user is designing the theme interactively, this function will be called whenever the graph state changes,
        /// so the theme engine can populate the scene (BuildDungeon will not be called if there is no need to rebuild the layout again)
        /// </summary>
        public override void EmitMarkers()
        {
            base.EmitMarkers();
            EmitCityMarkers();
        }

        /// <summary>
        /// Generate a layout and save it in the model
        /// </summary>
        void GenerateCityLayout()
        {
            var width = random.Range(demoConfig.minSize, demoConfig.maxSize);
            var length = random.Range(demoConfig.minSize, demoConfig.maxSize);

            demoModel.Cells = new SimpleCityCell[width, length];

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < length; z++)
                {
                    var cell = new SimpleCityCell();
                    cell.Position = new IntVector(x, 0, z);
                    cell.CellType = SimpleCityCellType.House;
                    cell.Rotation = GetRandomRotation();
                    demoModel.Cells[x, z] = cell;
                }
            }


            // Build a road network by removing some houses 
            // First build roads along the edge of the map
            for (int x = 0; x < width; x++)
            {
                MakeRoad(x, 0);
                MakeRoad(x, length - 1);
            }
            for (int z = 0; z < length; z++)
            {
                MakeRoad(0, z);
                MakeRoad(width - 1, z);
            }

            // Create roads in-between
            for (int x = GetRandomBlockSize() + 1; x < width; x += GetRandomBlockSize() + 1)
            {
                if (width - x <= 2) continue;
                for (int z = 0; z < length; z++)
                {
                    MakeRoad(x, z);
                }
            }
            for (int z = GetRandomBlockSize() + 1; z < length; z += GetRandomBlockSize() + 1)
            {
                if (length  - z <= 2) continue;
                for (int x = 0; x < width; x++)
                {
                    MakeRoad(x, z);
                }
            }
        }

        /// <summary>
        /// Turns a house cell into a road
        /// </summary>
        /// <param name="cell"></param>
        void MakeRoad(int x, int z)
        {
            var cell = demoModel.Cells[x, z];
            cell.CellType = SimpleCityCellType.Road;
            cell.Rotation = Quaternion.identity;
        }

        /// <summary>
        /// Emit marker points so that the theme can decorate the scene layout that we just built
        /// </summary>
        void EmitCityMarkers()
        {
            var basePosition = transform.position;
            var cells = demoModel.Cells;
            var width = cells.GetLength(0);
            var length = cells.GetLength(1);
            var cellSize = new Vector3(demoConfig.CellSize.x, 0, demoConfig.CellSize.y);

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < length; z++)
                {
                    var cell = cells[x, z];
                    var worldPosition = cell.Position * cellSize + basePosition;
                    string markerName;
                    Quaternion rotation;
                    if (cell.CellType == SimpleCityCellType.House)
                    {
                        markerName = "House";
                        rotation = cell.Rotation;
                    }
                    else
                    {
                        float angle = 0;
                        markerName = RoadBeautifier.GetRoadMarkerName(x, z, cells, out angle);
                        rotation = Quaternion.Euler(0, angle, 0);
                    }
                    var markerTransform = Matrix4x4.TRS(worldPosition, rotation, Vector3.one);
                    EmitMarker(markerName, markerTransform, cell.Position, -1);
                }
            }
        }

        Quaternion GetRandomRotation()
        {
            // Randomly rotate in steps of 90
            var angle = random.Next(0, 4) * 90;
            return Quaternion.Euler(0, angle, 0);
        }

        int GetRandomBlockSize()
        {
            return random.Next(demoConfig.minBlockSize, demoConfig.maxBlockSize + 1);
        }
    }
}