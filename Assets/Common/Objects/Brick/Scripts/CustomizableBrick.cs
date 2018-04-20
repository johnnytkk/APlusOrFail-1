using UnityEngine;

namespace APlusOrFail.Objects.Brick
{
    [ExecuteInEditMode]
    public class CustomizableBrick : MonoBehaviour, ICustomizableObject
    {
        [SerializeField] private int _minWidth = 1;
        public int minWidth { get { return _minWidth; } set { SetProperty(ref _minWidth, value); } }

        [SerializeField] private int _maxWidth = 4;
        public int maxWidth { get { return _maxWidth; } set { SetProperty(ref _maxWidth, value); } }

        [SerializeField] private int _width = 1;
        public int width { get { return _width; } set { SetProperty(ref _width, value); } }

        [SerializeField] private Color _color = Color.gray;
        public Color color { get { return _color; } set { SetProperty(ref _color, value); } }


        public ObjectGridRect brickRect;
        public SpriteRenderer brickRenderer;


        private void OnEnable()
        {
            UpdateProperties();
        }

        private void OnValidate()
        {
            UpdateProperties();
        }

        private void SetProperty<T>(ref T property, T value)
        {
            if (!property.Equals(value))
            {
                property = value;
                UpdateProperties();
            }
        }

        private void UpdateProperties()
        {
            _minWidth = Mathf.Clamp(_minWidth, 1, _maxWidth);
            _maxWidth = Mathf.Max(_maxWidth, _minWidth);
            _width = Mathf.Clamp(_width, _minWidth, _maxWidth);

            if (enabled)
            {
                RectInt rect = brickRect.gridLocalRect;
                rect.width = _width;
                brickRect.gridLocalRect = rect;
                Vector3 scale = brickRect.transform.localScale;
                scale.x = rect.width;
                brickRect.transform.localScale = scale;

                brickRenderer.color = _color;
            }
        }

        bool ICustomizableObject.NextSetting(int option)
        {
            switch (option)
            {
                case 0:
                    width = ((width - minWidth + 1) % (maxWidth + 1 - minWidth)) + minWidth;
                    return true;

                case 1:
                    float h, s, v;
                    Color.RGBToHSV(color, out h, out s, out v);
                    h = Mathf.Repeat(h + 0.1f, 1);
                    color = Color.HSVToRGB(h, s, v);
                    return false;

                default:
                    return false;
            }
        }
    }
}
