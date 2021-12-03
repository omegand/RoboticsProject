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
    private Animator hand;
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
        UltraSoundSensor();
        if (rotating) return;
        ModeChange();
        input = transform.right;
        switch (mode)
        {
            case 0:
                IRLineTrack();
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
        if (inFront && colorSensor.color == Color.yellow && !holding)
        {
            mode = 1; 
            return;
        }
        if (colorSensor.color == Color.green && !rotating) 
        {
            holding = false;
            rotating = true;
            hand.Play("Back");
            StartCoroutine(StartRotate(3));
            GameObject.FindGameObjectWithTag("gold").GetComponent<GoldColission>().armCount = 0;
            
        }
         //mode = 0;
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
            mode = 0;
            holding = true;
            rotating = true;
            hand.Play("Grab");
            StartCoroutine(StartRotate(0));
        }
        else
        {
            hands[1].Close(true);
            hands[0].Close(false);
        }
    }
    void OpenHands() {
        hands[0].Close(true);
        hands[1].Close(false);
    }
    IEnumerator StartRotate(int delay)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < 120; i++)
        {
            transform.Rotate(0, 1.5f, 0);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        rotating = false;   
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
        else inFront = false;
    }
    public void Drive(Vector3 input)
    {
        rb.MovePosition(input);
    }
}
