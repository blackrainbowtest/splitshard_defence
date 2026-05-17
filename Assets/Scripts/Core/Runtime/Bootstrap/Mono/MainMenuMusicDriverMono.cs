using UnityEngine;
using UnityEngine.SceneManagement;

using SHD.Core.Audio;

namespace SHD.Core.Bootstrap.Mono
{
	public class MainMenuMusicDriverMono : MonoBehaviour
	{
		[System.Serializable]
		private class SceneMusicBinding
		{
			[SerializeField] private string _scene_name;
			[SerializeField] private AudioCue _music_cue;
			[SerializeField] private float _crossfade_seconds = 0.8f;

			public string SceneName => _scene_name;
			public AudioCue MusicCue => _music_cue;
			public float CrossfadeSeconds => _crossfade_seconds;
		}

		[SerializeField] private SceneMusicBinding[] _scene_music_bindings;
		[SerializeField] private bool _restart_if_same_clip_is_playing = false;
		[SerializeField] private bool _stop_on_unbound_scene = true;
		[SerializeField] private float _stop_fade_seconds = 0.4f;

		private AudioManagerMono _audio_manager;

		private void Awake()
		{
			_audio_manager = GetComponent<AudioManagerMono>();
			if (_audio_manager == null)
				_audio_manager = gameObject.AddComponent<AudioManagerMono>();

			_audio_manager.EnsureInitialized();
		}

		private void OnEnable()
		{
			SceneManager.sceneLoaded += HandleSceneLoaded;
		}

		private void OnDisable()
		{
			SceneManager.sceneLoaded -= HandleSceneLoaded;
		}

		private void Start()
		{
			TryPlayForActiveScene();
		}

		private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			_ = mode;
			TryPlayForScene(scene.name);
		}

		private void TryPlayForActiveScene()
		{
			TryPlayForScene(SceneManager.GetActiveScene().name);
		}

		private void TryPlayForScene(string scene_name)
		{
			SceneMusicBinding binding;

			if (_audio_manager == null)
				return;

			binding = FindBinding(scene_name);
			if (binding == null)
			{
				if (_stop_on_unbound_scene == true)
					_audio_manager.StopMusic(_stop_fade_seconds);
				return;
			}
			if (binding.MusicCue == null || binding.MusicCue.Clips == null || binding.MusicCue.Clips.Length == 0)
			{
				if (_stop_on_unbound_scene == true)
					_audio_manager.StopMusic(_stop_fade_seconds);
				return;
			}

			_audio_manager.PlayMusicCue(binding.MusicCue, _restart_if_same_clip_is_playing, binding.CrossfadeSeconds);
		}

		private SceneMusicBinding FindBinding(string scene_name)
		{
			int i;
			SceneMusicBinding binding;

			if (_scene_music_bindings == null)
				return (null);

			i = 0;
			while (i < _scene_music_bindings.Length)
			{
				binding = _scene_music_bindings[i];
				if (binding != null && binding.SceneName == scene_name)
					return (binding);

				i++;
			}

			return (null);
		}
	}
}
