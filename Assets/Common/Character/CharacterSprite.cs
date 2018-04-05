using UnityEngine;

namespace APlusOrFail.Character
{
    [RequireComponent(typeof(CharacterPlayer))]
    public class CharacterSprite : MonoBehaviour
    {
        public GameObject overrideCharacterSprite;

        private GameObject _attachedSprite;
        public GameObject attachedSprite
        {
            get
            {
                return _attachedSprite;
            }
            private set
            {
                _attachedSprite = value;
                onAttachedSpriteChanged?.Invoke(this, value);
            }
        }
        public event EventHandler<CharacterSprite, GameObject> onAttachedSpriteChanged;
        
        private void Start()
        {
            CharacterPlayer charPlayer = GetComponent<CharacterPlayer>();
            SetPlayer(charPlayer.player);
            charPlayer.onPlayerChanged += OnPlayerChanged;
        }

        private void OnPlayerChanged(CharacterPlayer charPlayer, Player player)
        {
            SetPlayer(player);
        }

        private Player player;
        private void SetPlayer(Player player)
        {
            if (this.player != null)
            {
                this.player.onCharacterSpriteChanged -= OnCharacterSpriteChanged;
            }
            this.player = player;
            SetCharacterSprite(player?.characterSprite);
            if (player != null)
            {
                player.onCharacterSpriteChanged += OnCharacterSpriteChanged;
            }
        }

        private void OnCharacterSpriteChanged(Player player, GameObject sprite)
        {
            SetCharacterSprite(sprite);
        }

        private GameObject sprite;
        private void SetCharacterSprite(GameObject sprite)
        {
            sprite = overrideCharacterSprite ?? sprite;
            if (this.sprite != sprite)
            {
                this.sprite = sprite;
                if (attachedSprite != null)
                {
                    Destroy(attachedSprite);
                }
                if (sprite != null)
                {
                    attachedSprite = Instantiate(sprite, transform);
                }
                else
                {
                    attachedSprite = null;
                }
            }
        }
    }
}
