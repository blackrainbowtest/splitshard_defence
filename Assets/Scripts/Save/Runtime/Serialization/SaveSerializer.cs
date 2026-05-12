using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using SHD.Save.Data;

namespace SHD.Save.Serialization
{
	public class SaveSerializer
	{
		public string Serialize(SaveData save_data)
		{
			if (save_data == null)
				return (string.Empty);

			return (
				JsonConvert.SerializeObject(
					save_data,
					Formatting.Indented
				)
			);
		}

		public SaveData Deserialize(string json)
		{
			if (string.IsNullOrEmpty(json) == true)
				return (null);

			try
			{
				return (
					JsonConvert.DeserializeObject<SaveData>(json)
				);
			}
			catch
			{
				return (null);
			}
		}

		public bool CanDeserialize(string json)
		{
			JObject parsed_json;

			if (string.IsNullOrEmpty(json) == true)
				return (false);

			try
			{
				parsed_json = JObject.Parse(json);

				if (parsed_json["SaveVersion"] == null)
					return (false);

				if (parsed_json["SaveVersion"].Type != JTokenType.Integer)
					return (false);

				return (true);
			}
			catch
			{
				return (false);
			}
		}

		public int GetSaveVersion(string json)
		{
			JObject parsed_json;
			JToken save_version;

			if (string.IsNullOrEmpty(json) == true)
				return (-1);

			try
			{
				parsed_json = JObject.Parse(json);

				save_version = parsed_json["SaveVersion"];

				if (save_version == null)
					return (-1);

				return (save_version.Value<int>());
			}
			catch
			{
				return (-1);
			}
		}

		public bool IsVersionReadable(int save_version)
		{
			if (save_version < 1)
				return (false);

			return (true);
		}

		public string SerializeIndex(SaveIndex save_index)
		{
			if (save_index == null)
				return (string.Empty);

			return (
				JsonConvert.SerializeObject(
					save_index,
					Formatting.Indented
				)
			);
		}

		public SaveIndex DeserializeIndex(string json)
		{
			if (string.IsNullOrEmpty(json) == true)
				return (null);

			try
			{
				return (
					JsonConvert.DeserializeObject<SaveIndex>(json)
				);
			}
			catch
			{
				return (null);
			}
		}

		public bool CanDeserializeIndex(string json)
		{
			JObject parsed_json;

			if (string.IsNullOrEmpty(json) == true)
				return (false);

			try
			{
				parsed_json = JObject.Parse(json);

				if (parsed_json["IndexVersion"] == null)
					return (false);

				if (parsed_json["IndexVersion"].Type != JTokenType.Integer)
					return (false);

				if (parsed_json["Entries"] == null)
					return (false);

				if (parsed_json["Entries"].Type != JTokenType.Array)
					return (false);

				return (true);
			}
			catch
			{
				return (false);
			}
		}
	}
}
