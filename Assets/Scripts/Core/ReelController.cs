using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ReelController : MonoBehaviour
{
    [Header("Reel Settings")]
    public SymbolDatabase symbolDatabase;
    public Sprite[] symbols => symbolDatabase != null ? symbolDatabase.symbols : null;

    public float spinSpeed = 1000f; // Pixels per second untuk UI
    public float symbolSpacing = 150f; // Jarak antar simbol dalam piksel UI
    public float symbolScale = 1.0f; // Scale multiplier for the symbol sprites
    
    private Image[] symbolImages;
    private bool spinning = false;
    private int visibleSymbolsCount = 5; // Enough to wrap smoothly without popping

    void Start()
    {
        // Matikan Image utama karena hanya digunakan untuk referensi background/masking di Editor
        Image mainImg = GetComponent<Image>();
        if (mainImg != null)
        {
            mainImg.enabled = false;
        }

        // Auto-spawn child GameObjects for the symbols
        symbolImages = new Image[visibleSymbolsCount];

        for (int i = 0; i < symbolImages.Length; i++)
        {
            GameObject symbolObj = new GameObject("Symbol_" + i);
            symbolObj.transform.SetParent(this.transform, false);
            symbolObj.transform.localScale = new Vector3(symbolScale, symbolScale, symbolScale);
            
            RectTransform rt = symbolObj.AddComponent<RectTransform>();
            // Initial positioning: index 1 is roughly center (Y = 0)
            rt.anchoredPosition = new Vector2(0, (i - 1) * symbolSpacing);
            rt.sizeDelta = new Vector2(100, 100); // Default base size for symbol rects
            
            Image img = symbolObj.AddComponent<Image>();
            img.sprite = (symbols != null && symbols.Length > 0) ? symbols[Random.Range(0, symbols.Length)] : null;
            img.preserveAspect = true; // Penting untuk UI agar rasio sprite tetap terjaga
            
            symbolImages[i] = img;
        }
    }

    public void StartSpin()
    {
        // Normalize rotation to 0 in case the old placeholder logic was still somehow applied
        transform.localRotation = Quaternion.identity;
        spinning = true;
    }

    public IEnumerator StopSpin(int targetSymbolIndex)
    {
        spinning = false;
        
        // Snap positions neatly
        for (int i = 0; i < symbolImages.Length; i++)
        {
            symbolImages[i].rectTransform.anchoredPosition = new Vector2(0, (i - 1) * symbolSpacing);
        }

        // The center symbol (index 1) is set to the target symbol
        if (symbols != null && symbols.Length > 0 && targetSymbolIndex >= 0 && targetSymbolIndex < symbols.Length)
        {
            symbolImages[1].sprite = symbols[targetSymbolIndex];
            
            // Randomize the other visible symbols
            symbolImages[0].sprite = symbols[Random.Range(0, symbols.Length)];
            symbolImages[2].sprite = symbols[Random.Range(0, symbols.Length)];
            symbolImages[3].sprite = symbols[Random.Range(0, symbols.Length)];
            symbolImages[4].sprite = symbols[Random.Range(0, symbols.Length)];
        }

        yield return null;
    }

    void Update()
    {
        if (spinning)
        {
            for (int i = 0; i < symbolImages.Length; i++)
            {
                RectTransform t = symbolImages[i].rectTransform;
                t.anchoredPosition += Vector2.down * spinSpeed * Time.deltaTime;

                // When a symbol goes off the bottom, wrap it around to the top
                if (t.anchoredPosition.y <= -2f * symbolSpacing)
                {
                    float topY = t.anchoredPosition.y + (symbolImages.Length * symbolSpacing);
                    t.anchoredPosition = new Vector2(t.anchoredPosition.x, topY);
                    
                    if (symbols != null && symbols.Length > 0)
                    {
                        symbolImages[i].sprite = symbols[Random.Range(0, symbols.Length)];
                    }
                }
            }
        }
    }
}