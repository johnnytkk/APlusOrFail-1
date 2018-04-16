﻿using UnityEngine;
using UnityEngine.UI;

namespace APlusOrFail.Maps.SceneStates
{
    [RequireComponent(typeof(RectTransform))]
    public class KeyCursor : MonoBehaviour
    {
        protected RectTransform rectTransform;
        public RectTransform nameBackground;
        public Text nameText;

        private Player _player;
        public Player player {
            get
            {
                return _player;
            }
            set
            {
                if (_player != value)
                {
                    _player = value;
                    UpdateNameTag();
                }
            }
        }
        public float speed { get; set; } = 0.3f;
        
        public Vector2 viewportLocation => rectTransform != null ? rectTransform.anchorMin : Vector2.zero;


        protected virtual void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            UpdateNameTag();
        }

        protected virtual void Update()
        {
            if (player != null)
            {
                bool leftPressed = HasKeyPressed(player, Player.Action.Left);
                bool rightPressed = HasKeyPressed(player, Player.Action.Right);
                bool upPressed = HasKeyPressed(player, Player.Action.Up);
                bool downPressed = HasKeyPressed(player, Player.Action.Down);

                bool left = leftPressed && !rightPressed;
                bool right = rightPressed && !leftPressed;
                bool up = upPressed && !downPressed;
                bool down = downPressed && !upPressed;

                Vector2 currentLocation = rectTransform.anchorMin;

                if (left)
                {
                    currentLocation.x = Mathf.Max(currentLocation.x - speed * Time.deltaTime, 0);
                }
                else if (right)
                {
                    currentLocation.x = Mathf.Min(currentLocation.x + speed * Time.deltaTime, 1);
                }

                if (up)
                {
                    currentLocation.y = Mathf.Min(currentLocation.y + speed * Time.deltaTime, 1);
                }
                else if (down)
                {
                    currentLocation.y = Mathf.Max(currentLocation.y - speed * Time.deltaTime, 0);
                }

                rectTransform.anchorMin = rectTransform.anchorMax = currentLocation;
            }
        }

        protected void UpdateNameTag()
        {
            if (nameBackground != null)
            {
                nameBackground.gameObject.SetActive(player != null);
                nameText.text = player?.name ?? "";
                nameText.color = player?.color ?? Color.white;
            }
        }

        protected bool HasKeyPressed(Player player, Player.Action action)
        {
            KeyCode? key = player.GetKeyForAction(action);
            return key != null && Input.GetKey(key.Value);
        }
    }
}