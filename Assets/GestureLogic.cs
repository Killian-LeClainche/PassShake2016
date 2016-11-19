﻿using UnityEngine;
using System.Collections;
using Leap;
using Leap.Unity;
using System.Collections.Generic;
using System.IO;

public class GestureLogic : MonoBehaviour {

    private List<Hand> gestures = new List<Hand>();//List of gesture so far
    public List<Hand> correct = new List<Hand>();  //Correct passshake
    private float holdPositionTime;                //Time that certain position is held
    private static Hand holdHand;                  //Last state of hand before position hold
    public Hand endHand;
    private RigidHand hand;
    public int tolerance;                          //leeway in mm
    private bool success;
    private string path = "./password.txt";
    public bool passwordExists;

	void Start () {
        hand.InitHand();
        CopyHand(hand.GetLeapHand());
        holdPositionTime = Time.time;
        tolerance = 20;
        loadPassword();
        success = false;
	}

    // Update is called once per frame
    void Update()
    {
        if (DetectChange(hand.GetLeapHand()))
        {
            if (Time.time - holdPositionTime >= 2500)
            {
                gestures.Add(holdHand);
            }
            holdPositionTime = Time.time;
            holdHand = hand.GetLeapHand();
        }
        if (gestures.Count - 1 == correct.Count)
            success = CheckPass();
    }

    private void loadPassword()
    {
        if (File.Exists(path))
        {
            passwordExists = true;
            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                bool done = false;
                while(done == false)
                {
                    try
                    {

                    } catch (EndOfStreamException e)
                    {
                        done = true;
                    }
                }
            }
        }
        passwordExists = false;
    }
    
    public bool getSuccess()
    {
        return success;
    }

    void CopyHand(Hand curr)
    {
        holdHand = curr;
    }

    bool DetectChange(Hand curr)
    {
        if (Mathf.Abs(curr.PalmPosition.x - holdHand.PalmPosition.x) >= tolerance)
            return false;
        else if (Mathf.Abs(curr.PalmPosition.y - holdHand.PalmPosition.y) >= tolerance)
            return false;
        else if (Mathf.Abs(curr.PalmPosition.z - holdHand.PalmPosition.z) >= tolerance)
            return false;
        for(int i = 0; i < curr.Fingers.Count; i++)
        {
            if(Mathf.Abs(curr.Fingers[i].StabilizedTipPosition.x - holdHand.Fingers[i].StabilizedTipPosition.x) >= tolerance)
                return false;
            else if (Mathf.Abs(curr.Fingers[i].StabilizedTipPosition.y - holdHand.Fingers[i].StabilizedTipPosition.y) >= tolerance)
                return false;
            else if (Mathf.Abs(curr.Fingers[i].StabilizedTipPosition.z - holdHand.Fingers[i].StabilizedTipPosition.z) >= tolerance)
                return false;
        }
        return true;
    }

    //compares inputted gesture sequence to current set PassShake. Returns true for success, false for failure.
    bool CheckPass()
    {
            for(int i = 0; i < correct.Count; i++)
            {
                for(int j = 0; j < gestures[i].Fingers.Count; j++)
                {
                    if (Mathf.Abs((gestures[i].Fingers[j].StabilizedTipPosition.x - gestures[i].PalmPosition.x) - (correct[i].Fingers[j].StabilizedTipPosition.x - correct[i].PalmPosition.x)) >= tolerance)
                        return false;
                    if (Mathf.Abs((gestures[i].Fingers[j].StabilizedTipPosition.y - gestures[i].PalmPosition.y) - (correct[i].Fingers[j].StabilizedTipPosition.y - correct[i].PalmPosition.y)) >= tolerance)
                        return false;
                    if (Mathf.Abs((gestures[i].Fingers[j].StabilizedTipPosition.z - gestures[i].PalmPosition.z) - (correct[i].Fingers[j].StabilizedTipPosition.z - correct[i].PalmPosition.z)) >= tolerance)
                        return false;
                }
            }
            return true;
    }
}
