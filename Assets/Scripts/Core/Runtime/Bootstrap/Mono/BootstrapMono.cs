using UnityEngine;
using UnityEngine.SceneManagement;
using System;

using SHD.Core.Bootstrap;
using SHD.Core.Bootstrap.Startup;
using SHD.Loading.Mono;
using SHD.Loading.Orchestration;

namespace SHD.Core.Bootstrap.Mono
{
	public class BootstrapMono : MonoBehaviour
	{
		private GameBootstrap _game_bootstrap;
		private LoadingMonoBridge _loading_mono_bridge;
		private StartupFlowConfig _startup_flow_config;

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);

			InitializeBootstrap();
		}

		private async void Start()
		{
			await RunStartupFlowAsync();
		}

		private void InitializeBootstrap()
		{
			string settings_path;
			string save_directory;

			settings_path = Application.persistentDataPath + "/settings.json";
			save_directory = Application.persistentDataPath + "/saves";

			_game_bootstrap = new GameBootstrap(settings_path, save_directory);
			InitializeLoadingBridge();
			_startup_flow_config = StartupFlowConfigLoader.Load();
		}

		private void InitializeLoadingBridge()
		{
			_loading_mono_bridge = GetComponent<LoadingMonoBridge>();
			if (_loading_mono_bridge == null)
				_loading_mono_bridge = gameObject.AddComponent<LoadingMonoBridge>();

			_loading_mono_bridge.Initialize(_game_bootstrap.LoadingService);
		}

		public void SetLanguage(string language)
		{
			if (_game_bootstrap == null)
				return ;

			_game_bootstrap.SetLanguage(language);
		}

		private async System.Threading.Tasks.Task RunStartupFlowAsync()
		{
			string active_scene_name;
			float wait_seconds;
			int wait_milliseconds;

			if (_loading_mono_bridge == null || _startup_flow_config == null)
				return ;

			if (_startup_flow_config.AutoRun == false)
				return ;

			active_scene_name = SceneManager.GetActiveScene().name;
			if (active_scene_name != _startup_flow_config.BootstrapScene)
				return ;

			if (string.IsNullOrWhiteSpace(_startup_flow_config.LogoScene) == false)
			{
				await _loading_mono_bridge.LoadSceneByNameAsync(_startup_flow_config.LogoScene);

				wait_seconds = Mathf.Max(0f, _startup_flow_config.LogoMinDurationSeconds);
				wait_milliseconds = (int)Math.Ceiling(wait_seconds * 1000f);

				if (wait_milliseconds > 0)
					await System.Threading.Tasks.Task.Delay(wait_milliseconds);
			}

			if (string.IsNullOrWhiteSpace(_startup_flow_config.MainMenuScene) == false)
			{
				StartupFlowRuntimeState.SetPendingTarget(
					_startup_flow_config.MainMenuScene,
					_startup_flow_config.WaitForTapToContinueOnLoading
				);

				if (string.IsNullOrWhiteSpace(_startup_flow_config.LoadingScene) == false)
					await _loading_mono_bridge.LoadSceneByNameAsync(_startup_flow_config.LoadingScene);
				else
					await _loading_mono_bridge.LoadSceneByNameAsync(_startup_flow_config.MainMenuScene);
			}
		}
	}
}
