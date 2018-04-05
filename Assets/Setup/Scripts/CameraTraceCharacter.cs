using UnityEngine;

namespace APlusOrFail
{
    using MainCamera;

    public class CameraTraceCharacter : MonoBehaviour
    {
        
        private void Start()
        {
            AutoResizeCamera autoResizeCamera = Camera.main.GetComponent<AutoResizeCamera>();
            foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Character"))
            {
                autoResizeCamera.AddTracingSprite(gameObject);
            }
        }
    }
}
