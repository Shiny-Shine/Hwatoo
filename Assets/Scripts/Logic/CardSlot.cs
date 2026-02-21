using System;
using System.Collections;
using System.Collections.Generic;

// 바닥에 놓인 카드 슬롯, 같은 번호의 카드를 묶어서 보관.
public class CardSlot
{
    // 바닥 슬롯 중 해당 슬롯의 번호
    public int position { get; private set; }
    private List<Card> cards;
    public IReadOnlyList<Card> Cards => cards;

    public CardSlot(int position)
    {
        cards = new List<Card>();
        this.position = position;
        Reset();
    }
    
    public Card PopTopCard()
    {
        if (cards.Count <= 0)
            return null;
        
        int top = cards.Count - 1;
        Card card = cards[top];
        cards.RemoveAt(top);
        return card;
    }

    public void Reset()
    {
        cards.Clear();
    }

    public bool IsSame(int num)
    {
        if (!IsEmpty())
            return cards[cards.Count - 1].number == num;
        
        return false;
    }

    public void PushCard(Card card)
    {
        cards.Add(card);
    }

    public bool IsEmpty()
    {
        return cards.Count == 0;
    }
}