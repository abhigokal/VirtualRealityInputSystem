using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeSystem : MonoBehaviour
{

    public GameObject reticle;
    private Color inactiveReticleColor = Color.white;
    private Color activeReticleColor = Color.blue;

    private GazeableObject currentGazeObject;
    private RaycastHit lastHit;
    private GazeableObject currentSelectedObject;

    // Start is called before the first frame update
    void Start()
    {
        SetReticleColor(inactiveReticleColor);
    }

    // Update is called once per frame
    void Update()
    {
        processGaze();
        CheckforInput(lastHit);
    }
    public void processGaze()
    {
        Ray raycastRay = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        Debug.DrawRay(raycastRay.origin, raycastRay.direction * 100);

        if (Physics.Raycast(raycastRay, out hitInfo))
        {
            GameObject hitObj = hitInfo.collider.gameObject;
            GazeableObject gazeObj = hitObj.GetComponentInParent<GazeableObject>();

            if (gazeObj != null)
            {
                if (gazeObj != currentGazeObject)
                {
                    ClearCurrentObject();
                    currentGazeObject = gazeObj;
                    currentGazeObject.OnGazeEnter(hitInfo);
                    SetReticleColor(activeReticleColor);
                }
                else 
                {
                    currentGazeObject.OnGaze(hitInfo);
                }
            }
            else
            {
                ClearCurrentObject();
            }
            lastHit = hitInfo;

        }
        else
        {
            ClearCurrentObject();
        }
    }

    private void SetReticleColor(Color reticleColor)
    {

        reticle.GetComponent<Renderer>().material.SetColor("_Color", reticleColor);

    }

    private void CheckforInput(RaycastHit hitInfo)
    {
        //Check for down input
        if (Input.GetMouseButtonDown(0) && currentGazeObject != null)
        {
            currentSelectedObject = currentGazeObject;
            currentSelectedObject.OnPress(hitInfo);
        }
        else if (Input.GetMouseButton(0) && currentSelectedObject != null)
        {
            currentSelectedObject.OnHold(hitInfo);
        }
        else if (Input.GetMouseButtonUp(0)  && currentSelectedObject != null)
        {
            currentSelectedObject.OnRelease(hitInfo);
            currentSelectedObject = null;
        }
        //Check for up input


    }

    private void ClearCurrentObject()
    {
        if (currentGazeObject != null)
        {
            currentGazeObject.OnGazeExit();
            SetReticleColor(inactiveReticleColor);
            currentGazeObject = null;
        }
    }
}
