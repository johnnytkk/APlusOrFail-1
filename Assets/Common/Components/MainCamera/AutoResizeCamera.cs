using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace APlusOrFail.Components.AutoResizeCamera
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class AutoResizeCamera : MonoBehaviour
    {
        [SerializeField] private Rect _defaultInnerArea;
        public Rect defaultInnerArea { get { return _defaultInnerArea; } set { SetProperty(ref _defaultInnerArea, value); } }

        [SerializeField] private Vector2 _minInnerSize;
        public Vector2 innerSize { get { return _minInnerSize; } set { SetProperty(ref _minInnerSize, value); } }

        [SerializeField] private RectOffset _padding;
        public RectOffset padding { get { return _padding; } set { SetProperty(ref _padding, value); } }

        [Range(0, float.MaxValue)]
        [SerializeField] private float _lerpScale = 1;
        public float lerpScale { get { return _lerpScale; } set { SetProperty(ref _lerpScale, value); } }


        private new Camera camera;
        private readonly Dictionary<Transform, Rect> charTransforms = new Dictionary<Transform, Rect>();


        private void SetProperty<T>(ref T property, T value)
        {
            if (!property.Equals(value))
            {
                property = value;
            }
        }

        private void Awake()
        {
            camera = GetComponent<Camera>();
        }

        public void AddTracingSprite(GameObject gameObject, Rect bound = new Rect())
        {
            charTransforms.Add(gameObject.transform, bound);
        }

        public void RemoveTracingSprite(GameObject gameObject)
        {
            charTransforms.Remove(gameObject.transform);
        }

        public void UntraceAll()
        {
            charTransforms.Clear();
        }

        private void LateUpdate()
        {
            Rect area = charTransforms
                .Select(p => new Rect((Vector2)p.Key.position + p.Value.position, p.Value.size))
                .DefaultIfEmpty(defaultInnerArea)
                .Aggregate((ia, b) => new Rect
                {
                    xMin = Mathf.Min(ia.xMin, b.xMin),
                    xMax = Mathf.Max(ia.xMax, b.xMax),
                    yMin = Mathf.Min(ia.yMin, b.yMin),
                    yMax = Mathf.Max(ia.yMax, b.xMax)
                });
            Vector2 expand = new Vector2(Mathf.Max(innerSize.x - area.width, 0), Mathf.Max(innerSize.y - area.height, 0));
            area = new Rect(area.position - expand / 2, area.size + expand);
            area = new Rect(area.position - new Vector2(padding.left, padding.bottom), area.size + new Vector2(padding.horizontal, padding.vertical));
                
            camera.transform.position = Vector3.Lerp(
                camera.transform.position,
                (Vector3)area.center + Vector3.forward * camera.transform.position.z,
                Time.deltaTime * lerpScale
            );


            float areaAspect = area.width / area.height;
            float cameraAspect = camera.aspect;
            float cameraSize;
            if (areaAspect >= cameraAspect)
            {
                cameraSize = (area.width / cameraAspect) / 2;
            }
            else
            {
                cameraSize = area.height / 2;
            }
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, cameraSize, Time.deltaTime * lerpScale);
        }
    }
}
