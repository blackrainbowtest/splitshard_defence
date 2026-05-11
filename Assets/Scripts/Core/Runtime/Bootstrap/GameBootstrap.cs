using System;

namespace SHD.Core.Bootstrap
{
	public class GameBootstrap
	{
		private readonly string _settings_path;

		public string SettingsPath
		{
			get
			{
				return (_settings_path);
			}
		}

		public GameBootstrap(string settings_path)
		{
			if (string.IsNullOrWhiteSpace(settings_path) == true)
				throw new ArgumentException("Settings path cannot be null or empty.", nameof(settings_path));

			_settings_path = settings_path;
		}
	}
}
