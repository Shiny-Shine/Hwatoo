using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager
{
    private const int slotCnt = 12;
    private List<Card> startCards;

    // 같은 번호를 하나의 슬롯에 묶어서 보관.
    public List<CardSlot> slots { get; private set; }

    public FloorManager()
    {
        slots = new List<CardSlot>();

        // 바닥에 12개까지 카드를 놓을 수 있음.
        for (int i = 1; i <= slotCnt; i++)
            slots.Add(new CardSlot(i));

        startCards = new List<Card>();
    }

    public void reset()
    {
        startCards.Clear();
        for (int i = 1; i <= slotCnt; i++)
        {
            slots[i].reset();
        }
    }

    public bool isEmpty()
    {
        for (int i = 1; i < slots.Count; i++)
        {
            if (!slots[i].isEmpty())
                return false;
        }

        return true;
    }

    public void addStartCard(Card card)
    {
        startCards.Add(card);
    }

    CardSlot findSlot(int num)
    {
        CardSlot slot = slots.Find(o => o.isSame(num));
        return slot;
    }

    CardSlot findEmptySlot()
    {
        CardSlot slot = slots.Find(o => o.isEmpty());
        return slot;
    }

    public int floorCardCount()
    {
        int cnt = 0;
        for (int i = 1; i <= slots.Count; i++)
            cnt += slots[i].cards.Count;

        return cnt;
    }

    public int getSlotStack(int num)
    {
        CardSlot slot = findSlot(num);
        if (slot == null)
            return 0;
        return slot.cards.Count;
    }

    // 해당 위치에 카드를 놓는다.
    public void addCard(Card card)
    {
        // 해당 카드가 있는 슬롯을 찾고
        CardSlot curSlot = findSlot(card.number);
        // 없다면 빈 슬롯 아무데나 놓기
        if (curSlot == null)
        {
            curSlot = findEmptySlot();
            curSlot.addCard(card);
            return;
        }

        slots[curSlot.position].addCard(card);
    }

    public void removeCard(Card card)
    {
        CardSlot slot = findSlot(card.number);
        // 비어있으면 그냥 리턴
        if (slot != null)
        {
            slot.removeCard(card);
        }
        else
            UnityEngine.Debug.Log("[delCard]CardSlot is Empty");
    }

    public Card getCard(int num)
    {
        CardSlot slot = findSlot(num);

        if (slot == null)
            return null;

        return slot.cards[0];
    }

    public List<Card> removeBonusCard()
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

    public void refreshFloor()
    {
        for (int i = 1; i < startCards.Count; i++)
            addCard(startCards[i]);

        startCards.Clear();
    }

    public List<Card> GetMatchingCards(int number)
    {
        var slot = findSlot(number);
        return slot?.cards ?? new List<Card>();
    }
}