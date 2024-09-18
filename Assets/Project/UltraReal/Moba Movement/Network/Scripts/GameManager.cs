using UnityEngine;
using Mirror;
using Mirror.Examples.AdditiveLevels;
using Unity.VisualScripting;
public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    public GameObject gold;
    public GameObject chest;
    public Transform[] spawnPoints;
    public Transform[] chestPoints;

    public bool isDead = false;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            foreach (var point in spawnPoints)
            {
                GameObject go = Instantiate(gold,point.position,point.rotation);
                NetworkServer.Spawn(go);
            }
            foreach (var point in chestPoints)
            {
                GameObject go = Instantiate(chest, point.position, point.rotation);
                NetworkServer.Spawn(go);
            }
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && isDead)
        {
            NetworkClient.AddPlayer();
            isDead = false;
        }
    }

}
