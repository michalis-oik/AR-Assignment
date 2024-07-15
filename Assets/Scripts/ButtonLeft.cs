using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLeft : MonoBehaviour
{
    public static ButtonLeft Instance { get; private set; }
    public event EventHandler OnLeftButtonClicked;

    private void Awake() 
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void LeftButtonClicked()
    {
        OnLeftButtonClicked?.Invoke(this,EventArgs.Empty);
    }
}
