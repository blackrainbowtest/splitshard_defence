using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using SHD.Core.Constants;

namespace SHD.UI.Mono
{
	public class AboutScenePresenterMono : MonoBehaviour
	{
		[Header("Version")]
		[SerializeField] private TMP_Text _version_value_tmp_text;
		[SerializeField] private string _version_prefix = "v";

		[Header("Close Controls")]
		[SerializeField] private Button _backdrop_button;

		[Header("Donaters")]
		[SerializeField] private TMP_Text _donaters_tmp_text;
		[SerializeField] private RectTransform _donaters_container;
		[SerializeField] private TMP_Text _donater_line_template;
		[SerializeField] private List<string> _donaters = new List<string>();
		[SerializeField] private float _donater_line_spacing = 56f;

		[Header("Credits Scroll")]
		[SerializeField] private RectTransform _credits_root;
		[SerializeField] private RectTransform _scroll_start_point;
		[SerializeField] private RectTransform _scroll_end_point;
		[SerializeField] private float _scroll_speed = 40f;
		private readonly List<TMP_Text> _generated_donater_lines = new List<TMP_Text>();

		private void Awake()
		{
			WireCloseControls();
			ApplyVersion();
			ApplyDonaters();
			ResetCreditsPosition();
		}

		private void Update()
		{
			UpdateCreditsScroll(Time.unscaledDeltaTime);
		}

		private void OnDestroy()
		{
			ClearGeneratedDonaterLines();
		}

		public void OnBackPressed()
		{
			TryCloseOverlayInstance();
		}

		private void WireCloseControls()
		{
			if (_backdrop_button == null)
				return;

			_backdrop_button.onClick.RemoveListener(OnBackPressed);
			_backdrop_button.onClick.AddListener(OnBackPressed);
		}

		[ContextMenu("Apply Donaters")]
		public void ApplyDonaters()
		{
			if (_donaters_container != null && _donater_line_template != null)
			{
				ApplyDonatersFromTemplate();
				return;
			}

			ApplyDonatersAsSingleText();
		}

		[ContextMenu("Reset Credits Position")]
		public void ResetCreditsPosition()
		{
			if (_credits_root == null || _scroll_start_point == null)
				return;

			_credits_root.anchoredPosition = new Vector2(_credits_root.anchoredPosition.x, _scroll_start_point.anchoredPosition.y);
		}

		private void ApplyVersion()
		{
			if (_version_value_tmp_text == null)
				return;

			_version_value_tmp_text.text = _version_prefix + GameInfo.GameVersion;
		}

		private void ApplyDonatersAsSingleText()
		{
			if (_donaters_tmp_text == null)
				return;

			_donaters_tmp_text.text = string.Join("\n", _donaters);
		}

		private void ApplyDonatersFromTemplate()
		{
			TMP_Text line_instance;
			RectTransform instance_rect_transform;
			RectTransform template_rect_transform;
			Vector2 template_position;
			int i;

			ClearGeneratedDonaterLines();
			_donater_line_template.gameObject.SetActive(false);
			template_rect_transform = _donater_line_template.rectTransform;
			template_position = template_rect_transform.anchoredPosition;

			for (i = 0; i < _donaters.Count; i++)
			{
				line_instance = Instantiate(_donater_line_template, _donaters_container, false);
				line_instance.gameObject.name = "DonaterLine_" + i.ToString();
				line_instance.text = _donaters[i];
				instance_rect_transform = line_instance.rectTransform;
				instance_rect_transform.anchoredPosition = new Vector2(template_position.x, template_position.y - (_donater_line_spacing * i));
				line_instance.gameObject.SetActive(true);
				_generated_donater_lines.Add(line_instance);
			}
		}

		private void ClearGeneratedDonaterLines()
		{
			int i;

			for (i = 0; i < _generated_donater_lines.Count; i++)
			{
				if (_generated_donater_lines[i] != null)
					Destroy(_generated_donater_lines[i].gameObject);
			}

			_generated_donater_lines.Clear();
		}

		private void UpdateCreditsScroll(float delta_time)
		{
			Vector2 anchored_position;
			float start_y;
			float end_y;
			float next_y;

			if (_credits_root == null || _scroll_start_point == null || _scroll_end_point == null)
				return;
			if (_scroll_speed <= 0f)
				return;

			start_y = _scroll_start_point.anchoredPosition.y;
			end_y = _scroll_end_point.anchoredPosition.y;
			if (end_y <= start_y)
				return;

			anchored_position = _credits_root.anchoredPosition;
			next_y = anchored_position.y + (_scroll_speed * Mathf.Max(0f, delta_time));
			if (next_y >= end_y)
				next_y = start_y;

			_credits_root.anchoredPosition = new Vector2(anchored_position.x, next_y);
		}

		private bool TryCloseOverlayInstance()
		{
			Transform current;

			current = transform;
			while (current != null && current.parent != null)
			{
				if (current.parent.name == "OverlayRoot")
				{
					Destroy(current.gameObject);
					return (true);
				}

				current = current.parent;
			}

			return (false);
		}
	}
}
