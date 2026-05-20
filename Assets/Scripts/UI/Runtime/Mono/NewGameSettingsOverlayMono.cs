using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SHD.Save.Data;
using SHD.Save.Managers;

namespace SHD.UI.Mono
{
	public class NewGameSettingsOverlayMono : MonoBehaviour
	{
		[SerializeField] private Button _backdrop_button;
		[SerializeField] private Button _close_button;
		[SerializeField] private Button _start_button;
		[SerializeField] private Button _save_slot_button;
		[SerializeField] private TMP_InputField _faction_name_input;
		[SerializeField] private TMP_Text _difficulty_value_tmp_text;
		[SerializeField] private TMP_Text _emblem_value_tmp_text;
		[SerializeField] private TMP_Text _appearance_value_tmp_text;
		[SerializeField] private TMP_Text _slot_value_tmp_text;
		[SerializeField] private Image _commander_preview_image;
		[SerializeField] private Button _difficulty_left_button;
		[SerializeField] private Button _difficulty_right_button;
		[SerializeField] private Button _emblem_left_button;
		[SerializeField] private Button _emblem_right_button;
		[SerializeField] private Button _appearance_left_button;
		[SerializeField] private Button _appearance_right_button;
		[SerializeField] private Button _slot_left_button;
		[SerializeField] private Button _slot_right_button;

		private static readonly string[] DifficultyIds = { "easy", "normal", "hard", "nightmare" };
		private static readonly string[] DifficultyLabels = { "Easy", "Normal", "Hard", "Nightmare" };
		private static readonly string[] EmblemLabels = { "Lion", "Wolf", "Dragon", "Eagle", "Crown" };
		private static readonly string[] AppearanceLabels = { "Commander 1", "Commander 2", "Commander 3", "Commander 4", "Commander 5" };
		private static readonly Color[] AppearanceColors =
		{
			new Color(0.75f, 0.20f, 0.20f, 1f),
			new Color(0.18f, 0.48f, 0.82f, 1f),
			new Color(0.20f, 0.65f, 0.34f, 1f),
			new Color(0.65f, 0.46f, 0.16f, 1f),
			new Color(0.42f, 0.24f, 0.70f, 1f)
		};

		private SaveManager _save_manager;
		private MainMenuScenePresenterMono _presenter;
		private int _difficulty_index = 1;
		private int _emblem_index;
		private int _appearance_index;
		private int _manual_slot = 1;

		public void Bind(MainMenuScenePresenterMono presenter)
		{
			_presenter = presenter;
		}

		[ContextMenu("Build Default Template")]
		public void BuildDefaultTemplate()
		{
			ResetTemplateReferences();
			ClearTemplateChildren();
			BuildRuntimeFallbackLayout();
			RefreshView();
		}

		private void Awake()
		{
			_save_manager = ResolveSaveManager();
			if (HasAllReferences() == false)
			{
				Debug.LogError("NewGameSettingsOverlayMono: prefab references are not assigned. Use 'Build Default Template' in editor and apply prefab.");
				return;
			}
			Wire();
			RefreshView();
		}

		private void Wire()
		{
			if (_backdrop_button != null)
				_backdrop_button.onClick.AddListener(CloseOverlay);
			if (_close_button != null)
				_close_button.onClick.AddListener(CloseOverlay);
			if (_start_button != null)
				_start_button.onClick.AddListener(HandleStartPressed);
			if (_save_slot_button != null)
				_save_slot_button.onClick.AddListener(HandleSaveSlotPressed);

			if (_difficulty_left_button != null)
				_difficulty_left_button.onClick.AddListener(() => { _difficulty_index = WrapIndex(_difficulty_index - 1, DifficultyLabels.Length); RefreshView(); });
			if (_difficulty_right_button != null)
				_difficulty_right_button.onClick.AddListener(() => { _difficulty_index = WrapIndex(_difficulty_index + 1, DifficultyLabels.Length); RefreshView(); });
			if (_emblem_left_button != null)
				_emblem_left_button.onClick.AddListener(() => { _emblem_index = WrapIndex(_emblem_index - 1, EmblemLabels.Length); RefreshView(); });
			if (_emblem_right_button != null)
				_emblem_right_button.onClick.AddListener(() => { _emblem_index = WrapIndex(_emblem_index + 1, EmblemLabels.Length); RefreshView(); });
			if (_appearance_left_button != null)
				_appearance_left_button.onClick.AddListener(() => { _appearance_index = WrapIndex(_appearance_index - 1, AppearanceLabels.Length); RefreshView(); });
			if (_appearance_right_button != null)
				_appearance_right_button.onClick.AddListener(() => { _appearance_index = WrapIndex(_appearance_index + 1, AppearanceLabels.Length); RefreshView(); });
			if (_slot_left_button != null)
				_slot_left_button.onClick.AddListener(() => { _manual_slot = Mathf.Clamp(_manual_slot - 1, 1, 10); RefreshView(); });
			if (_slot_right_button != null)
				_slot_right_button.onClick.AddListener(() => { _manual_slot = Mathf.Clamp(_manual_slot + 1, 1, 10); RefreshView(); });
		}

		private void HandleStartPressed()
		{
			SaveData data;

			if (_save_manager == null)
			{
				Debug.LogError("NewGameSettingsOverlayMono: SaveManager not found.");
				return;
			}

			data = BuildSaveDataFromSettings();
			_save_manager.SaveAutosave(data);

			if (_presenter != null)
				_presenter.StartGameplayFromNewGameSettings();
		}

		private void HandleSaveSlotPressed()
		{
			SaveData data;

			if (_save_manager == null)
			{
				Debug.LogError("NewGameSettingsOverlayMono: SaveManager not found.");
				return;
			}

			data = BuildSaveDataFromSettings();
			_save_manager.SaveManualSlot(_manual_slot, data);
			Debug.Log("NewGameSettingsOverlayMono: settings saved to slot " + _manual_slot + ".");
		}

		private SaveData BuildSaveDataFromSettings()
		{
			SaveData data;
			string faction_name;

			data = new SaveData();
			faction_name = _faction_name_input != null ? _faction_name_input.text : string.Empty;
			if (string.IsNullOrWhiteSpace(faction_name) == true)
				faction_name = "My Faction";

			data.DifficultyId = DifficultyIds[Mathf.Clamp(_difficulty_index, 0, DifficultyIds.Length - 1)];
			data.FactionEmblemId = _emblem_index;
			data.FactionName = faction_name.Trim();
			data.CommanderAppearanceId = _appearance_index;
			data.SaveTime = System.DateTime.Now;
			data.Score = 0;
			data.SurvivedWaves = 0;
			data.ArmySize = 0;
			return (data);
		}

		private void RefreshView()
		{
			if (_difficulty_value_tmp_text != null)
				_difficulty_value_tmp_text.text = DifficultyLabels[_difficulty_index];
			if (_emblem_value_tmp_text != null)
				_emblem_value_tmp_text.text = EmblemLabels[_emblem_index];
			if (_appearance_value_tmp_text != null)
				_appearance_value_tmp_text.text = AppearanceLabels[_appearance_index];
			if (_slot_value_tmp_text != null)
				_slot_value_tmp_text.text = "Slot " + _manual_slot;
			if (_commander_preview_image != null)
				_commander_preview_image.color = AppearanceColors[_appearance_index];
		}

		private int WrapIndex(int value, int count)
		{
			if (count <= 0)
				return (0);
			if (value < 0)
				return (count - 1);
			if (value >= count)
				return (0);
			return (value);
		}

		public void CloseOverlay()
		{
			Destroy(gameObject);
		}

		private SaveManager ResolveSaveManager()
		{
			MonoBehaviour[] behaviours;
			int i;

			behaviours = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude);
			i = 0;
			while (i < behaviours.Length)
			{
				if (behaviours[i] != null && behaviours[i].GetType().FullName == "SHD.Core.Bootstrap.Mono.BootstrapMono")
				{
					object value;
					var property = behaviours[i].GetType().GetProperty("SaveManager");
					if (property == null)
						return (null);
					value = property.GetValue(behaviours[i]);
					return (value as SaveManager);
				}
				i++;
			}

			return (null);
		}

		private bool HasAllReferences()
		{
			return (
				_backdrop_button != null &&
				_close_button != null &&
				_start_button != null &&
				_save_slot_button != null &&
				_faction_name_input != null &&
				_difficulty_value_tmp_text != null &&
				_emblem_value_tmp_text != null &&
				_appearance_value_tmp_text != null &&
				_slot_value_tmp_text != null &&
				_commander_preview_image != null &&
				_difficulty_left_button != null &&
				_difficulty_right_button != null &&
				_emblem_left_button != null &&
				_emblem_right_button != null &&
				_appearance_left_button != null &&
				_appearance_right_button != null &&
				_slot_left_button != null &&
				_slot_right_button != null
			);
		}

		private void BuildRuntimeFallbackLayout()
		{
			RectTransform root;
			RectTransform panel;
			TMP_Text tmp;

			root = transform as RectTransform;
			if (root == null)
				root = EnsureUiRootRectTransform();
			if (root == null)
				return;

			Stretch(root);
			_backdrop_button = CreateBackdrop(root);
			panel = CreatePanel(root);

			CreateLabel(panel, "New Game", new Vector2(0f, -30f), 44f, FontStyles.Bold);
			_faction_name_input = CreateInput(panel, "Faction Name", new Vector2(0f, -120f));

			tmp = CreateLabel(panel, "Difficulty", new Vector2(0f, -210f), 28f, FontStyles.Bold);
			_difficulty_left_button = CreateSmallButton(panel, "<", new Vector2(-220f, -260f));
			_difficulty_right_button = CreateSmallButton(panel, ">", new Vector2(220f, -260f));
			_difficulty_value_tmp_text = CreateLabel(panel, "Normal", new Vector2(0f, -260f), 30f, FontStyles.Normal);

			tmp = CreateLabel(panel, "Emblem", new Vector2(0f, -330f), 28f, FontStyles.Bold);
			_emblem_left_button = CreateSmallButton(panel, "<", new Vector2(-220f, -380f));
			_emblem_right_button = CreateSmallButton(panel, ">", new Vector2(220f, -380f));
			_emblem_value_tmp_text = CreateLabel(panel, "Lion", new Vector2(0f, -380f), 30f, FontStyles.Normal);

			tmp = CreateLabel(panel, "Commander", new Vector2(0f, -450f), 28f, FontStyles.Bold);
			_commander_preview_image = CreateAvatarSquare(panel, new Vector2(0f, -580f));
			_appearance_left_button = CreateSmallButton(panel, "<", new Vector2(-220f, -580f));
			_appearance_right_button = CreateSmallButton(panel, ">", new Vector2(220f, -580f));
			_appearance_value_tmp_text = CreateLabel(panel, "Commander 1", new Vector2(0f, -690f), 28f, FontStyles.Normal);

			CreateLabel(panel, "Manual Slot", new Vector2(0f, -760f), 26f, FontStyles.Bold);
			_slot_left_button = CreateSmallButton(panel, "<", new Vector2(-160f, -810f));
			_slot_right_button = CreateSmallButton(panel, ">", new Vector2(160f, -810f));
			_slot_value_tmp_text = CreateLabel(panel, "Slot 1", new Vector2(0f, -810f), 28f, FontStyles.Normal);

			_save_slot_button = CreateMainButton(panel, "Save to Slot", new Vector2(-170f, -910f));
			_start_button = CreateMainButton(panel, "Start Game", new Vector2(170f, -910f));
			_close_button = CreateMainButton(panel, "Close", new Vector2(0f, -1000f));
		}

		private RectTransform EnsureUiRootRectTransform()
		{
			Transform existing;
			GameObject go;
			RectTransform rt;

			existing = transform.Find("UIRoot");
			if (existing != null)
			{
				rt = existing as RectTransform;
				if (rt != null)
					return (rt);
			}

			go = new GameObject("UIRoot", typeof(RectTransform));
			go.transform.SetParent(transform, false);
			rt = go.GetComponent<RectTransform>();
			Stretch(rt);
			return (rt);
		}

		private void ResetTemplateReferences()
		{
			_backdrop_button = null;
			_close_button = null;
			_start_button = null;
			_save_slot_button = null;
			_faction_name_input = null;
			_difficulty_value_tmp_text = null;
			_emblem_value_tmp_text = null;
			_appearance_value_tmp_text = null;
			_slot_value_tmp_text = null;
			_commander_preview_image = null;
			_difficulty_left_button = null;
			_difficulty_right_button = null;
			_emblem_left_button = null;
			_emblem_right_button = null;
			_appearance_left_button = null;
			_appearance_right_button = null;
			_slot_left_button = null;
			_slot_right_button = null;
		}

		private void ClearTemplateChildren()
		{
			int i;

			for (i = transform.childCount - 1; i >= 0; i--)
			{
#if UNITY_EDITOR
				if (Application.isPlaying == false)
					DestroyImmediate(transform.GetChild(i).gameObject);
				else
					Destroy(transform.GetChild(i).gameObject);
#else
				Destroy(transform.GetChild(i).gameObject);
#endif
			}
		}

		private Button CreateBackdrop(RectTransform parent)
		{
			GameObject go;
			Image image;
			Button button;

			go = new GameObject("Backdrop", typeof(RectTransform), typeof(Image), typeof(Button));
			go.transform.SetParent(parent, false);
			Stretch(go.GetComponent<RectTransform>());
			image = go.GetComponent<Image>();
			image.color = new Color(0.05f, 0.12f, 0.30f, 0.70f);
			button = go.GetComponent<Button>();
			button.targetGraphic = image;
			return (button);
		}

		private RectTransform CreatePanel(RectTransform parent)
		{
			GameObject go;
			RectTransform rt;
			Image image;

			go = new GameObject("Panel", typeof(RectTransform), typeof(Image));
			go.transform.SetParent(parent, false);
			rt = go.GetComponent<RectTransform>();
			rt.anchorMin = new Vector2(0.5f, 0.5f);
			rt.anchorMax = new Vector2(0.5f, 0.5f);
			rt.pivot = new Vector2(0.5f, 0.5f);
			rt.sizeDelta = new Vector2(920f, 1120f);
			image = go.GetComponent<Image>();
			image.color = new Color(0.96f, 0.98f, 1f, 0.98f);
			return (rt);
		}

		private TMP_Text CreateLabel(RectTransform parent, string text, Vector2 pos, float size, FontStyles style)
		{
			GameObject go;
			RectTransform rt;
			TMP_Text tmp;

			go = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
			go.transform.SetParent(parent, false);
			rt = go.GetComponent<RectTransform>();
			rt.anchorMin = new Vector2(0.5f, 1f);
			rt.anchorMax = new Vector2(0.5f, 1f);
			rt.pivot = new Vector2(0.5f, 1f);
			rt.anchoredPosition = pos;
			rt.sizeDelta = new Vector2(780f, 60f);
			tmp = go.GetComponent<TextMeshProUGUI>();
			tmp.text = text;
			tmp.fontSize = size;
			tmp.fontStyle = style;
			tmp.alignment = TextAlignmentOptions.Center;
			tmp.color = new Color(0.10f, 0.22f, 0.42f, 1f);
			tmp.raycastTarget = false;
			return (tmp);
		}

		private TMP_InputField CreateInput(RectTransform parent, string placeholder, Vector2 pos)
		{
			GameObject root;
			RectTransform rt;
			Image bg;
			TMP_InputField input;
			TMP_Text text;
			TMP_Text ph;

			root = new GameObject("FactionNameInput", typeof(RectTransform), typeof(Image), typeof(TMP_InputField));
			root.transform.SetParent(parent, false);
			rt = root.GetComponent<RectTransform>();
			rt.anchorMin = new Vector2(0.5f, 1f);
			rt.anchorMax = new Vector2(0.5f, 1f);
			rt.pivot = new Vector2(0.5f, 1f);
			rt.anchoredPosition = pos;
			rt.sizeDelta = new Vector2(700f, 64f);
			bg = root.GetComponent<Image>();
			bg.color = Color.white;
			input = root.GetComponent<TMP_InputField>();

			text = CreateLabel(rt, "", new Vector2(0f, -12f), 28f, FontStyles.Normal);
			text.alignment = TextAlignmentOptions.Left;
			text.rectTransform.offsetMin = new Vector2(18f, 6f);
			text.rectTransform.offsetMax = new Vector2(-18f, -6f);
			text.text = "My Faction";
			text.color = new Color(0.1f, 0.1f, 0.1f, 1f);
			text.name = "Text";

			ph = CreateLabel(rt, placeholder, new Vector2(0f, -12f), 28f, FontStyles.Italic);
			ph.alignment = TextAlignmentOptions.Left;
			ph.rectTransform.offsetMin = new Vector2(18f, 6f);
			ph.rectTransform.offsetMax = new Vector2(-18f, -6f);
			ph.color = new Color(0.50f, 0.56f, 0.64f, 1f);
			ph.name = "Placeholder";

			input.textComponent = text;
			input.placeholder = ph;
			input.text = "My Faction";
			return (input);
		}

		private Button CreateSmallButton(RectTransform parent, string text, Vector2 pos)
		{
			return CreateButton(parent, text, pos, new Vector2(88f, 56f), 34f);
		}

		private Button CreateMainButton(RectTransform parent, string text, Vector2 pos)
		{
			return CreateButton(parent, text, pos, new Vector2(300f, 64f), 28f);
		}

		private Button CreateButton(RectTransform parent, string text, Vector2 pos, Vector2 size, float font)
		{
			GameObject go;
			RectTransform rt;
			Image image;
			Button button;
			TMP_Text label;

			go = new GameObject(text + "Button", typeof(RectTransform), typeof(Image), typeof(Button));
			go.transform.SetParent(parent, false);
			rt = go.GetComponent<RectTransform>();
			rt.anchorMin = new Vector2(0.5f, 1f);
			rt.anchorMax = new Vector2(0.5f, 1f);
			rt.pivot = new Vector2(0.5f, 1f);
			rt.anchoredPosition = pos;
			rt.sizeDelta = size;
			image = go.GetComponent<Image>();
			image.color = new Color(0.12f, 0.36f, 0.74f, 1f);
			button = go.GetComponent<Button>();
			button.targetGraphic = image;

			label = CreateLabel(rt, text, new Vector2(0f, 0f), font, FontStyles.Bold);
			Stretch(label.rectTransform);
			label.color = Color.white;
			return (button);
		}

		private Image CreateAvatarSquare(RectTransform parent, Vector2 pos)
		{
			GameObject go;
			RectTransform rt;
			Image img;

			go = new GameObject("CommanderAvatar", typeof(RectTransform), typeof(Image));
			go.transform.SetParent(parent, false);
			rt = go.GetComponent<RectTransform>();
			rt.anchorMin = new Vector2(0.5f, 1f);
			rt.anchorMax = new Vector2(0.5f, 1f);
			rt.pivot = new Vector2(0.5f, 1f);
			rt.anchoredPosition = pos;
			rt.sizeDelta = new Vector2(220f, 220f);
			img = go.GetComponent<Image>();
			img.color = AppearanceColors[0];
			return (img);
		}

		private void Stretch(RectTransform rt)
		{
			rt.anchorMin = Vector2.zero;
			rt.anchorMax = Vector2.one;
			rt.offsetMin = Vector2.zero;
			rt.offsetMax = Vector2.zero;
			rt.localScale = Vector3.one;
			rt.localPosition = Vector3.zero;
		}
	}
}
