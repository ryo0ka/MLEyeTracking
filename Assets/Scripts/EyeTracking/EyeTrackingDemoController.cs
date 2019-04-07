using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace EyeTracking
{
	public class EyeTrackingDemoController : MonoBehaviour
	{
		[SerializeField]
		Transform _eyePosIndicator;

		[SerializeField]
		MeshRenderer _eyeRenderer;

		[SerializeField]
		bool _useSmoothBuffer;

		[SerializeField, Range(1, 10)]
		int _smoothBufferSize;

		[SerializeField, Range(0f, 1f)]
		float _smoothBufferconfidenceThreshold;

		Camera _mainCamera;
		EyePositionBuffer _eyePositions;

		void OnValidate()
		{
			_eyePositions = new EyePositionBuffer(
				_smoothBufferSize, _smoothBufferconfidenceThreshold);
		}

		void Start()
		{
			_eyePositions = new EyePositionBuffer(
				_smoothBufferSize, _smoothBufferconfidenceThreshold);

			_mainCamera = Camera.main;

			MLEyes.Start();
		}

		void OnDisable()
		{
			MLEyes.Stop();
		}

		void LateUpdate()
		{
			if (!MLEyes.IsStarted) return;

			// Distance between camera and indicator
			const float Distance = 2f;

			var confidence = MLEyes.FixationConfidence;
			var eyePos = MLEyes.FixationPoint;

			// Force indicator to stay at a fixed distance from camera
			var camPos = _mainCamera.transform.position;
			var vec = (eyePos - camPos).normalized * Distance;
			eyePos = camPos + vec;

			if (_useSmoothBuffer)
			{
				// Smooth eye position
				eyePos = _eyePositions.Add(eyePos, confidence);
			}

			_eyePosIndicator.position = eyePos;

			// Display current confidence on indicator
			float h = confidence * .33f; // red to green
			var color = Color.HSVToRGB(h, 1f, 1f);
			_eyeRenderer.material.color = color;
		}
	}
}