using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Reflection;

using SHD.Save.Data;
using SHD.Save.Managers;

namespace SHD.UI.Mono
{
	public class SavesOverlayMono : MonoBehaviour
	{
		private const float PanelWidth = 860f;
		private const float PanelHeight = 1180f;

		[SerializeField] private Button _backdrop_button;
		[SerializeField] private Button _close_button;
		[SerializeField] private Button _load_button;
		[SerializeField] private Button _save_button;
		[SerializeField] private Button _delete_button;
		[SerializeField] private TMP_Text _slots_placeholder_tmp_text;
		[SerializeField] private RectTransform _slots_content_root;
		[SerializeField] private SavesSlotItemMono _slot_item_template;
		[SerializeField] private TMP_Text _selected_slot_tmp_text;
		[SerializeField] private Color _backdrop_color = new Color(0.05f, 0.15f, 0.35f, 0.62f);
		[SerializeField] private Color _panel_color = new Color(0.96f, 0.98f, 1f, 0.98f);
		[SerializeField] private Color _button_color = new Color(0.12f, 0.36f, 0.74f, 1f);
		[SerializeField] private Color _title_color = new Color(0.08f, 0.24f, 0.52f, 1f);
		[SerializeField] private Color _text_color = new Color(0.14f, 0.22f, 0.38f, 1f);
		[SerializeField] private Color _slot_bg_color = new Color(0.85f, 0.92f, 1f, 1f);
		[SerializeField] private Color _slot_details_color = new Color(0.23f, 0.34f, 0.52f, 1f);

		private readonly List<SavesSlotItemMono> _slot_items = new List<SavesSlotItemMono>();
		private SaveManager _save_manager;
		private int _selected_slot_id = 0;

		private void Awake()
		{
			BuildFallbackLayoutIfNeeded();
			_save_manager = ResolveSaveManager();
			WireButtons();
			RebuildSlots();
		}

		[ContextMenu("Build Default Template")]
		public void BuildDefaultTemplate()
		{
			ResetTemplateReferences();
			ClearTemplateChildren();
			BuildFallbackLayoutIfNeeded();
			ApplyDefaultPlaceholder();
		}

		public void CloseOverlay()
		{
			Destroy(gameObject);
		}

		private void WireButtons()
		{
			if (_backdrop_button != null)
				_backdrop_button.onClick.AddListener(CloseOverlay);
			if (_close_button != null)
				_close_button.onClick.AddListener(CloseOverlay);
			if (_load_button != null)
				_load_button.onClick.AddListener(HandleLoadPressed);
			if (_save_button != null)
				_save_button.onClick.AddListener(HandleSavePressed);
			if (_delete_button != null)
				_delete_button.onClick.AddListener(HandleDeletePressed);
		}

		private void ApplyDefaultPlaceholder()
		{
			if (_slots_placeholder_tmp_text == null)
				return;

			_slots_placeholder_tmp_text.text =
				"Autosave\nSlot 1\nSlot 2\nSlot 3";
		}

		private void HandleLoadPressed()
		{
			SaveSlot slot;

			if (_save_manager == null)
			{
				Debug.LogError("SavesOverlayMono: SaveManager not found.");
				return;
			}

			_save_manager.LoadAllSaves();
			slot = _save_manager.GetSlot(_selected_slot_id);
			if (slot == null || slot.HasSave == false)
			{
				Debug.Log("SavesOverlayMono: selected slot has no save.");
				return;
			}

			Debug.Log("SavesOverlayMono: loaded slot " + _selected_slot_id.ToString() + ".");
			RebuildSlots();
		}

		private void HandleSavePressed()
		{
			SaveData save_data;

			if (_save_manager == null)
			{
				Debug.LogError("SavesOverlayMono: SaveManager not found.");
				return;
			}

			save_data = CreateRuntimeSaveData();
			if (_selected_slot_id == 0)
				_save_manager.SaveAutosave(save_data);
			else
				_save_manager.SaveManualSlot(_selected_slot_id, save_data);

			Debug.Log("SavesOverlayMono: saved to slot " + _selected_slot_id.ToString() + ".");
			RebuildSlots();
		}

		private void HandleDeletePressed()
		{
			if (_save_manager == null)
			{
				Debug.LogError("SavesOverlayMono: SaveManager not found.");
				return;
			}

			_save_manager.DeleteSave(_selected_slot_id);
			Debug.Log("SavesOverlayMono: deleted slot " + _selected_slot_id.ToString() + ".");
			RebuildSlots();
		}

		private SaveManager ResolveSaveManager()
		{
			MonoBehaviour[] behaviours;
			MonoBehaviour candidate;
			PropertyInfo save_manager_property;
			object value;
			int i;

			behaviours = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude);
			i = 0;
			while (i < behaviours.Length)
			{
				candidate = behaviours[i];
				if (candidate == null || candidate.GetType().FullName != "SHD.Core.Bootstrap.Mono.BootstrapMono")
				{
					i++;
					continue;
				}

				save_manager_property = candidate.GetType().GetProperty("SaveManager", BindingFlags.Public | BindingFlags.Instance);
				if (save_manager_property == null)
					return (null);

				value = save_manager_property.GetValue(candidate);
				return (value as SaveManager);
			}

			return (null);
		}

		private SaveData CreateRuntimeSaveData()
		{
			SaveData save_data;

			save_data = new SaveData();
			save_data.SaveTime = System.DateTime.Now;
			save_data.Score = Random.Range(0, 9999);
			save_data.SurvivedWaves = Random.Range(0, 50);
			save_data.ArmySize = Random.Range(0, 200);
			return (save_data);
		}

		private void RebuildSlots()
		{
			SaveSlot slot;
			int slot_id;

			EnsureSlotTemplate();
			ClearSlotItems();

			if (_save_manager != null)
				_save_manager.LoadAllSaves();

			slot_id = 0;
			while (slot_id < 64)
			{
				if (_save_manager == null)
				{
					if (slot_id > 10)
						break;
					slot = null;
				}
				else
				{
					slot = _save_manager.GetSlot(slot_id);
					if (slot == null)
						break;
				}

				CreateSlotItem(slot_id, slot);
				slot_id++;
			}

			if (_slot_items.Count > 0)
				SelectSlot(_selected_slot_id);
			ApplyDefaultPlaceholder();
		}

		private void EnsureSlotTemplate()
		{
			if (_slot_item_template != null && _slots_content_root != null)
				return;

			if (_slots_content_root == null)
				_slots_content_root = CreateSlotsRoot();
			if (_slot_item_template == null)
				_slot_item_template = CreateSlotTemplate(_slots_content_root);
		}

		private RectTransform CreateSlotsRoot()
		{
			GameObject root_go;
			RectTransform root_rect;
			VerticalLayoutGroup layout_group;
			ContentSizeFitter size_fitter;

			root_go = new GameObject("SlotsContentRoot", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
			root_go.transform.SetParent(_slots_placeholder_tmp_text.transform.parent, false);
			root_rect = root_go.GetComponent<RectTransform>();
			root_rect.anchorMin = _slots_placeholder_tmp_text.rectTransform.anchorMin;
			root_rect.anchorMax = _slots_placeholder_tmp_text.rectTransform.anchorMax;
			root_rect.pivot = _slots_placeholder_tmp_text.rectTransform.pivot;
			root_rect.anchoredPosition = _slots_placeholder_tmp_text.rectTransform.anchoredPosition;
			root_rect.sizeDelta = _slots_placeholder_tmp_text.rectTransform.sizeDelta;

			layout_group = root_go.GetComponent<VerticalLayoutGroup>();
			layout_group.spacing = 8f;
			layout_group.childControlHeight = true;
			layout_group.childControlWidth = true;
			layout_group.childForceExpandHeight = false;
			layout_group.childForceExpandWidth = true;

			size_fitter = root_go.GetComponent<ContentSizeFitter>();
			size_fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
			size_fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			return (root_rect);
		}

		private SavesSlotItemMono CreateSlotTemplate(RectTransform parent)
		{
			GameObject slot_go;
			RectTransform slot_rect;
			Image bg_image;
			Button button;
			LayoutElement layout_element;
			SavesSlotItemMono slot_mono;
			TMP_Text name_tmp;
			TMP_Text details_tmp;

			slot_go = new GameObject(
				"SlotItemTemplate",
				typeof(RectTransform),
				typeof(Image),
				typeof(Button),
				typeof(LayoutElement),
				typeof(SavesSlotItemMono));
			slot_go.transform.SetParent(parent, false);

			slot_rect = slot_go.GetComponent<RectTransform>();
			slot_rect.sizeDelta = new Vector2(0f, 80f);

			layout_element = slot_go.GetComponent<LayoutElement>();
			layout_element.preferredHeight = 80f;

			bg_image = slot_go.GetComponent<Image>();
			bg_image.color = _slot_bg_color;

			button = slot_go.GetComponent<Button>();
			button.targetGraphic = bg_image;

			name_tmp = CreateSlotText(slot_rect, "SlotName", new Vector2(20f, -14f), 30f, FontStyles.Bold);
			details_tmp = CreateSlotText(slot_rect, "SlotDetails", new Vector2(20f, -48f), 22f, FontStyles.Normal);
			details_tmp.color = _slot_details_color;

			slot_mono = slot_go.GetComponent<SavesSlotItemMono>();
			slot_mono.SetReferences(button, name_tmp, details_tmp, bg_image);
			slot_go.SetActive(false);
			return (slot_mono);
		}

		private TMP_Text CreateSlotText(RectTransform parent, string name, Vector2 pos, float size, FontStyles style)
		{
			GameObject text_go;
			RectTransform text_rect;
			TMP_Text tmp;

			text_go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
			text_go.transform.SetParent(parent, false);
			text_rect = text_go.GetComponent<RectTransform>();
			text_rect.anchorMin = new Vector2(0f, 1f);
			text_rect.anchorMax = new Vector2(1f, 1f);
			text_rect.pivot = new Vector2(0f, 1f);
			text_rect.anchoredPosition = pos;
			text_rect.sizeDelta = new Vector2(-40f, 28f);

			tmp = text_go.GetComponent<TextMeshProUGUI>();
			tmp.text = name;
			tmp.fontSize = size;
			tmp.fontStyle = style;
			tmp.alignment = TextAlignmentOptions.Left;
			tmp.color = new Color(0.95f, 0.95f, 0.95f, 1f);
			tmp.raycastTarget = false;
			return (tmp);
		}

		private void CreateSlotItem(int slot_id, SaveSlot slot)
		{
			SavesSlotItemMono item;
			string slot_name;
			string details;

			item = Instantiate(_slot_item_template, _slots_content_root, false);
			item.gameObject.name = slot_id == 0 ? "Slot_Autosave" : "Slot_" + slot_id.ToString();
			item.gameObject.SetActive(true);

			slot_name = slot_id == 0 ? "Autosave" : "Slot " + slot_id.ToString();
			details = BuildSlotDetails(slot);

			item.Initialize(slot_id, slot_name, details, SelectSlot);
			_slot_items.Add(item);
		}

		private string BuildSlotDetails(SaveSlot slot)
		{
			SaveData data;

			if (slot == null || slot.HasSave == false)
				return ("Empty");

			data = slot.SaveData;
			if (data == null)
				return ("Empty");

			return ("Saved: " + data.SaveTime.ToString("yyyy-MM-dd HH:mm") +
				" | Score: " + data.Score.ToString() +
				" | Waves: " + data.SurvivedWaves.ToString());
		}

		private void SelectSlot(int slot_id)
		{
			int i;

			_selected_slot_id = slot_id;
			for (i = 0; i < _slot_items.Count; i++)
				_slot_items[i].SetSelected(_slot_items[i].SlotId == slot_id);

			UpdateSelectedSlotLabel();
		}

		private void UpdateSelectedSlotLabel()
		{
			if (_selected_slot_tmp_text == null)
				return;

			if (_selected_slot_id == 0)
				_selected_slot_tmp_text.text = "Selected: Autosave";
			else
				_selected_slot_tmp_text.text = "Selected: Slot " + _selected_slot_id.ToString();
		}

		private void ClearSlotItems()
		{
			int i;

			for (i = 0; i < _slot_items.Count; i++)
			{
				if (_slot_items[i] != null)
					Destroy(_slot_items[i].gameObject);
			}

			_slot_items.Clear();
		}

		private void BuildFallbackLayoutIfNeeded()
		{
			RectTransform root_rect;
			RectTransform parent_rect;
			GameObject backdrop_go;
			GameObject panel_go;
			RectTransform panel_rect;

			if (_backdrop_button != null &&
				_close_button != null &&
				_load_button != null &&
				_save_button != null &&
				_delete_button != null &&
				_slots_placeholder_tmp_text != null)
				return;

			root_rect = transform as RectTransform;
			if (root_rect == null)
			{
				GameObject ui_root_go;

				ui_root_go = new GameObject("UIRoot", typeof(RectTransform));
				parent_rect = transform.parent as RectTransform;
				if (parent_rect != null)
					ui_root_go.transform.SetParent(parent_rect, false);
				else
					ui_root_go.transform.SetParent(transform, false);
				root_rect = ui_root_go.GetComponent<RectTransform>();
			}

			StretchRect(root_rect);

			backdrop_go = new GameObject("Backdrop", typeof(RectTransform), typeof(Image), typeof(Button));
			backdrop_go.transform.SetParent(root_rect, false);
			StretchRect(backdrop_go.GetComponent<RectTransform>());
			backdrop_go.GetComponent<Image>().color = _backdrop_color;
			_backdrop_button = backdrop_go.GetComponent<Button>();

			panel_go = new GameObject("Panel", typeof(RectTransform), typeof(Image));
			panel_go.transform.SetParent(root_rect, false);
			panel_rect = panel_go.GetComponent<RectTransform>();
			panel_rect.anchorMin = new Vector2(0.5f, 0.5f);
			panel_rect.anchorMax = new Vector2(0.5f, 0.5f);
			panel_rect.pivot = new Vector2(0.5f, 0.5f);
			panel_rect.sizeDelta = new Vector2(PanelWidth, PanelHeight);
			panel_rect.anchoredPosition = Vector2.zero;
			panel_go.GetComponent<Image>().color = _panel_color;

			CreateText(panel_rect, "Title", "SAVES", new Vector2(740f, 90f), new Vector2(0f, -70f), 56f, FontStyles.Bold, TextAlignmentOptions.Center);
			_slots_placeholder_tmp_text = CreateText(
				panel_rect,
				"SlotsPlaceholder",
				"Placeholder",
				new Vector2(740f, 760f),
				new Vector2(0f, -180f),
				34f,
				FontStyles.Normal,
				TextAlignmentOptions.TopLeft);
			_slots_content_root = CreateSlotsRoot();

			_load_button = CreateButton(panel_rect, "LoadButton", "LOAD", new Vector2(-210f, 90f));
			_save_button = CreateButton(panel_rect, "SaveButton", "SAVE", new Vector2(0f, 90f));
			_delete_button = CreateButton(panel_rect, "DeleteButton", "DELETE", new Vector2(210f, 90f));
			_close_button = CreateButton(panel_rect, "CloseButton", "CLOSE", new Vector2(0f, 22f));
			_selected_slot_tmp_text = CreateText(
				panel_rect,
				"SelectedSlotLabel",
				"Selected: Autosave",
				new Vector2(740f, 40f),
				new Vector2(0f, -950f),
				24f,
				FontStyles.Normal,
				TextAlignmentOptions.Center);
			_slot_item_template = CreateSlotTemplate(_slots_content_root);
		}

		private void ResetTemplateReferences()
		{
			_backdrop_button = null;
			_close_button = null;
			_load_button = null;
			_save_button = null;
			_delete_button = null;
			_slots_placeholder_tmp_text = null;
			_slots_content_root = null;
			_slot_item_template = null;
			_selected_slot_tmp_text = null;
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

		private Button CreateButton(RectTransform parent, string name, string text, Vector2 anchored_position)
		{
			GameObject button_go;
			RectTransform button_rect;
			Image image;
			Button button;
			TMP_Text label;

			button_go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
			button_go.transform.SetParent(parent, false);

			button_rect = button_go.GetComponent<RectTransform>();
			button_rect.anchorMin = new Vector2(0.5f, 0f);
			button_rect.anchorMax = new Vector2(0.5f, 0f);
			button_rect.pivot = new Vector2(0.5f, 0.5f);
			button_rect.sizeDelta = new Vector2(180f, 56f);
			button_rect.anchoredPosition = anchored_position;

			image = button_go.GetComponent<Image>();
			image.color = _button_color;

			button = button_go.GetComponent<Button>();
			button.targetGraphic = image;

			label = CreateText(button_rect, "Label", text, Vector2.zero, Vector2.zero, 26f, FontStyles.Bold, TextAlignmentOptions.Center);
			StretchRect(label.rectTransform);
			return (button);
		}

		private TMP_Text CreateText(
			RectTransform parent,
			string name,
			string text,
			Vector2 size,
			Vector2 anchored_position,
			float font_size,
			FontStyles font_style,
			TextAlignmentOptions alignment)
		{
			GameObject text_go;
			RectTransform text_rect;
			TMP_Text tmp_text;

			text_go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
			text_go.transform.SetParent(parent, false);

			text_rect = text_go.GetComponent<RectTransform>();
			text_rect.anchorMin = new Vector2(0.5f, 1f);
			text_rect.anchorMax = new Vector2(0.5f, 1f);
			text_rect.pivot = new Vector2(0.5f, 1f);
			text_rect.sizeDelta = size;
			text_rect.anchoredPosition = anchored_position;

			tmp_text = text_go.GetComponent<TextMeshProUGUI>();
			tmp_text.text = text;
			tmp_text.fontSize = font_size;
			tmp_text.fontStyle = font_style;
			tmp_text.alignment = alignment;
			tmp_text.color = (alignment == TextAlignmentOptions.Center) ? Color.white : _text_color;
			if (name == "Title")
				tmp_text.color = _title_color;
			tmp_text.raycastTarget = false;

			return (tmp_text);
		}

		private void StretchRect(RectTransform rect_transform)
		{
			rect_transform.anchorMin = Vector2.zero;
			rect_transform.anchorMax = Vector2.one;
			rect_transform.pivot = new Vector2(0.5f, 0.5f);
			rect_transform.anchoredPosition = Vector2.zero;
			rect_transform.sizeDelta = Vector2.zero;
			rect_transform.localScale = Vector3.one;
		}
	}
}
