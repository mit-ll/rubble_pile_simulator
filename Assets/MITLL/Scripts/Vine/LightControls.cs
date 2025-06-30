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

public class LightControls : MonoBehaviour
{
    private Transform lightTransform;

    private Light lightObj;

    private CustomArgs customArgs;

    private RandomManager randomManager;
    // Start is called before the first frame update
    void Start()
    {
        customArgs = CustomArgs.Instance;
        randomManager = RandomManager.Instance;
        lightTransform = gameObject.transform;
        lightObj = GetComponent<Light>();
        RandomizeRotation();
    }

    private void InitializeLight()
    {
        // 0 Spot
        // 1 Directional
        // 2 Point
        int lightType = (int)CustomArgs.GetWithDefault("lighttype", 1f);
        switch (lightType)
        {
            case 0:
                lightObj.type = LightType.Spot;
                break;
            case 1:
                lightObj.type = LightType.Directional;
                break;
            case 2:
                lightObj.type = LightType.Point;
                break;
        }
        
        lightObj.intensity = CustomArgs.GetWithDefault("lightintensity", 10f);
    }
    
    public void SetPosRot(Vector3 pos, Quaternion quat)
    {
        lightTransform.position = pos;
        lightTransform.rotation = quat;
    }

    public void RandomizeRotation()
    {
        float x = randomManager.GetDynamicFloat(0f, 180f);
        float y = randomManager.GetDynamicFloat(0f, 180f);
        float z = randomManager.GetDynamicFloat(0f, 180f);

        Vector3 rot = new Vector3(x, y, z);
        
        lightTransform.rotation = Quaternion.Euler(rot);
    }
    
    public void RandomizePosition()
    {
        float x = randomManager.GetDynamicFloat(0f, 180f);
        float y = randomManager.GetDynamicFloat(0f, 180f);
        float z = randomManager.GetDynamicFloat(0f, 180f);

        Vector3 pos = new Vector3(x, y, z);

        lightTransform.position = pos;
    }
}
