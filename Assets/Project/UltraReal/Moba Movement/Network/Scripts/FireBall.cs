using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float speed;
    private Vector3 direction;
    private Rigidbody rb;
    
    public void Init(Vector3 dir)
    {
        direction = dir;
        rb = GetComponent<Rigidbody>();

        rb.AddForce(dir * speed);
    }


    
}
