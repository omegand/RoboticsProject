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
            case 3:
                DropGold();
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
        if(inFront && colorSensor.RockColor == Color.red)
        {
            mode = 4;
            return;
        }
        if(colorSensor.RockColor == Color.gray && inFront && mode != 2)
        {
            mode = 2;
            return;
        }
        if (colorSensor.RockColor == Color.green && holding) 
        {
            mode = 3;
            return;
            
        }
    }
    private void GoldLogic()
    {
        if(!handMovements.armsClosed() && !robotDrop)
        {
            handMovements.StartClosing();
        }
        else if(!rotating && !holding)
        {
            holding = true;
            rotating = true;
            hand.Play("Grab");

            StartCoroutine(StartRotateAngle(1, 1f, 180));
        }
        else if(!rotating && holding)
        {
            mode = 0;
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
    private void DropGold()
    {
        if(!robotStartHandMove)
        {
            robotRotated = false;
            robotStartHandMove = true;
            hand.Play("Back");
        }
        else if(holding && !AnimatorIsPlaying("Back"))
        {
            holding = false;
            robotHandMoved = true;
            handMovements.StartOpening();
            StartCoroutine(OpenHand());
        }
        else if(robotMoving && !holding)
        {
            hand.Play("Grab");
            robotMoving = false;
        }
        else if(!holding && !AnimatorIsPlaying("Grab") && !rotating && !robotHandMoved && !robotRotated)
        {
            rotating = true;
            StartCoroutine(StartRotateAngle(1, 1f, 180));
        }
        else if(robotRotated && !rotating)
        {
            hand.Play("Back");
            rotating = true;
           // robotMoving = true;
        }
        else if(rotating && !AnimatorIsPlaying("Back") && robotRotated)
        {
            mode = 0;
            ResetStates();
        }
       // GameObject.FindGameObjectWithTag("gold").GetComponent<GoldColission>().armCount = 0;
    }
    private void RockLogic()
    {
        if (!handMovements.armsClosed() && !robotDrop)
        {
            handMovements.StartClosing();
        }
        else if(robotDrop)
        {
            if(!robotHandOpening && !AnimatorIsPlaying("Back"))
            {
                robotMoving = false;
                robotHandOpening = true;
                robotRotated = false;
                rotating = false;
                handMovements.StartOpening();
                StartCoroutine(OpenHand());
            }
            else if(robotMoving && !AnimatorIsPlaying("Back"))
            {
                robotMoving = false;
                StartCoroutine(MoveBackWard());
            }
            else if(rotating && !AnimatorIsPlaying("Back"))
            {
                rotating = false;
                StartCoroutine(StartRotateAngle(1, 1f, 90));
            }
            else if(!rotating && robotRotated && !AnimatorIsPlaying("Back"))
            {
                ResetStates();
                mode = 0;
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
    private void ResetStates()
    {
        robotRotated = false;
        rotating = false;
        robotDrop = false;
        robotStartHandMove = false;
        robotHandOpening = false;
        robotHandMoved = false;
        robotMoving = false;
        throwRockLeft = true;
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
    }
    IEnumerator MoveBackWard()
    {
        while(!rightSensor.InsideTrack() && !leftSensor.InsideTrack())
        {
            Drive(transform.position + -1 * input * Time.fixedDeltaTime * robotspeed);

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        robotRotated = false;
        rotating = true;
    }
    IEnumerator OpenHand()
    {
        while(handMovements.opening)
        {
            handMovements.OpenHands();
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        robotMoving = true;
        robotHandMoved = false;
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
        return AnimatorIsPlaying() && hand.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
    private bool AnimatorIsPlaying()
    {
        return hand.GetCurrentAnimatorStateInfo(0).length >
               hand.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
