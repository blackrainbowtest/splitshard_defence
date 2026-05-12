namespace SHD.Core.Bootstrap.Startup
{
	[System.Serializable]
	public class StartupFlowConfig
	{
		public bool AutoRun;
		public string BootstrapScene;
		public string LogoScene;
		public float LogoMinDurationSeconds;
		public string LoadingScene;
		public bool WaitForTapToContinueOnLoading;
		public string MainMenuScene;
	}
}
