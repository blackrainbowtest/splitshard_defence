/**
 * The manager will:
 *
 * store all windows;
 * open the one you need;
 * close the current one;
 * prevent two windows from being open at the same time.
 */

using System.Collections.Generic;

using SHD.Core.Enums;
using SHD.UI.Interfaces;

namespace SHD.UI.Managers
{
	public class MenuWindowManager
	{
		private Dictionary<EMenuWindow, IMenuWindow> _windows;
		private IMenuWindow _current_window;

		public MenuWindowManager()
		{
			_windows = new Dictionary<EMenuWindow, IMenuWindow>();
		}

		public void RegisterWindow(IMenuWindow window)
		{
			if (window == null)
				return ;

			if (_windows.ContainsKey(window.WindowType))
				return ;

			_windows.Add(window.WindowType, window);
		}

		public void OpenWindow(EMenuWindow window_type)
		{
			IMenuWindow window;

			if (_current_window != null)
				_current_window.Close();

			if (_windows.TryGetValue(window_type, out window) == false)
				return ;

			window.Open();
			_current_window = window;
		}

		public void CloseCurrentWindow()
		{
			if (_current_window == null)
				return ;

			_current_window.Close();
			_current_window = null;
		}
	}
}