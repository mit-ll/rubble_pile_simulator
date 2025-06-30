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

public class GameManager : MonoBehaviour
{
    //Reset sim
    public delegate void DoReset();
    public static DoReset doReset;
    //Run init functions
    public delegate void DoInit();

    public static DoInit doInit;
    // Start is called before the first frame update

    private void Start()
    {
        StartCoroutine(LateStart(0.1f));
    }

    IEnumerator LateStart(float startDelay)
    {
        yield return new WaitForSeconds(startDelay);
        Reset();
    }
    public void Reset()
    {
        doReset?.Invoke();
    }

    public void Initialize()
    {
        doInit?.Invoke();
    }
}
