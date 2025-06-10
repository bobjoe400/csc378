using UnityEngine;
using UnityEngine.SceneManagement;
  
public class SceneSelectButton : MonoBehaviour
{
    public void LoadSceneSelect(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}