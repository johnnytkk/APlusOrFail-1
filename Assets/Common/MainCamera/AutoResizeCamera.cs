using UnityEngine;
using System.Collections.Generic;

namespace APlusOrFail.MainCamera
{
    [RequireComponent(typeof(Camera))]
    public class AutoResizeCamera : MonoBehaviour
    {
        [Range(0, float.MaxValue)]
        public float areaPaddingTop = 2;
        [Range(0, float.MaxValue)]
        public float areaPaddingBottom = 2;
        [Range(0, float.MaxValue)]
        public float areaPaddingLeft = 2;
        [Range(0, float.MaxValue)]
        public float areaPaddingRight = 2;

        [Range(0, float.MaxValue)]
        public float minAreaWidth = 5;
        [Range(0, float.MaxValue)]
        public float minAreaHeight = 5;

        [Range(0, float.MaxValue)]
        public float lerpScale = 1;

        private new Camera camera;
        private readonly HashSet<Transform> charTransforms = new HashSet<Transform>();

        private void Start()
        {
            camera = GetComponent<Camera>();
        }

        public void AddTracingSprite(GameObject gameObject)
        {
            charTransforms.Add(gameObject.GetComponent<Transform>());
        }

        public void RemoveTracingSprite(GameObject gameObject)
        {
            charTransforms.Remove(gameObject.GetComponent<Transform>());
        }

        private void LateUpdate()
        {
            if (charTransforms.Count > 0)
            {
                float xMin = float.MaxValue;
                float xMax = float.MinValue;
                float yMin = float.MaxValue;
                float yMax = float.MinValue;
                foreach (Transform charTransform in charTransforms)
                {
                    xMin = Mathf.Min(xMin, charTransform.position.x - 0.5f);
                    xMax = Mathf.Max(xMax, charTransform.position.x + 0.5f);
                    yMin = Mathf.Min(yMin, charTransform.position.y - 1);
                    yMax = Mathf.Max(yMax, charTransform.position.y + 1);
                }

                xMin -= areaPaddingLeft;
                xMax += areaPaddingRight;
                yMin -= areaPaddingBottom;
                yMax += areaPaddingTop;

                float areaWidth = xMax - xMin;
                float areaHeight = yMax - yMin;

                float areaHalfWidthDiff = Mathf.Max(minAreaWidth - areaWidth, 0) / 2;
                float areaHalfHeightDiff = Mathf.Max(minAreaHeight - areaHeight, 0) / 2;

                xMin -= areaHalfWidthDiff;
                xMax += areaHalfWidthDiff;
                yMin -= areaHalfHeightDiff;
                yMax += areaHalfHeightDiff;

                camera.transform.position = Vector3.Lerp(
                    camera.transform.position,
                    new Vector3((xMin + xMax) / 2, (yMin + yMax) / 2, camera.transform.position.z),
                    Time.deltaTime * lerpScale
                );

                float areaAspect = areaWidth / areaHeight;
                float cameraAspect = camera.aspect;

                float cameraSize;

                if (areaAspect >= cameraAspect)
                {
                    cameraSize = (areaWidth / cameraAspect) / 2;
                }
                else
                {
                    cameraSize = areaHeight / 2;
                }

                camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, cameraSize, Time.deltaTime * lerpScale);
            }
        }
    }
}
