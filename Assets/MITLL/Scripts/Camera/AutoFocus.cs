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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AutoFocus : MonoBehaviour
{
    // Start is called before the first frame update

    public float minFocusDistance;
    public float maxFocusDistance;
    public float focusSpeed;
    public float focusSensorRadius;
    public VolumeProfile cameraVolume;
    private DepthOfField dof;
    private MinFloatParameter curDistance;
    void Start()
    {
        cameraVolume.TryGet<DepthOfField>(out dof);
        curDistance = dof.focusDistance;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position,focusSensorRadius, transform.forward, out hit))
        {
            if (hit.distance > maxFocusDistance)
            {
                hit.distance = maxFocusDistance;
            }

            if (hit.distance < minFocusDistance)
            {
                hit.distance = minFocusDistance;
            }

            curDistance.value = Mathf.Lerp(curDistance.value, hit.distance, focusSpeed);
        }
    }
}
