namespace SHD.Settings.Data
{
	public class SettingsData
	{
		private float _music_volume;
		private float _sound_volume;
		private float _ambient_volume;

		private string _language;

		public float MusicVolume
		{
			get
			{
				return (_music_volume);
			}

			set
			{
				_music_volume = value;
			}
		}

		public float SoundVolume
		{
			get
			{
				return (_sound_volume);
			}

			set
			{
				_sound_volume = value;
			}
		}

		public float AmbientVolume
		{
			get
			{
				return (_ambient_volume);
			}

			set
			{
				_ambient_volume = value;
			}
		}

		public string Language
		{
			get
			{
				return (_language);
			}

			set
			{
				_language = value;
			}
		}

		public SettingsData()
		{
			_music_volume = 1.0f;
			_sound_volume = 1.0f;
			_ambient_volume = 1.0f;

			_language = "en";
		}
	}
}