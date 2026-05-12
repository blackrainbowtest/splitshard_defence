namespace SHD.Save.Data
{
	public class SaveSlot
	{
		private int _slot_id;

		private bool _is_autosave;

		private SaveData _save_data;

		public int SlotId
		{
			get
			{
				return (_slot_id);
			}
		}

		public bool IsAutosave
		{
			get
			{
				return (_is_autosave);
			}
		}

		public SaveData SaveData
		{
			get
			{
				return (_save_data);
			}
		}

		public bool HasSave
		{
			get
			{
				return (_save_data != null);
			}
		}

		public SaveSlot(int slot_id, bool is_autosave)
		{
			_slot_id = slot_id;
			_is_autosave = is_autosave;

			_save_data = null;
		}

		public void SetSaveData(SaveData save_data)
		{
			_save_data = save_data;
		}

		public void Clear()
		{
			_save_data = null;
		}
	}
}