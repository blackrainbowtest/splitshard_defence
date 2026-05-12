namespace SHD.Loading.Domain
{
	public class LoadSceneProgress
	{
		private float _raw_progress;
		private float _normalized_progress;
		private bool _is_ready_to_activate;
		private bool _is_done;

		public float RawProgress
		{
			get
			{
				return (_raw_progress);
			}
		}

		public float NormalizedProgress
		{
			get
			{
				return (_normalized_progress);
			}
		}

		public bool IsReadyToActivate
		{
			get
			{
				return (_is_ready_to_activate);
			}
		}

		public bool IsDone
		{
			get
			{
				return (_is_done);
			}
		}

		public LoadSceneProgress(
			float raw_progress,
			float normalized_progress,
			bool is_ready_to_activate,
			bool is_done
		)
		{
			_raw_progress = raw_progress;
			_normalized_progress = normalized_progress;
			_is_ready_to_activate = is_ready_to_activate;
			_is_done = is_done;
		}
	}
}
