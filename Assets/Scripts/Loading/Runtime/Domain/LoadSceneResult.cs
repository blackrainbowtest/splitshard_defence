namespace SHD.Loading.Domain
{
	public class LoadSceneResult
	{
		private bool _succeeded;
		private bool _cancelled;
		private string _error_message;

		public bool Succeeded
		{
			get
			{
				return (_succeeded);
			}
		}

		public bool Cancelled
		{
			get
			{
				return (_cancelled);
			}
		}

		public string ErrorMessage
		{
			get
			{
				return (_error_message);
			}
		}

		private LoadSceneResult(bool succeeded, bool cancelled, string error_message)
		{
			_succeeded = succeeded;
			_cancelled = cancelled;
			_error_message = error_message;
		}

		public static LoadSceneResult Success()
		{
			return (new LoadSceneResult(true, false, string.Empty));
		}

		public static LoadSceneResult CancelledResult()
		{
			return (new LoadSceneResult(false, true, string.Empty));
		}

		public static LoadSceneResult Failure(string error_message)
		{
			return (new LoadSceneResult(false, false, error_message));
		}
	}
}
