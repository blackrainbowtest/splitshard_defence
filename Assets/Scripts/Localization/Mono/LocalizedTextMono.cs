using UnityEngine;
using TMPro;

using SHD.Localization.Domain;
using SHD.Localization.Services;

namespace SHD.Localization.Mono
{
	public class LocalizedTextMono : MonoBehaviour
	{
		[SerializeField] private string _localization_key;
		[SerializeField] private string _fallback_text;
		[SerializeField] private TMP_Text _tmp_text;

		private ILocalizationService _service;

		private void OnEnable()
		{
			LocalizationServiceLocator.ServiceChanged += HandleServiceChanged;
			AttachService(LocalizationServiceLocator.Service);
		}

		private void OnDisable()
		{
			LocalizationServiceLocator.ServiceChanged -= HandleServiceChanged;
			DetachService();
		}

		public void SetKey(string localization_key)
		{
			_localization_key = localization_key;
			Refresh();
		}

		private void HandleServiceChanged(ILocalizationService service)
		{
			AttachService(service);
		}

		private void AttachService(ILocalizationService service)
		{
			if (_service == service)
			{
				Refresh();
				return ;
			}

			DetachService();
			_service = service;

			if (_service != null)
				_service.LanguageChanged += HandleLanguageChanged;

			Refresh();
		}

		private void DetachService()
		{
			if (_service == null)
				return ;

			_service.LanguageChanged -= HandleLanguageChanged;
			_service = null;
		}

		private void HandleLanguageChanged(string language)
		{
			_ = language;
			Refresh();
		}

		private void Refresh()
		{
			string text_value;

			if (string.IsNullOrWhiteSpace(_localization_key) == true)
				text_value = _fallback_text;
			else if (_service == null)
				text_value = _fallback_text;
			else
				text_value = _service.GetText(_localization_key);

			ApplyText(text_value);
		}

		private void ApplyText(string text_value)
		{
			if (_tmp_text != null)
				_tmp_text.text = text_value;
		}
	}
}
