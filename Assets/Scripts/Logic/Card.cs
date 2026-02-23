using System.Collections;
using System.Collections.Generic;
using System;

public enum CardType
{
	P, // 피
	K, // 광
	Y, // 열
	T, // 띠
	SpCount, // 쌍피 카드 수
	PValue, // 피 점수(쌍피 포함)
	GodoriCount, // 고도리
	HongdanCount, // 홍단
	CheongdanCount, // 청단
	ChodanCount, // 초단
	BiKwangCount, // 비광
	NineYeolCount // 9월 열끗 선택
}

public enum CardStat
{
	// 고도리, 초단, 청단, 홍단, 쌍피, 
	None,
	Godori,
	Chodan,
	Cheongdan,
	Hongdan,
	Sp
}

public class Card
{
	public CardType CType { get; }
	public CardStat CStat { get; }

	// 1 ~ 12월
	public int number { get; }

	// 1 ~ 4
	public int position { get; }
	
	bool IsRealCardType(CardType type)
	{
		return type == CardType.P || type == CardType.K || type == CardType.Y || type == CardType.T;
	}

	// 생성자
	public Card(int num, int pos, CardType type, CardStat stat = CardStat.None)
	{
		if (!IsRealCardType(type))
			throw new ArgumentException($"올바른 카드 타입이 아닙니다.: {type}", nameof(type));

		
		this.number = num;
		this.position = pos;
		this.CType = type;
		this.CStat = stat;
	}
}