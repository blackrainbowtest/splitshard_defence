using SHD.Save.Data;
using SHD.Save.FileSystem;

namespace SHD.Save.Managers
{
	public class SaveManager
	{
		private const int ManualSaveSlotsCount = 10;

		private SaveSlot _autosave;

		private SaveSlot[] _manual_slots;

		private SaveFileHandler _file_handler;

		public SaveSlot Autosave
		{
			get
			{
				return (_autosave);
			}
		}

		public SaveManager(SaveFileHandler file_handler)
		{
			int i;

			_file_handler = file_handler;

			_autosave = new SaveSlot(0, true);

			_manual_slots = new SaveSlot[ManualSaveSlotsCount];

			i = 0;
			while (i < ManualSaveSlotsCount)
			{
				_manual_slots[i] = new SaveSlot(i + 1, false);
				i++;
			}

			LoadAllSaves();
		}

		public SaveSlot GetSlot(int slot_id)
		{
			if (slot_id == 0)
				return (_autosave);

			if (slot_id < 1 || slot_id > ManualSaveSlotsCount)
				return (null);

			return (_manual_slots[slot_id - 1]);
		}

		public void SaveAutosave(SaveData save_data)
		{
			string file_name;

			if (save_data == null)
				return ;

			_autosave.SetSaveData(save_data);

			file_name = _file_handler.GetAutosaveFileName();

			_file_handler.SaveToFile(file_name, save_data);
			PersistSaveIndex();
		}

		public void SaveManualSlot(int slot_id, SaveData save_data)
		{
			SaveSlot slot;
			string file_name;

			slot = GetSlot(slot_id);

			if (slot == null)
				return ;

			if (save_data == null)
				return ;

			slot.SetSaveData(save_data);

			file_name = _file_handler.GetSaveFileName(slot_id);

			_file_handler.SaveToFile(file_name, save_data);
			PersistSaveIndex();
		}

		public void DeleteSave(int slot_id)
		{
			SaveSlot slot;
			string file_name;

			slot = GetSlot(slot_id);

			if (slot == null)
				return ;

			slot.Clear();

			if (slot_id == 0)
				file_name = _file_handler.GetAutosaveFileName();
			else
				file_name = _file_handler.GetSaveFileName(slot_id);

			_file_handler.DeleteFile(file_name);
			PersistSaveIndex();
		}

		public void LoadAllSaves()
		{
			LoadAutosave();
			LoadManualSaves();
			PersistSaveIndex();
		}

		public SaveIndex GetSaveIndex()
		{
			SaveIndex index;

			index = _file_handler.LoadIndex();
			if (index != null)
				return (index);

			index = BuildSaveIndex();
			_file_handler.SaveIndex(index);
			return (index);
		}

		private void LoadAutosave()
		{
			SaveData save_data;
			string file_name;

			file_name = _file_handler.GetAutosaveFileName();

			save_data = _file_handler.LoadFromFile(file_name);

			if (save_data == null)
				return ;

			_autosave.SetSaveData(save_data);
		}

		private void LoadManualSaves()
		{
			int i;
			SaveData save_data;
			string file_name;

			i = 1;
			while (i <= ManualSaveSlotsCount)
			{
				file_name = _file_handler.GetSaveFileName(i);

				save_data = _file_handler.LoadFromFile(file_name);

				if (save_data != null)
					_manual_slots[i - 1].SetSaveData(save_data);

				i++;
			}
		}

		private void PersistSaveIndex()
		{
			_file_handler.SaveIndex(BuildSaveIndex());
		}

		private SaveIndex BuildSaveIndex()
		{
			SaveIndex index;
			SaveIndexEntry[] entries;
			int i;

			index = new SaveIndex();
			entries = new SaveIndexEntry[ManualSaveSlotsCount + 1];

			entries[0] = BuildEntry(_autosave);

			i = 0;
			while (i < ManualSaveSlotsCount)
			{
				entries[i + 1] = BuildEntry(_manual_slots[i]);
				i++;
			}

			index.Entries = entries;
			return (index);
		}

		private SaveIndexEntry BuildEntry(SaveSlot slot)
		{
			SaveData save_data;
			SaveIndexEntry entry;

			save_data = slot.SaveData;
			entry = new SaveIndexEntry();

			entry.SlotId = slot.SlotId;
			entry.IsAutosave = slot.IsAutosave;
			entry.HasSave = slot.HasSave;
			entry.LastWriteUtc = System.DateTime.UtcNow;
			entry.IsCorrupted = false;

			if (save_data != null)
			{
				entry.SaveTime = save_data.SaveTime;
				entry.Score = save_data.Score;
				entry.SaveVersion = save_data.SaveVersion;
			}
			else
			{
				entry.SaveTime = System.DateTime.MinValue;
				entry.Score = 0;
				entry.SaveVersion = 0;
			}

			return (entry);
		}
	}
}
