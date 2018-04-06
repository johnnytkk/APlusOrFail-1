using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace APlusOrFail.Character
{
    public class CharacterHealth : MonoBehaviour
    {
        public class ChangeDatum
        {
            public readonly int changes;

            public ChangeDatum(int changes)
            {
                this.changes = changes;
            }
        }


        public int initialHealth;

        public int health { get; private set; }
        public readonly ReadOnlyCollection<ChangeDatum> changeData;
        public event EventHandler<CharacterHealth, ChangeDatum> onHealthChanged;

        private readonly List<ChangeDatum> _changeData = new List<ChangeDatum>();


        public CharacterHealth()
        {
            changeData = new ReadOnlyCollection<ChangeDatum>(_changeData);
        }

        private void Start()
        {
            health = initialHealth;
        }

        public void Change(ChangeDatum changeDatum)
        {
            health += changeDatum.changes;
            _changeData.Add(changeDatum);
            onHealthChanged?.Invoke(this, changeDatum);
        }
        
    }
}
