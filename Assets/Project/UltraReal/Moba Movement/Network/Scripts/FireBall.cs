using Mirror;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    private Rigidbody rb;

    public NetworkIdentity owner;

    public void Init(Vector3 dir, NetworkIdentity conn)
    {
        direction = dir;
        rb = GetComponent<Rigidbody>();

        rb.AddForce(dir * speed);
        owner = conn;
    }



}
