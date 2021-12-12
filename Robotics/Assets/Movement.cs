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

    [SerializeField]
    private MoveLogic handMovements;

    private bool throwRockLeft = true;

    private bool robotRotated;
    private bool robotDrop;
    private bool robotStartHandMove;

    private bool robotHandMoved;

    private bool robotMoving;

    private bool robotHandOpening;


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
            case 2:
                RockLogic();
                break;
            default:
                break;
        }
    }

    private void ModeChange()
    {
        if (inFront && colorSensor.RockColor == Color.yellow && !holding)
        {
            mode = 1; 
            return;
        }
        if(colorSensor.RockColor == Color.gray && inFront && mode != 2)
        {
            mode = 2;
            return;
        }
        if (colorSensor.RockColor == Color.green && !rotating) 
        {
            holding = false;
            rotating = true;
            hand.Play("Back");
            StartCoroutine(StartRotate(3));
            GameObject.FindGameObjectWithTag("gold").GetComponent<GoldColission>().armCount = 0;
            
        }
    }
    private void GoldLogic()
    {
        if(!handMovements.armsClosed() && !robotDrop)
        {
            handMovements.StartClosing();
        }
        else
        {
            mode = 0;
            holding = true;
            rotating = true;
            hand.Play("Grab");
            StartCoroutine(StartRotate(0));
        }
        /*
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
        */
    }
    private void RockLogic()
    {
        if (!handMovements.armsClosed())
        {
            handMovements.StartClosing();
        }
        else if(robotDrop)
        {
            if(!robotHandOpening && !AnimatorIsPlaying("Back"))
            {
                StartCoroutine(OpenHand());
            }
        }
        else
        {
           if(!AnimatorIsPlaying("Grab") && robotStartHandMove)
            {
                robotHandMoved = true;
            }
           if(throwRockLeft && !rotating && !robotRotated)
            {
                rotating = true;
                StartCoroutine(StartRotateAngle(1, -1f, 90));
            }
           else if(robotRotated && !robotHandMoved && !AnimatorIsPlaying("Grab"))
            {
                hand.Play("Grab");
                robotStartHandMove = true;
            }
           else if(robotRotated && !robotDrop && robotHandMoved)
            {
                if(!robotMoving)
                {
                    robotMoving = true;
                    StartCoroutine(MoveForward());
                }
            }
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
    IEnumerator StartRotateAngle(int delay, float angle, int amount)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < amount; i++)
        {
            transform.Rotate(0, angle, 0);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        rotating = false;
        robotRotated = true;
    }
    IEnumerator MoveForward()
    {
        for(int i = 0; i < 50; i++)
        {
            Drive(transform.position + input * Time.fixedDeltaTime * robotspeed);

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        hand.Play("Back");
        robotDrop = true;
        handMovements.OpenBoth();

    }
    IEnumerator OpenHand()
    {
        for(int i =0; i < 50; i++)
        {
            handMovements.OpenHands();
            yield return new WaitForSeconds(0.05f);
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
        else inFront = false;
    }
    public void Drive(Vector3 input)
    {
        rb.MovePosition(input);
    }
    private bool AnimatorIsPlaying(string stateName)
    {
        return hand.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
    private bool AnimatorIsPlaying()
    {
        return hand.GetCurrentAnimatorStateInfo(0).length >
               hand.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
