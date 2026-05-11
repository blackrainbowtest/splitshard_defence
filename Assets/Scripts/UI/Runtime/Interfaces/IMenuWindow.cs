/**
 * Common rules for
 * NewGameWindow
 * SavesWindow
 * SettingsWindow
 * AboutWindow
 *
 * - say which window it is
 * - open
 * - close
 * - say whether they are open or not
 */

using SHD.Core.Enums;

namespace SHD.UI.Interfaces
{
	public interface IMenuWindow
	{
		EMenuWindow WindowType { get; }
		bool IsOpened { get; }

		void Open();
		void Close();
	}
}