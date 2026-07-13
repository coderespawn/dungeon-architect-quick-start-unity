//$ Copyright 2015-25, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;

namespace DungeonArchitect.Samples.ShooterGame {
	public class RandomParticlePoint : MonoBehaviour 
	{
	    [Range(0f, 1f)]
	    public float normalizedTime;


	    void OnValidate()
	    {
	        GetComponent<ParticleSystem>().Simulate (normalizedTime, true, true);
	    }
	}
}