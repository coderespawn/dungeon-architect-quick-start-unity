using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DungeonArchitect.Samples.Navigation
{
    public class NavigationDemoNPC : MonoBehaviour
    {
        NavMeshAgent agent;
        CharacterController character;

        public Transform target;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            character = GetComponent<CharacterController>();
        }

        void Update()
        {
            if (target != null)
            {
                agent.SetDestination(target.position);

                if (agent.remainingDistance > agent.stoppingDistance)
                {
                    character.SimpleMove(agent.desiredVelocity);
                }
                else
                {
                    character.SimpleMove(Vector3.zero);
                }
            }
            
        }
    }
}
