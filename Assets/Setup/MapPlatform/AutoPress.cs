using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace APlusOrFail.Setup.MapPlatform
{
    public class AutoPress : MonoBehaviour
    {
        public SpringJoint2D springJoint;

        private int _characterCount;
        private int characterCount
        {
            get
            {
                return _characterCount;
            }
            set
            {
                if (_characterCount != value)
                {
                    _characterCount = value;
                    springJoint.enabled = value == 0;
                    onCharacterCount?.Invoke(this, value);
                }
            }
        }
        public event EventHandler<AutoPress, int> onCharacterCount;
        
        private void Start()
        {
            springJoint.autoConfigureDistance = false;
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerId.Characters)
            {
                ++characterCount;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerId.Characters)
            {
                --characterCount;
            }
        }


    }
}
