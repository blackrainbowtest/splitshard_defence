using UnityEngine;

using SHD.Localization.Domain;

namespace SHD.Localization.Storage
{
	public static class LocalizationConfigLoader
	{
		private const string ConfigPath = "Localization/config";

		public static LocalizationConfig Load()
		{
			TextAsset text_asset;
			LocalizationConfig config;

			text_asset = Resources.Load<TextAsset>(ConfigPath);
			if (text_asset == null)
				return (null);

			config = JsonUtility.FromJson<LocalizationConfig>(text_asset.text);
			return (config);
		}
	}
}
