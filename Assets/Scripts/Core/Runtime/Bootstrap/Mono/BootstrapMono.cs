using UnityEngine;

using SHD.Core.Bootstrap;

namespace SHD.Core.Bootstrap.Mono
{
	public class BootstrapMono : MonoBehaviour
	{
		private GameBootstrap _game_bootstrap;

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);

			InitializeBootstrap();
		}

		private void InitializeBootstrap()
		{
			string settings_path;

			settings_path = Application.persistentDataPath
				+ "/settings.json";

			_game_bootstrap = new GameBootstrap(settings_path);
		}
	}
}