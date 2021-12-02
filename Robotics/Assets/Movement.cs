using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    void Update()
    {
        Drive(new Vector3(10, 10, 10));
    }
   public void Drive(Vector3 input)
    {
        rb.MovePosition(input);
    }
}
