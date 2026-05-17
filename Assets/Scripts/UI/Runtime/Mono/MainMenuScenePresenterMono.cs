using UnityEngine;
using SHD.Loading.Mono;
using SHD.Loading.Orchestration;

namespace SHD.UI.Mono
{
	public class MainMenuScenePresenterMono : MonoBehaviour
	{
		[Header("Scenes")]
		[SerializeField] private string _new_game_scene_name = "S_GameChoice";

		[Header("Overlays")]
		[SerializeField] private Transform _overlay_root;
		[SerializeField] private GameObject _saves_overlay_prefab;
		[SerializeField] private GameObject _settings_overlay_prefab;
		[SerializeField] private GameObject _about_overlay_prefab;

		private GameObject _active_overlay_instance;
		private LoadingMonoBridge _loading_bridge;
		private StartupFlowConfigDto _startup_flow_config;

		[System.Serializable]
		private class StartupFlowConfigDto
		{
			public string LoadingScene;
			public bool WaitForTapToContinueOnLoading;
		}

		private void Awake()
		{
			_loading_bridge = Object.FindAnyObjectByType<LoadingMonoBridge>();
			_startup_flow_config = LoadStartupFlowConfig();
		}

		public void OnNewGamePressed()
		{
			LoadSceneByName(_new_game_scene_name);
		}

		public void OnSavesPressed()
		{
			OpenOverlay(_saves_overlay_prefab);
		}

		public void OnSettingsPressed()
		{
			OpenOverlay(_settings_overlay_prefab);
		}

		public void OnAboutPressed()
		{
			OpenOverlay(_about_overlay_prefab);
		}

		public void OnExitPressed()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}

		public void CloseActiveOverlay()
		{
			if (_active_overlay_instance == null)
				return;

			Destroy(_active_overlay_instance);
			_active_overlay_instance = null;
		}

		private void OpenOverlay(GameObject prefab)
		{
			Object instantiated_object;
			GameObject instantiated_game_object;
			Transform parent;

			if (prefab == null)
			{
				Debug.LogError("MainMenuScenePresenterMono: overlay prefab is not assigned.");
				return;
			}

			CloseActiveOverlay();

			parent = _overlay_root != null ? _overlay_root : transform;
			instantiated_object = Instantiate((Object)prefab, parent, false);
			instantiated_game_object = ResolveGameObject(instantiated_object);
			if (instantiated_game_object == null)
			{
				Debug.LogWarning("MainMenuScenePresenterMono: prefab instantiate cast failed, using fallback overlay object.");
				instantiated_game_object = new GameObject("OverlayFallback", typeof(RectTransform));
				instantiated_game_object.transform.SetParent(parent, false);
				if (prefab != null && prefab.name.Contains("Saves") == true)
					instantiated_game_object.AddComponent<SavesOverlayMono>();
			}

			_active_overlay_instance = instantiated_game_object;
			StretchToParent(_active_overlay_instance.transform);
			if (_active_overlay_instance.transform is RectTransform == false)
			{
				Transform ui_root;

				ui_root = _active_overlay_instance.transform.Find("UIRoot");
				if (ui_root != null)
					StretchToParent(ui_root);
			}
		}

		private GameObject ResolveGameObject(Object instance)
		{
			GameObject game_object;
			Component component;

			game_object = instance as GameObject;
			if (game_object != null)
				return (game_object);

			component = instance as Component;
			if (component != null)
				return (component.gameObject);

			return (null);
		}

		private void LoadSceneByName(string scene_name)
		{
			string loading_scene_name;
			bool wait_for_tap;

			if (_loading_bridge == null)
				_loading_bridge = Object.FindAnyObjectByType<LoadingMonoBridge>();
			if (_startup_flow_config == null)
				_startup_flow_config = LoadStartupFlowConfig();

			if (string.IsNullOrWhiteSpace(scene_name) == true)
			{
				Debug.LogError("MainMenuScenePresenterMono: target scene name is empty.");
				return;
			}

			if (_loading_bridge == null)
			{
				Debug.LogError("MainMenuScenePresenterMono: LoadingMonoBridge was not found.");
				return;
			}

			loading_scene_name = _startup_flow_config != null ? _startup_flow_config.LoadingScene : string.Empty;
			wait_for_tap = _startup_flow_config != null && _startup_flow_config.WaitForTapToContinueOnLoading;

			if (string.IsNullOrWhiteSpace(loading_scene_name) == true)
			{
				_loading_bridge.LoadSceneByName(scene_name);
				return;
			}

			StartupFlowRuntimeState.SetPendingTarget(scene_name, wait_for_tap);
			_loading_bridge.LoadSceneByName(loading_scene_name);
		}

		private StartupFlowConfigDto LoadStartupFlowConfig()
		{
			const string config_path = "StartupFlow/config";
			TextAsset text_asset;

			text_asset = Resources.Load<TextAsset>(config_path);
			if (text_asset == null || string.IsNullOrWhiteSpace(text_asset.text) == true)
				return (null);

			return (JsonUtility.FromJson<StartupFlowConfigDto>(text_asset.text));
		}

		private void StretchToParent(Transform instance_transform)
		{
			RectTransform rect_transform;

			rect_transform = instance_transform as RectTransform;
			if (rect_transform == null)
				return;

			rect_transform.anchorMin = Vector2.zero;
			rect_transform.anchorMax = Vector2.one;
			rect_transform.offsetMin = Vector2.zero;
			rect_transform.offsetMax = Vector2.zero;
			rect_transform.localScale = Vector3.one;
			rect_transform.localPosition = Vector3.zero;
		}
	}
}
