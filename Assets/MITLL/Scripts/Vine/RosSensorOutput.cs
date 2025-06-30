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
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using UnityEngine;
using UnityEngine.Profiling;

public class RosSensorOutput : MonoBehaviour
{
    public Camera rgbCam;
    public Camera depthCam;
    
    public string posRotTopic = "pos_rot";

    public string rgbCamTopic = "rgb_cam";

    public string depthCamTopic = "depth_cam";

    public float posRotPublishFreq = 0.2f;
    
    public float camPublishFreq = 0.5f;

    private float posRotTimer = 0f;
    private float camTimer = 0f;
    
    private ROSConnection ros;
    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RosIPAddress = CustomArgs.Instance.ROSIP;
        ros.RosPort = (int)CustomArgs.GetWithDefault("rosport", 10000f);
        ros.Connect();
        ros.RegisterPublisher<PoseMsg>(posRotTopic);
        ros.RegisterPublisher<ImageMsg>(rgbCamTopic);
        ros.RegisterPublisher<ImageMsg>(depthCamTopic);
    }

    void TimedPublish(ref float timer, float freq, Action callback)
    {
        timer += Time.deltaTime;

        if (timer > freq)
        {
            callback?.Invoke();

            timer = 0f;
        }
    }

    void PublishPose()
    {
        PoseMsg pose = new PoseMsg
        (
        new PointMsg
                (
                    transform.position.x,
                    transform.position.y,
                    transform.position.z
                ),
                new QuaternionMsg
                (
                    transform.rotation.x,
                    transform.rotation.y,
                    transform.rotation.z,
                    transform.rotation.w
                )
        );
        ros.Publish(posRotTopic, pose);
    }

    void PublishImages()
    {
        SendImage(rgbCam,rgbCamTopic);
        SendImage(depthCam,depthCamTopic);
    }

    private Byte[] GetRenderTarget(Camera targetCamera)
    {
        //Inspired by https://discussions.unity.com/t/publish-game-camera-stream-via-ros/844448 
        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = targetCamera.targetTexture;
        
        targetCamera.Render();

        Texture2D camTexture = new Texture2D(targetCamera.targetTexture.width, targetCamera.targetTexture.height);
        camTexture.ReadPixels(new Rect(0,0,targetCamera.targetTexture.width,targetCamera.targetTexture.height),0,0);
        camTexture.Apply();
        RenderTexture.active = rt;
        
        return camTexture.EncodeToPNG();
    }
    void SendImage(Camera targetCamera, string topicName)
    {
        //Inspired by https://discussions.unity.com/t/publish-game-camera-stream-via-ros/844448 
        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = targetCamera.targetTexture;
        targetCamera.Render();
        Texture2D camTexture = new Texture2D(targetCamera.targetTexture.width, targetCamera.targetTexture.height);
        
        camTexture.ReadPixels(new Rect(0,0,targetCamera.targetTexture.width,targetCamera.targetTexture.height),0,0);
        camTexture.Apply();
        RenderTexture.active = rt;
        ImageMsg imageMsg = camTexture.ToImageMsg(new HeaderMsg());
        ros.Publish(topicName, imageMsg);
    }
    
    
    // Update is called once per frame
    void Update()
    {
        if (ros != null)
        {
           TimedPublish(ref posRotTimer, posRotPublishFreq, PublishPose);
           TimedPublish(ref camTimer, camPublishFreq, PublishImages);     
        }
    }
}
