using SHD.Core.Enums;
using SHD.UI.Interfaces;

namespace SHD.UI.Windows
{
	public class AboutWindow : IMenuWindow
	{
		private bool _is_opened;

		public EMenuWindow WindowType
		{
			get
			{
				return (EMenuWindow.About);
			}
		}

		public bool IsOpened
		{
			get
			{
				return (_is_opened);
			}
		}

		public void Open()
		{
			_is_opened = true;
		}

		public void Close()
		{
			_is_opened = false;
		}
	}
}