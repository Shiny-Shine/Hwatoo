// 인스턴스를 만들지 않고 사용하므로 static 으로정의

public static class ScoreCalculator
{
	public static int CalculateBaseScore(Player player)
	{
		int score = 0;

		int p = player.GetTypeCount(CardType.PValue);
		if (p >= 10) score += (p - 9);

		int t = player.GetTypeCount(CardType.T);
		if (t >= 5) score += (t - 4);

		int y = player.GetTypeCount(CardType.Y);
		if (y >= 5) score += (y - 4);

		int kwang = player.GetTypeCount(CardType.K);
		int biKwang = player.GetTypeCount(CardType.BiKwangCount);

		if (kwang == 3) score += (biKwang > 0 ? 2 : 3); // 비삼광/삼광
		else if (kwang == 4) score += 4;
		else if (kwang == 5) score += 15;

		if (player.GetTypeCount(CardType.HongdanCount) >= 3) score += 3;
		if (player.GetTypeCount(CardType.CheongdanCount) >= 3) score += 3;
		if (player.GetTypeCount(CardType.ChodanCount) >= 3) score += 3;
		if (player.GetTypeCount(CardType.GodoriCount) >= 3) score += 5;

		return score;
	}
	
	public static int CalculateStopFinalScore(Player winner, Player loser)
	{
		int score = CalculateBaseScore(winner);

		// 1고 +1, 2고 +2, 3고 x2, 4고 x3, 5고 이상 x4
		if (winner.goCnt == 1) score += 1;
		else if (winner.goCnt == 2) score += 2;
		else if (winner.goCnt == 3) score *= 2;
		else if (winner.goCnt == 4) score *= 3;
		else if (winner.goCnt >= 5) score *= 4;

		int bakMultiplier = 1;

		// 피박
		if (winner.GetTypeCount(CardType.PValue) >= 10 &&
		    loser.GetTypeCount(CardType.PValue) <= 5)
		{
			bakMultiplier *= 2;
		}

		// 광박
		if (winner.GetTypeCount(CardType.K) >= 3 &&
		    loser.GetTypeCount(CardType.K) == 0)
		{
			bakMultiplier *= 2;
		}

		// 고를 하고 패배한 경우 고박
		if (loser.goCnt > 0)
		{
			bakMultiplier *= 2;
		}

		score *= bakMultiplier;

		// TODO: 흔들기/폭탄 구현 필요, 현재는 배수 로직만 구현해둠
		// // 흔들기/폭탄 배수(각각 x2 누적)
		// int extraMultiplier = 1;
		// for (int i = 0; i < winner.shakeCnt; i++) extraMultiplier *= 2;
		// for (int i = 0; i < winner.bombCnt; i++) extraMultiplier *= 2;
		//
		// score *= extraMultiplier;
		
		if (score < 1) score = 1;
		return score;
	}
}