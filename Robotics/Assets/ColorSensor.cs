using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSensor : MonoBehaviour
{
    public Color RockColor = Color.black;

    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit, 2f))
        {
            switch (hit.collider.tag)
            {
                case "gold":
                    RockColor = Color.yellow;
                    return;
                case "rock":
                    RockColor = Color.gray; 
                    return;
                case "obstacle":
                    RockColor = Color.red;
                    return;        
                case "dropzone":
                    RockColor = Color.green;
                    return;
                default:
                    break;
            }
        }
        RockColor = Color.black;
    }
    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.right * 2f, Color.red);
    }

}
