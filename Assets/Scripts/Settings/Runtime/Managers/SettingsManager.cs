using SHD.Settings.Data;

namespace SHD.Settings.Managers
{
	public class SettingsManager
	{
		private SettingsData _settings_data;

		public SettingsData SettingsData
		{
			get
			{
				return (_settings_data);
			}
		}

		public SettingsManager()
		{
			_settings_data = new SettingsData();
		}

		public void SetMusicVolume(float volume)
		{
			_settings_data.MusicVolume = volume;
		}

		public void SetSoundVolume(float volume)
		{
			_settings_data.SoundVolume = volume;
		}

		public void SetAmbientVolume(float volume)
		{
			_settings_data.AmbientVolume = volume;
		}

		public void SetLanguage(string language)
		{
			_settings_data.Language = language;
		}
	}
}