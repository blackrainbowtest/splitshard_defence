using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using SHD.Loading.Mono;
using SHD.Loading.Domain;
using SHD.Loading.Orchestration;
using TMPro;
using DG.Tweening;

namespace SHD.UI.Mono
{
	public class LoadingSceneFlowMono : MonoBehaviour
	{
		[SerializeField] private Slider _progress_slider;
		[SerializeField] private TMP_Text _progress_tmp_text;
		[SerializeField] private TMP_Text _tap_to_continue_tmp_text;

		private LoadingMonoBridge _loading_bridge;
		private bool _is_ready;
		private bool _has_preload_started;
		private bool _is_transitioning;
		private Tween _tap_pulse_tween;
		private CanvasGroup _tap_to_continue_canvas_group;
		private const float TransitionDelaySeconds = 0.4f;

		private async void Start()
		{
			LoadSceneResult result;
			string target_scene;

			_loading_bridge = Object.FindAnyObjectByType<LoadingMonoBridge>();
			if (_loading_bridge == null)
			{
				Debug.LogError("LoadingSceneFlowMono: LoadingMonoBridge not found.");
				return ;
			}

			target_scene = StartupFlowRuntimeState.PendingTargetScene;
			if (string.IsNullOrWhiteSpace(target_scene) == true)
			{
				Debug.LogError("LoadingSceneFlowMono: pending target scene is empty.");
				return ;
			}

			SetTapToContinueVisible(false);
			await WaitUntilLoaderIsIdleAsync();

			_has_preload_started = true;
			_loading_bridge.LoadingController.ProgressChanged += HandleProgressChanged;

			result = await _loading_bridge.PreloadSceneByNameAsync(target_scene);
			if (result.Succeeded == false)
			{
				Debug.LogError("LoadingSceneFlowMono: preload failed: " + result.ErrorMessage);
				_loading_bridge.LoadingController.ProgressChanged -= HandleProgressChanged;
				_has_preload_started = false;
				return ;
			}

			_is_ready = true;
			if (StartupFlowRuntimeState.WaitForTapToContinue == true)
			{
				SetTapToContinueVisible(true);
			}
			else
			{
				ActivateTargetScene();
			}
		}

		private void Update()
		{
			if (_is_ready == false)
				return ;
			if (_is_transitioning == true)
				return ;

			if (StartupFlowRuntimeState.WaitForTapToContinue == false)
				return ;

			if (WasTapOrClickPressedThisFrame() == false)
				return ;

			ActivateTargetSceneWithDelay();
		}

		private void OnDestroy()
		{
			if (_loading_bridge != null && _has_preload_started == true)
				_loading_bridge.LoadingController.ProgressChanged -= HandleProgressChanged;

			StopTapPulseAnimation();
		}

		private async System.Threading.Tasks.Task WaitUntilLoaderIsIdleAsync()
		{
			if (_loading_bridge == null || _loading_bridge.LoadingController == null)
				return ;

			while (_loading_bridge.LoadingController.IsLoading == true)
				await System.Threading.Tasks.Task.Yield();
		}

		private void HandleProgressChanged(LoadSceneProgress progress)
		{
			float normalized;
			string percent;

			normalized = Mathf.Clamp01(progress.NormalizedProgress);

			if (_progress_slider != null)
				_progress_slider.value = normalized;

			percent = Mathf.RoundToInt(normalized * 100f).ToString() + "%";

			if (_progress_tmp_text != null)
				_progress_tmp_text.text = percent;
		}

		private void ActivateTargetScene()
		{
			_is_ready = false;
			_is_transitioning = false;
			_has_preload_started = false;

			SetTapToContinueVisible(false);

			if (_loading_bridge != null && _loading_bridge.LoadingController != null)
				_loading_bridge.LoadingController.ProgressChanged -= HandleProgressChanged;

			StartupFlowRuntimeState.Clear();
			_loading_bridge.ActivatePreloadedScene();
		}

		private async void ActivateTargetSceneWithDelay()
		{
			if (_is_transitioning == true)
				return;

			_is_transitioning = true;
			SetTapToContinueVisible(false);

			await System.Threading.Tasks.Task.Delay((int)(TransitionDelaySeconds * 1000f));
			ActivateTargetScene();
		}

		private void SetTapToContinueVisible(bool visible)
		{
			if (_tap_to_continue_tmp_text != null)
				_tap_to_continue_tmp_text.gameObject.SetActive(visible);

			if (visible == true)
				StartTapPulseAnimation();
			else
				StopTapPulseAnimation();
		}

		private void StartTapPulseAnimation()
		{
			if (_tap_to_continue_tmp_text == null)
				return;

			if (_tap_to_continue_canvas_group == null)
			{
				_tap_to_continue_canvas_group = _tap_to_continue_tmp_text.GetComponent<CanvasGroup>();
				if (_tap_to_continue_canvas_group == null)
					_tap_to_continue_canvas_group = _tap_to_continue_tmp_text.gameObject.AddComponent<CanvasGroup>();
			}

			StopTapPulseAnimation();
			_tap_to_continue_canvas_group.alpha = 1f;
			_tap_pulse_tween = _tap_to_continue_canvas_group
				.DOFade(0.45f, 0.9f)
				.SetEase(Ease.InOutSine)
				.SetLoops(-1, LoopType.Yoyo);
		}

		private void StopTapPulseAnimation()
		{
			if (_tap_pulse_tween != null && _tap_pulse_tween.IsActive() == true)
				_tap_pulse_tween.Kill();

			_tap_pulse_tween = null;

			if (_tap_to_continue_canvas_group != null)
				_tap_to_continue_canvas_group.alpha = 1f;
		}

		private bool WasTapOrClickPressedThisFrame()
		{
			if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame == true)
				return (true);

			if (Touchscreen.current != null)
			{
				if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame == true)
					return (true);
			}

			return (false);
		}
	}
}
