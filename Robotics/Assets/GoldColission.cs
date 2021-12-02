using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldColission : MonoBehaviour
{
    bool glued;
    GameObject arm;
    private void FixedUpdate()
    {
        if (glued) 
        {
            transform.position = arm.transform.position;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "arm")
        {

            glued = true;
            arm = collision.gameObject;
            arm.GetComponent<Arm>().closed = true;

        }
    }
}
