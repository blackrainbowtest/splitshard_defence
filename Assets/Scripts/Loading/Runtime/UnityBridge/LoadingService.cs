using System;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;

using SHD.Loading.Domain;

namespace SHD.Loading.UnityBridge
{
	public class LoadingService : ILoadingService
	{
		private const float ReadyProgress = 0.9f;

		private bool _is_loading;
		private AsyncOperation _pending_operation;

		public bool IsLoading
		{
			get
			{
				return (_is_loading);
			}
		}

		public bool HasPendingActivation
		{
			get
			{
				return (_pending_operation != null);
			}
		}

		public event Action<LoadSceneProgress> ProgressChanged;

		public async Task<LoadSceneResult> LoadSceneAsync(
			LoadSceneRequest request,
			CancellationToken cancellation_token
		)
		{
			AsyncOperation operation;
			float normalized_progress;
			bool ready_to_activate;

			if (request == null)
				return (LoadSceneResult.Failure("Load request is null."));

			if (string.IsNullOrWhiteSpace(request.SceneName) == true)
				return (LoadSceneResult.Failure("Scene name is empty."));

			if (_is_loading == true)
				return (LoadSceneResult.Failure("Another scene loading operation is already running."));

			if (cancellation_token.IsCancellationRequested == true)
				return (LoadSceneResult.CancelledResult());

			operation = SceneManager.LoadSceneAsync(request.SceneName, request.LoadMode);
			if (operation == null)
				return (LoadSceneResult.Failure("SceneManager.LoadSceneAsync returned null operation."));

			_is_loading = true;
			_pending_operation = null;

			operation.allowSceneActivation = request.ActivateOnReady;

			try
			{
				while (operation.isDone == false)
				{
					if (cancellation_token.IsCancellationRequested == true)
					{
						if (request.ActivateOnReady == true)
							return (LoadSceneResult.CancelledResult());

						return (
							LoadSceneResult.Failure(
								"Preload cancellation is not supported after operation start."
							)
						);
					}

					ready_to_activate = operation.progress >= ReadyProgress;
					normalized_progress = CalculateNormalizedProgress(operation.progress, ready_to_activate);

					EmitProgress(normalized_progress, operation.progress, ready_to_activate, false);

					if (ready_to_activate == true)
					{
						if (request.ActivateOnReady == true)
						{
							operation.allowSceneActivation = true;
						}
						else
						{
							operation.allowSceneActivation = false;
							_pending_operation = operation;
							return (LoadSceneResult.Success());
						}
					}

					await Task.Yield();
				}

				EmitProgress(1f, 1f, true, true);
				return (LoadSceneResult.Success());
			}
			catch (Exception exception)
			{
				return (LoadSceneResult.Failure(exception.Message));
			}
			finally
			{
				if (_pending_operation == null || _pending_operation != operation)
					_is_loading = false;
			}
		}

		public bool ActivateLoadedScene()
		{
			if (_pending_operation == null)
				return (false);

			_pending_operation.allowSceneActivation = true;
			_pending_operation = null;
			_is_loading = false;

			return (true);
		}

		private float CalculateNormalizedProgress(float raw_progress, bool ready_to_activate)
		{
			if (ready_to_activate == true)
				return (1f);

			if (raw_progress <= 0f)
				return (0f);

			return (Mathf.Clamp01(raw_progress / ReadyProgress));
		}

		private void EmitProgress(
			float normalized_progress,
			float raw_progress,
			bool is_ready_to_activate,
			bool is_done
		)
		{
			Action<LoadSceneProgress> progress_changed;

			progress_changed = ProgressChanged;
			if (progress_changed == null)
				return ;

			progress_changed.Invoke(
				new LoadSceneProgress(
					raw_progress,
					normalized_progress,
					is_ready_to_activate,
					is_done
				)
			);
		}
	}
}
