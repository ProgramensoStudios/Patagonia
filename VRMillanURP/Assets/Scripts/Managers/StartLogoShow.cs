using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartLogoShow : MonoBehaviour
{
    [SerializeField] private Image[] logoImages;
    [SerializeField] private float visibleTime = 5f;
    [SerializeField] private float fadeDuration = 2f;

    private void Start()
    {
        if (logoImages.Length > 0)
            StartCoroutine(LogoRoutine());
    }

    private IEnumerator LogoRoutine()
    {
        //  Asegurar que todas empiecen visibles
        foreach (var img in logoImages)
        {
            if (img == null) continue;
            Color c = img.color;
            img.color = new Color(c.r, c.g, c.b, 1f);
        }

        // Espera
        yield return new WaitForSeconds(visibleTime);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            float alpha = Mathf.Lerp(1f, 0f, smoothT);

            foreach (var img in logoImages)
            {
                if (img == null) continue;

                Color c = img.color;
                img.color = new Color(c.r, c.g, c.b, alpha);
            }

            yield return null;
        }

        // Desactivar todas
        foreach (var img in logoImages)
        {
            if (img == null) continue;

            Color c = img.color;
            img.color = new Color(c.r, c.g, c.b, 0f);
            img.gameObject.SetActive(false);
        }
    }
}