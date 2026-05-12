using System;
using System.Threading;
using System.Threading.Tasks;

using SHD.Loading.Domain;

namespace SHD.Loading.Orchestration
{
	public class LoadingController
	{
		private ILoadingService _loading_service;
		private CancellationTokenSource _cancellation_token_source;
		private LoadSceneProgress _last_progress;

		public LoadSceneProgress LastProgress
		{
			get
			{
				return (_last_progress);
			}
		}

		public bool IsLoading
		{
			get
			{
				if (_loading_service == null)
					return (false);

				return (_loading_service.IsLoading);
			}
		}

		public bool HasPendingActivation
		{
			get
			{
				if (_loading_service == null)
					return (false);

				return (_loading_service.HasPendingActivation);
			}
		}

		public event Action<LoadSceneProgress> ProgressChanged;

		public LoadingController(ILoadingService loading_service)
		{
			_loading_service = loading_service;
			_loading_service.ProgressChanged += HandleProgressChanged;
		}

		public async Task<LoadSceneResult> BeginLoadSceneAsync(LoadSceneRequest request)
		{
			CancelCurrentLoad();
			_cancellation_token_source = new CancellationTokenSource();

			return (
				await _loading_service.LoadSceneAsync(
					request,
					_cancellation_token_source.Token
				)
			);
		}

		public void CancelCurrentLoad()
		{
			if (_cancellation_token_source == null)
				return ;

			if (_cancellation_token_source.IsCancellationRequested == false)
				_cancellation_token_source.Cancel();

			_cancellation_token_source.Dispose();
			_cancellation_token_source = null;
		}

		public bool ActivateLoadedScene()
		{
			return (_loading_service.ActivateLoadedScene());
		}

		private void HandleProgressChanged(LoadSceneProgress progress)
		{
			Action<LoadSceneProgress> progress_changed;

			_last_progress = progress;
			progress_changed = ProgressChanged;

			if (progress_changed == null)
				return ;

			progress_changed.Invoke(progress);
		}
	}
}
