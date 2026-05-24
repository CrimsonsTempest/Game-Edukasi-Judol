using UnityEngine;
using System.Collections;

public class ReelController : MonoBehaviour
{
    [Header("Reel Settings")]
    public SymbolDatabase symbolDatabase;
    public Sprite[] symbols => symbolDatabase != null ? symbolDatabase.symbols : null;

    public float spinSpeed = 25f; // Units per second
    public float symbolSpacing = 2.5f;
    public float symbolScale = 1.0f; // Scale multiplier for the symbol sprites
    
    private SpriteRenderer[] symbolRenderers;
    private bool spinning = false;
    private int visibleSymbolsCount = 5; // Enough to wrap smoothly without popping

    void Start()
    {
        // Matikan SpriteRenderer utama karena hanya digunakan untuk referensi ukuran di Editor
        SpriteRenderer mainSr = GetComponent<SpriteRenderer>();
        if (mainSr != null)
        {
            mainSr.enabled = false;
        }

        // Auto-spawn child GameObjects for the symbols
        symbolRenderers = new SpriteRenderer[visibleSymbolsCount];

        for (int i = 0; i < symbolRenderers.Length; i++)
        {
            GameObject symbolObj = new GameObject("Symbol_" + i);
            symbolObj.transform.SetParent(this.transform);
            symbolObj.transform.localScale = new Vector3(symbolScale, symbolScale, symbolScale);
            
            // Initial positioning: index 1 is roughly center (Y = 0)
            symbolObj.transform.localPosition = new Vector3(0, (i - 1) * symbolSpacing, 0);
            
            SpriteRenderer sr = symbolObj.AddComponent<SpriteRenderer>();
            sr.sprite = (symbols != null && symbols.Length > 0) ? symbols[Random.Range(0, symbols.Length)] : null;
            
            symbolRenderers[i] = sr;
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
        for (int i = 0; i < symbolRenderers.Length; i++)
        {
            symbolRenderers[i].transform.localPosition = new Vector3(0, (i - 1) * symbolSpacing, 0);
        }

        // The center symbol (index 1) is set to the target symbol
        if (symbols != null && symbols.Length > 0 && targetSymbolIndex >= 0 && targetSymbolIndex < symbols.Length)
        {
            symbolRenderers[1].sprite = symbols[targetSymbolIndex];
            
            // Randomize the other visible symbols
            symbolRenderers[0].sprite = symbols[Random.Range(0, symbols.Length)];
            symbolRenderers[2].sprite = symbols[Random.Range(0, symbols.Length)];
            symbolRenderers[3].sprite = symbols[Random.Range(0, symbols.Length)];
            symbolRenderers[4].sprite = symbols[Random.Range(0, symbols.Length)];
        }

        yield return null;
    }

    void Update()
    {
        if (spinning)
        {
            for (int i = 0; i < symbolRenderers.Length; i++)
            {
                Transform t = symbolRenderers[i].transform;
                t.localPosition += Vector3.down * spinSpeed * Time.deltaTime;

                // When a symbol goes off the bottom, wrap it around to the top
                if (t.localPosition.y <= -2f * symbolSpacing)
                {
                    float topY = t.localPosition.y + (symbolRenderers.Length * symbolSpacing);
                    t.localPosition = new Vector3(0, topY, 0);
                    
                    if (symbols != null && symbols.Length > 0)
                    {
                        symbolRenderers[i].sprite = symbols[Random.Range(0, symbols.Length)];
                    }
                }
            }
        }
    }
}