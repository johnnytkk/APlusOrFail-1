using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace APlusOrFail.Components.NameTag
{
    using Character;

    [RequireComponent(typeof(RectTransform))]
    public class NameTag : MonoBehaviour
    {
        [NonSerialized] public new Camera camera;
        [NonSerialized] public RectTransform canvasRectTransform;
        private RectTransform rectTransform;
        public Text nameText;
        public Vector2 worldOffset = Vector2.zero;

        private CharacterPlayer _charPlayer;
        public CharacterPlayer charPlayer
        {
            get
            {
                return _charPlayer;
            }
            set
            {
                if (!ReferenceEquals(_charPlayer, value))
                {
                    _charPlayer = value;
                    UpdateProperties();
                }
            }
        }


        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            UpdateProperties();
        }

        private void UpdateProperties()
        {
            gameObject.SetActive(charPlayer != null);
            if (charPlayer != null)
            {
                nameText.text = charPlayer.player.name;
                nameText.color = charPlayer.player.color;
            }
        }

        public void LateUpdate()
        {
            Vector2 screenPoint = camera.WorldToScreenPoint(charPlayer.transform.position + ((Vector3)worldOffset));
            Vector2 canvasPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPoint, null, out canvasPoint);
            Rect rect = canvasRectTransform.rect;
            Vector2 anchorMinAndMax = new Vector2(canvasPoint.x / rect.width + 0.5f, canvasPoint.y / rect.height + 0.5f);
            rectTransform.anchorMin = rectTransform.anchorMax = anchorMinAndMax;
        }
    }
}
