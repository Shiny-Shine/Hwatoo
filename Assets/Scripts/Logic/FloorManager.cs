using System.Collections;
using System.Collections.Generic;

public class FloorManager
{
	private const int SlotCnt = 12;
	private List<Card> startCards;

	// 같은 번호를 하나의 슬롯에 묶어서 보관.
	private readonly List<CardSlot> slots;
	public int SlotCount => slots.Count;

	public FloorManager()
	{
		slots = new List<CardSlot>();

		// 바닥에 12개까지 카드를 놓을 수 있음.
		for (int i = 0; i < SlotCnt; i++)
			slots.Add(new CardSlot(i));

		startCards = new List<Card>();
	}

	// 슬롯 인덱스에 있는 카드 개수 반환.
	public int GetSlotCardCountByIndex(int index)
	{
		if (index < 0 || index >= slots.Count) return 0;
		return slots[index].Cards.Count;
	}

	public void Reset()
	{
		startCards.Clear();
		for (int i = 0; i < SlotCnt; i++)
		{
			slots[i].Reset();
		}
	}

	public bool IsEmpty()
	{
		for (int i = 0; i < slots.Count; i++)
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
		for (int i = 0; i < slots.Count; i++)
			cnt += slots[i].Cards.Count;

		return cnt;
	}

	public int GetSlotStack(int num)
	{
		CardSlot slot = FindSlot(num);
		if (slot == null)
			return 0;
		return slot.Cards.Count;
	}

	// 해당 위치에 카드를 놓는다.
	public CardSlot PutCard(Card card)
	{
		if (card == null) return null;
		// 해당 카드가 있는 슬롯을 찾고
		CardSlot curSlot = FindSlot(card.number);
		// 없다면 빈 슬롯 아무데나 놓기
		if (curSlot == null)
		{
			curSlot = FindEmptySlot();
			if (curSlot == null) return null; // 슬롯 부족 방어
		}

		curSlot.PushCard(card);
		return curSlot;
	}

	public void RefreshFloor()
	{
		for (int i = 0; i < startCards.Count; i++)
			PutCard(startCards[i]);

		startCards.Clear();
	}

	public bool GetMatchingCards(int number)
	{
		var slot = FindSlot(number);
		return slot != null;
	}

	// 렌더링 연결용
	public List<Card>[] GetFloorSlotsSnapshot()
	{
		List<Card>[] result = new List<Card>[SlotCnt];
		for (int i = 0; i < SlotCnt; i++)
			result[i] = new List<Card>(slots[i].Cards);
		return result;
	}
}