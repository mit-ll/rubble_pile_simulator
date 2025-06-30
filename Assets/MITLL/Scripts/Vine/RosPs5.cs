// DISTRIBUTION STATEMENT A. Approved for public release. Distribution is unlimited.
//  
// This material is based upon work supported by the Department of the Air Force under Air Force Contract No. FA8702-15-D-0001. Any opinions, findings, conclusions or recommendations expressed in this material are those of the author(s) and do not necessarily reflect the views of the Department of the Air Force.
//  
// © 2024 Massachusetts Institute of Technology.
// Subject to FAR52.227-11 Patent Rights - Ownership by the contractor (May 2014)
//  
// The software/firmware is provided to you on an As-Is basis
//  
// Delivered to the U.S. Government with Unlimited Rights, as defined in DFARS Part 252.227-7013 or 7014 (Feb 2014). Notwithstanding any copyright notice, U.S. Government rights in this work are defined by DFARS 252.227-7013 or DFARS 252.227-7014 as detailed above. Use of this work other than as specifically authorized by the U.S. Government may violate any copyrights that exist in this work.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Sensor;
using Unity.Robotics.ROSTCPConnector;

public class RosPs5 : MonoBehaviour
{
    ROSConnection ros;
    private JoyMsg lastMsg;
    public string topicName = "joy";
    private Rigidbody rb;
    public VineController vine;
    
    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RosIPAddress = CustomArgs.Instance.ROSIP;
        ros.RosPort = (int)CustomArgs.GetWithDefault("rosport", 10000f);
        ros.Connect();
        ros.Subscribe<JoyMsg>(topicName, UpdateControls);
       
    }

    void UpdateControls(JoyMsg joy)
    {
        Move(joy);
        ButtonInputs(joy);
    }

    private void ButtonInputs(JoyMsg joy)
    {
        ToggleLight(joy);
        ChangeLightIntensity(joy);
        ChangeLightFocus();
        Move(joy);
        EmergencyStop(joy);
        RandomizePosition(joy);
    }

    private void ChangeLightFocus()
    {
        vine.ChangeLightAngle(true);
    }

    private void ChangeLightIntensity(JoyMsg joy)
    {
        if (joy.buttons[5] == 1)
        {
            vine.ChangeLightIntensity(true);
        }

        if (joy.buttons[4] == 1)
        {
            vine.ChangeLightIntensity(false);
        }
    }

    private void ToggleLight(JoyMsg joy)
    {
        if (joy.buttons[11] == 1)
        {
            vine.ToggleLight();
        }
    }

    private void EmergencyStop(JoyMsg joy)
    {
        if (joy.buttons[12] == 1)
        {
            vine.EmergencyStop();
        }
    }

    private void RandomizePosition(JoyMsg joy)
    {
        if (joy.buttons[2] == 1)
        {
            vine.RandomizePosition();
        }
    }
    
    //The ROS node does not constantly update so we need to store the last msg
    //in order to prevent stuttering and allow for smooth control.
    void Move(JoyMsg joy)
    {
        Vector3 movementInput = new Vector3(0, 0, joy.axes[3]);
        Vector3 rotationInput = new Vector3(joy.axes[1], -joy.axes[0], 0f);
        vine.Move(movementInput, rotationInput);
        lastMsg = joy;
    }
    void FixedUpdate()
    {
        if (lastMsg != null)
        {
           Move(lastMsg); 
        }
    }
}
