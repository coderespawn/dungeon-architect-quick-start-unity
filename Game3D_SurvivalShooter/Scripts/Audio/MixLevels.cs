//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;
using UnityEngine.Audio;

namespace DungeonArchitect.Samples.ShooterGame {
	public class MixLevels : MonoBehaviour {

		public AudioMixer masterMixer;

		public void SetSfxLvl(float sfxLvl)
		{
			masterMixer.SetFloat("sfxVol", sfxLvl);
		}

		public void SetMusicLvl (float musicLvl)
		{
			masterMixer.SetFloat ("musicVol", musicLvl);
		}
	}
}
