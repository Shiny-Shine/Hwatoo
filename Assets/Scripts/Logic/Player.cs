using System;
using System.Collections;
using System.Collections.Generic;

public class Player
{
	private int playerIdx;
	private List<Card> handCard;
	private List<Card>[] fieldCard = new List<Card>[4];
	private Dictionary<CardType, int> typeCount;
	private int nextGoMinScore;

	public int score { get; private set; }
	public int goCnt { get; private set; }
	public int fuckCnt { get; private set; }
	public int bombCnt { get; private set; }

	public int HandCardCount => handCard.Count;
	public bool HasPendingNineYeolChoice => typeCount[CardType.NineYeolCount] > 0;
	public bool CanGoStop => !HasPendingNineYeolChoice && score >= nextGoMinScore;


	public Player(int idx)
	{
		playerIdx = idx;
		handCard = new List<Card>();
		typeCount = new Dictionary<CardType, int>();
		Reset();
	}

	public void Reset()
	{
		score = 0;
		goCnt = 0;
		fuckCnt = 0;
		nextGoMinScore = 7;
		handCard.Clear();
		typeCount.Clear();
		foreach (CardType type in Enum.GetValues(typeof(CardType)))
			typeCount[type] = 0;
		for (int i = 0; i < fieldCard.Length; i++)
			fieldCard[i] = new List<Card>();
	}
	
	public List<Card> GetHandSnapshot()
	{
		return new List<Card>(handCard);
	}
	
	public List<Card>[] GetFieldSnapshot()
	{
		List<Card>[] snapshot = new List<Card>[4];
		for (int i = 0; i < fieldCard.Length; i++)
			snapshot[i] = new List<Card>(fieldCard[i]);
		return snapshot;
	}
	
	public int GetTypeCount(CardType type) => typeCount[type];

	public void AddPlayerHand(Card card)
	{
		handCard.Add(card);
	}
	
	void RecalculateScore()
	{
		score = ScoreCalculator.CalculateBaseScore(this);
	}

	public bool ApplyGo()
	{
		if (!CanGoStop) return false;
		goCnt++;
		nextGoMinScore = score + 1; // 다음 Go는 최소 1점 오른 뒤 가능
		return true;
	}

	// asSsangP: true면 쌍피로 변환, false면 열로 유지(확정만 함)
	public bool ResolveNineYeolChoice(bool asSsangP)
	{
		if (typeCount[CardType.NineYeolCount] <= 0)
			return false;

		typeCount[CardType.NineYeolCount] -= 1;

		// 열로 유지 확정
		if (!asSsangP)
		{
			RecalculateScore();
			return true;
		}

		// 열 -> 쌍피 변환
		if (typeCount[CardType.Y] <= 0) return false;

		typeCount[CardType.Y] -= 1;
		typeCount[CardType.P] += 1;
		typeCount[CardType.PValue] += 2;
		typeCount[CardType.SpCount] += 1;

		RecalculateScore();
		return true;
	}

	public void AddCardFloor(Card card)
	{
		if (card == null) return;
		
		switch (card.CType)
		{
			case CardType.P:
				typeCount[CardType.P] += 1;
				fieldCard[(int)CardType.P].Add(card);
				bool isSsangP = card.CStat == CardStat.Sp;
				typeCount[CardType.PValue] += isSsangP ? 2 : 1;
				if (isSsangP) typeCount[CardType.SpCount] += 1;
				break;

			case CardType.K:
				typeCount[CardType.K] += 1;
				fieldCard[(int)CardType.K].Add(card);
				if (IsBiGwang(card)) typeCount[CardType.BiKwangCount] = 1;
				break;

			case CardType.Y:
				typeCount[CardType.Y] += 1;
				fieldCard[(int)CardType.Y].Add(card);
				if (card.CStat == CardStat.Godori) typeCount[CardType.GodoriCount] += 1;
				// 9월 열끗(쌍피 선택 가능) -> 일단 열로 먹고 선택 대기만 증가
				if (card.number == 9 && card.CStat == CardStat.Sp)
					typeCount[CardType.NineYeolCount] += 1;
				break;

			case CardType.T:
				typeCount[CardType.T] += 1;
				fieldCard[(int)CardType.T].Add(card);
				if (card.CStat == CardStat.Hongdan) typeCount[CardType.HongdanCount] += 1;
				if (card.CStat == CardStat.Cheongdan) typeCount[CardType.CheongdanCount] += 1;
				if (card.CStat == CardStat.Chodan) typeCount[CardType.ChodanCount] += 1;
				break;
		}
		RecalculateScore();
	}
	
	// 피 한 장 뺏기: 일반피 우선, 없으면 쌍피 1장
	public bool TakeOnePiCard(out bool isSsangPi)
	{
		isSsangPi = false;

		if (typeCount[CardType.P] <= 0)	// 피 한장 없는 그지쉐끼 컷
			return false;

		int normalPiCount = typeCount[CardType.P] - typeCount[CardType.SpCount];

		typeCount[CardType.P] -= 1;

		if (normalPiCount > 0)	// 일반피가 남아있는 경우 일반피 한장 & 피 점수 1점 깎음
		{
			typeCount[CardType.PValue] -= 1;
			RecalculateScore();
			return true;
		}

		// 일반피는 없지만 쌍피가 남아있는 경우 쌍피 한장 & 피 점수 2점 깎음
		if (typeCount[CardType.SpCount] > 0)
		{
			typeCount[CardType.SpCount] -= 1;
			typeCount[CardType.PValue] -= 2;
			isSsangPi = true;
			RecalculateScore();
			return true;
		}

		return false;
	}

	public void AddStolenPiCard(bool isSsangPi)
	{
		typeCount[CardType.P] += 1;
		typeCount[CardType.PValue] += isSsangPi ? 2 : 1;
		if (isSsangPi) typeCount[CardType.SpCount] += 1;
		RecalculateScore();
	}

	// public bool PopPlayerFloor(CardType type, int cnt)
	// {
	// 	if (cnt <= 0) return false;
	// 	if (!typeCount.TryGetValue(type, out var value)) return false;
	// 	if (cnt > value) return false;
	// 	typeCount[type] -= cnt;
	//
	// 	return true;
	// }

	public Card PopPlayerHand(Card selCard)
	{
		if (selCard == null) return null;

		Card card = handCard.Find(o =>
			o.number == selCard.number &&
			o.CType == selCard.CType &&
			o.position == selCard.position);

		if (card == null) return null;
		handCard.Remove(card);
		return card;
	}

	public void GetFuck()
	{
		fuckCnt++;
	}
	
	bool IsBiGwang(Card card)
	{
		// 12월 광은 비광
		return card.CType == CardType.K && card.number == 12 && card.position == 1;
	}
}