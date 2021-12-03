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
    bool inFront = false;
    private float mode = 0;
    private bool holding;

    private bool rotating;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.zero;
    }

    void FixedUpdate()
    {
        if(rotating)
        {
            return;
        }
        Debug.Log(colorSensor.color.ToString());
        ModeChange();
        input = transform.right;
        switch (mode)
        {
            case 0:
                IRLineTrack();
                UltraSoundSensor();
                break;
            case 1:
                GoldLogic();
                break;
            default:
                break;
        }
    }

    private void ModeChange()
    {
        if (inFront && colorSensor.color == Color.yellow && !holding) {
            mode = 1; return;
        }
        if(inFront && holding)
        mode = 0;
    }
    private bool BothClosed()
    {
        if (hands[1].closed && hands[0].closed)
            return true;

        return false;
    }
    private void GoldLogic()
    {
        if (BothClosed())
        {
            rotating = true;
           //transform.Rotate(0, 180, 0);
            mode = 0;
            holding = true;

            StartCoroutine(StartRotate(Mathf.FloorToInt(transform.rotation.eulerAngles.y)));
        }
        else
        {
            hands[1].Close(true);
            hands[0].Close(false);
        }
    }
    IEnumerator StartRotate(int initialY)
    {
        while (rotating)
        {
            int eulerY = Mathf.FloorToInt(transform.rotation.eulerAngles.y);
            if (eulerY != (initialY + 180))
            {
                transform.Rotate(0, 1, 0);
                yield return new WaitForSeconds(0.05f);
            }
            else
            {
                rotating = false;
            }
        }
    }
    private void IRLineTrack()
    {
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
        if (ultraSound.found) inFront = true;
    }

    public void Drive(Vector3 input)
    {
        rb.MovePosition(input);
    }
}
