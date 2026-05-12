using Newtonsoft.Json.Linq;

using SHD.Save.Data;

namespace SHD.Save.Migration
{
	public class SaveMigrationService
	{
		public bool NeedsMigration(int save_version)
		{
			return (save_version < SaveData.CurrentSaveVersion);
		}

		public string Migrate(string json, int save_version)
		{
			if (NeedsMigration(save_version) == false)
				return (json);

			try
			{
				if (save_version == 1)
					return (MigrateFromVersion1(json));
			}
			catch
			{
				return (null);
			}

			return (null);
		}

		private string MigrateFromVersion1(string json)
		{
			JObject parsed_json;

			parsed_json = JObject.Parse(json);

			parsed_json["SaveVersion"] = SaveData.CurrentSaveVersion;

			return (parsed_json.ToString());
		}
	}
}
