﻿using UnityEngine;

namespace APlusOrFail.Setup.States.DefaultSceneState
{
    using Character;
    using CharacterOptionState;
    using PlayerNameAndColorSetupState;
    using PlayerActionKeySetupState;

    public class DefaultState : SceneStateBehavior<object, object>
    {

        public PlayerNameAndColorSetupState inputPlayerNameUIScene;
        public PlayerActionKeySetupState actionKeySetupUIScene;
        public CharacterOptionsState charOptionUIScene;

        private PlayerNameAndColorSetupState activeNameColorSetupScene;
        private PlayerActionKeySetupState activeKeySetupScene;

        private void Start()
        {
            foreach (Selectable selectable in FindObjectsOfType(typeof(Selectable)))
            {
                selectable.OnSelected += OnCharacterSelected;
            }
        }

        private void OnCharacterSelected(Selectable selectedChar)
        {
            if (phase.IsAtLeast(SceneStatePhase.Activated))
            {
                CharacterPlayer charPlayer = selectedChar.GetComponent<CharacterPlayer>();
                if (charPlayer.player == null)
                {
                    charPlayer.player = new Player()
                    {
                        characterSprite = selectedChar.GetComponent<CharacterSprite>().overrideCharacterSprite
                    };
                    SceneStateManager.instance.PushSceneState(inputPlayerNameUIScene, null);
                    activeNameColorSetupScene = inputPlayerNameUIScene;
                    activeNameColorSetupScene.character = selectedChar.gameObject;
                }
                else
                {
                    SceneStateManager.instance.PushSceneState(charOptionUIScene, null);
                    charOptionUIScene.character = selectedChar.gameObject;
                }
            }
        }

        protected override void OnActivate(ISceneState unloadedSceneState, object result)
        {
            if (activeNameColorSetupScene != null)
            {
                if (activeNameColorSetupScene.cancelled)
                {
                    CharacterPlayer charPlayer = activeNameColorSetupScene.character.GetComponent<CharacterPlayer>();
                    charPlayer.player.Delete();
                    charPlayer.player = null;
                }
                else
                {
                    SceneStateManager.instance.PushSceneState(actionKeySetupUIScene, null);
                    activeKeySetupScene = actionKeySetupUIScene;
                    activeKeySetupScene.character = activeNameColorSetupScene.character;
                }
                activeNameColorSetupScene = null;
            }
            else if (activeKeySetupScene != null)
            {
                if (activeKeySetupScene.cancelled)
                {
                    CharacterPlayer charPlayer = activeKeySetupScene.character.GetComponent<CharacterPlayer>();
                    charPlayer.player.Delete();
                    charPlayer.player = null;
                }
                activeKeySetupScene = null;
            }
        }

    }
}
