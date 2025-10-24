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

public class VineController : MonoBehaviour
{
    public GameManager manager;
    private Rigidbody rb;
    public float moveSpeed;
    public float rotSpeed;
    public Light vineLight;
    public float lightAngleIncrement;
    public float lightIntensityIncrement;
    private bool isLightOn = false;
    private Vector3 initialPos;
    private Quaternion initialRot;
    public UIController uiController;

    private RandomManager random;
    // Start is called before the first frame update
    void Start()
    {   
        random = RandomManager.Instance;
        rb = GetComponent<Rigidbody>();
        initialPos = transform.position;
        initialRot = transform.rotation;
        uiController = FindObjectOfType<UIController>();
    }

    private void OnEnable()
    {
        GameManager.doReset += Reset;
        GameManager.doInit += Initialize;
    }

    private void OnDisable()
    {
        GameManager.doReset -= Reset;
        GameManager.doInit -= Initialize;
    }

    void Reset()
    {
        transform.position = initialPos;
        transform.rotation = initialRot;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }

    public void RandomizePosition()
    {
        float x = random.GetDynamicFloat(-10, 10);
        float y = 100f;
        float z = random.GetDynamicFloat(-10, 10);

        Vector3 origin = new Vector3(x, y, z);
        
        RaycastHit hit;
        if (Physics.Raycast(origin, Vector3.down, out hit))
        {
            transform.position = new Vector3(hit.point.x, hit.point.y + 2f, hit.point.z);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            Vector3 lookdir = Vector3.zero - transform.position ;
            transform.rotation = Quaternion.LookRotation(lookdir);
            Debug.Log($"I hit {hit.collider.gameObject.name}");
        }
        
    }

    void Initialize()
    {
        rb.isKinematic = false;
    }

    public void Move(Vector3 position, Vector3 rotation)
    {
        rb.AddRelativeForce(position * moveSpeed);
        rb.AddRelativeTorque(rotation * rotSpeed);
    }

    public void ToggleLight()
    {
        isLightOn = !isLightOn;

        if (!isLightOn)
        {
            vineLight.gameObject.SetActive(false);
            return;
        }

        vineLight.gameObject.SetActive(true);
    }

    public void EmergencyStop()
    {
        manager.Reset();
    }

    public void ChangeLightIntensity(bool increment)
    {
        if (increment)
        {
            vineLight.intensity += lightIntensityIncrement;
            return;
        }

        vineLight.intensity -= lightIntensityIncrement;

    }
    public void ChangeLightAngle(bool increment)
    {
        if (increment)
        {
            vineLight.spotAngle += lightAngleIncrement;
            return;
        }

        vineLight.spotAngle -= lightAngleIncrement;

    }

    public void ToggleInstructions()
    {
        uiController.ToggleInstructions();
    }
}