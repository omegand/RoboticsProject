using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField]
    private float robotspeed;
    [SerializeField]
    private Sensoring rightSensor;
    [SerializeField]
    private Sensoring leftSensor;
    [SerializeField]
    private USSensor ultraSound;
    [SerializeField]
    private ColorSensor colorSensor;
    Vector3 input;
    float turnRatio = 2;
    Rigidbody rb;
    bool stopped = false;
    private int modes = 0;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        input = transform.right;
        if (stopped && modes == 0)
        {
            CheckColor();
        }
        switch (modes)
        {

            case 1:
                break;
            default:
                break;
        }

        IRLineTrack();
        UltraSoundSensor();
    }

    private void CheckColor()
    {
        if (colorSensor.color == Color.yellow)
        {
            modes = 1;
        }
    }

    private void IRLineTrack()
    {
        if (stopped) return;
        if (rightSensor.InsideTrack() && leftSensor.InsideTrack())
        {
            Drive(transform.position + input * Time.fixedDeltaTime * robotspeed);
        }
        else
        {
            transform.Rotate(0, turnRatio, 0);
        }
    }
    private void UltraSoundSensor()
    {
        if (ultraSound.found) stopped = true;
    }

    public void Drive(Vector3 input)
    {
        rb.MovePosition(input);
    }
}
