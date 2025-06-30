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
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoyInput : MonoBehaviour
{
    public VineController vine;
    
    //Actions
    public SproutControls sproutControls;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction emergencyStopAction;
    private InputAction lightIncrementAction;
    private InputAction lightDecrementAction;
    private InputAction randomizePositionAction;
    // Start is called before the first frame update

    private void Awake()
    {
        sproutControls = new SproutControls();
    }

    private void OnEnable()
    {
        moveAction = sproutControls.Player.Move;
        moveAction.Enable();
        
        lookAction = sproutControls.Player.Look;
        lookAction.Enable();
        
        emergencyStopAction = sproutControls.Player.EmergencyStop;
        emergencyStopAction.Enable();
        emergencyStopAction.performed += EmergencyStop;

        lightIncrementAction = sproutControls.Player.IncrementLight;
        lightIncrementAction.Enable();
        lightIncrementAction.performed += LightIncrement;

        lightDecrementAction = sproutControls.Player.DecrementLight;
        lightDecrementAction.Enable();
        lightDecrementAction.performed += LightDecrement;

        randomizePositionAction = sproutControls.Player.RandomizePosition;
        randomizePositionAction.Enable();
        randomizePositionAction.performed += RandomizePosition;

    }

    private void RandomizePosition(InputAction.CallbackContext context)
    {
        vine.RandomizePosition();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        emergencyStopAction.Disable();
        lightDecrementAction.Disable();
        lightIncrementAction.Disable();
        randomizePositionAction.Disable();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }
   void Move()
       {
           Vector3 movementInput = new Vector3(0, 0, moveAction.ReadValue<Vector2>().y);
           Vector3 rotationInput = new Vector3(lookAction.ReadValue<Vector2>().y, lookAction.ReadValue<Vector2>().x, 0f);
           vine.Move(movementInput, rotationInput);
       }

       private void EmergencyStop(InputAction.CallbackContext context)
       {
           vine.EmergencyStop();
       }

       private void LightIncrement(InputAction.CallbackContext context)
       {
           vine.ChangeLightIntensity(true);
       }

    private void LightDecrement(InputAction.CallbackContext obj)
    {
        vine.ChangeLightIntensity(false);
    }
    
}
