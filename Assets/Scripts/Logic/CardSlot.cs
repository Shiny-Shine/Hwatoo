using System.Collections;
using System.Collections.Generic;

// 바닥에 놓인 카드 슬롯, 같은 번호의 카드를 묶어서 보관.
public class CardSlot
{
    // 바닥 슬롯 중 해당 슬롯의 번호
    public int position { get; private set; }
    public List<Card> cards { get; private set; }

    public CardSlot(int position)
    {
        cards = new List<Card>();
        this.position = position;
        Reset();
    }

    public void Reset()
    {
        cards.Clear();
    }

    public bool IsSame(int num)
    {
        if (cards.Count <= 0)
            return false;

        return cards[0].number == num;
    }

    public void AddCard(Card card)
    {
        cards.Add(card);
    }

    public bool IsEmpty()
    {
        return cards.Count <= 0;
    }
}