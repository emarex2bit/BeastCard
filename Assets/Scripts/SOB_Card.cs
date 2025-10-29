using UnityEngine;



[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class SOB_Card : ScriptableObject
{
    public Sprite sprite;
    public int cardNumber;
    public int cartValue;
    public CardSeed cardSeed;
    
}
public enum CardSeed
{
    Bastoni = 0,
    Spade = 1,
    Denari = 2,
    Coppe = 3
}
