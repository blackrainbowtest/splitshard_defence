using System;
using System.Collections.Generic;

using SHD.Localization.Domain;

namespace SHD.Localization.Services
{
	public class LocalizationService : ILocalizationService
	{
		private const string DefaultFallbackLanguage = "en";
		private static readonly string[] DefaultCategories = new string[] { "ui" };

		private ILocalizationStorage _storage;
		private Dictionary<string, string> _current_entries;
		private Dictionary<string, string> _fallback_entries;
		private string[] _categories;
		private string _fallback_language;
		private string _current_language;

		public string CurrentLanguage
		{
			get
			{
				return (_current_language);
			}
		}

		public event Action<string> LanguageChanged;

		public LocalizationService(
			ILocalizationStorage storage,
			LocalizationConfig config,
			string initial_language
		)
		{
			_storage = storage;
			_current_entries = new Dictionary<string, string>(StringComparer.Ordinal);
			_fallback_entries = new Dictionary<string, string>(StringComparer.Ordinal);
			_categories = BuildCategories(config);
			_fallback_language = BuildFallbackLanguage(config);
			_current_language = _fallback_language;

			BuildFallbackTable();
			SetLanguage(initial_language);
		}

		public bool SetLanguage(string language)
		{
			Dictionary<string, string> new_entries;
			string normalized_language;
			bool exact_match;

			normalized_language = NormalizeLanguage(language);
			exact_match = TryBuildTable(normalized_language, out new_entries);
			if (exact_match == false)
			{
				normalized_language = _fallback_language;
				new_entries = _fallback_entries;
			}

			_current_language = normalized_language;
			_current_entries = new_entries;
			LanguageChanged?.Invoke(_current_language);

			return (exact_match);
		}

		public string GetText(string key)
		{
			string value;

			if (string.IsNullOrWhiteSpace(key) == true)
				return (string.Empty);

			if (_current_entries.TryGetValue(key, out value) == true)
				return (value);

			if (_fallback_entries.TryGetValue(key, out value) == true)
				return (value);

			return ("#" + key);
		}

		private void BuildFallbackTable()
		{
			Dictionary<string, string> table;

			if (TryBuildTable(_fallback_language, out table) == true)
			{
				_fallback_entries = table;
				return ;
			}

			_fallback_entries = new Dictionary<string, string>(StringComparer.Ordinal);
		}

		private bool TryBuildTable(string language, out Dictionary<string, string> table)
		{
			int i;
			int j;
			LocalizationTable loaded_table;
			bool has_any_category;

			table = new Dictionary<string, string>(StringComparer.Ordinal);
			has_any_category = false;

			if (_storage == null)
				return (false);

			i = 0;
			while (i < _categories.Length)
			{
				if (_storage.TryLoadTable(language, _categories[i], out loaded_table) == true)
				{
					has_any_category = true;

					j = 0;
					while (j < loaded_table.Entries.Length)
					{
						if (string.IsNullOrWhiteSpace(loaded_table.Entries[j].Key) == false)
							table[loaded_table.Entries[j].Key] = loaded_table.Entries[j].Value ?? string.Empty;

						j++;
					}
				}

				i++;
			}

			return (has_any_category);
		}

		private string NormalizeLanguage(string language)
		{
			if (string.IsNullOrWhiteSpace(language) == true)
				return (_fallback_language);

			return (language.Trim().ToLowerInvariant());
		}

		private string BuildFallbackLanguage(LocalizationConfig config)
		{
			if (config == null)
				return (DefaultFallbackLanguage);

			if (string.IsNullOrWhiteSpace(config.FallbackLanguage) == true)
				return (DefaultFallbackLanguage);

			return (config.FallbackLanguage.Trim().ToLowerInvariant());
		}

		private string[] BuildCategories(LocalizationConfig config)
		{
			string[] categories;
			int valid_count;
			int i;
			int j;

			if (config == null || config.Categories == null || config.Categories.Length == 0)
				return (DefaultCategories);

			categories = new string[config.Categories.Length];
			valid_count = 0;
			i = 0;
			while (i < config.Categories.Length)
			{
				if (string.IsNullOrWhiteSpace(config.Categories[i]) == false)
				{
					categories[valid_count] = config.Categories[i].Trim().ToLowerInvariant();
					valid_count++;
				}

				i++;
			}

			if (valid_count == 0)
				return (DefaultCategories);

			if (valid_count == categories.Length)
				return (categories);

			config.Categories = new string[valid_count];
			j = 0;
			while (j < valid_count)
			{
				config.Categories[j] = categories[j];
				j++;
			}

			return (config.Categories);
		}
	}
}
