//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect;
using DungeonArchitect.Builders.Grid;

public class AdjacentCorridorTracker : MonoBehaviour {

    public Dungeon dungeon;
    GridDungeonModel gridModel;
    Material materialCursor;
    Material materialCorridors;
    Material materialRooms;

    List<int> ConnectedCorridors = new List<int>();
    List<int> ConnectedRooms = new List<int>();
    int cursorCellId;

    void Start()
    {
        if (dungeon != null)
        {
            // Requires a rebuild for now as the state is not fully saved (data in hash sets is not serialized properly)
            dungeon.Build();
            gridModel = dungeon.GetComponent<GridDungeonModel>();
        }

        // Setup materials
        materialCursor = CreateMaterial(Color.white);
        materialCorridors = CreateMaterial(Color.yellow);
        materialRooms = CreateMaterial(Color.red);
    }

    Material CreateMaterial(Color color)
    {
        var shader = Shader.Find("Hidden/Internal-Colored");
        var mat = new Material(shader);
        mat.hideFlags = HideFlags.HideAndDontSave;
        mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        mat.SetInt("_ZWrite", 0);
        mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
        mat.SetColor("_Color", color);

        return mat;
    }

    void Update() {

        ConnectedCorridors.Clear();
        ConnectedRooms.Clear();

        var cellAtMouse = FindCellAtMousePosition(Camera.main);
        if (cellAtMouse == null) return;
        
        GridBuilderUtils.GetAdjacentCorridors(gridModel, cellAtMouse.Id, ref ConnectedCorridors, ref ConnectedRooms);

        cursorCellId = cellAtMouse.Id;
        // We don't want to override the color of the cell under the mouse 
        ConnectedCorridors.Remove(cursorCellId);
        ConnectedRooms.Remove(cursorCellId);
    }
    
    void OnPostRender()
    {

        // activate the first shader pass (in this case we know it is the only pass)
        
        // draw a quad over whole screen
        DrawCells(ConnectedCorridors, materialCorridors);
        DrawCells(ConnectedRooms, materialRooms);

        var cursorCells = new List<int>();
        cursorCells.Add(cursorCellId);
        DrawCells(cursorCells, materialCursor);
    }

    void DrawCells(List<int> cellIds, Material mat)
    {
        if (gridModel == null || gridModel.Config == null) return;
        var gridSize = gridModel.Config.GridCellSize;
        
        mat.SetPass(0);

        GL.Begin(GL.QUADS);

        foreach (var cellId in cellIds)
        {
            var cell = gridModel.GetCell(cellId);
            if (cell == null) continue;

            var start = cell.Bounds.Location * gridSize;
            var size = cell.Bounds.Size * gridSize;

            DrawQuad(start, size);
        }

        GL.End();
    }

    void DrawQuad(Vector3 start, Vector3 size)
    {
        float y = start.y;
        float x0 = start.x;
        float x1 = start.x + size.x;
        float z0 = start.z;
        float z1 = start.z + size.z;
        
        GL.Vertex3(x0, y, z0);
        GL.Vertex3(x1, y, z0);
        GL.Vertex3(x1, y, z1);
        GL.Vertex3(x0, y, z1);
    }

    Cell FindCellAtMousePosition(Camera cam) {
        if (gridModel == null || gridModel.Config == null) return null;
        var gridSize = gridModel.Config.GridCellSize;
        
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        Cell bestCell = null;
        float bestDistance = float.MaxValue;

        // plane Z to world position hits
        var hitPositionAtY = new Dictionary<float, Vector3>();
        foreach (var cell in gridModel.Cells)
        {
            var worldY = cell.Bounds.Location.y * gridSize.y;
            if (!hitPositionAtY.ContainsKey(worldY))
            {
                // raycast from here
                var plane = new Plane(Vector3.up, new Vector3(0, worldY, 0));
                float rayDistance;
                plane.Raycast(ray, out rayDistance);
                var hitPoint = ray.GetPoint(rayDistance);
                hitPositionAtY.Add(worldY, hitPoint);
            }

            Vector3 worldHitPoint = hitPositionAtY[worldY];

            // Check if we are within the bounds of the cell
            var start = cell.Bounds.Location * gridSize;
            var size = cell.Bounds.Size * gridSize;

            // Check if we are within the bounds
            Rect bounds2D = new Rect(start.x, start.z, size.x, size.z);
            if (bounds2D.Contains(new Vector2(worldHitPoint.x, worldHitPoint.z)))
            {
                var distance = (worldHitPoint - cam.transform.position).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestCell = cell;
                }
            }
        }

        return bestCell;
    }

    
    
}
