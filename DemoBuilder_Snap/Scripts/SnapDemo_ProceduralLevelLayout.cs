//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Grammar;

public class SnapDemo_ProceduralLevelLayout : MonoBehaviour, IGrammarGraphBuildScript
{
    public void Generate(IGrammarGraphBuilder grammarBuilder)
    {
        var random = new System.Random();
        int execIndex = 0;
        var spawnRoom = grammarBuilder.CreateNode("SpawnRoom", execIndex++);
        var shop = grammarBuilder.CreateNode("Shop", execIndex++);
        grammarBuilder.LinkNodes(spawnRoom, shop);

        var lastNodeId = spawnRoom;
        int numRoomGraphs = random.Range(2, 5);
        for (int i = 0; i < numRoomGraphs; i++)
        {
            var roomNode = grammarBuilder.CreateNode("RoomGraph", execIndex++);
            grammarBuilder.LinkNodes(lastNodeId, roomNode);
            lastNodeId = roomNode;
            
            // Create branches from here if necessary...
        }

        var bossRoom = grammarBuilder.CreateNode("Boss", execIndex++);
        grammarBuilder.LinkNodes(lastNodeId, bossRoom);

        var exitRoom = grammarBuilder.CreateNode("Exit", execIndex++);
        grammarBuilder.LinkNodes(bossRoom, exitRoom);
    }
}
