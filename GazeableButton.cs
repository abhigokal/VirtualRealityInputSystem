using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class GazeableButton : GazeableObject
{
    protected VRcanvas parentPanel;
    

    void Start() 
    {
        parentPanel = GetComponentInParent<VRcanvas>();    
    }

    public void setButtonColor(Color buttonColor)
    {
        GetComponent<Image>().color = buttonColor;
    }
    public override void OnPress(RaycastHit hitInfo)
    {
        base.OnPress(hitInfo);
        if (parentPanel != null)
        {
            parentPanel.setActiveButton(this);
        }
        else
        {
            Debug.Log("Error", this);
        }
    }

}
