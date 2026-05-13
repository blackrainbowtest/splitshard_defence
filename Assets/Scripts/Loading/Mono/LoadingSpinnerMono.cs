using UnityEngine;

namespace SHD.Loading.Mono
{
	public class LoadingSpinnerMono : MonoBehaviour
	{
		[SerializeField]
		private float _rotation_speed = 180.0f;

		private RectTransform _rect_transform;

		private void Awake()
		{
			_rect_transform = GetComponent<RectTransform>();
		}

		private void Update()
		{
			_rect_transform.Rotate(0.0f, 0.0f, -_rotation_speed * Time.deltaTime);
		}
	}
}