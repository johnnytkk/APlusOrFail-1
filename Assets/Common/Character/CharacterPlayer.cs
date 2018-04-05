using UnityEngine;

namespace APlusOrFail.Character
{
    public class CharacterPlayer : MonoBehaviour
    {
        private Player _player;
        public Player player
        {
            get
            {
                return _player;
            }
            set
            {
                _player = value;
                onPlayerChanged?.Invoke(this, value);
            }
        }
        public event EventHandler<CharacterPlayer, Player> onPlayerChanged;
    }
}
