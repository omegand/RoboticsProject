using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestColission : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("ane");
            //var xd = GameObject.FindGameObjectsWithTag("arm");
            //foreach (var item in xd)
            //{
            //    item.GetComponent<Arm>().closed = true;
            //}

            
        }
    }
}
