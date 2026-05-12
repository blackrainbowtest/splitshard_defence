using UnityEngine;

using SHD.Localization.Domain;

namespace SHD.Localization.Storage
{
	public class ResourcesLocalizationStorage : ILocalizationStorage
	{
		private const string TablesRoot = "Localization/";

		public bool TryLoadTable(string language, string category, out LocalizationTable table)
		{
			TextAsset text_asset;
			string normalized_language;
			string normalized_category;

			table = null;

			if (string.IsNullOrWhiteSpace(language) == true)
				return (false);

			if (string.IsNullOrWhiteSpace(category) == true)
				return (false);

			normalized_language = language.ToLowerInvariant();
			normalized_category = category.ToLowerInvariant();

			text_asset = Resources.Load<TextAsset>(TablesRoot + normalized_category + "/" + normalized_language);
			if (text_asset == null)
				return (false);

			table = JsonUtility.FromJson<LocalizationTable>(text_asset.text);
			if (table == null || table.Entries == null)
				return (false);

			return (true);
		}
	}
}
