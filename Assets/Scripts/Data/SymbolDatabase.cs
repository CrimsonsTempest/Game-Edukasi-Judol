using UnityEngine;

[CreateAssetMenu(fileName = "NewSymbolDatabase", menuName = "Slot/Symbol Database")]
public class SymbolDatabase : ScriptableObject
{
    [Header("Slot Symbols")]
    public Sprite[] symbols;
}
