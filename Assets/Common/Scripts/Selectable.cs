using UnityEngine;

namespace APlusOrFail
{
    public class Selectable : MonoBehaviour
    {

        public event EventHandler<Selectable> OnSelected;
    
        private void OnMouseDown()
        {
            OnSelected?.Invoke(this);
        }
    }
}
