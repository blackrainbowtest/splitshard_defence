using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

using SHD.Core.Audio;

namespace SHD.UI.Mono
{
	[RequireComponent(typeof(Button))]
	public class UIButtonClickSoundMono : MonoBehaviour
	{
		[SerializeField] private UIButtonSoundProfile _sound_profile;
		[SerializeField] private AudioCue _override_click_cue;

		private Button _button;
		private MonoBehaviour _audio_manager_mono;
		private MethodInfo _play_cue_method;

		private void Awake()
		{
			_button = GetComponent<Button>();
			ResolveAudioManager();
		}

		private void OnEnable()
		{
			if (_button != null)
				_button.onClick.AddListener(HandleClicked);
		}

		private void OnDisable()
		{
			if (_button != null)
				_button.onClick.RemoveListener(HandleClicked);
		}

		private void HandleClicked()
		{
			AudioCue cue;
			float volume_scale;

			cue = _override_click_cue != null ? _override_click_cue : (_sound_profile != null ? _sound_profile.ClickCue : null);
			volume_scale = _sound_profile != null ? _sound_profile.VolumeScale : 1f;

			if (cue == null)
				return;

			if (_audio_manager_mono == null || _play_cue_method == null)
				ResolveAudioManager();
			if (_audio_manager_mono == null || _play_cue_method == null)
				return;

			_play_cue_method.Invoke(_audio_manager_mono, new object[] { cue, volume_scale });
		}

		private void ResolveAudioManager()
		{
			MonoBehaviour[] behaviours;
			MonoBehaviour candidate;
			int i;

			_audio_manager_mono = null;
			_play_cue_method = null;

			behaviours = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude);
			i = 0;
			while (i < behaviours.Length)
			{
				candidate = behaviours[i];
				if (candidate != null && candidate.GetType().FullName == "SHD.Core.Bootstrap.Mono.AudioManagerMono")
				{
					_audio_manager_mono = candidate;
					_play_cue_method = candidate.GetType().GetMethod("PlayCueOneShot", BindingFlags.Public | BindingFlags.Instance);
					return;
				}

				i++;
			}
		}
	}
}
