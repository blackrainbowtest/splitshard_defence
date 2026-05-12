using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

using SHD.Loading.Domain;
using SHD.Loading.Orchestration;

namespace SHD.Loading.Mono
{
	public class LoadingMonoBridge : MonoBehaviour
	{
		private LoadingController _loading_controller;

		public LoadingController LoadingController
		{
			get
			{
				return (_loading_controller);
			}
		}

		public void Initialize(ILoadingService loading_service)
		{
			if (loading_service == null)
				return ;

			_loading_controller = new LoadingController(loading_service);
		}

		public async void LoadSceneByName(string scene_name)
		{
			_ = await LoadSceneByNameAsync(scene_name);
		}

		public async Task<LoadSceneResult> LoadSceneByNameAsync(string scene_name)
		{
			LoadSceneRequest request;
			LoadSceneResult result;

			if (_loading_controller == null)
				return (LoadSceneResult.Failure("Loading controller is not initialized."));

			request = new LoadSceneRequest(scene_name, LoadSceneMode.Single, true);
			result = await _loading_controller.BeginLoadSceneAsync(request);

			if (result.Succeeded == false && result.Cancelled == false)
				Debug.LogError("Scene loading failed: " + result.ErrorMessage);

			return (result);
		}

		public async void PreloadSceneByName(string scene_name)
		{
			_ = await PreloadSceneByNameAsync(scene_name);
		}

		public async Task<LoadSceneResult> PreloadSceneByNameAsync(string scene_name)
		{
			LoadSceneRequest request;
			LoadSceneResult result;

			if (_loading_controller == null)
				return (LoadSceneResult.Failure("Loading controller is not initialized."));

			request = new LoadSceneRequest(scene_name, LoadSceneMode.Single, false);
			result = await _loading_controller.BeginLoadSceneAsync(request);

			if (result.Succeeded == false && result.Cancelled == false)
				Debug.LogError("Scene preload failed: " + result.ErrorMessage);

			return (result);
		}

		public void ActivatePreloadedScene()
		{
			if (_loading_controller == null)
				return ;

			if (_loading_controller.ActivateLoadedScene() == false)
				Debug.LogWarning("No preloaded scene is waiting for activation.");
		}

		public void CancelLoading()
		{
			if (_loading_controller == null)
				return ;

			_loading_controller.CancelCurrentLoad();
		}
	}
}
