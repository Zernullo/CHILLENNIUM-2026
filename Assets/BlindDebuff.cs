using UnityEngine;
using UnityEngine.UI; // Required for RawImage
using System.Collections;

public class BlindDebuff : MonoBehaviour
{
    private RawImage whiteRawImage; // Changed from Image to RawImage
    private AudioSource bangNoise;

    void Awake()
    {
        // Update the GetComponent to look for RawImage
        GameObject imageObj = GameObject.FindGameObjectWithTag("WhiteImage");
        if (imageObj != null) whiteRawImage = imageObj.GetComponent<RawImage>();

        GameObject audioObj = GameObject.FindGameObjectWithTag("Bang");
        if (audioObj != null) bangNoise = audioObj.GetComponent<AudioSource>();
    }

    public void ActivateFlash()
    {
        StopAllCoroutines();
        StartCoroutine(WhiteFade());
    }

    private IEnumerator WhiteFade()
    {
        // Same logic as before, RawImage still uses .color
        whiteRawImage.color = new Color(1, 1, 1, 1);
        if (bangNoise != null) bangNoise.Play();

        float fadeAlpha = 1f;
        while (fadeAlpha > 0)
        {
            fadeAlpha -= 0.05f;
            whiteRawImage.color = new Color(1, 1, 1, fadeAlpha);
            yield return new WaitForSeconds(0.05f);
        }
        
        whiteRawImage.color = new Color(1, 1, 1, 0); // Ensure it's fully invisible
    }
}