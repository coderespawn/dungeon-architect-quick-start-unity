//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;
using UnityEngine.UI;

namespace DungeonArchitect.Samples.ShooterGame
{
    public class ScoreManager : MonoBehaviour
    {
        public static int score;        // The player's score.


        Text text;                      // Reference to the Text component.


        void Awake ()
        {
            // Set up the reference.
            text = GetComponent <Text> ();

            // Reset the score.
            score = 0;
        }


        void Update ()
        {
            // Set the displayed text to be the word "Score" followed by the score value.
            text.text = "Score: " + score;
        }
    }
}