using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    private bool usRotated;

    private GameObject USComponent;

    private bool robotAvoidedObstacle;
    public TextMeshProUGUI Text;

    enum AvoidObstacleMode
    {
        None, RotateLeft, RotateRight, Move, DetectionUS, MoveCheck, Check, MoveUntilBack
    }

    private AvoidObstacleMode obstacleMode;
    private bool stop = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        USComponent = GameObject.Find("US");
        rb.centerOfMass = Vector3.zero;
        obstacleMode = AvoidObstacleMode.RotateLeft;
    }

    void FixedUpdate()
    {
        if (stop) return;
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
            case 4:
                MoveAroundObstacle();
                break;
            case 9:
                End();
                break;
            default:
                break;
        }
    }

    private void End()
    {
        Text.text = "The robot finished the track!!";
        stop = true;
    }

    private void ModeChange()
    {
        if (colorSensor.Color == Color.white)
        {
            mode = 9;
            return;
        }
        if (inFront && colorSensor.Color == Color.yellow && !holding)
        {
            mode = 1;
            return;
        }
        if (inFront && colorSensor.Color == Color.red)
        {
            mode = 4;
            return;
        }
        if (colorSensor.Color == Color.gray && inFront && mode != 2)
        {
            mode = 2;
            return;
        }
        if (colorSensor.Color == Color.green && holding)
        {
            mode = 3;
            return;

        }
    }
    private void GoldLogic()
    {
        if (!handMovements.armsClosed() && !robotDrop)
        {
            handMovements.closing = true;
        }
        else if (!rotating && !holding)
        {
            holding = true;
            rotating = true;
            hand.Play("Grab");

            StartCoroutine(StartRotateAngle(1, 2f, 90));
        }
        else if (!rotating && holding)
        {
            ResetStates();
            mode = 0;
        }
    }
    private void DropGold()
    {
        if (!robotStartHandMove)
        {
            robotRotated = false;
            robotStartHandMove = true;
            hand.Play("Back");
        }
        else if (!robotHandMoved && !AnimatorIsPlaying("Back") && handMovements.touchedObject != null)
        {
            robotHandMoved = true;
            handMovements.StartOpening();
            StartCoroutine(OpenHand());
        }
        else if (!holding && !rotating && !robotHandMoved && !robotRotated)
        {
            rotating = true;
            StartCoroutine(StartRotateAngle(1, 2f, 90));
        }
        else if (!rotating && !AnimatorIsPlaying("Back") && robotRotated)
        {
            mode = 0;
            ResetStates();
        }
    }
    private void RockLogic()
    {
        if (!handMovements.armsClosed() && !robotDrop)
        {
            handMovements.closing = true;
        }
        else if (robotDrop)
        {
            if (!robotHandOpening && !AnimatorIsPlaying("Back"))
            {
                robotMoving = false;
                robotHandOpening = true;
                robotRotated = false;
                rotating = false;
                handMovements.StartOpening();
                StartCoroutine(OpenHand());
            }
            else if (robotMoving && !AnimatorIsPlaying("Back"))
            {
                robotMoving = false;
                StartCoroutine(MoveBackWard());
            }
            else if (rotating && !AnimatorIsPlaying("Back"))
            {
                rotating = false;
                StartCoroutine(StartRotateAngle(1, 1f, 90));
            }
            else if (!rotating && robotRotated && !AnimatorIsPlaying("Back"))
            {
                ResetStates();
                mode = 0;
            }
        }
        else
        {
            if (!AnimatorIsPlaying("Grab") && robotStartHandMove)
            {
                robotHandMoved = true;
            }
            if (throwRockLeft && !rotating && !robotRotated)
            {
                rotating = true;
                StartCoroutine(StartRotateAngle(1, -1f, 90));
            }
            else if (robotRotated && !robotDrop)
            {
                if (!robotMoving)
                {
                    robotMoving = true;
                    StartCoroutine(MoveForward());
                }
            }
        }
    }
    private void MoveAroundObstacle()
    {
        if (obstacleMode == AvoidObstacleMode.None)
        {
            if (!rotating && !robotRotated)
            {
                rotating = true;
                robotRotated = false;
                StartCoroutine(StartRotateAngle(1, -1f, 90));
            }
            else if (robotRotated)
            {
                obstacleMode = AvoidObstacleMode.RotateLeft;
                ResetStates();
                mode = 0;
            }
        }
        else if (!robotMoving && obstacleMode == AvoidObstacleMode.MoveUntilBack)
        {
            if (InsideTrack())
            {
                rotating = false;
                robotRotated = false;
                obstacleMode = AvoidObstacleMode.None;
                ultraSound.checkingObstacle = false;
            }
            else
            {
                robotMoving = true;
                StartCoroutine(MoveForwardUntilTrack(10));
            }
        }
        else if (!robotMoving && obstacleMode == AvoidObstacleMode.RotateLeft)
        {
            obstacleMode = AvoidObstacleMode.Move;

            robotRotated = false;
            StartCoroutine(StartRotateAngle(1, -1f, 90));
        }
        else if (robotRotated && obstacleMode == AvoidObstacleMode.Move)
        {

            if (robotAvoidedObstacle)
            {

                obstacleMode = AvoidObstacleMode.MoveUntilBack;
            }
            else
            {
                robotMoving = true;

                obstacleMode = AvoidObstacleMode.RotateRight;

                StartCoroutine(MoveForwardNew(50));
            }
        }
        else if (!robotMoving && obstacleMode == AvoidObstacleMode.RotateRight)
        {
            robotRotated = false;
            if (robotAvoidedObstacle)
            {
                obstacleMode = AvoidObstacleMode.Move;
            }
            else
                obstacleMode = AvoidObstacleMode.DetectionUS;

            StartCoroutine(StartRotateAngle(1, 1f, 90));
        }
        else if (robotRotated && obstacleMode == AvoidObstacleMode.DetectionUS)
        {
            obstacleMode = AvoidObstacleMode.MoveCheck;
            ultraSound.checkingObstacle = true;
            StartCoroutine(RotateUS(90, 1f));
        }
        else if (usRotated && !robotMoving && obstacleMode == AvoidObstacleMode.MoveCheck)
        {
            obstacleMode = AvoidObstacleMode.Check;
            robotMoving = true;
            StartCoroutine(MoveForwardNew(60));
        }
        else if (usRotated && !robotMoving && obstacleMode == AvoidObstacleMode.Check)
        {
            if (ultraSound.found)
            {
                obstacleMode = AvoidObstacleMode.MoveCheck;
            }
            else
            {
                StartCoroutine(RotateUS(90, -1f));
                robotAvoidedObstacle = true;
                obstacleMode = AvoidObstacleMode.RotateRight;
            }
        }
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
        usRotated = false;
        robotAvoidedObstacle = false;
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
        for (int i = 0; i < 50; i++)
        {
            Drive(transform.position + input * Time.fixedDeltaTime * robotspeed);

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        robotDrop = true;
    }
    IEnumerator MoveForwardNew(int frames)
    {
        for (int i = 0; i < frames; i++)
        {
            Drive(transform.position + input * Time.fixedDeltaTime * robotspeed);

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        robotMoving = false;
    }
    IEnumerator MoveForwardUntilTrack(int frames)
    {
        for (int i = 0; i < frames; i++)
        {
            if (InsideTrack())
            {
                break;
            }
            Drive(transform.position + input * Time.fixedDeltaTime * robotspeed);

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        robotMoving = false;
    }
    IEnumerator MoveBackWard()
    {
        while (!InsideTrack())
        {
            Drive(transform.position + -1 * input * Time.fixedDeltaTime * robotspeed);

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        robotRotated = false;
        rotating = true;
    }
    IEnumerator RotateUS(int angle, float rotateStep)
    {
        for (int i = 0; i < angle; i++)
        {
            USComponent.transform.Rotate(0, rotateStep, 0);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        usRotated = true;

    }
    IEnumerator OpenHand()
    {
        while (handMovements.opening)
        {
            handMovements.OpenHands();
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        holding = false;
        robotMoving = true;
        robotHandMoved = false;
    }
    private void IRLineTrack()
    {
        if (InsideTrack())
        {
            Drive(transform.position + input * Time.fixedDeltaTime * robotspeed);
        }
        else
        {
            if (!rightSensor.hitGround && leftSensor.hitGround)
                transform.Rotate(0, -turnRatio, 0);

            else transform.Rotate(0, turnRatio, 0);

        }
    }
    private bool InsideTrack()
    {
        return rightSensor.hitGround && leftSensor.hitGround;
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
