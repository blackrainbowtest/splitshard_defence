using UnityEngine;
using UnityEngine.Audio;

namespace SHD.Core.Audio
{
	public enum AudioCueBus
	{
		Music = 0,
		UI = 1,
		SfxCombat = 2,
		SfxWorld = 3,
		SfxSystem = 4,
		Ambience = 5,
		Voice = 6
	}

	[CreateAssetMenu(fileName = "audio_cue", menuName = "SHD/Audio Cue", order = 100)]
	public class AudioCue : ScriptableObject
	{
		[Header("Identity")]
		[SerializeField] private string _cue_id = "audio.cue.id";
		[SerializeField] private AudioCueBus _bus = AudioCueBus.SfxSystem;

		[Header("Clips")]
		[SerializeField] private AudioClip[] _clips;
		[SerializeField] private bool _shuffle_without_repeat = true;

		[Header("Playback")]
		[SerializeField] private bool _loop = false;
		[SerializeField] private bool _allow_overlap = true;
		[SerializeField] private float _cooldown_seconds = 0f;
		[SerializeField] private int _max_simultaneous = 6;
		[SerializeField] private int _priority = 128;

		[Header("Volume / Pitch")]
		[SerializeField] private float _volume = 1f;
		[SerializeField] private Vector2 _volume_random_range = new Vector2(0.95f, 1.05f);
		[SerializeField] private float _pitch = 1f;
		[SerializeField] private Vector2 _pitch_random_range = new Vector2(0.98f, 1.02f);

		[Header("Spatial (Use 0 for 2D mobile policy)")]
		[SerializeField] private float _spatial_blend = 0f;
		[SerializeField] private float _min_distance = 1f;
		[SerializeField] private float _max_distance = 20f;

		[Header("Optional Routing")]
		[SerializeField] private AudioMixerGroup _explicit_output_group;

		public string CueId => _cue_id;
		public AudioCueBus Bus => _bus;
		public AudioClip[] Clips => _clips;
		public bool ShuffleWithoutRepeat => _shuffle_without_repeat;
		public bool Loop => _loop;
		public bool AllowOverlap => _allow_overlap;
		public float CooldownSeconds => _cooldown_seconds;
		public int MaxSimultaneous => _max_simultaneous;
		public int Priority => _priority;
		public float Volume => _volume;
		public Vector2 VolumeRandomRange => _volume_random_range;
		public float Pitch => _pitch;
		public Vector2 PitchRandomRange => _pitch_random_range;
		public float SpatialBlend => _spatial_blend;
		public float MinDistance => _min_distance;
		public float MaxDistance => _max_distance;
		public AudioMixerGroup ExplicitOutputGroup => _explicit_output_group;

		private void OnValidate()
		{
			_volume = Mathf.Max(0f, _volume);
			_pitch = Mathf.Clamp(_pitch, -3f, 3f);
			_spatial_blend = Mathf.Clamp01(_spatial_blend);
			_cooldown_seconds = Mathf.Max(0f, _cooldown_seconds);
			_max_simultaneous = Mathf.Max(1, _max_simultaneous);
			_priority = Mathf.Clamp(_priority, 0, 256);

			_volume_random_range.x = Mathf.Max(0f, _volume_random_range.x);
			_volume_random_range.y = Mathf.Max(_volume_random_range.x, _volume_random_range.y);

			_pitch_random_range.x = Mathf.Clamp(_pitch_random_range.x, -3f, 3f);
			_pitch_random_range.y = Mathf.Clamp(_pitch_random_range.y, _pitch_random_range.x, 3f);

			_min_distance = Mathf.Max(0.01f, _min_distance);
			_max_distance = Mathf.Max(_min_distance, _max_distance);
		}
	}
}
