namespace SHD.Loading.Orchestration
{
	public static class StartupFlowRuntimeState
	{
		public static string PendingTargetScene;
		public static bool WaitForTapToContinue;

		public static void SetPendingTarget(string scene_name, bool wait_for_tap)
		{
			PendingTargetScene = scene_name;
			WaitForTapToContinue = wait_for_tap;
		}

		public static void Clear()
		{
			PendingTargetScene = string.Empty;
			WaitForTapToContinue = false;
		}
	}
}
