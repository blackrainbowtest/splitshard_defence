using UnityEngine.SceneManagement;

namespace SHD.Loading.Domain
{
	public class LoadSceneRequest
	{
		private string _scene_name;
		private LoadSceneMode _load_mode;
		private bool _activate_on_ready;

		public string SceneName
		{
			get
			{
				return (_scene_name);
			}
		}

		public LoadSceneMode LoadMode
		{
			get
			{
				return (_load_mode);
			}
		}

		public bool ActivateOnReady
		{
			get
			{
				return (_activate_on_ready);
			}
		}

		public LoadSceneRequest(
			string scene_name,
			LoadSceneMode load_mode = LoadSceneMode.Single,
			bool activate_on_ready = true
		)
		{
			_scene_name = scene_name;
			_load_mode = load_mode;
			_activate_on_ready = activate_on_ready;
		}
	}
}
