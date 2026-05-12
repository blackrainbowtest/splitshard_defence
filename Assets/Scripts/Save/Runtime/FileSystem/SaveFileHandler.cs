using System.IO;

using SHD.Save.Data;
using SHD.Save.Migration;
using SHD.Save.Serialization;

namespace SHD.Save.FileSystem
{
	public class SaveFileHandler
	{
		private const string SaveIndexFileName = "save_index.json";

		private string _save_directory;

		private SaveSerializer _serializer;
		private SaveMigrationService _migration_service;

		public SaveFileHandler(
			string save_directory,
			SaveSerializer serializer,
			SaveMigrationService migration_service
		)
		{
			_save_directory = save_directory;

			_serializer = serializer;
			_migration_service = migration_service;

			CreateSaveDirectory();
		}

		private void CreateSaveDirectory()
		{
			if (Directory.Exists(_save_directory) == true)
				return ;

			Directory.CreateDirectory(_save_directory);
		}

		public string GetAutosaveFileName()
		{
			return ("autosave.json");
		}

		public string GetSaveFileName(int slot_id)
		{
			return ("save_" + slot_id + ".json");
		}

		public string GetSaveIndexFileName()
		{
			return (SaveIndexFileName);
		}

		private string GetFilePath(string file_name)
		{
			return (Path.Combine(_save_directory, file_name));
		}

		public bool SaveExists(string file_name)
		{
			return (File.Exists(GetFilePath(file_name)));
		}

		public void SaveToFile(string file_name, SaveData save_data)
		{
			string json;

			if (save_data == null)
				return ;

			json = _serializer.Serialize(save_data);
			if (_serializer.CanDeserialize(json) == false)
				return ;

			WriteJsonAtomically(file_name, json);
		}

		public void SaveIndex(SaveIndex save_index)
		{
			string json;

			if (save_index == null)
				return ;

			json = _serializer.SerializeIndex(save_index);
			if (_serializer.CanDeserializeIndex(json) == false)
				return ;

			WriteJsonAtomically(GetSaveIndexFileName(), json);
		}

		public SaveData LoadFromFile(string file_name)
		{
			string file_path;
			string backup_file_path;
			string json;
			int save_version;
			string migrated_json;
			SaveData loaded_data;

			file_path = GetFilePath(file_name);
			backup_file_path = file_path + ".bak";

			if (TryReadSaveJson(file_path, out json) == false)
			{
				if (TryReadSaveJson(backup_file_path, out json) == false)
					return (null);
			}

			save_version = _serializer.GetSaveVersion(json);
			if (_serializer.IsVersionReadable(save_version) == false)
				return (null);

			if (save_version > SaveData.CurrentSaveVersion)
				return (null);

			if (_migration_service.NeedsMigration(save_version) == true)
			{
				migrated_json = _migration_service.Migrate(json, save_version);

				if (string.IsNullOrEmpty(migrated_json) == true)
					return (null);

				if (_serializer.CanDeserialize(migrated_json) == false)
					return (null);

				save_version = _serializer.GetSaveVersion(migrated_json);
				if (save_version != SaveData.CurrentSaveVersion)
					return (null);

				loaded_data = _serializer.Deserialize(migrated_json);
				if (loaded_data == null)
					return (null);

				SaveToFile(file_name, loaded_data);
				return (loaded_data);
			}

			if (save_version != SaveData.CurrentSaveVersion)
				return (null);

			loaded_data = _serializer.Deserialize(json);
			return (loaded_data);
		}

		public SaveIndex LoadIndex()
		{
			string file_path;
			string backup_file_path;
			string json;
			SaveIndex loaded_index;

			file_path = GetFilePath(GetSaveIndexFileName());
			backup_file_path = file_path + ".bak";

			if (TryReadIndexJson(file_path, out json) == false)
			{
				if (TryReadIndexJson(backup_file_path, out json) == false)
					return (null);
			}

			loaded_index = _serializer.DeserializeIndex(json);
			if (loaded_index == null)
				return (null);

			if (loaded_index.IndexVersion != SHD.Save.Data.SaveIndex.CurrentIndexVersion)
				return (null);

			if (loaded_index.Entries == null)
				loaded_index.Entries = new SaveIndexEntry[0];

			return (loaded_index);
		}

		private void WriteJsonAtomically(string file_name, string json)
		{
			string file_path;
			string temp_file_path;
			string backup_file_path;

			file_path = GetFilePath(file_name);
			temp_file_path = file_path + ".tmp";
			backup_file_path = file_path + ".bak";

			try
			{
				File.WriteAllText(temp_file_path, json);

				if (File.Exists(file_path) == false)
				{
					File.Move(temp_file_path, file_path);
					return ;
				}

				File.Replace(temp_file_path, file_path, backup_file_path, true);
			}
			catch
			{
				if (File.Exists(temp_file_path) == true)
					File.Delete(temp_file_path);
			}
		}

		private bool TryReadSaveJson(string file_path, out string json)
		{
			json = string.Empty;

			if (File.Exists(file_path) == false)
				return (false);

			try
			{
				json = File.ReadAllText(file_path);
			}
			catch
			{
				return (false);
			}

			return (_serializer.CanDeserialize(json));
		}

		private bool TryReadIndexJson(string file_path, out string json)
		{
			json = string.Empty;

			if (File.Exists(file_path) == false)
				return (false);

			try
			{
				json = File.ReadAllText(file_path);
			}
			catch
			{
				return (false);
			}

			return (_serializer.CanDeserializeIndex(json));
		}

		public void DeleteFile(string file_name)
		{
			string file_path;
			string backup_file_path;
			string temp_file_path;

			file_path = GetFilePath(file_name);
			backup_file_path = file_path + ".bak";
			temp_file_path = file_path + ".tmp";

			try
			{
				if (File.Exists(file_path) == true)
					File.Delete(file_path);

				if (File.Exists(backup_file_path) == true)
					File.Delete(backup_file_path);

				if (File.Exists(temp_file_path) == true)
					File.Delete(temp_file_path);
			}
			catch
			{
				return ;
			}
		}
	}
}
