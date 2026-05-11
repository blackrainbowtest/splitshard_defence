using SHD.Core.Enums;
using SHD.UI.Managers;
using SHD.UI.Windows;

namespace SHD.UI.Controllers
{
	public class MainMenuController
	{
		private MenuWindowManager _window_manager;

		public MainMenuController()
		{
			_window_manager = new MenuWindowManager();

			RegisterWindows();
		}

		private void RegisterWindows()
		{
			_window_manager.RegisterWindow(new NewGameWindow());
			_window_manager.RegisterWindow(new SavesWindow());
			_window_manager.RegisterWindow(new SettingsWindow());
			_window_manager.RegisterWindow(new AboutWindow());
		}

		public void OpenNewGame()
		{
			_window_manager.OpenWindow(EMenuWindow.NewGame);
		}

		public void OpenSaves()
		{
			_window_manager.OpenWindow(EMenuWindow.Saves);
		}

		public void OpenSettings()
		{
			_window_manager.OpenWindow(EMenuWindow.Settings);
		}

		public void OpenAbout()
		{
			_window_manager.OpenWindow(EMenuWindow.About);
		}

		public void ExitGame()
		{
			// later:
			// Application.Quit();
		}
	}
}