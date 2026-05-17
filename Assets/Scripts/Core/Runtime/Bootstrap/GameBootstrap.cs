using System;

using SHD.Localization.Domain;
using SHD.Localization.Services;
using SHD.Localization.Storage;
using SHD.Loading.Domain;
using SHD.Loading.UnityBridge;
using SHD.Save.FileSystem;
using SHD.Save.Managers;
using SHD.Save.Migration;
using SHD.Save.Serialization;

using SHD.Settings.Managers;

using SHD.UI.Controllers;

namespace SHD.Core.Bootstrap
{
	public class GameBootstrap
	{
		private SettingsManager _settings_manager;
		private SaveManager _save_manager;
		private ILoadingService _loading_service;
		private ILocalizationService _localization_service;
		private MainMenuController _main_menu_controller;

		public SettingsManager SettingsManager
		{
			get
			{
				return (_settings_manager);
			}
		}

		public SaveManager SaveManager
		{
			get
			{
				return (_save_manager);
			}
		}

		public ILoadingService LoadingService
		{
			get
			{
				return (_loading_service);
			}
		}

		public ILocalizationService LocalizationService
		{
			get
			{
				return (_localization_service);
			}
		}

		public MainMenuController MainMenuController
		{
			get
			{
				return (_main_menu_controller);
			}
		}

		public GameBootstrap(string settings_path, string save_directory)
		{
			if (string.IsNullOrWhiteSpace(settings_path) == true)
				throw new ArgumentException("Settings path cannot be null or empty.", nameof(settings_path));

			if (string.IsNullOrWhiteSpace(save_directory) == true)
				throw new ArgumentException("Save directory cannot be null or empty.", nameof(save_directory));

			InitializeSettings(settings_path);
			InitializeLocalizationSystem();
			InitializeSaveSystem(save_directory);
			InitializeLoadingSystem();
			InitializeMainMenu();
		}

		private void InitializeSettings(string settings_path)
		{
			_settings_manager = new SettingsManager(settings_path);
		}

		private void InitializeSaveSystem(string save_directory)
		{
			SaveSerializer serializer;
			SaveMigrationService migration_service;
			SaveFileHandler file_handler;

			serializer = new SaveSerializer();
			migration_service = new SaveMigrationService();

			file_handler = new SaveFileHandler(
				save_directory,
				serializer,
				migration_service
			);

			_save_manager = new SaveManager(file_handler);
		}

		private void InitializeLoadingSystem()
		{
			_loading_service = new LoadingService();
		}

		private void InitializeLocalizationSystem()
		{
			ResourcesLocalizationStorage storage;
			LocalizationConfig config;
			string initial_language;

			storage = new ResourcesLocalizationStorage();
			config = LocalizationConfigLoader.Load();
			initial_language = _settings_manager.SettingsData.Language;

			_localization_service = new LocalizationService(storage, config, initial_language);
			LocalizationServiceLocator.SetService(_localization_service);
		}

		public bool SetLanguage(string language)
		{
			bool changed;

			changed = _localization_service.SetLanguage(language);
			_settings_manager.SetLanguage(_localization_service.CurrentLanguage);

			return (changed);
		}

		public void SetMusicVolume(float volume)
		{
			_settings_manager.SetMusicVolume(volume);
		}

		public void SetSfxVolume(float volume)
		{
			_settings_manager.SetSfxVolume(volume);
		}

		public void SetUiVolume(float volume)
		{
			_settings_manager.SetUiVolume(volume);
		}

		private void InitializeMainMenu()
		{
			_main_menu_controller = new MainMenuController();
		}
	}
}


