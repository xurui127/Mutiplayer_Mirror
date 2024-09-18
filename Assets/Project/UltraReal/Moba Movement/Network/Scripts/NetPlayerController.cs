using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
namespace UltraReal.MobaMovement
{
    public class NetPlayerController : NetworkBehaviour
    {
        public GameObject[] avatars;
        private RaycastHit hitInfo;

        public GameObject fireBall;
        public Transform castPosition;

        [SyncVar(hook = nameof(OnHealthChanged))]
        private int health = 3;

        private const float deathAnimDelay = 3f;
        public GameObject player1Hat;
        public GameObject player2Hat;
        // Start is called before the first frame update
        void Start()
        {
            if (isLocalPlayer)
            {
                var nma = GetComponent<NavMeshAgent>();
                nma.enabled = true;
                var mm = GetComponent<MobaMover>();
                mm.enabled = true;
                var ma = GetComponent<MobaAnimate>();
                ma.enabled = true;
                UIController.instance.UpdateHealth(health);
                player1Hat.SetActive(true);
                player2Hat.SetActive(false);
            }
            else
            {
                player1Hat.SetActive(false);
                player2Hat.SetActive(true);
            }
        }
        private void Update()
        {
            if (!isLocalPlayer) return;

            if (Input.GetKey(KeyCode.Q))
            {
                CmdChangePlayerState(0);
            }
            if (Input.GetKey(KeyCode.W))
            {
                CmdChangePlayerState(1);
            }
            if (Input.GetKey(KeyCode.E))
            {
                CmdChangePlayerState(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                GetComponent<NavMeshAgent>().destination = transform.position;
                transform.LookAt(new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z));
                GetComponent<MobaAnimate>()._animator.SetTrigger("Spell");

                CmdSpell(hitInfo.point);
            }
            GetHoverInfo();
            LeftMouseClick();
        }

        [Command]
        private void CmdChangePlayerState(int index)
        {
            RpcChangePlayerState(index);
        }
        [ClientRpc]
        private void RpcChangePlayerState(int index)
        {
            foreach (var avatar in avatars)
            {
                avatar.SetActive(false);
                avatars[index].SetActive(true);
                GetComponent<MobaAnimate>()._animator = avatars[index].GetComponent<Animator>();
                GetComponent<NetworkAnimator>().animator = avatars[index].GetComponent<Animator>();
            }
        }
        [TargetRpc]
        private void TargerAddGold(NetworkConnection conn, int count)
        {
            UIController.instance.AddGold(count);
        }
        private void OnTriggerEnter(UnityEngine.Collider other)
        {
            if (other.gameObject.tag.Equals("Gold") && isServer)
            {
                Destroy(other.gameObject);
                var netIdentity = GetComponent<NetworkIdentity>();
                TargerAddGold(netIdentity.connectionToClient, 1);
            }


        }
        [ServerCallback]
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag.Equals("Spell"))
            {
                if (health > 0)
                {
                    health--;
                }

                if (health <= 0)
                {
                    StartCoroutine(DestroyPlayer(deathAnimDelay));
                }
            }
        }
        private void OnHealthChanged(int oldValue, int newValue)
        {
            if (isLocalPlayer)
            {

                UIController.instance.UpdateHealth(health);
            }


            HandleAnimations(health);

        }
        private void HandleAnimations(int currentHealth)
        {
            if (currentHealth > 0)
            {
                GetComponent<MobaAnimate>()._animator.SetTrigger("Hit");
            }
            else
            {
                GetComponent<MobaAnimate>()._animator.SetTrigger("Dead");
            }

            if (currentHealth == 0 && isLocalPlayer)
            {
                GetComponent<NavMeshAgent>().isStopped = true;
            }
        }
        private IEnumerator DestroyPlayer(float delay)
        {
            yield return new WaitForSeconds(delay);
            var networkIdentity = GetComponent<NetworkIdentity>();
            TargetPlayerDead(networkIdentity.connectionToClient);
            Destroy(gameObject);
        }
        [TargetRpc]
        private void TargetPlayerDead(NetworkConnection conn)
        {
            GameManager.instance.isDead = true;
        }

        [Command]
        private void DestroyCharacter()
        {
            NetworkServer.Destroy(gameObject);

        }
        private void GetHoverInfo()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hitInfo);
        }
        private void LeftMouseClick()
        {

            if (Input.GetMouseButton(0) &&
                hitInfo.collider.tag.Equals("Chest") &&
                !hitInfo.collider.GetComponent<ChestController>().isOpened)
            {

                float distance = Vector3.Distance(transform.position, hitInfo.collider.transform.position);
                if (distance < 2f)
                {
                    CmdOpenChest(hitInfo.collider.gameObject);
                }
            }
        }

        [Command]
        private void CmdOpenChest(GameObject chest)
        {
            var netIdentity = GetComponent<NetworkIdentity>();
            TargerAddGold(netIdentity.connectionToClient, 100);
            RpcOpenChest(chest);
        }
        [ClientRpc]
        private void RpcOpenChest(GameObject chest)
        {
            chest.GetComponent<ChestController>().OpenChest();
        }
        [Command]
        private void CmdSpell(Vector3 hitPoint)
        {
            var lookAtPosition = new Vector3(hitPoint.x, transform.position.y, hitPoint.z);
            transform.LookAt(lookAtPosition);

            var fireball = Instantiate(fireBall, castPosition.transform.position, castPosition.rotation);
            var dir = (hitPoint - castPosition.transform.position).normalized;

            fireball.GetComponent<FireBall>().Init(dir);
            NetworkServer.Spawn(fireball);

            RpcSyncFireball(fireball, dir);
            Destroy(fireball, 3f);
        }
        [ClientRpc]
        private void RpcSyncFireball(GameObject fireball, Vector3 direction)
        {
            fireball.GetComponent<FireBall>().Init(direction);
        }
    }
}