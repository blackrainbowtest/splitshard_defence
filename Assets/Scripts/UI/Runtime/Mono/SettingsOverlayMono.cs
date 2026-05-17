using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SHD.UI.Mono
{
	public class SettingsOverlayMono : MonoBehaviour
	{
		[SerializeField] private Button _backdrop_button;
		[SerializeField] private Button _close_button;
		[SerializeField] private Button _save_button;
		[SerializeField] private Button _language_button;
		[SerializeField] private TMP_Text _language_value_tmp_text;
		[SerializeField] private TMP_Text _unsaved_changes_tmp_text;
		[SerializeField] private Slider _music_slider;
		[SerializeField] private Slider _sfx_slider;
		[SerializeField] private Slider _ui_slider;
		[SerializeField] private string[] _language_options = new[] { "en", "ru" };

		private MonoBehaviour _bootstrap_mono;
		private MethodInfo _set_language_method;
		private MethodInfo _set_music_volume_method;
		private MethodInfo _set_sfx_volume_method;
		private MethodInfo _set_ui_volume_method;
		private PropertyInfo _current_language_property;
		private PropertyInfo _current_music_volume_property;
		private PropertyInfo _current_sfx_volume_property;
		private PropertyInfo _current_ui_volume_property;
		private string _pending_language;
		private float _pending_music_volume = 1f;
		private float _pending_sfx_volume = 1f;
		private float _pending_ui_volume = 1f;
		private string _initial_language;
		private float _initial_music_volume = 1f;
		private float _initial_sfx_volume = 1f;
		private float _initial_ui_volume = 1f;

		private void Awake()
		{
			if (HasRequiredReferences() == false)
			{
				Debug.LogError("SettingsOverlayMono: required prefab references are missing. Open SettingsOverlay prefab in editor to auto-build template.");
				return;
			}

			ResolveServices();
			BindUi();
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (Application.isPlaying == true)
				return;

			EnsureEditorTemplate();
		}
#endif

		public void CloseOverlay()
		{
			Destroy(gameObject);
		}

		private void ResolveServices()
		{
			MonoBehaviour[] behaviours;
			int i;

			behaviours = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude);
			for (i = 0; i < behaviours.Length; i++)
			{
				if (behaviours[i] != null && behaviours[i].GetType().FullName == "SHD.Core.Bootstrap.Mono.BootstrapMono")
				{
					_bootstrap_mono = behaviours[i];
					break;
				}
			}

			if (_bootstrap_mono == null)
				return;

			_set_language_method = _bootstrap_mono.GetType().GetMethod("SetLanguage", BindingFlags.Public | BindingFlags.Instance);
			_set_music_volume_method = _bootstrap_mono.GetType().GetMethod("SetMusicVolume", BindingFlags.Public | BindingFlags.Instance);
			_set_sfx_volume_method = _bootstrap_mono.GetType().GetMethod("SetSfxVolume", BindingFlags.Public | BindingFlags.Instance);
			_set_ui_volume_method = _bootstrap_mono.GetType().GetMethod("SetUiVolume", BindingFlags.Public | BindingFlags.Instance);
			_current_language_property = _bootstrap_mono.GetType().GetProperty("CurrentLanguage", BindingFlags.Public | BindingFlags.Instance);
			_current_music_volume_property = _bootstrap_mono.GetType().GetProperty("CurrentMusicVolume", BindingFlags.Public | BindingFlags.Instance);
			_current_sfx_volume_property = _bootstrap_mono.GetType().GetProperty("CurrentSfxVolume", BindingFlags.Public | BindingFlags.Instance);
			_current_ui_volume_property = _bootstrap_mono.GetType().GetProperty("CurrentUiVolume", BindingFlags.Public | BindingFlags.Instance);
		}

		private void BindUi()
		{
			if (_backdrop_button != null)
			{
				_backdrop_button.onClick.RemoveAllListeners();
				_backdrop_button.onClick.AddListener(CloseOverlay);
			}

			if (_close_button != null)
			{
				_close_button.onClick.RemoveAllListeners();
				_close_button.onClick.AddListener(CloseOverlay);
			}
			if (_save_button != null)
			{
				_save_button.onClick.RemoveAllListeners();
				_save_button.onClick.AddListener(HandleSaveClicked);
			}

			if (_language_button != null)
			{
				_language_button.onClick.RemoveAllListeners();
				_language_button.onClick.AddListener(HandleLanguageButtonClicked);
			}

			_pending_language = GetStoredLanguageOrDefault();
			_pending_music_volume = GetStoredSliderValue(_current_music_volume_property, 1f);
			_pending_sfx_volume = GetStoredSliderValue(_current_sfx_volume_property, 1f);
			_pending_ui_volume = GetStoredSliderValue(_current_ui_volume_property, 1f);
			_initial_language = _pending_language;
			_initial_music_volume = _pending_music_volume;
			_initial_sfx_volume = _pending_sfx_volume;
			_initial_ui_volume = _pending_ui_volume;

			BindSlider(_music_slider, HandleMusicVolumeChanged, _pending_music_volume);
			BindSlider(_sfx_slider, HandleSfxVolumeChanged, _pending_sfx_volume);
			BindSlider(_ui_slider, HandleUiVolumeChanged, _pending_ui_volume);
			RefreshLanguageLabel(_pending_language);
			RefreshUnsavedIndicator();
		}

		private void BindSlider(Slider slider, UnityEngine.Events.UnityAction<float> handler, float initial_value)
		{
			if (slider == null)
				return;

			slider.minValue = 0f;
			slider.maxValue = 1f;
			slider.value = Mathf.Clamp01(initial_value);
			slider.onValueChanged.RemoveAllListeners();
			slider.onValueChanged.AddListener(handler);
		}

		private void HandleLanguageButtonClicked()
		{
			string language;
			int current_index;
			int next_index;

			if (_set_language_method == null || _language_options == null || _language_options.Length == 0)
				return;

			current_index = GetCurrentLanguageIndex();
			next_index = (current_index + 1) % _language_options.Length;
			language = _language_options[next_index];

			_pending_language = language;
			RefreshLanguageLabel(_pending_language);
			RefreshUnsavedIndicator();
		}

		private void HandleMusicVolumeChanged(float value)
		{
			_pending_music_volume = Mathf.Clamp01(value);
			RefreshUnsavedIndicator();
		}

		private void HandleSfxVolumeChanged(float value)
		{
			_pending_sfx_volume = Mathf.Clamp01(value);
			RefreshUnsavedIndicator();
		}

		private void HandleUiVolumeChanged(float value)
		{
			_pending_ui_volume = Mathf.Clamp01(value);
			RefreshUnsavedIndicator();
		}

		private void HandleSaveClicked()
		{
			if (_set_music_volume_method != null)
				_set_music_volume_method.Invoke(_bootstrap_mono, new object[] { _pending_music_volume });
			if (_set_sfx_volume_method != null)
				_set_sfx_volume_method.Invoke(_bootstrap_mono, new object[] { _pending_sfx_volume });
			if (_set_ui_volume_method != null)
				_set_ui_volume_method.Invoke(_bootstrap_mono, new object[] { _pending_ui_volume });
			if (_set_language_method != null && string.IsNullOrWhiteSpace(_pending_language) == false)
				_set_language_method.Invoke(_bootstrap_mono, new object[] { _pending_language });

			_initial_language = _pending_language;
			_initial_music_volume = _pending_music_volume;
			_initial_sfx_volume = _pending_sfx_volume;
			_initial_ui_volume = _pending_ui_volume;
			RefreshUnsavedIndicator();

			CloseOverlay();
		}

		private bool HasRequiredReferences()
		{
			return (_backdrop_button != null &&
				_close_button != null &&
				_save_button != null &&
				_language_button != null &&
				_language_value_tmp_text != null &&
				_music_slider != null &&
				_sfx_slider != null &&
				_ui_slider != null &&
				_unsaved_changes_tmp_text != null);
		}

		private void BuildFallbackLayout()
		{
			RectTransform root;
			RectTransform panel;

			root = EnsureUiRoot();
			_backdrop_button = CreateBackdrop(root);
			panel = CreatePanel(root);

			CreateTitle(panel, "SETTINGS");
			CreateTextLabel(panel, "Language", new Vector2(-250f, -136f));
			_language_button = CreateButton(panel, "LanguageButton", new Vector2(100f, -170f), "");
			_language_value_tmp_text = _language_button.GetComponentInChildren<TMP_Text>();
			_music_slider = CreateLabeledSlider(panel, "Music", new Vector2(0f, -290f));
			_sfx_slider = CreateLabeledSlider(panel, "SFX", new Vector2(0f, -390f));
			_ui_slider = CreateLabeledSlider(panel, "UI", new Vector2(0f, -490f));
			_unsaved_changes_tmp_text = CreateText(panel, "UnsavedChanges", "Unsaved changes", 24f, FontStyles.Bold, TextAlignmentOptions.Center);
			_unsaved_changes_tmp_text.color = new Color(0.12f, 0.36f, 0.74f, 1f);
			{
				RectTransform u = _unsaved_changes_tmp_text.rectTransform;
				u.anchorMin = u.anchorMax = new Vector2(0.5f, 1f);
				u.pivot = new Vector2(0.5f, 1f);
				u.sizeDelta = new Vector2(420f, 36f);
				u.anchoredPosition = new Vector2(0f, -570f);
			}
			_save_button = CreateButton(panel, "Save", new Vector2(-120f, 90f), "SAVE");
			_close_button = CreateButton(panel, "Close", new Vector2(120f, 90f), "CLOSE");
		}

#if UNITY_EDITOR
		private void EnsureEditorTemplate()
		{
			if (HasRequiredReferences() == true)
				return;

			ClearTemplateChildren();
			ResetTemplateReferences();
			BuildFallbackLayout();
			EditorUtility.SetDirty(this);
		}

		private void ClearTemplateChildren()
		{
			int i;

			for (i = transform.childCount - 1; i >= 0; i--)
				DestroyImmediate(transform.GetChild(i).gameObject);
		}
#endif

		private void ResetTemplateReferences()
		{
			_backdrop_button = null;
			_close_button = null;
			_save_button = null;
			_language_button = null;
			_language_value_tmp_text = null;
			_unsaved_changes_tmp_text = null;
			_music_slider = null;
			_sfx_slider = null;
			_ui_slider = null;
		}

		private RectTransform EnsureUiRoot()
		{
			RectTransform rect;

			rect = transform as RectTransform;
			if (rect != null)
				return rect;

			GameObject ui_root = new GameObject("UIRoot", typeof(RectTransform));
			ui_root.transform.SetParent(transform.parent != null ? transform.parent : transform, false);
			rect = ui_root.GetComponent<RectTransform>();
			Stretch(rect);
			return rect;
		}

		private Button CreateBackdrop(RectTransform parent)
		{
			GameObject go = new GameObject("Backdrop", typeof(RectTransform), typeof(Image), typeof(Button));
			go.transform.SetParent(parent, false);
			Stretch(go.GetComponent<RectTransform>());
			go.GetComponent<Image>().color = new Color(0.05f, 0.15f, 0.35f, 0.62f);
			return go.GetComponent<Button>();
		}

		private RectTransform CreatePanel(RectTransform parent)
		{
			GameObject go = new GameObject("Panel", typeof(RectTransform), typeof(Image));
			RectTransform rect;

			go.transform.SetParent(parent, false);
			rect = go.GetComponent<RectTransform>();
			rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.sizeDelta = new Vector2(820f, 1000f);
			rect.anchoredPosition = Vector2.zero;
			go.GetComponent<Image>().color = new Color(0.96f, 0.98f, 1f, 0.98f);
			return rect;
		}

		private void CreateTitle(RectTransform parent, string text)
		{
			TMP_Text tmp = CreateText(parent, "Title", text, 54f, FontStyles.Bold, TextAlignmentOptions.Center);
			RectTransform rect = tmp.rectTransform;
			rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
			rect.pivot = new Vector2(0.5f, 1f);
			rect.sizeDelta = new Vector2(700f, 90f);
			rect.anchoredPosition = new Vector2(0f, -70f);
		}

		private Slider CreateLabeledSlider(RectTransform parent, string label, Vector2 pos)
		{
			CreateTextLabel(parent, label, pos + new Vector2(-250f, 34f));

			GameObject go = new GameObject(label + "Slider", typeof(RectTransform), typeof(Slider));
			go.transform.SetParent(parent, false);
			RectTransform rect = go.GetComponent<RectTransform>();
			rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
			rect.pivot = new Vector2(0.5f, 1f);
			rect.sizeDelta = new Vector2(500f, 40f);
			rect.anchoredPosition = pos;

			GameObject bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
			bg.transform.SetParent(rect, false);
			Stretch(bg.GetComponent<RectTransform>());
			bg.GetComponent<Image>().color = new Color(0.78f, 0.86f, 1f, 1f);

			GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
			fillArea.transform.SetParent(rect, false);
			Stretch(fillArea.GetComponent<RectTransform>());

			GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
			fill.transform.SetParent(fillArea.transform, false);
			Stretch(fill.GetComponent<RectTransform>());
			fill.GetComponent<Image>().color = new Color(0.12f, 0.36f, 0.74f, 1f);

			GameObject handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
			handle.transform.SetParent(rect, false);
			RectTransform h = handle.GetComponent<RectTransform>();
			h.sizeDelta = new Vector2(28f, 44f);
			handle.GetComponent<Image>().color = Color.white;

			Slider slider = go.GetComponent<Slider>();
			slider.fillRect = fill.GetComponent<RectTransform>();
			slider.handleRect = h;
			slider.targetGraphic = handle.GetComponent<Image>();
			slider.direction = Slider.Direction.LeftToRight;
			return slider;
		}

		private Button CreateButton(RectTransform parent, string name, Vector2 pos, string text)
		{
			GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
			go.transform.SetParent(parent, false);
			RectTransform rect = go.GetComponent<RectTransform>();
			rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
			rect.pivot = new Vector2(0.5f, 1f);
			rect.sizeDelta = new Vector2(220f, 58f);
			rect.anchoredPosition = pos;
			go.GetComponent<Image>().color = new Color(0.12f, 0.36f, 0.74f, 1f);

			TMP_Text label = CreateText(go.transform as RectTransform, "Label", text, 28f, FontStyles.Bold, TextAlignmentOptions.Center);
			Stretch(label.rectTransform);
			label.color = Color.white;
			return go.GetComponent<Button>();
		}

		private TMP_Text CreateTextLabel(RectTransform parent, string text, Vector2 pos)
		{
			TMP_Text tmp = CreateText(parent, text + "Label", text, 30f, FontStyles.Bold, TextAlignmentOptions.Left);
			RectTransform rect = tmp.rectTransform;
			rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
			rect.pivot = new Vector2(0f, 1f);
			rect.sizeDelta = new Vector2(500f, 40f);
			rect.anchoredPosition = pos;
			return tmp;
		}

		private TMP_Text CreateText(RectTransform parent, string name, string value, float size, FontStyles style, TextAlignmentOptions align)
		{
			GameObject go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
			go.transform.SetParent(parent, false);
			TMP_Text tmp = go.GetComponent<TextMeshProUGUI>();
			tmp.text = value;
			tmp.fontSize = size;
			tmp.fontStyle = style;
			tmp.alignment = align;
			tmp.color = new Color(0.14f, 0.22f, 0.38f, 1f);
			tmp.raycastTarget = false;
			return tmp;
		}

		private void Stretch(RectTransform rect)
		{
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.anchoredPosition = Vector2.zero;
			rect.sizeDelta = Vector2.zero;
		}

		private int GetCurrentLanguageIndex()
		{
			string current;
			int i;

			current = _language_value_tmp_text != null ? _language_value_tmp_text.text : string.Empty;
			current = current.ToLowerInvariant();
			for (i = 0; i < _language_options.Length; i++)
			{
				if (_language_options[i] == current)
					return i;
			}
			return 0;
		}

		private void RefreshLanguageLabel(string force_language = null)
		{
			string language;

			if (_language_value_tmp_text == null)
				return;

			language = !string.IsNullOrWhiteSpace(force_language)
				? force_language
				: ((_language_options != null && _language_options.Length > 0) ? _language_options[0] : "en");

			_language_value_tmp_text.text = language.ToLowerInvariant();
		}

		private string GetStoredLanguageOrDefault()
		{
			object value;
			string language;

			if (_current_language_property == null || _bootstrap_mono == null)
				return ((_language_options != null && _language_options.Length > 0) ? _language_options[0] : "en");

			value = _current_language_property.GetValue(_bootstrap_mono);
			language = value as string;
			if (string.IsNullOrWhiteSpace(language))
				return ((_language_options != null && _language_options.Length > 0) ? _language_options[0] : "en");
			return (language.ToLowerInvariant());
		}

		private float GetStoredSliderValue(PropertyInfo property, float fallback)
		{
			object value;

			if (property == null || _bootstrap_mono == null)
				return fallback;

			value = property.GetValue(_bootstrap_mono);
			if (value is float f)
				return Mathf.Clamp01(f);

			return fallback;
		}

		private void RefreshUnsavedIndicator()
		{
			bool has_changes;

			if (_unsaved_changes_tmp_text == null)
				return;

			has_changes =
				Mathf.Abs(_initial_music_volume - _pending_music_volume) > 0.0001f ||
				Mathf.Abs(_initial_sfx_volume - _pending_sfx_volume) > 0.0001f ||
				Mathf.Abs(_initial_ui_volume - _pending_ui_volume) > 0.0001f ||
				string.Equals(_initial_language, _pending_language, System.StringComparison.OrdinalIgnoreCase) == false;

			_unsaved_changes_tmp_text.gameObject.SetActive(has_changes);
		}
	}
}
