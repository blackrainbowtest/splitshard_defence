using UnityEngine;
using UnityEngine.UI;

namespace SHD.UI.Mono
{
	public class LogoScenePresenterMono : MonoBehaviour
	{
		public enum ELogoAnimationMode
		{
			FadeIn = 0,
			Pulse = 1
		}

		[SerializeField] private Image _logo_image;
		[SerializeField] private ELogoAnimationMode _animation_mode = ELogoAnimationMode.FadeIn;
		[SerializeField] private float _fade_in_duration = 0.6f;
		[SerializeField] private float _pulse_speed = 1.4f;
		[SerializeField] private float _pulse_min_alpha = 0.45f;
		[SerializeField] private float _pulse_max_alpha = 1.0f;

		private float _start_time;

		private void Awake()
		{
			_start_time = Time.unscaledTime;
			ApplyAlpha(0f);
		}

		private void Update()
		{
			if (_logo_image == null)
				return ;

			if (_animation_mode == ELogoAnimationMode.FadeIn)
				UpdateFadeIn();
			else
				UpdatePulse();
		}

		private void UpdateFadeIn()
		{
			float elapsed;
			float duration;
			float alpha;

			duration = Mathf.Max(0.01f, _fade_in_duration);
			elapsed = Time.unscaledTime - _start_time;
			alpha = Mathf.Clamp01(elapsed / duration);

			ApplyAlpha(alpha);
		}

		private void UpdatePulse()
		{
			float t;
			float alpha;
			float min_alpha;
			float max_alpha;

			min_alpha = Mathf.Clamp01(_pulse_min_alpha);
			max_alpha = Mathf.Clamp01(_pulse_max_alpha);

			if (max_alpha < min_alpha)
			{
				float temp;
				temp = min_alpha;
				min_alpha = max_alpha;
				max_alpha = temp;
			}

			t = Mathf.Sin(Time.unscaledTime * Mathf.Max(0.01f, _pulse_speed));
			t = (t + 1f) * 0.5f;
			alpha = Mathf.Lerp(min_alpha, max_alpha, t);

			ApplyAlpha(alpha);
		}

		private void ApplyAlpha(float alpha)
		{
			Color color;

			if (_logo_image == null)
				return ;

			color = _logo_image.color;
			color.a = Mathf.Clamp01(alpha);
			_logo_image.color = color;
		}
	}
}
