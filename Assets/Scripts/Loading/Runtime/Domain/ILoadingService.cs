using System;
using System.Threading;
using System.Threading.Tasks;

namespace SHD.Loading.Domain
{
	public interface ILoadingService
	{
		bool IsLoading { get; }

		bool HasPendingActivation { get; }

		event Action<LoadSceneProgress> ProgressChanged;

		Task<LoadSceneResult> LoadSceneAsync(
			LoadSceneRequest request,
			CancellationToken cancellation_token
		);

		bool ActivateLoadedScene();
	}
}
