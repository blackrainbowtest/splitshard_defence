using UnityEngine;
using UnityEngine.SceneManagement;

namespace SHD.UI.Mono
{
	public class MainMenuScenePresenterMono : MonoBehaviour
	{
		[Header("Scenes")]
		[SerializeField] private string _new_game_scene_name = "S_GameChoice";
		[SerializeField] private string _about_scene_name = "S_About";

		[Header("Overlays")]
		[SerializeField] private Transform _overlay_root;
		[SerializeField] private GameObject _saves_overlay_prefab;
		[SerializeField] private GameObject _settings_overlay_prefab;

		private GameObject _active_overlay_instance;

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
			LoadSceneByName(_about_scene_name);
		}

		public void OnExitPressed()
		{
			Application.Quit();
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
			Transform parent;

			if (prefab == null)
			{
				Debug.LogError("MainMenuScenePresenterMono: overlay prefab is not assigned.");
				return;
			}

			CloseActiveOverlay();

			parent = _overlay_root != null ? _overlay_root : transform;
			_active_overlay_instance = Instantiate(prefab, parent, false);
			StretchToParent(_active_overlay_instance.transform);
		}

		private void LoadSceneByName(string scene_name)
		{
			if (string.IsNullOrWhiteSpace(scene_name) == true)
			{
				Debug.LogError("MainMenuScenePresenterMono: target scene name is empty.");
				return;
			}

			if (Application.CanStreamedLevelBeLoaded(scene_name) == false)
			{
				Debug.LogError("MainMenuScenePresenterMono: scene is not in Build Settings: " + scene_name);
				return;
			}

			SceneManager.LoadScene(scene_name, LoadSceneMode.Single);
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
