//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Samples.ShooterGame
{
    public class MiniMapRenderer : MonoBehaviour
    {
        public Texture miniMapRtt;
        public Material miniMapMat;
        public int width = 256;
        public int height = 256;


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
        }


        void OnGUI()
        {
            if (Event.current.type.Equals(EventType.Repaint))
            {
                var padding = 10;
                var x = Screen.width - width - padding;
                var y = Screen.height - height - padding;
                var rect = new Rect(x, y, width, height);
                Graphics.DrawTexture(rect, miniMapRtt, miniMapMat);
            }

        }

    }
}
