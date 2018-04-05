using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.UI;

namespace APlusOrFail.Setup.States.PlayerActionKeySetupState
{
    using Character;

    public class PlayerActionKeySetupState : SceneState
    {
        private static readonly ReadOnlyCollection<Player.Action> actionSequence = new ReadOnlyCollection<Player.Action>(new Player.Action[]{
            Player.Action.Left,
            Player.Action.Right,
            Player.Action.Up,
            Player.Action.Down,
            Player.Action.Select,
            Player.Action.Cancel
        });

        private static string TextForAction(Player.Action action)
        {
            switch (action)
            {
                case Player.Action.Left: return "left";
                case Player.Action.Right: return "right";
                case Player.Action.Up: return "jump";
                case Player.Action.Down: return "squat";
                case Player.Action.Select: return "select";
                case Player.Action.Cancel: return "cancel";
                default: return "";
            }
        }


        public RectTransform uiScene;
        public Text enterKeyMessageText;
        public Button cancelButton;

        public GameObject character { get; set; }
        public bool cancelled { get; private set; }

        private CharacterPlayer charPlayer;
        private Dictionary<Player.Action, KeyCode> actionKeyMap;
        private int setupingActionIndex = 0;


        private void Start()
        {
            cancelButton.onClick.AddListener(OnCancelButtonClicked);
            HideUI();
        }

        protected override void OnLoad()
        {
            charPlayer = character.GetComponent<CharacterPlayer>();
            actionKeyMap = new Dictionary<Player.Action, KeyCode>();
            setupingActionIndex = 0;

            cancelled = false;
        }

        protected override void OnActivate()
        {
            ShowUI();
            enterKeyMessageText.text = $"Key for {TextForAction(actionSequence[setupingActionIndex])}";
        }

        protected override void OnDeactivate()
        {
            HideUI();
        }

        protected override void OnUnLoad()
        {
            charPlayer = null;
            actionKeyMap = null;
        }

        private void Update()
        {
            if (state.IsAtLeast(State.Activated))
            {
                KeyCode? key = KeyDetector.GetKeyDowned();
                if (key != null)
                {
                    OnKeyDown(key.Value);
                }
            }
        }

        private void OnKeyDown(KeyCode key)
        {
            if (PlayerInputRegistry.HasRegisteredByOther(key, charPlayer.player))
            {
                enterKeyMessageText.text = "The key has already used by other player!";
            }
            else if (actionKeyMap.ContainsValue(key))
            {
                enterKeyMessageText.text = "The key is used for other action!";
            }
            else
            {
                actionKeyMap[actionSequence[setupingActionIndex]] = key;
                ++setupingActionIndex;
                if (setupingActionIndex < actionSequence.Count)
                {
                    enterKeyMessageText.text = $"Key for {TextForAction(actionSequence[setupingActionIndex])}";
                }
                else
                {
                    charPlayer.player.UnmapAllActionFromKey();
                    foreach (KeyValuePair<Player.Action, KeyCode> pair in actionKeyMap)
                    {
                        charPlayer.player.MapActionToKey(pair.Key, pair.Value);
                    }
                    SceneStateManager.instance.PopSceneState();
                }
            }
        }

        private void OnCancelButtonClicked()
        {
            if (state.IsAtLeast(State.Activated))
            {
                SceneStateManager.instance.PopSceneState();
                cancelled = true;
            }
        }

        private void ShowUI()
        {
            uiScene.gameObject.SetActive(true);
        }

        private void HideUI()
        {
            uiScene.gameObject.SetActive(false);
        }
    }
}
