using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup layoutGroup;
    [SerializeField] private GameObject[] healthImgList;

    public float blinkDuration = 0.5f;
    public int blinkCount = 3;
    // Start is called before the first frame update
    void Start()
    {
        layoutGroup.spacing = -99f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
    }

    public void SetHealthImage(int currentHealth)
    {
        if (currentHealth > healthImgList.Length)
        {
            return;
        }
        for (int i = 0; i < healthImgList.Length; i++)
        {
            if (i > currentHealth - 1)
            {
                StartCoroutine(ReduceHealthImg(i, currentHealth));
            }
        }


    }

    private IEnumerator ReduceHealthImg(int index, int currentHealth)
    {
        SpriteRenderer spriteRenderer = healthImgList[index].GetComponent<SpriteRenderer>();

        for (int i = 0; i < blinkCount; i++)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
            yield return new WaitForSeconds(blinkDuration);

            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
            yield return new WaitForSeconds(blinkDuration);
        }

        healthImgList[index].SetActive(false);

        if (currentHealth == 4)
        {
            layoutGroup.spacing = -99.1f;
        }
        else if (currentHealth == 3)
        {
            layoutGroup.spacing = -99.3f;
        }
        else if (currentHealth == 2)
        {
            layoutGroup.spacing = -99.5f;
        }
    }


}
