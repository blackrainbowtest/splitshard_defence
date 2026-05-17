using UnityEngine;

namespace SHD.Core.Audio
{
	[CreateAssetMenu(fileName = "ui_button_sound_profile", menuName = "SHD/Audio/UI Button Sound Profile", order = 101)]
	public class UIButtonSoundProfile : ScriptableObject
	{
		[SerializeField] private AudioCue _click_cue;
		[SerializeField] private float _volume_scale = 1f;

		public AudioCue ClickCue => _click_cue;
		public float VolumeScale => Mathf.Max(0f, _volume_scale);
	}
}
