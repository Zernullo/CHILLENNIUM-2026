using UnityEngine;
using UnityEngine.UI; 
using System.Collections;

public class FlashSpell : MonoBehaviour
{
    private RawImage whiteRawImage; 
    private AudioSource bangNoise;

    void Awake()
    {
       
        GameObject imageObj = GameObject.FindGameObjectWithTag("WhiteImage");
        
        if (imageObj != null) 
        {
           
            whiteRawImage = imageObj.GetComponent<RawImage>();
        }

        GameObject audioObj = GameObject.FindGameObjectWithTag("BangNoise");
        if (audioObj != null) 
        {
            bangNoise = audioObj.GetComponent<AudioSource>();
        }
    }

    public void ActivateFlash()
    {
        if (whiteRawImage != null)
        {
            StopAllCoroutines();
            StartCoroutine(WhiteFade());
        }
        else
        {
            Debug.LogError("FlashSpell: RawImage component not found on object tagged 'WhiteImage'!");
        }
    }

    private IEnumerator WhiteFade()
    {
        whiteRawImage.color = new Color(1, 1, 1, 1);
        if (bangNoise != null) bangNoise.Play();

        float fadeAlpha = 1f;
        
        while (fadeAlpha > 0)
        {
            fadeAlpha -= 0.02f; 
            whiteRawImage.color = new Color(1, 1, 1, fadeAlpha);
            
            yield return new WaitForSeconds(0.02f);
        }

        whiteRawImage.color = new Color(1, 1, 1, 0);
    }
}