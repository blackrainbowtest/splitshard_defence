namespace SHD.Localization.Domain
{
	public interface ILocalizationStorage
	{
		bool TryLoadTable(string language, string category, out LocalizationTable table);
	}
}
