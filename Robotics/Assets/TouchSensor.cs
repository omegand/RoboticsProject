using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchSensor : MonoBehaviour
{
    public GameObject touchedSomething;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
       if(collision.collider.tag.Equals("gold") || collision.collider.tag.Equals("rock"))
        {
            touchedSomething = collision.collider.gameObject;
        }
    }

}
