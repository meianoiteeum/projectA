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
        public float angleDeg;
    }

    public class MapAnimator : MonoBehaviour
    {
        public IEnumerator AnimateRotation(List<RotationStep> plan, Vector3 pivot,
                                           float duration, Action onComplete)
        {
            if (plan == null || plan.Count == 0 || duration <= 0f)
            {
                if (plan != null)
                {
                    foreach (var step in plan)
                    {
                        if (step.line == null) continue;
                        step.line.SetEndpoint(step.endpointIndex, step.finalEndpoint);
                    }
                }
                onComplete?.Invoke();
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                foreach (var step in plan)
                {
                    if (step.line == null) continue;
                    float currentAngle = Mathf.Lerp(0f, step.angleDeg, t);
                    Quaternion rot = Quaternion.Euler(0f, 0f, currentAngle);
                    Vector3 offset = step.startEndpoint - pivot;
                    step.line.SetEndpoint(step.endpointIndex, pivot + rot * offset);
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
