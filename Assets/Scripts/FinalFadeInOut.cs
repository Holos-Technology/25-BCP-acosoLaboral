using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalFadeInOut : MonoBehaviour
{
    [SerializeField] private SceneManager load;

    public void LoadSceneOnFinalEvent()
    {
        SceneManager.LoadScene(1);
    }
}
