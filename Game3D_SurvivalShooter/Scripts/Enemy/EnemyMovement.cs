using UnityEngine;
using System.Collections;
using DungeonArchitect;
using DungeonArchitect.Navigation;

namespace DAShooter
{
    public class EnemyMovement : MonoBehaviour
    {
        Transform player;               // Reference to the player's position.
        PlayerHealth playerHealth;      // Reference to the player's health.
        EnemyHealth enemyHealth;        // Reference to this enemy's health.
		DungeonNavAgent navAgent;

        void Awake ()
        {
            // Set up the references.
            player = GameObject.FindGameObjectWithTag ("Player").transform;
            playerHealth = player.GetComponent <PlayerHealth> ();
            enemyHealth = GetComponent <EnemyHealth> ();
			navAgent = GetComponent<DungeonNavAgent>();
        }

        void Update ()
        {
            // If the enemy and the player have health left...
            if(enemyHealth.currentHealth > 0 && playerHealth.currentHealth > 0)
            {
                // ... set the destination of the nav mesh agent to the player.
				navAgent.Destination = player.position;
            }
            // Otherwise...
            else
            {
                // ... disable the nav mesh agent.
				navAgent.enabled = false;
            }
        }
    }
}