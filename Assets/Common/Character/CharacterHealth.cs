using UnityEngine;

namespace APlusOrFail.Character
{
    public class CharacterHealth : MonoBehaviour
    {
        [HideInInspector, SerializeField]
        private int _health;
        [EditorPropertyField]
        public int health
        {
            get
            {
                return _health;
            }
            set
            {
                _health = value;
                onHealthChanged?.Invoke(this, value);
            }
        }

        public event EventHandler<CharacterHealth, int> onHealthChanged;
        
    }
}
