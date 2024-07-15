using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonRight : MonoBehaviour
{
    public static ButtonRight Instance { get; private set; }
    public event EventHandler OnRightButtonClicked;

    private void Awake() 
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RightButtonClicked()
    {
        OnRightButtonClicked?.Invoke(this,EventArgs.Empty);
    }

}
