using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSettings : MonoBehaviour
{
    private bool isOpen = false;
    [SerializeField]
    private Image menuObj;
    [SerializeField]
    private BeastManager manager;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OpenCloseSetting();
        }
    }

    private void OpenCloseSetting()
    {
        if(!isOpen)
        {
            menuObj.rectTransform.LeanMoveX(-460, 0.5f);
            manager.OnCloseSettings();
            isOpen = true;
        }
        else
        {
            menuObj.rectTransform.LeanMoveX(-1000, 0.5f);
            manager.OnOpenSettings();
            isOpen = false;
        }
    }

}
