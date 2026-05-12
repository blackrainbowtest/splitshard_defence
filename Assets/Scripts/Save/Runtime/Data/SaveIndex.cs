namespace SHD.Save.Data
{
	public class SaveIndex
	{
		public const int CurrentIndexVersion = 1;

		private int _index_version;
		private SaveIndexEntry[] _entries;

		public int IndexVersion
		{
			get
			{
				return (_index_version);
			}
			set
			{
				_index_version = value;
			}
		}

		public SaveIndexEntry[] Entries
		{
			get
			{
				return (_entries);
			}
			set
			{
				_entries = value;
			}
		}

		public SaveIndex()
		{
			_index_version = CurrentIndexVersion;
			_entries = new SaveIndexEntry[0];
		}
	}
}
