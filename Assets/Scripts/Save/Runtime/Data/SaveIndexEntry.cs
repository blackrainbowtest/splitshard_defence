using System;

namespace SHD.Save.Data
{
	public class SaveIndexEntry
	{
		private int _slot_id;
		private bool _is_autosave;
		private bool _has_save;
		private DateTime _save_time;
		private int _score;
		private int _save_version;
		private DateTime _last_write_utc;
		private bool _is_corrupted;

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

		public bool IsAutosave
		{
			get
			{
				return (_is_autosave);
			}
			set
			{
				_is_autosave = value;
			}
		}

		public bool HasSave
		{
			get
			{
				return (_has_save);
			}
			set
			{
				_has_save = value;
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

		public DateTime LastWriteUtc
		{
			get
			{
				return (_last_write_utc);
			}
			set
			{
				_last_write_utc = value;
			}
		}

		public bool IsCorrupted
		{
			get
			{
				return (_is_corrupted);
			}
			set
			{
				_is_corrupted = value;
			}
		}
	}
}
