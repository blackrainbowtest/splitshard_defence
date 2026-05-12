using System;

namespace SHD.Localization.Domain
{
	public interface ILocalizationService
	{
		string CurrentLanguage { get; }

		event Action<string> LanguageChanged;

		bool SetLanguage(string language);

		string GetText(string key);
	}
}
