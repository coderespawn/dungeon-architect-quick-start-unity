//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEngine.Audio;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DungeonArchitect.Samples.ShooterGame {
	public class PauseManager : MonoBehaviour {
		
		public AudioMixerSnapshot paused;
		public AudioMixerSnapshot unpaused;
		
		Canvas canvas;
		
		void Start()
		{
			canvas = GetComponent<Canvas>();
		}
		
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				canvas.enabled = !canvas.enabled;
				Pause();
			}
		}
		
		public void Pause()
		{
			Time.timeScale = Time.timeScale == 0 ? 1 : 0;
			Lowpass ();
			
		}
		
		void Lowpass()
		{
			if (Time.timeScale == 0)
			{
				paused.TransitionTo(.01f);
			}
			
			else
				
			{
				unpaused.TransitionTo(.01f);
			}
		}
		
		public void Quit()
		{
			#if UNITY_EDITOR 
			EditorApplication.isPlaying = false;
			#else 
			Application.Quit();
			#endif
		}
	}
}
