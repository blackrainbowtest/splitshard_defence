using System;
using SHD.Core.Constants;

namespace SHD.Save.Data
{
	public class SaveData
	{
		public const int CurrentSaveVersion = 1;

		private int _save_version;
		private string _game_version;

		private int _slot_id;

		private int _score;
		private int _survived_waves;
		private int _army_size;

		private DateTime _save_time;

		public int SaveVersion
		{
			get
			{
				return (_save_version);
			}

			set
			{
				_save_version = value;
			}
		}

		public string GameVersion
		{
			get
			{
				return (_game_version);
			}

			set
			{
				_game_version = value;
			}
		}

		public int SlotId
		{
			get
			{
				return (_slot_id);
			}

			set
			{
				_slot_id = value;
			}
		}

		public int Score
		{
			get
			{
				return (_score);
			}

			set
			{
				_score = value;
			}
		}

		public int SurvivedWaves
		{
			get
			{
				return (_survived_waves);
			}

			set
			{
				_survived_waves = value;
			}
		}

		public int ArmySize
		{
			get
			{
				return (_army_size);
			}

			set
			{
				_army_size = value;
			}
		}

		public DateTime SaveTime
		{
			get
			{
				return (_save_time);
			}

			set
			{
				_save_time = value;
			}
		}

		public SaveData()
		{
			_save_version = CurrentSaveVersion;
			_game_version = GameInfo.GameVersion;

			_slot_id = 0;

			_score = 0;
			_survived_waves = 0;
			_army_size = 0;

			_save_time = DateTime.Now;
		}
	}
}