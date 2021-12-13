using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLogic : MonoBehaviour
{

    public Arms arms;

    private bool closed = false;
    private bool closing = false;

    public bool opening = false;

    public GameObject touchedObject;

    private GameObject handObject;

    private Vector3 leftHandMoved;
    private Vector3 rightHandMoved;
    void Start()
    {
        handObject = GameObject.Find("hands");
    }
    private void FixedUpdate()
    {
        if(BothTouchSensorTouched() && touchedObject != null)
        {
            touchedObject.transform.SetPositionAndRotation(Vector3.Lerp(
                arms.lefthand.transform.position,
                arms.righthand.transform.position,
                0.5f), handObject.transform.rotation);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(closing)
        {
            CloseBoth();

            if(BothTouchSensorTouched())
            {
                touchedObject = arms.leftSensor.touchedSomething;
                closed = true;
                closing = false;
            }
        }
    }
    public bool BothTouchSensorTouched()
    {
        if (arms.leftSensor.touchedSomething != null && arms.rightSensor.touchedSomething != null)
            return true;

        return false;
    }
    public bool BothAreNull()
    {
        if (arms.leftSensor.touchedSomething == null && arms.rightSensor.touchedSomething == null)
            return true;

        return false;
    }
    public void StartClosing()
    {
        closing = true;
    }
    public void StartOpening()
    {
        opening = true;

    }
    public void CloseBoth()
    {
        if (!closed)
        {
            if(arms.leftSensor.touchedSomething == null)
            {
                leftHandMoved += Vector3.back * Time.deltaTime;
                arms.lefthand.transform.Translate(Vector3.back * Time.deltaTime);
            }
            if(arms.rightSensor.touchedSomething == null)
            {
                rightHandMoved += Vector3.forward * Time.deltaTime;
                arms.righthand.transform.Translate(Vector3.forward * Time.deltaTime);
            }

        }
    }
    public void OpenHands()
    {
        if (arms.leftSensor.touchedSomething != null && leftHandMoved.z <= 0)
        {
            leftHandMoved += Vector3.forward * Time.deltaTime;
            arms.lefthand.transform.Translate(Vector3.forward * Time.deltaTime);
        }
        else
            arms.leftSensor.touchedSomething = null;

        if (arms.rightSensor.touchedSomething != null && rightHandMoved.z >= 0)
        {
            rightHandMoved += Vector3.back * Time.deltaTime;
            arms.righthand.transform.Translate(Vector3.back * Time.deltaTime);
        }
        else
            arms.rightSensor.touchedSomething = null;

        if(BothAreNull())
        {
            closed = false;
            touchedObject = null;
            opening = false;
        }
    }
    public void OpenBoth()
    {

    }
    public bool armsClosed()
    {
        return closed;
    }
    [Serializable]
    public class Arms
    {
        public GameObject lefthand;
        public GameObject righthand;

        public TouchSensor leftSensor;

        public TouchSensor rightSensor;
    }
}
