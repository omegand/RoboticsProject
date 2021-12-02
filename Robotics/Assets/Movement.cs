using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField]
    private float Robotspeed;

    [SerializeField]
    private Sensoring rightSensor;

    [SerializeField]
    private Sensoring leftSensor;


    private bool coroutineFinished = false;


    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    void FixedUpdate()
    {

        Vector3 input = transform.right;
        if (rightSensor.InsideTrack() && leftSensor.InsideTrack())
        {
            Drive(transform.position + input * Time.fixedDeltaTime * Robotspeed);
        }
        else if (leftSensor.InsideTrack() && !rightSensor.InsideTrack())
        {
            Quaternion quart = Quaternion.Euler(0, -5, 0);
            StartCoroutine(StartingLoop(quart, 1f, input));
        }
        else if(!leftSensor.InsideTrack() && rightSensor.InsideTrack())
        {
            Quaternion quart = Quaternion.Euler(0, 5, 0);
            StartCoroutine(StartingLoop(quart, 1f, input));

        }
        else if(!leftSensor.InsideTrack() && !rightSensor.InsideTrack())
        {
            Quaternion quart = Quaternion.Euler(0, 5, 0);
            StartCoroutine(StartingLoop(quart, 1f, input));
        }
    }
    IEnumerator StartingLoop(Quaternion rotate, float rotatingTime, Vector3 moveTo)
    {
        Vector3 newAngle = rb.rotation.eulerAngles + rotate.eulerAngles;
        rb.MoveRotation(Quaternion.Euler(newAngle.x, newAngle.y, newAngle.z));
        RaycastHit hit;
        bool hitGround = false;
        if (Physics.Raycast(transform.position, new Vector3(0, -1, 0), out hit, 3f))
        {
            if (hit.collider.gameObject.tag.Equals("Ground"))
            {
                hitGround = true;
            }
        }
        /*
        if(!hitGround)
        {
            yield return StartingLoop(rotate, rotatingTime, moveTo);
        }
        */
        yield return new WaitForSeconds(rotatingTime);
        Drive(transform.position + moveTo * Time.fixedDeltaTime * Robotspeed);
    }
    public void Drive(Vector3 input)
    {
        rb.MovePosition(input);
    }
}
