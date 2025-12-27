using UnityEngine;

public class FloorSlot
{
    public List<Card> cards { get; private set; }
    public int slotNumber { get; private set; }

    public FloorSlot(int number)
    {
        slotNumber = number;
        cards = new List<Card>();
    }

    public void Reset()
    {
        cards.Clear();
    }

    public bool IsEmpty()
    {
        return cards.Count <= 0;
    }

    public bool IsSame(int number)
    {
        if(cards.Count <= 0)
            return false;

        return cards[0].number == number;
    }

    public void AddCard(Card card)
    {
        cards.Add(card);
    }

    public void RemoveCard(Card card)
    {
        cards.Remove(card);
    }
}