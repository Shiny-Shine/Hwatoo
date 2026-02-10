using System.Collections;
using System.Collections.Generic;
using System;

public enum CardType
{
    P, // 피
    K, // 광
    Y, // 열
    T // 띠
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
    public CardType CType { get; private set; }
    public CardStat CStat { get; private set; }

    // 1 ~ 12월
    public int number { get; private set; }

    // 1 ~ 4
    public int position { get; private set; }

    // 생성자
    public Card(int num, int pos, CardType type, CardStat stat = CardStat.None)
    {
        this.number = num;
        this.position = pos;
        this.CType = type;
        this.CStat = CardStat.None;
    }

    // setter
    public void SetCardType(CardType type)
    {
        this.CType = type;
    }

    // setter
    public void SetCardStat(CardStat stat)
    {
        this.CStat = stat;
    }
}