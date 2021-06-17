using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GazeableObject : MonoBehaviour
{

    public bool isTransformable = false;
    private int objectLayer;
    private const int IGNORE_RAYCAST_LAYER = 2;

    private Vector3 initialObjectRotation;
    private Vector3 initialPlayerRotation;

    private Vector3 initialObjectScale;

    public virtual void OnGazeEnter(RaycastHit hitInfo)
    {
        Debug.Log("Gaze Entered on" + gameObject.name);
        if (isTransformable && (Player.instance.activeMode == InputMode.TRANSLATE || Player.instance.activeMode == InputMode.ROTATE || Player.instance.activeMode == InputMode.SCALE))
        {
            GetComponentInChildren<cakeslice.Outline>().eraseRenderer = false;
        }
    }

    public virtual void OnGaze(RaycastHit hitInfo)
    {
        Debug.Log("Gaze Hold on" + gameObject.name);
    }

    public virtual void OnGazeExit()
    {
        Debug.Log("Gaze Exit" + gameObject.name);
        if (isTransformable)
        {
            GetComponentInChildren<cakeslice.Outline>().eraseRenderer = true;
        }
    }
    public virtual void OnPress(RaycastHit hitInfo)
    {
        Debug.Log("Button Press");
        if (isTransformable)
        {
            objectLayer = gameObject.layer;
            gameObject.layer = IGNORE_RAYCAST_LAYER;

            initialObjectRotation = transform.rotation.eulerAngles;
            initialPlayerRotation = Camera.main.transform.rotation.eulerAngles;

            initialObjectScale = transform.localScale;
        }
    }

    public virtual void OnHold(RaycastHit hitInfo)
    {
        Debug.Log("Button Hold");

        if (isTransformable)
        {
            GazeTranfrom(hitInfo);
        }
    }

    public virtual void OnRelease(RaycastHit hitInfo)
    {
        Debug.Log("Button Release");
        if (isTransformable)
        {
            gameObject.layer = objectLayer;
        }

    }
    
    public virtual void GazeTranfrom(RaycastHit hitInfo)
    {
        // Call the right transform function
        switch(Player.instance.activeMode)
        {
            case InputMode.TRANSLATE:
                GazeTranslate(hitInfo);
                break;

            case InputMode.ROTATE:
                GazeRotate(hitInfo);
                break;

            case InputMode.SCALE:
                GazeScale(hitInfo);
                break;
        }
    }

    public virtual void GazeTranslate(RaycastHit hitInfo)
    {
        // Move the object's position
        if (hitInfo.collider != null && hitInfo.collider.GetComponent<Floor>())
        {
            transform.position = hitInfo.point; 
        }
    }
    
    public virtual void GazeRotate(RaycastHit hitInfo)
    {
        // Change the object's oreintation(rotation)
        float rotationSpeed = 10f;
        Vector3 currentPlayerRotation  = Camera.main.transform.rotation.eulerAngles;
        Vector3 currentObjectRotation = transform.rotation.eulerAngles;

        Vector3 rotationDelta = currentPlayerRotation - initialPlayerRotation;

        Vector3 newRotation = new Vector3(currentObjectRotation.x, initialObjectRotation.y + (rotationDelta.y * rotationSpeed), currentObjectRotation.z);
        transform.rotation = Quaternion.Euler(newRotation);

    }
    
    public virtual void GazeScale(RaycastHit hitInfo)
    {
        // Resize the object
        float scaleSpeed = 0.1f;

        float scaleFactor = 1;

        Vector3 currentPlayerRotation = Camera.main.transform.eulerAngles;
        Vector3 rotationDelta = currentPlayerRotation - initialPlayerRotation;
        
        // If user looking up
        if (rotationDelta.x < 0 && rotationDelta.x > -180 || rotationDelta.x > 180 && rotationDelta.x < 360)
        {
            // If greater than 180, map it between 0 - 180
            if (rotationDelta.x > 180)
            {
                rotationDelta.x = 360 - rotationDelta.x;
            }
            scaleFactor = 1 + Mathf.Abs(rotationDelta.x) * scaleSpeed;
        }
        else 
        {
            if (rotationDelta.x < -180)
            {
                rotationDelta.x = 360 + rotationDelta.x;
            }
            scaleFactor = Mathf.Max(0.1f, 0.1f - (Mathf.Abs(rotationDelta.x) * (1/scaleSpeed))/ 180);
        }

        transform.localScale = scaleFactor * initialObjectScale;
    }
}
