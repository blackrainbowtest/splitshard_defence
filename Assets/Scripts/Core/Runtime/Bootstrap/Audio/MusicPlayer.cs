using UnityEngine;

using SHD.Core.Audio;

namespace SHD.Core.Bootstrap.Audio
{
	public class MusicPlayer
	{
		private readonly AudioSource _source_a;
		private readonly AudioSource _source_b;

		private AudioSource _active_source;
		private AudioSource _idle_source;

		private float _fade_duration;
		private float _fade_elapsed;
		private float _fade_from_base_volume;
		private float _fade_to_base_volume;
		private bool _is_fading;
		private bool _is_fading_out_only;
		private float _music_volume_scale = 1f;
		private float _active_base_volume;
		private float _idle_base_volume;

		public MusicPlayer(AudioSource source_a, AudioSource source_b)
		{
			_source_a = source_a;
			_source_b = source_b;
			_active_source = _source_a;
			_idle_source = _source_b;
		}

		public void PlayCue(AudioCue cue, bool restart_if_same_clip_is_playing, float crossfade_seconds)
		{
			AudioClip clip;
			float target_volume;
			float target_pitch;

			if (cue == null || cue.Clips == null || cue.Clips.Length == 0)
				return;
			if (_active_source == null || _idle_source == null)
				return;

			clip = PickClip(cue.Clips);
			if (clip == null)
				return;

			if (restart_if_same_clip_is_playing == false &&
				_active_source.isPlaying == true &&
				_active_source.clip == clip)
				return;

			target_volume = Mathf.Clamp01(cue.Volume * _music_volume_scale);
			target_pitch = Mathf.Clamp(cue.Pitch, -3f, 3f);
			_idle_base_volume = Mathf.Clamp01(cue.Volume);

			_idle_source.Stop();
			_idle_source.clip = clip;
			_idle_source.loop = cue.Loop;
			_idle_source.pitch = target_pitch;
			_idle_source.spatialBlend = 0f;
			_idle_source.volume = 0f * _music_volume_scale;
			_idle_source.Play();

			if (_active_source.isPlaying == false || crossfade_seconds <= 0.01f)
			{
				_active_source.Stop();
				_idle_source.volume = target_volume;
				_active_base_volume = _idle_base_volume;
				SwapSources();
				_is_fading = false;
				return;
			}

			_fade_duration = crossfade_seconds;
			_fade_elapsed = 0f;
			_fade_from_base_volume = _active_base_volume;
			_fade_to_base_volume = _idle_base_volume;
			_is_fading = true;
		}

		public void Tick(float delta_time)
		{
			float t;

			if (_is_fading == false)
				return;
			if (_active_source == null || _idle_source == null)
			{
				_is_fading = false;
				return;
			}

			_fade_elapsed += Mathf.Max(0f, delta_time);
			t = _fade_duration <= 0.001f ? 1f : Mathf.Clamp01(_fade_elapsed / _fade_duration);

			_active_base_volume = Mathf.Lerp(_fade_from_base_volume, 0f, t);
			_active_source.volume = _active_base_volume * _music_volume_scale;
			if (_is_fading_out_only == false)
			{
				_idle_base_volume = Mathf.Lerp(0f, _fade_to_base_volume, t);
				_idle_source.volume = _idle_base_volume * _music_volume_scale;
			}

			if (t >= 1f)
			{
				_active_source.Stop();
				_active_base_volume = 0f;
				if (_is_fading_out_only == false)
					SwapSources();
				_is_fading = false;
				_is_fading_out_only = false;
			}
		}

		public void Stop(float fade_out_seconds)
		{
			if (_active_source == null)
				return;

			if (_active_source.isPlaying == false)
			{
				_active_source.Stop();
				return;
			}

			if (fade_out_seconds <= 0.01f)
			{
				_active_source.Stop();
				return;
			}

			_fade_duration = fade_out_seconds;
			_fade_elapsed = 0f;
			_fade_from_base_volume = _active_base_volume;
			_fade_to_base_volume = 0f;
			_is_fading = true;
			_is_fading_out_only = true;
		}

		public void SetMusicVolumeScale(float scale)
		{
			_music_volume_scale = Mathf.Clamp01(scale);
			ApplyScaledVolumes();
		}

		private AudioClip PickClip(AudioClip[] clips)
		{
			int index;

			if (clips == null || clips.Length == 0)
				return (null);

			index = Random.Range(0, clips.Length);
			return (clips[index]);
		}

		private void SwapSources()
		{
			AudioSource temp;
			float temp_base;

			temp = _active_source;
			_active_source = _idle_source;
			_idle_source = temp;

			temp_base = _active_base_volume;
			_active_base_volume = _idle_base_volume;
			_idle_base_volume = temp_base;
		}

		private void ApplyScaledVolumes()
		{
			if (_active_source != null)
				_active_source.volume = Mathf.Clamp01(_active_base_volume * _music_volume_scale);
			if (_idle_source != null)
				_idle_source.volume = Mathf.Clamp01(_idle_base_volume * _music_volume_scale);
		}
	}
}
