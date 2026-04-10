using UnityEngine;
using Unity;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
  
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(0);
        }
    }
}
