using UnityEngine;

namespace APlusOrFail.Objects.Brick
{
    public class CustomizableBrick : MonoBehaviour, ICustomizableObject
    {
        public ObjectGridRect brickRect;

        public bool NextSetting(int option)
        {
            switch (option)
            {
                case 0:
                    RectInt rect = brickRect.gridLocalRect;
                    rect.width = (rect.width + 1) % 4 + 1;
                    brickRect.gridLocalRect = rect;
                    Vector3 scale = transform.localScale;
                    scale.x = rect.width;
                    transform.localScale = scale;
                    return true;

                case 1:
                    SpriteRenderer renderer = brickRect.GetComponent<SpriteRenderer>();
                    float h, s, v;
                    Color.RGBToHSV(renderer.color, out h, out s, out v);
                    h = Mathf.Repeat(h + 0.1f, 1);
                    renderer.color = Color.HSVToRGB(h, 1, 1);
                    return false;

                default:
                    return false;
            }
        }
    }
}
