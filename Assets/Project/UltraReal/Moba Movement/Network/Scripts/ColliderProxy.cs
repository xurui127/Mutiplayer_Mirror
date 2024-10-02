
using UnityEngine;
using Mirror;

namespace UltraReal.MobaMovement
{
    public class ColliderProxy : NetworkBehaviour
    {
        [ServerCallback]
        public void OnCollisionEnter(Collision other)
        {
            GetComponentInParent<NetPlayerController>().OnCollisionEnter(other);
        }
    }
}