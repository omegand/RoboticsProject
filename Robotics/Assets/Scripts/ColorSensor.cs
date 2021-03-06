using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSensor : MonoBehaviour
{
    public Color Color = Color.black;

    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit, 2f))
        {
            switch (hit.collider.tag)
            {
                case "gold":
                    Color = Color.yellow;
                    return;
                case "rock":
                    Color = Color.gray; 
                    return;
                case "obstacle":
                    Color = Color.red;
                    return;        
                case "dropzone":
                    Color = Color.green;
                    return;
                case "finish":
                    Color = Color.white;
                    return;
                default:
                    break;
            }
        }
        Color = Color.black;
    }
    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.right * 2f, Color.red);
    }

}
