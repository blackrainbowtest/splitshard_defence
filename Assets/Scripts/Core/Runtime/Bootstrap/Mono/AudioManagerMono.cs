using UnityEngine;
using System.Collections.Generic;
using SHD.Core.Audio;
using SHD.Core.Bootstrap.Audio;

namespace SHD.Core.Bootstrap.Mono
{
	public class AudioManagerMono : MonoBehaviour
	{
		[SerializeField] private Transform _audio_manager_root;
		[SerializeField] private AudioSource _music_source_a;
		[SerializeField] private AudioSource _music_source_b;
		[SerializeField] private Transform _sfx_pool_root;
		[SerializeField] private int _sfx_pool_size = 16;
		[SerializeField] [Range(0f, 1f)] private float _music_volume = 1f;
		[SerializeField] [Range(0f, 1f)] private float _sfx_volume = 1f;
		[SerializeField] [Range(0f, 1f)] private float _ui_volume = 1f;

		private MusicPlayer _music_player;
		private readonly List<AudioSource> _sfx_sources = new List<AudioSource>();
		private int _sfx_cursor;

		public AudioSource MusicSourceA
		{
			get
			{
				return (_music_source_a);
			}
		}

		public AudioSource MusicSourceB
		{
			get
			{
				return (_music_source_b);
			}
		}

		public Transform SfxPoolRoot
		{
			get
			{
				return (_sfx_pool_root);
			}
		}

		public MusicPlayer MusicPlayer
		{
			get
			{
				return (_music_player);
			}
		}

		public void EnsureInitialized()
		{
			Transform source_a_transform;
			Transform source_b_transform;

			_audio_manager_root = EnsureChild(transform, "AudioManagerRoot");
			source_a_transform = EnsureChild(_audio_manager_root, "MusicSourceA");
			source_b_transform = EnsureChild(_audio_manager_root, "MusicSourceB");
			_sfx_pool_root = EnsureChild(_audio_manager_root, "SfxPoolRoot");

			_music_source_a = EnsureAudioSource(source_a_transform, "MusicSourceA");
			_music_source_b = EnsureAudioSource(source_b_transform, "MusicSourceB");
			EnsureSfxPool();

			if (_music_player == null)
				_music_player = new MusicPlayer(_music_source_a, _music_source_b);
			_music_player.SetMusicVolumeScale(_music_volume);
		}

		private void Update()
		{
			if (_music_player == null)
				return;

			_music_player.Tick(Time.unscaledDeltaTime);
		}

		public void PlayMusicCue(AudioCue cue, bool restart_if_same_clip_is_playing, float crossfade_seconds)
		{
			if (_music_player == null)
				EnsureInitialized();

			if (_music_player == null)
				return;

			_music_player.PlayCue(cue, restart_if_same_clip_is_playing, crossfade_seconds);
		}

		public void StopMusic(float fade_out_seconds)
		{
			if (_music_player == null)
				EnsureInitialized();

			if (_music_player == null)
				return;

			_music_player.Stop(fade_out_seconds);
		}

		public void PlayCueOneShot(AudioCue cue, float volume_scale = 1f)
		{
			AudioSource source;
			AudioClip clip;
			float volume_random;
			float pitch_random;

			if (cue == null || cue.Clips == null || cue.Clips.Length == 0)
				return;

			EnsureSfxPool();
			source = GetNextSfxSource();
			if (source == null)
				return;

			clip = PickRandomClip(cue.Clips);
			if (clip == null)
				return;

			volume_random = Random.Range(cue.VolumeRandomRange.x, cue.VolumeRandomRange.y);
			pitch_random = Random.Range(cue.PitchRandomRange.x, cue.PitchRandomRange.y);

			source.priority = cue.Priority;
			source.spatialBlend = Mathf.Clamp01(cue.SpatialBlend);
			source.minDistance = cue.MinDistance;
			source.maxDistance = cue.MaxDistance;
			source.pitch = Mathf.Clamp(cue.Pitch * pitch_random, -3f, 3f);
			source.PlayOneShot(clip, Mathf.Clamp01(cue.Volume * volume_random * Mathf.Max(0f, volume_scale) * ResolveBusVolume(cue)));
		}

		public void SetMusicVolume(float value)
		{
			_music_volume = Mathf.Clamp01(value);
			if (_music_player != null)
				_music_player.SetMusicVolumeScale(_music_volume);

			if (_music_source_a != null && _music_source_a.isPlaying == true)
				_music_source_a.volume = Mathf.Clamp01(_music_source_a.volume);
			if (_music_source_b != null && _music_source_b.isPlaying == true)
				_music_source_b.volume = Mathf.Clamp01(_music_source_b.volume);
		}

		public void SetSfxVolume(float value)
		{
			_sfx_volume = Mathf.Clamp01(value);
		}

		public void SetUiVolume(float value)
		{
			_ui_volume = Mathf.Clamp01(value);
		}

		private Transform EnsureChild(Transform parent, string child_name)
		{
			Transform child;
			GameObject child_object;

			child = parent.Find(child_name);
			if (child != null)
				return (child);

			child_object = new GameObject(child_name);
			child_object.transform.SetParent(parent, false);
			return (child_object.transform);
		}

		private AudioSource EnsureAudioSource(Transform parent, string debug_name)
		{
			AudioSource source;

			source = parent.GetComponent<AudioSource>();
			if (source == null)
				source = parent.gameObject.AddComponent<AudioSource>();

			source.playOnAwake = false;
			source.loop = false;
			source.spatialBlend = 0f;
			source.dopplerLevel = 0f;

			if (source.outputAudioMixerGroup == null)
				Debug.Log("AudioManagerMono: " + debug_name + " has no AudioMixerGroup yet.");

			return (source);
		}

		private void EnsureSfxPool()
		{
			int i;
			AudioSource source;
			Transform child;

			if (_sfx_pool_root == null)
				return;

			if (_sfx_pool_size < 1)
				_sfx_pool_size = 1;

			if (_sfx_sources.Count >= _sfx_pool_size)
				return;

			i = _sfx_sources.Count;
			while (i < _sfx_pool_size)
			{
				child = EnsureChild(_sfx_pool_root, "SfxSource_" + i.ToString("D2"));
				source = child.GetComponent<AudioSource>();
				if (source == null)
					source = child.gameObject.AddComponent<AudioSource>();

				source.playOnAwake = false;
				source.loop = false;
				source.spatialBlend = 0f;
				source.dopplerLevel = 0f;
				_sfx_sources.Add(source);
				i++;
			}
		}

		private AudioSource GetNextSfxSource()
		{
			int i;
			int index;

			if (_sfx_sources.Count == 0)
				return (null);

			i = 0;
			while (i < _sfx_sources.Count)
			{
				index = (_sfx_cursor + i) % _sfx_sources.Count;
				if (_sfx_sources[index] != null && _sfx_sources[index].isPlaying == false)
				{
					_sfx_cursor = (index + 1) % _sfx_sources.Count;
					return (_sfx_sources[index]);
				}
				i++;
			}

			_sfx_cursor = (_sfx_cursor + 1) % _sfx_sources.Count;
			return (_sfx_sources[_sfx_cursor]);
		}

		private AudioClip PickRandomClip(AudioClip[] clips)
		{
			int index;

			if (clips == null || clips.Length == 0)
				return (null);

			index = Random.Range(0, clips.Length);
			return (clips[index]);
		}

		private float ResolveBusVolume(AudioCue cue)
		{
			if (cue == null)
				return (1f);

			if (cue.Bus == AudioCueBus.UI)
				return (_ui_volume);

			if (cue.Bus == AudioCueBus.SfxCombat || cue.Bus == AudioCueBus.SfxSystem || cue.Bus == AudioCueBus.SfxWorld)
				return (_sfx_volume);

			return (1f);
		}
	}
}
