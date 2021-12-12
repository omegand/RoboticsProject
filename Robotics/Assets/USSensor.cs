using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USSensor : MonoBehaviour
{
    public bool found = false;
    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit, 0.6f))
        {
            if (hit.collider.tag.Equals("gold"))
            {
                found = true;
                return;
            }
            else if(hit.collider.tag.Equals("rock"))
            {
                found = true;
                return;
            }
        }
        found = false;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.right * 0.6f, Color.green);
    }

}
