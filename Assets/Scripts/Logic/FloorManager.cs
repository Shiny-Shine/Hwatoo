using System.Collections;
using System.Collections.Generic;

public class FloorManager
{
    private const int SlotCnt = 12;
    private List<Card> startCards;

    // 같은 번호를 하나의 슬롯에 묶어서 보관.
    public List<CardSlot> slots { get; private set; }

    public FloorManager()
    {
        slots = new List<CardSlot>();

        // 바닥에 12개까지 카드를 놓을 수 있음.
        for (int i = 1; i <= SlotCnt; i++)
            slots.Add(new CardSlot(i));

        startCards = new List<Card>();
    }

    public void Reset()
    {
        startCards.Clear();
        for (int i = 1; i <= SlotCnt; i++)
        {
            slots[i].Reset();
        }
    }

    public bool IsEmpty()
    {
        for (int i = 1; i < slots.Count; i++)
        {
            if (!slots[i].IsEmpty())
                return false;
        }

        return true;
    }

    public void AddStartCard(Card card)
    {
        startCards.Add(card);
    }

    CardSlot FindSlot(int num)
    {
        // 같은 번호의 슬롯을 찾는다(람다식 사용).
        CardSlot slot = slots.Find(o => o.IsSame(num));
        return slot;
    }

    CardSlot FindEmptySlot()
    {
        CardSlot slot = slots.Find(o => o.IsEmpty());
        return slot;
    }

    public int FloorCardCount()
    {
        int cnt = 0;
        for (int i = 1; i <= slots.Count; i++)
            cnt += slots[i].cards.Count;

        return cnt;
    }

    public int GetSlotStack(int num)
    {
        CardSlot slot = FindSlot(num);
        if (slot == null)
            return 0;
        return slot.cards.Count;
    }

    // 해당 위치에 카드를 놓는다.
    public CardSlot AddCard(Card card)
    {
        // 해당 카드가 있는 슬롯을 찾고
        CardSlot curSlot = FindSlot(card.number);
        // 없다면 빈 슬롯 아무데나 놓기
        if (curSlot == null)
        {
            curSlot = FindEmptySlot();
            curSlot.AddCard(card);
            return curSlot;
        }

        slots[curSlot.position].AddCard(card);
        return curSlot;
    }

    public void RemoveCard(Card card)
    {
        CardSlot slot = FindSlot(card.number);
        // 비어있으면 그냥 리턴
        if (slot != null)
        {
            slot.RemoveCard(card);
        }
        else
            UnityEngine.Debug.Log("[delCard]CardSlot is Empty");
    }

    public Card GetCard(int num)
    {
        CardSlot slot = FindSlot(num);

        if (slot == null)
            return null;

        return slot.cards[0];
    }

    public List<Card> RemoveBonusCard()
    {
        List<Card> bonusCards = new List<Card>();

        for (int i = 1; i <= startCards.Count; i++)
        {
            if (startCards[i].number == 13)
                bonusCards.Add(startCards[i]);
        }

        for (int i = 0; i < bonusCards.Count; ++i)
            startCards.Remove(bonusCards[i]);

        return bonusCards;
    }

    public void RefreshFloor()
    {
        for (int i = 0; i < startCards.Count; i++)
            AddCard(startCards[i]);

        startCards.Clear();
    }

    public bool GetMatchingCards(int number)
    {
        var slot = FindSlot(number);
        return slot != null;
    }
}