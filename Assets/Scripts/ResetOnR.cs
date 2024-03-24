using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetOnR : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
