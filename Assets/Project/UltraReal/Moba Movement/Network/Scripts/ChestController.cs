using UnityEngine;

public class ChestController : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] ParticleSystem coinRain;
    public bool isOpened;
    // Start is called before the first frame update
    private void Start()
    {
        anim = GetComponent<Animator>();
        isOpened = false;
    }
    public void OpenChest()
    {
        anim.SetTrigger("Open");
        isOpened = true;    
    }
    public void StartCoinRain() 
    {
        coinRain.Play();
    }
}
