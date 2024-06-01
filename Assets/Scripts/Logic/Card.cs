using System.Collections;
using System.Collections.Generic;
using System;

public enum CardType : byte
{
    P, // 피
    K, // 광
    Y, // 열
    T // 띠
}

public enum CardStat : byte
{
    // 고도리, 초단, 청단, 홍단, 쌍피, 
    NONE,
    GODORI,
    CHODAN,
    CHEONGDAN,
    HONGDAN,
    SP
}

public class Card
{
    private CardType CType;
    private CardStat CStat;

    // 1 ~ 12월
    public byte number { get; private set; }

    // 1 ~ 4
    public byte position { get; private set; }

    // 생성자
    public Card(byte num, byte pos, CardType type, CardStat stat = CardStat.NONE)
    {
        this.number = num;
        this.position = pos;
        this.CType = type;
        this.CStat = CardStat.NONE;
    }

    // setter
    public void setCardType(CardType type)
    {
        this.CType = type;
    }

    // setter
    public void setCardStat(CardStat stat)
    {
        this.CStat = stat;
    }
}