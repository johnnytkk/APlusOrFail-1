using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace APlusOrFail.Setup.States.PlayerNameAndColorSetupState
{
    using Character;

    public class PlayerNameAndColorSetupState : SceneStateBehavior<object, object>
    {
        public RectTransform uiScene;
        public InputField nameInputField;
        public List<ColorButton> colorButtons;
        public Button enterButton;
        public Button cancelButton;
        
        public GameObject character { get; set; }
        public bool cancelled { get; private set; }

        private Player player;
        private Color color;
        
        private void Start()
        {
            enterButton.onClick.AddListener(OnEnterButtonClicked);
            cancelButton.onClick.AddListener(OnCancelButtonClicked);
            foreach (ColorButton button in colorButtons)
            {
                button.onSelected += OnColorButtonSelected;
            }
            HideUI();
        }

        protected override void OnLoad(object arg)
        {
            cancelled = false;
            player = character.GetComponent<CharacterPlayer>().player;
            color = player.color;
            nameInputField.text = player.name;
        }

        protected override void OnActivate(ISceneState unloadedSceneState, object result)
        {
            ShowUI();
        }

        protected override void OnDeactivate()
        {
            HideUI();
        }

        protected override object OnUnload()
        {
            player = null;
            return null;
        }

        private void OnColorButtonSelected(ColorButton button)
        {
            if (phase.IsAtLeast(SceneStatePhase.Activated))
            {
                color = button.color;
            }
        }

        private void OnEnterButtonClicked()
        {
            if (phase.IsAtLeast(SceneStatePhase.Activated))
            {
                player.name = nameInputField.text;
                player.color = color;
                SceneStateManager.instance.PopSceneState();
            }
        }

        private void OnCancelButtonClicked()
        {
            if (phase.IsAtLeast(SceneStatePhase.Activated))
            {
                cancelled = true;
                SceneStateManager.instance.PopSceneState();
            }
        }

        private void HideUI()
        {
            uiScene.gameObject.SetActive(false);
        }

        private void ShowUI()
        {
            uiScene.gameObject.SetActive(true);
            nameInputField.Select();
        }
    }
}
