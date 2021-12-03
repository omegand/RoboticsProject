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
    [SerializeField]
    Arm[] hands;
    Vector3 input;
    float turnRatio = 2;
    Rigidbody rb;
    bool stopped = false;
    private int mode = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.zero;
    }

    void FixedUpdate()
    {
        input = transform.right;
        if (stopped && mode == 0)
        {
            CheckColor();
        }
        switch (mode)
        {

            case 1:
                GoldLogic();
                break;
            default:
                break;
        }

        IRLineTrack();
        UltraSoundSensor();
    }

    private void GoldLogic()
    {
        LockArms();
    }

    private void LockArms()
    {
        if (hands[1].closed && hands[0].closed)
        {
            transform.Rotate(0, 180, 0);
            mode = 0;
            stopped = false;

        }
        else
        {
            hands[1].Close(true);
            hands[0].Close(false);
        }
    }

    private void CheckColor()
    {
        if (colorSensor.color == Color.yellow)
        {
            mode = 1;
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
            if (!rightSensor.InsideTrack() && leftSensor.InsideTrack())
                transform.Rotate(0, -turnRatio, 0);

            else transform.Rotate(0, turnRatio, 0);

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
