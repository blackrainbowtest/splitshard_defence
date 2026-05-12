using System;

using SHD.Localization.Domain;

namespace SHD.Localization.Services
{
	public static class LocalizationServiceLocator
	{
		private static ILocalizationService _service;

		public static ILocalizationService Service
		{
			get
			{
				return (_service);
			}
		}

		public static event Action<ILocalizationService> ServiceChanged;

		public static void SetService(ILocalizationService service)
		{
			_service = service;
			ServiceChanged?.Invoke(_service);
		}
	}
}
