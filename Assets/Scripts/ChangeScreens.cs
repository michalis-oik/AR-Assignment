using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScreens : MonoBehaviour
{
    public void ChangeScreen(int SceneID)
    {
        SceneManager.LoadScene(SceneID);
    }
}
