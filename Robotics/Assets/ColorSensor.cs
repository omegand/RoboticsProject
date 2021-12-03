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
                    color = Color.yellow;
                    return;
                case "rock":
                    color = Color.gray; 
                    return;
                case "obstacle":
                    color = Color.red;
                    return;        
                case "dropzone":
                    color = Color.green;
                    return;
                default:
                    break;
            }
        }
        color = Color.black;
    }
    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.right * 2f, Color.red);
    }

}
