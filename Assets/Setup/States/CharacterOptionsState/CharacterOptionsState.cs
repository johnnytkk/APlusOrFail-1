using UnityEngine;
using UnityEngine.UI;

namespace APlusOrFail.Setup.States.CharacterOptionState
{
    using Character;
    using PlayerNameAndColorSetupState;
    using CharacterSelectionState;
    using PlayerActionKeySetupState;

    public class CharacterOptionsState : SceneState
    {
        public RectTransform uiScene;
        public Button changeNameColorButton;
        public Button chooseCharacterButton;
        public Button remapActionKeyButton;
        public Button closeButton;
        public Button deletePlayerButton;

        public PlayerNameAndColorSetupState nameColorSetupUIScene;
        public CharacterSelectionState charSelectionUIScene;
        public PlayerActionKeySetupState actionKeySetupUIScene;

        public GameObject character { get; set; }

        private CharacterSelectionState activeCharSelectionUIScene;

        private void Start()
        {
            changeNameColorButton.onClick.AddListener(OnChangeNameColorButtonClicked);
            chooseCharacterButton.onClick.AddListener(OnChooseCharacterButtonClicked);
            remapActionKeyButton.onClick.AddListener(OnRemapActionKeyButtonClicked);
            closeButton.onClick.AddListener(OnCloseButtonClicked);
            deletePlayerButton.onClick.AddListener(OnDeletePlayerButtonClicked);

            HideUI();
        }

        protected override void OnActivate()
        {
            ShowUI();
            if (activeCharSelectionUIScene != null)
            {
                if (activeCharSelectionUIScene.selectedCharacter != character)
                {
                    character = activeCharSelectionUIScene.selectedCharacter;
                }
                activeCharSelectionUIScene = null;
            }
        }

        protected override void OnDeactivate()
        {
            HideUI();
        }

        private void OnChangeNameColorButtonClicked()
        {
            if (state.IsAtLeast(State.Activated))
            {
                SceneStateManager.instance.PushSceneState(nameColorSetupUIScene);
                nameColorSetupUIScene.character = character;
            }
        }

        private void OnChooseCharacterButtonClicked()
        {
            if (state.IsAtLeast(State.Activated))
            {
                SceneStateManager.instance.PushSceneState(charSelectionUIScene);
                activeCharSelectionUIScene = charSelectionUIScene;
                activeCharSelectionUIScene.originalCharacter = character;
            }
        }

        private void OnRemapActionKeyButtonClicked()
        {
            if (state.IsAtLeast(State.Activated))
            {
                SceneStateManager.instance.PushSceneState(actionKeySetupUIScene);
                actionKeySetupUIScene.character = character;
            }
        }

        private void OnCloseButtonClicked()
        {
            if (state.IsAtLeast(State.Activated))
            {
                SceneStateManager.instance.PopSceneState();
            }
        }

        private void OnDeletePlayerButtonClicked()
        {
            if (state.IsAtLeast(State.Activated))
            {
                CharacterPlayer charPlayer = character.GetComponent<CharacterPlayer>();
                charPlayer.player.Delete();
                charPlayer.player = null;
                SceneStateManager.instance.PopSceneState();
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
