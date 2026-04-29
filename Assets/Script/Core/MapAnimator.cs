using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Script.Core
{
    public class RotationStep
    {
        public MapLine line;
        public int endpointIndex;
        public Vector3 startEndpoint;
        public Vector3 finalEndpoint;
    }

    public class MapAnimator : MonoBehaviour
    {
        public IEnumerator AnimateRotation(List<RotationStep> plan, Vector3 pivot,
                                           float totalAngleDeg, float duration,
                                           Action onComplete)
        {
            if (plan == null || plan.Count == 0 || duration <= 0f)
            {
                onComplete?.Invoke();
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float currentAngle = Mathf.Lerp(0f, totalAngleDeg, t);
                Quaternion rot = Quaternion.Euler(0f, 0f, currentAngle);

                foreach (var step in plan)
                {
                    if (step.line == null) continue;
                    Vector3 offset = step.startEndpoint - pivot;
                    Vector3 rotated = rot * offset;
                    step.line.SetEndpoint(step.endpointIndex, pivot + rotated);
                }
                yield return null;
            }

            foreach (var step in plan)
            {
                if (step.line == null) continue;
                step.line.SetEndpoint(step.endpointIndex, step.finalEndpoint);
            }

            onComplete?.Invoke();
        }
    }
}
