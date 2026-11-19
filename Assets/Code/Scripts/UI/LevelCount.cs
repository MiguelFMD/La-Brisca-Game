using UnityEngine;
using TMPro;

public class LevelCount : MonoBehaviour
{
    private TMP_Text textCount;
    int curretLevel = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textCount = GetComponent<TMP_Text>();
        ResetCount();
    }

    public void AddCount()
    {
        curretLevel++;
        textCount.text = "Level: " + curretLevel;
    }

    public void ResetCount()
    {
        curretLevel = 1;
        textCount.text = "Level: " + curretLevel;
    }

}
