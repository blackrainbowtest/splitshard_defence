using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SHD.UI.Mono
{
	public class SavesSlotItemMono : MonoBehaviour
	{
		[SerializeField] private Button _button;
		[SerializeField] private TMP_Text _slot_name_tmp_text;
		[SerializeField] private TMP_Text _slot_details_tmp_text;
		[SerializeField] private Image _background_image;
		[SerializeField] private Color _normal_color = new Color(0.17f, 0.17f, 0.17f, 0.95f);
		[SerializeField] private Color _selected_color = new Color(0.28f, 0.34f, 0.42f, 0.98f);

		private int _slot_id;
		private System.Action<int> _on_selected;

		public int SlotId
		{
			get
			{
				return (_slot_id);
			}
		}

		public void Initialize(int slot_id, string slot_name, string details, System.Action<int> on_selected)
		{
			_slot_id = slot_id;
			_on_selected = on_selected;

			if (_slot_name_tmp_text != null)
				_slot_name_tmp_text.text = slot_name;
			if (_slot_details_tmp_text != null)
				_slot_details_tmp_text.text = details;

			if (_button != null)
			{
				_button.onClick.RemoveAllListeners();
				_button.onClick.AddListener(HandleClicked);
			}

			SetSelected(false);
		}

		public void SetReferences(Button button, TMP_Text slot_name_tmp_text, TMP_Text slot_details_tmp_text, Image background_image)
		{
			_button = button;
			_slot_name_tmp_text = slot_name_tmp_text;
			_slot_details_tmp_text = slot_details_tmp_text;
			_background_image = background_image;
		}

		public void SetSelected(bool selected)
		{
			if (_background_image == null)
				return;

			_background_image.color = selected ? _selected_color : _normal_color;
		}

		private void HandleClicked()
		{
			if (_on_selected != null)
				_on_selected.Invoke(_slot_id);
		}
	}
}
