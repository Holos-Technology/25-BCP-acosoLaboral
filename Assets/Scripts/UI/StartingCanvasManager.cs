using UnityEngine;

public class StartingCanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject[] panels;
    private int index = 0;

    public void NextPanel()
    {
        panels[index].SetActive(false);
        index++;
        panels[index].SetActive(true);
    }

    public void PreviousPanel()
    {
        panels[index].SetActive(false);
        index--;
        panels[index].SetActive(true);
    }
}
