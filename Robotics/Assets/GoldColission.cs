using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldColission : MonoBehaviour
{
    int armCount = 0;
    GameObject[] arms = new GameObject[2];

    private GameObject handObject;
    public void Start()
    {
        handObject = GameObject.Find("hands");
    }
    private void FixedUpdate()
    {
        if (armCount == 2)
        {
            transform.SetPositionAndRotation(Vector3.Lerp(
                arms[0].transform.position,
                arms[1].transform.position,
                0.5f), handObject.transform.rotation);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "arm" && armCount < 2)
        {
            arms[armCount] = collision.gameObject;
            arms[armCount].GetComponent<Arm>().closed = true;
            armCount++;
        }
    }
}
