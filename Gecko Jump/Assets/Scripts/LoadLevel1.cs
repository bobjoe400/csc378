using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
  
public class SceneSelectButton : MonoBehaviour
{
    public void LoadSceneSelect()
    {
        SceneManager.LoadScene("Level 1");
    }
}