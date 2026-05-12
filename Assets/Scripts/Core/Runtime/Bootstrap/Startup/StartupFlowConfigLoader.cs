using UnityEngine;

namespace SHD.Core.Bootstrap.Startup
{
	public static class StartupFlowConfigLoader
	{
		private const string ConfigPath = "StartupFlow/config";

		public static StartupFlowConfig Load()
		{
			TextAsset text_asset;
			StartupFlowConfig config;

			text_asset = Resources.Load<TextAsset>(ConfigPath);
			if (text_asset == null)
				return (null);

			config = JsonUtility.FromJson<StartupFlowConfig>(text_asset.text);
			return (config);
		}
	}
}
