﻿using UnityEngine;
using UnityEngine.UI;

namespace APlusOrFail.Setup.States.CharacterSelectionState
{
    using Character;

    public class CharacterSelectionState : SceneState
    {
        public RectTransform uiScene;
        public Text messageText;
        public Button cancelButton;

        public GameObject originalCharacter { get; set; }

        public bool cancelled { get; private set; }
        public GameObject selectedCharacter { get; private set; }
        

        private void Start()
        {
            foreach (Selectable selectable in FindObjectsOfType<Selectable>())
            {
                selectable.OnSelected += OnCharactedSelected;
            }
            cancelButton.onClick.AddListener(OnCancelButtonClicked);
            HideUI();
        }

        protected override void OnLoad()
        {
            cancelled = false;
            selectedCharacter = null;
        }

        protected override void OnActivate()
        {
            ShowUI();
        }

        protected override void OnDeactivate()
        {
            HideUI();
        }

        private void OnCancelButtonClicked()
        {
            if (state.IsAtLeast(State.Activated))
            {
                cancelled = true;
                SceneStateManager.instance.PopSceneState();
            }
        }

        private void OnCharactedSelected(Selectable selectedChar)
        {
            if (state.IsAtLeast(State.Activated))
            {
                CharacterPlayer selectedCharPlayer = selectedChar.GetComponent<CharacterPlayer>();
                if (selectedChar.gameObject != originalCharacter && selectedCharPlayer.player != null)
                {
                    messageText.text = "Character has been selected!";
                }
                else
                {
                    selectedCharacter = selectedChar.gameObject;
                    if (selectedChar.gameObject != originalCharacter)
                    {
                        CharacterPlayer originalCharPlayer = originalCharacter.GetComponent<CharacterPlayer>();
                        selectedCharPlayer.player = originalCharPlayer.player;
                        originalCharPlayer.player = null;
                        selectedCharPlayer.player.characterSprite = selectedChar.GetComponent<CharacterSprite>().overrideCharacterSprite;
                    }
                    SceneStateManager.instance.PopSceneState();
                }
                
            }
        }

        private void HideUI()
        {
            uiScene.gameObject.SetActive(false);
        }

        private void ShowUI()
        {
            uiScene.gameObject.SetActive(true);
        }
    }
}
