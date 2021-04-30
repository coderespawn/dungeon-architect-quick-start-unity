//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Samples.ShooterGame;
using UnityEngine;
using UnityEngine.AI;

namespace DungeonArchitect.Samples
{
    public class GFNavNPCController : MonoBehaviour
    {
        public Transform target;
        
        private NavMeshAgent agent;
        private CharacterController character;
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            character = GetComponent<CharacterController>();
            InvokeRepeating(nameof(SeekToTarget), 0.5f, 1.0f);
        }

        void Update()
        {
            if (agent != null && agent.hasPath)
            {
                character.SimpleMove(agent.desiredVelocity);
            }
        }
        
        void SeekToTarget()
        {
            if (target == null)
            {
                FindTarget();
            }

            if (target != null)
            {
                agent.SetDestination(target.position);
            }
        }

        void FindTarget()
        {
            // Place your logic here.  For this sample, we search for the player in the entire map
            
            var player = GameObject.FindGameObjectWithTag(GameTags.Player);
            if (player != null)
            {
                target = player.transform;
            }
        }
    }
}
