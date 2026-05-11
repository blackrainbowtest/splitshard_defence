using System.Collections.Generic;

using SHD.Save.Data;

namespace SHD.Save.Managers
{
	public class SaveManager
	{
		private SaveData _autosave;
		private Dictionary<int, SaveData> _saves;

		public SaveData Autosave
		{
			get
			{
				return (_autosave);
			}
		}

		public SaveManager()
		{
			_saves = new Dictionary<int, SaveData>();
			_autosave = CreateSave(0);
		}

		public SaveData CreateSave(int slot_id)
		{
			SaveData save_data;

			save_data = new SaveData();
			save_data.SlotId = slot_id;

			return (save_data);
		}

		public void SetAutosave(SaveData save_data)
		{
			if (save_data == null)
				return ;

			save_data.SlotId = 0;
			_autosave = save_data;
		}

		public void SetSave(int slot_id, SaveData save_data)
		{
			if (save_data == null)
				return ;

			if (slot_id <= 0)
				return ;

			save_data.SlotId = slot_id;
			_saves[slot_id] = save_data;
		}

		public SaveData GetSave(int slot_id)
		{
			SaveData save_data;

			if (slot_id == 0)
				return (_autosave);

			if (_saves.TryGetValue(slot_id, out save_data) == false)
				return (null);

			return (save_data);
		}

		public bool HasSave(int slot_id)
		{
			if (slot_id == 0)
				return (_autosave != null);

			return (_saves.ContainsKey(slot_id));
		}

		public void DeleteSave(int slot_id)
		{
			if (slot_id == 0)
			{
				_autosave = null;
				return ;
			}

			if (_saves.ContainsKey(slot_id) == true)
				_saves.Remove(slot_id);
		}
	}
}