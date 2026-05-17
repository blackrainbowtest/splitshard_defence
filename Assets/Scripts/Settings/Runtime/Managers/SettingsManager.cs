using SHD.Settings.Data;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

namespace SHD.Settings.Managers
{
	public class SettingsManager
	{
		private SettingsData _settings_data;
		private readonly string _settings_path;

		public SettingsData SettingsData
		{
			get
			{
				return (_settings_data);
			}
		}

		public SettingsManager(string settings_path)
		{
			_settings_path = settings_path;
			_settings_data = LoadOrDefault(settings_path);
		}

		public void SetMusicVolume(float volume)
		{
			_settings_data.MusicVolume = volume;
			Save();
		}

		public void SetSfxVolume(float volume)
		{
			_settings_data.SoundVolume = volume;
			Save();
		}

		public void SetUiVolume(float volume)
		{
			_settings_data.UiVolume = volume;
			Save();
		}

		public void SetAmbientVolume(float volume)
		{
			_settings_data.AmbientVolume = volume;
			Save();
		}

		public void SetLanguage(string language)
		{
			_settings_data.Language = language;
			Save();
		}

		private SettingsData LoadOrDefault(string settings_path)
		{
			string json;
			SettingsData loaded;

			if (string.IsNullOrWhiteSpace(settings_path) == true)
				return (new SettingsData());

			try
			{
				if (File.Exists(settings_path) == false)
					return (new SettingsData());

				json = File.ReadAllText(settings_path);
				if (string.IsNullOrWhiteSpace(json) == true)
					return (new SettingsData());

				loaded = JsonConvert.DeserializeObject<SettingsData>(json);
				return (loaded ?? new SettingsData());
			}
			catch (System.Exception ex)
			{
				Debug.LogError("SettingsManager: failed to load settings: " + ex.Message);
				return (new SettingsData());
			}
		}

		private void Save()
		{
			string directory;
			string json;

			if (string.IsNullOrWhiteSpace(_settings_path) == true)
				return;

			try
			{
				directory = Path.GetDirectoryName(_settings_path);
				if (string.IsNullOrWhiteSpace(directory) == false && Directory.Exists(directory) == false)
					Directory.CreateDirectory(directory);

				json = JsonConvert.SerializeObject(_settings_data, Formatting.Indented);
				File.WriteAllText(_settings_path, json);
			}
			catch (System.Exception ex)
			{
				Debug.LogError("SettingsManager: failed to save settings: " + ex.Message);
			}
		}
	}
}
