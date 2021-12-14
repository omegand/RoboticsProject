using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USSensor : MonoBehaviour
{
    public bool found = false;
    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit, 4f))
        {
            Debug.Log(hit.collider.tag);
            if (hit.collider.tag.Equals("gold") && hit.distance < 0.6)
            {
                found = true;
                return;
            }
            else if(hit.collider.tag.Equals("rock") && hit.distance < 0.6)
            {
                found = true;
                return;
            }
            else if (hit.collider.tag.Equals("obstacle") && hit.distance < 2.0)
            {
                found = true;
                return;
            }
        }
        found = false;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.right * 4f, Color.green);
    }

}
