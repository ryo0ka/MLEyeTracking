using System.Collections.Generic;
using UnityEngine;

namespace EyeTracking
{
	public class EyePositionBuffer
	{
		readonly int _size;
		readonly float _confidenceThreshold;
		readonly Queue<(Vector3, float)> _buffer;

		public EyePositionBuffer(int size, float confidenceThreshold)
		{
			_size = size;
			_confidenceThreshold = confidenceThreshold;
			_buffer = new Queue<(Vector3, float)>();
		}

		public Vector3 Add(Vector3 newVector, float newConfidence)
		{
			if (newConfidence < _confidenceThreshold)
			{
				return CalcAverage();
			}
			
			_buffer.Enqueue((newVector, newConfidence));

			while (_buffer.Count > _size)
			{
				_buffer.Dequeue();
			}

			return CalcAverage();
		}

		Vector3 CalcAverage()
		{
			Vector3 top = Vector3.zero;
			float bottom = 0f;
			foreach (var (vec, confidence) in _buffer)
			{
				top += vec * confidence;
				bottom += confidence;
			}

			if (bottom == 0f)
			{
				return Vector3.up;
			}

			return top / bottom;
		}
	}
}