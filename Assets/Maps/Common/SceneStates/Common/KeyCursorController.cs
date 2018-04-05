using System.Collections.Generic;
using UnityEngine;

namespace APlusOrFail.Maps.SceneStates.KeyCursorController
{
    public class KeyCursorController : MonoBehaviour
    {
        public class KeyCursor
        {
            private readonly KeyCursorController keyCursorController;
            public readonly Player player;
            private KeyCursorStructure cursorStructure;
            private RectTransform cursorRectTransform;

            public KeyCursor(KeyCursorController keyCursorController, Player player)
            {
                this.keyCursorController = keyCursorController;
                this.player = player;
                cursorStructure = Instantiate(keyCursorController.cursorPrefab, keyCursorController.canvas.transform);
                cursorRectTransform = cursorStructure.GetComponent<RectTransform>();

                name = player.name;
                color = player.color;
                player.onNameChanged += OnPlayerNameChanged;
                player.onColorChanged += OnPlayerColorChanged;
            }

            public Vector2 location
            {
                get
                {
                    return keyCursorController.camera.ViewportToWorldPoint(cursorRectTransform.anchorMin);
                }
            }

            public string name
            {
                get
                {
                    return cursorStructure.nameText.text;
                }
                private set
                {
                    value = value?.Trim() ?? "";
                    cursorStructure.nameBackground.gameObject.SetActive(value.Length > 0);
                    cursorStructure.nameText.text = value ?? "";
                }
            }

            public Color color
            {
                get
                {
                    return cursorStructure.nameText.color;
                }
                private set
                {
                    cursorStructure.nameText.color = value;
                }
            }

            private void OnPlayerNameChanged(Player player, string name)
            {
                this.name = name;
            }

            private void OnPlayerColorChanged(Player player, Color color)
            {
                this.color = color;
            }

            public void Update()
            {
                bool leftPressed = HasKeyPressed(player, Player.Action.Left);
                bool rightPressed = HasKeyPressed(player, Player.Action.Right);
                bool upPressed = HasKeyPressed(player, Player.Action.Up);
                bool downPressed = HasKeyPressed(player, Player.Action.Down);

                bool left = leftPressed && !rightPressed;
                bool right = rightPressed && !leftPressed;
                bool up = upPressed && !downPressed;
                bool down = downPressed && !upPressed;

                Vector2 currentLocation = cursorRectTransform.anchorMin;

                if (left)
                {
                    currentLocation.x = Mathf.Max(currentLocation.x - keyCursorController.horizontalSpeed * Time.deltaTime, 0);
                }
                else if (right)
                {
                    currentLocation.x = Mathf.Min(currentLocation.x + keyCursorController.horizontalSpeed * Time.deltaTime, 1);
                }

                if (up)
                {
                    currentLocation.y = Mathf.Min(currentLocation.y + keyCursorController.verticalSpeed * Time.deltaTime, 1);
                }
                else if (down)
                {
                    currentLocation.y = Mathf.Max(currentLocation.y - keyCursorController.verticalSpeed * Time.deltaTime, 0);
                }

                cursorRectTransform.anchorMin = cursorRectTransform.anchorMax = currentLocation;
            }

            public void Remove()
            {
                player.onNameChanged -= OnPlayerNameChanged;
                player.onColorChanged -= OnPlayerColorChanged;
                keyCursorController.RemoveKeyCursorInternal(this);
                Destroy(cursorRectTransform.gameObject);
            }

            private bool HasKeyPressed(Player player, Player.Action action)
            {
                KeyCode? key = player.GetKeyForAction(action);
                return key != null && Input.GetKey(key.Value);
            }
        }

        public RectTransform canvas;
        public KeyCursorStructure cursorPrefab;
        public float horizontalSpeed = 0.3f;
        public float verticalSpeed = 0.3f;

        private new Camera camera;
        private readonly List<KeyCursor> keyCursors = new List<KeyCursor>();
        private KeyCursor[] keyCursorsForUpdate;
        private bool keyCursorsModified;

        private void Start()
        {
            camera = Camera.main;
        }

        public KeyCursor AddKeyCursor(Player player)
        {
            KeyCursor keyCursor = new KeyCursor(this, player);
            keyCursors.Add(keyCursor);
            keyCursorsModified = true;
            return keyCursor;
        }

        public void RemoveKeyCursorInternal(KeyCursor keyCursor)
        {
            if (keyCursors.Remove(keyCursor))
            {
                keyCursorsModified = true;
            }
        }
        
        private void Update()
        {
            if (keyCursorsModified)
            {
                keyCursorsForUpdate = keyCursors.Count > 0 ? keyCursors.ToArray() : null;
                keyCursorsModified = false;
            }
            if (keyCursorsForUpdate != null)
            {
                foreach (KeyCursor keyCursor in keyCursorsForUpdate)
                {
                    keyCursor.Update();
                }
            }

        }
    }
}
