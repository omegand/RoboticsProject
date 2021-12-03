using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USSensor : MonoBehaviour
{
    public bool found = false;
    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit, 1f))
        {
            if (hit.collider.tag.Equals("gold"))
            {
                found = true;
                return;
            }
        }
        found = false;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.right * 1f, Color.green);
    }

}
