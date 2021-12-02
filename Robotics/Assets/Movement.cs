using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField]
    private float Robotspeed;


    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 input = new Vector3(1, 0, 0);
        Drive(transform.position + input * Time.fixedDeltaTime * Robotspeed);
    }
   public void Drive(Vector3 input)
    {
        rb.MovePosition(input);
    }
}
