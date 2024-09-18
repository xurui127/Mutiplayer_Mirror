using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    public TMP_Text goldCount;
    public TMP_Text healthTxt;

    private int count;

    private void Awake()
    {
        instance = this;
    }
    public void AddGold(int num)
    {
        count += num;
        goldCount.text = count.ToString();
    }

    public void UpdateHealth(int currentHealth)
    {
        healthTxt.text = currentHealth.ToString();
    }
}
