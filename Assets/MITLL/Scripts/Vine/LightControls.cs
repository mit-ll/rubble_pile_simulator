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
    private bool setLightPos;
    private bool setLightRot;

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
        InitializeLight();
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
        
        lightObj.intensity = CustomArgs.GetWithDefault("lightintensity", 2f);

        // check for position and rotation
        setLightPos = CustomArgs.FloatToBool(CustomArgs.GetWithDefault("setlightpos", 0f));
        setLightRot = CustomArgs.FloatToBool(CustomArgs.GetWithDefault("setlightrot", 0f));

        if (setLightPos)
        {
            SetPos(GetPosFromArg());
        }

        if (setLightRot)
        {
            SetRot(GetRotFromArg()); 
        }
    }
    
    public void SetPos(Vector3 pos)
    {
        lightTransform.position = pos;
    }
    public void SetRot(Vector3 rot)
    {
        lightTransform.rotation = Quaternion.Euler(rot);
    }
    public void SetRot(Quaternion quat)
    {
        lightTransform.rotation = quat;
    }

    public void SetPosRot(Vector3 pos, Vector3 rot)
    {
        lightTransform.position = pos;
        lightTransform.rotation = Quaternion.Euler(rot);
    }
    public void SetPosRot(Vector3 pos, Quaternion quat)
    {
        lightTransform.position = pos;
        lightTransform.rotation = quat;
    } 
    public Vector3 GetPosFromArg()
    {
        Vector3 lightPos = Vector3.zero;

        lightPos.x = CustomArgs.GetWithDefault("lightposx", 0f);
        lightPos.y = CustomArgs.GetWithDefault("lightposy", 0f);
        lightPos.z = CustomArgs.GetWithDefault("lightposz", 0f);

        return lightPos;
    }

    public Vector3 GetRotFromArg()
    {
        Vector3 lightRot = Vector3.zero;
            
        lightRot.x = CustomArgs.GetWithDefault("lightrotx", 0f);
        lightRot.y = CustomArgs.GetWithDefault("lightroty", 0f);
        lightRot.z = CustomArgs.GetWithDefault("lightrotz", 0f);

        return lightRot;
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
