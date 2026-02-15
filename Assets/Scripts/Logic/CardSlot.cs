using System.Collections;
using System.Collections.Generic;

// 바닥에 놓인 카드 슬롯, 같은 번호의 카드를 묶어서 보관.
public class CardSlot
{
    // 바닥 슬롯 중 해당 슬롯의 번호
    public int position { get; private set; }
    public Stack<Card> cards { get; private set; }

    public CardSlot(int position)
    {
        cards = new Stack<Card>();
        this.position = position;
        Reset();
    }
    
    public Card PopTopCard()
    {
        if (cards.Count <= 0)
            return null;

        return cards.Pop();
    }

    public void Reset()
    {
        cards.Clear();
    }

    public bool IsSame(int num)
    {
        if (!IsEmpty())
            return cards.Peek().number == num;
        
        UnityEngine.Debug.Log("[CardSlot/IsSame]slot is empty");
        return false;

    }

    public void PushCard(Card card)
    {
        if (!IsSame(card.number))
            return;
        cards.Push(card);
    }

    public bool IsEmpty()
    {
        return cards.Count <= 0;
    }
}