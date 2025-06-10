using UnityEngine;
using UnityEngine.SceneManagement;
  
public class SceneSelectButton : MonoBehaviour
{
    public void LoadSceneSelect()
    {
        SceneManager.LoadScene("Level 1");
    }
}