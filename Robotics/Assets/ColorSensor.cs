using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSensor : MonoBehaviour
{
    public Color color = Color.black;
    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit, 2f))
        {
            switch (hit.collider.tag)
            {
                case "gold":
                    color = Color.yellow; break;
                case "rock":
                    color = Color.gray; break;
                case "obstacle":
                    color = Color.red; break;
                default: color = Color.black; break;
            }
        }
    }

}
