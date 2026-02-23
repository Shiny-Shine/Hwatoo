using System.Collections;
using System.Collections.Generic;
using System;

public class HwatooEngine
{
	private CardManager cManager;
	private Queue<Card> cardQ;

	public bool IsRoundEnded { get; private set; }
	public int WinnerIdx { get; private set; } = -1;

	private FloorManager fManager;
	private Player[] players;
	public IReadOnlyList<Player> Players => players; // 외부에서 조회만 가능하도록 IReadOnlyList로 설정.

	public HwatooEngine()
	{
		cManager = new CardManager();
		fManager = new FloorManager();
		cardQ = new Queue<Card>();
		players = new Player[2];
		players[0] = new Player(0); // user
		players[1] = new Player(1); // enemy
	}

	public bool StartRound()
	{
		Reset();
		Shuffle();

		IsRoundEnded = false;
		WinnerIdx = -1;

		return CardDistribution();
	}

	private Card PopMainDeck()
	{
		if (cardQ.Count == 0) return null;
		return cardQ.Dequeue();
	}

	private void Reset()
	{
		players[0].Reset();
		players[1].Reset();
		cManager.MakeCards();
		fManager.Reset();
	}

	void Shuffle()
	{
		cardQ.Clear();
		cManager.Shuffle();
		cardQ = cManager.ExportToQ();
	}
	
	public List<Card> GetPlayerHandSnapshot(int playerIdx)
	{
		if (playerIdx < 0 || playerIdx >= players.Length) return new List<Card>();
		return players[playerIdx].GetHandSnapshot();
	}
	
	public List<Card>[] GetPlayerFieldSnapshot(int playerIdx)
	{
		if (playerIdx < 0 || playerIdx >= players.Length) return new List<Card>[4];
		return players[playerIdx].GetFieldSnapshot();
	}

	public List<Card>[] GetFloorSlotsSnapshot()
	{
		return fManager.GetFloorSlotsSnapshot();
	}

	public int GetDeckCount()
	{
		return cardQ.Count;
	}

	public int GetPlayerScore(int playerIdx)
	{
		if (playerIdx < 0 || playerIdx >= players.Length) return 0;
		return players[playerIdx].score;
	}

	// 상대방의 피를 가져옴
	void TakeJunk(int cnt, int playerIdx)
	{
		if (cnt <= 0) return;
		for (int i = 0; i < cnt; i++)
		{
			if (!players[(playerIdx + 1) % 2].TakeOnePiCard(out bool isSsangPi))
				break;
			players[playerIdx].AddStolenPiCard(isSsangPi);
		}
	}

	bool CardDistribution()
	{
		// 2번 반복
		for (int t = 0; t < 2; t++)
		{
			// 바닥 패 4장
			for (int i = 0; i < 4; i++)
			{
				Card c = PopMainDeck(); // NULL 카드가 손패/바닥으로 들어가는 경우 방지.
				if (c == null) return false;
				fManager.AddStartCard(c);
			}

			// 플레이어 5장.
			for (int i = 0; i < 5; ++i)
			{
				Card c = PopMainDeck();
				if (c == null) return false;
				players[0].AddPlayerHand(c);
			}

			// 상대 5장.
			for (int i = 0; i < 5; ++i)
			{
				Card c = PopMainDeck();
				if (c == null) return false;
				players[1].AddPlayerHand(c);
			}
		}

		fManager.RefreshFloor();
		return true;
	}

	// 카드 슬롯의 모든 카드를 플레이어가 가져감.
	void PopAllFromSlot(CardSlot slot, int playerIdx)
	{
		if (slot == null) return;

		while (!slot.IsEmpty())
		{
			Card c = slot.PopTopCard();
			players[playerIdx].AddCardFloor(c);
		}

		slot.Reset();
	}

	// 바닥 2장 + 내가 낸/뽑은 1장 => 2장만 먹고 1장 남김
	void TakeTwoFromTripleSlot(CardSlot slot, int playerIdx)
	{
		if (slot == null) return;
		if (slot.Cards.Count < 3) return;

		Card first = slot.PopTopCard();  // 방금 낸(또는 뽑은) 카드
		Card second = slot.PopTopCard(); // 바닥 카드 1장
		//TODO: 바닥에 있던 카드 2장 중 1장을 선택하는 로직 필요, 현재는 무조건 위에 있는 카드가 먹히도록 구현.

		if (first != null) players[playerIdx].AddCardFloor(first);
		if (second != null) players[playerIdx].AddCardFloor(second);
	}

	public TurnResult ProcessTurn(Card selectedHandCard, int playerIdx)
	{
		if (IsRoundEnded)
		{
			return new TurnResult
			{
				IsValidTurn = false,
				RoundEnded = true,
				WinnerIdx = WinnerIdx,
				EndReason = "라운드가 이미 종료되었습니다."
			};
		}

		if (players[0].HasPendingNineYeolChoice || players[1].HasPendingNineYeolChoice)
		{
			return new TurnResult
			{
				IsValidTurn = false,
				EndReason = "9월 열끗 선택을 먼저 확정해야 합니다."
			};
		}

		// NRE 체크
		if (selectedHandCard == null || playerIdx < 0 || playerIdx >= players.Length)
		{
			return new TurnResult
			{
				IsValidTurn = false,
				EndReason = "잘못된 턴 입력입니다."
			};
		}

		TurnResult result = new TurnResult { IsValidTurn = true };

		// 플레이어가 패에서 카드를 낸다.
		Card playedCard = players[playerIdx].PopPlayerHand(selectedHandCard);
		if (playedCard == null)
		{
			result.IsValidTurn = false;
			result.EndReason = "손패에 없는 카드를 선택했습니다.";
			return result;
		}

		// 같은 번호의 카드가 바닥에 몇 장 있는지 체크 후 카드를 놓는다.
		int beforePlayMatch = fManager.GetSlotStack(playedCard.number);

		CardSlot playedSlot = fManager.PutCard(playedCard);
		if (playedSlot == null)
		{
			result.IsValidTurn = false;
			result.EndReason = "손패 슬롯 처리에 실패했습니다.";
			return result;
		}

		// 덱에서 카드를 뽑는다
		Card drawnCard = PopMainDeck();
		if (drawnCard == null)
		{
			return EndRoundAsNagari(playerIdx, result, "덱이 비어 라운드를 종료했습니다.");
		}

		// 특수 규칙: 뒤집은 카드가 낸 카드와 같은 월
		if (drawnCard.number == playedCard.number)
		{
			CardSlot sameSlot = fManager.PutCard(drawnCard);
			if (sameSlot == null)
			{
				result.IsValidTurn = false;
				result.EndReason = "특수 규칙 처리에 실패했습니다.";
				return result;
			}

			if (beforePlayMatch == 0) // 쪽
			{
				PopAllFromSlot(sameSlot, playerIdx); // 2장
				TakeJunk(1, playerIdx);
				result.IsJjok = true;
				return CompleteTurn(playerIdx, result);
			}

			if (beforePlayMatch == 1) // 뻑
			{
				players[playerIdx].GetFuck(); // 3장 바닥에 그대로 놔둠
				result.IsPpuk = true;

				TurnResult samPpukResult = TryHandleSamPpuk(playerIdx, result); // 혹시 삼뻑인지 확인
				if (samPpukResult != null)
					return samPpukResult;

				return CompleteTurn(playerIdx, result);
			}

			if (beforePlayMatch == 2) // 따닥
			{
				PopAllFromSlot(sameSlot, playerIdx); // 4장
				TakeJunk(1, playerIdx);
				result.IsDdadak = true;
				return CompleteTurn(playerIdx, result);
			}

			return CompleteTurn(playerIdx, result);
		}

		// 일반 규칙: 뒤집은 카드가 다른 월, beforePlayMatch == 0이면 바닥에 맞는 패가 없는 경우이므로 놔둠
		if (beforePlayMatch == 1)
			PopAllFromSlot(playedSlot, playerIdx);
		else if (beforePlayMatch == 2)
			TakeTwoFromTripleSlot(playedSlot, playerIdx); // 2장 먹고 1장 남김

		// 뽑은 카드도 똑같이 체크
		int beforeDrawMatch = fManager.GetSlotStack(drawnCard.number);
		CardSlot drawnSlot = fManager.PutCard(drawnCard);
		if (drawnSlot == null)
		{
			result.IsValidTurn = false;
			result.EndReason = "뽑은 카드 슬롯 처리에 실패했습니다.";
			return result;
		}

		if (beforeDrawMatch == 1)
			PopAllFromSlot(drawnSlot, playerIdx);
		else if (beforeDrawMatch == 2)
			TakeTwoFromTripleSlot(drawnSlot, playerIdx);

		return CompleteTurn(playerIdx, result);
	}

	// 턴 종료 시점 공통 후처리
	TurnResult CompleteTurn(int playerIdx, TurnResult result)
	{
		if (fManager.IsEmpty()) // 판쓰리
		{
			TakeJunk(1, playerIdx);
			result.IsPanSweep = true;
		}

		TurnResult exhaustedResult = TryEndRoundByExhaustion(playerIdx, result);
		if (exhaustedResult != null) return exhaustedResult;

		return FinalizeTurnResult(playerIdx, result);
	}

	TurnResult FinalizeTurnResult(int playerIdx, TurnResult result)
	{
		Player player = players[playerIdx];

		result.NeedNineYeolChoice = player.HasPendingNineYeolChoice;
		result.CurrentScore = player.score;
		result.CanGoStop = player.CanGoStop;
		result.RoundEnded = IsRoundEnded;
		result.WinnerIdx = WinnerIdx;

		return result;
	}

	TurnResult EndRoundAsNagari(int playerIdx, TurnResult result, string reason)
	{
		IsRoundEnded = true;
		WinnerIdx = -1;

		result.IsValidTurn = true;
		result.IsNagari = true;
		result.RoundEnded = true;
		result.WinnerIdx = -1;
		result.FinalScore = 0;
		result.CurrentScore = players[playerIdx].score;
		result.NeedNineYeolChoice = false;
		result.CanGoStop = false;
		result.EndReason = reason;

		return result;
	}

	TurnResult TryEndRoundByExhaustion(int playerIdx, TurnResult result)
	{
		if (cardQ.Count > 0) return null;
		if (players[0].HandCardCount > 0 || players[1].HandCardCount > 0) return null;

		return EndRoundAsNagari(playerIdx, result, "덱과 손패가 모두 소진되어 라운드를 종료했습니다.");
	}

	TurnResult TryHandleSamPpuk(int playerIdx, TurnResult result)
	{
		Player winner = players[playerIdx];
		if (winner.fuckCnt < 3) return null;

		Player loser = players[(playerIdx + 1) % 2];
		int finalScore = ScoreCalculator.CalculateStopFinalScore(winner, loser);
		if (finalScore < 3) finalScore = 3; // 삼뻑 최소 3점

		IsRoundEnded = true;
		WinnerIdx = playerIdx;

		result.RoundEnded = true;
		result.WinnerIdx = playerIdx;
		result.FinalScore = finalScore;
		result.EndReason = "삼뻑으로 라운드를 종료했습니다.";

		return FinalizeTurnResult(playerIdx, result);
	}

	public TurnResult SelectNineYeolChoice(int playerIdx, bool asSsangP)
	{
		if (IsRoundEnded)
		{
			return new TurnResult
			{
				IsValidTurn = false,
				RoundEnded = true,
				WinnerIdx = WinnerIdx,
				EndReason = "라운드가 이미 종료되었습니다."
			};
		}

		if (playerIdx < 0 || playerIdx >= players.Length)
		{
			return new TurnResult
			{
				IsValidTurn = false,
				EndReason = "잘못된 플레이어 인덱스입니다."
			};
		}

		bool ok = players[playerIdx].ResolveNineYeolChoice(asSsangP);
		if (!ok)
		{
			return new TurnResult
			{
				IsValidTurn = false,
				EndReason = "9월 열끗 선택 대기 상태가 아닙니다."
			};
		}

		return FinalizeTurnResult(playerIdx, new TurnResult
		{
			IsValidTurn = true,
			EndReason = asSsangP ? "9월 열끗을 쌍피로 확정했습니다." : "9월 열끗을 열로 확정했습니다."
		});
	}

	public TurnResult GoStopDecision(int playerIdx, bool go)
	{
		if (IsRoundEnded)
		{
			return new TurnResult
			{
				IsValidTurn = false,
				RoundEnded = true,
				WinnerIdx = WinnerIdx,
				EndReason = "라운드가 이미 종료되었습니다."
			};
		}

		if (playerIdx < 0 || playerIdx >= players.Length)
		{
			return new TurnResult
			{
				IsValidTurn = false,
				EndReason = "잘못된 플레이어 인덱스입니다."
			};
		}

		Player player = players[playerIdx];
		Player enemy = players[(playerIdx + 1) % 2];

		if (player.HasPendingNineYeolChoice)
		{
			return new TurnResult
			{
				IsValidTurn = false,
				EndReason = "9월 열끗 선택을 먼저 확정해야 합니다."
			};
		}

		if (!player.CanGoStop)
		{
			return new TurnResult
			{
				IsValidTurn = false,
				EndReason = "현재 점수로는 Go/Stop을 선택할 수 없습니다."
			};
		}

		if (go)
		{
			if (!player.ApplyGo())
			{
				return new TurnResult
				{
					IsValidTurn = false,
					EndReason = "Go 처리에 실패했습니다."
				};
			}

			return FinalizeTurnResult(playerIdx, new TurnResult
			{
				IsValidTurn = true,
				EndReason = "Go를 선언했습니다."
			});
		}

		int finalScore = ScoreCalculator.CalculateStopFinalScore(player, enemy);

		IsRoundEnded = true;
		WinnerIdx = playerIdx;

		return new TurnResult
		{
			IsValidTurn = true,
			RoundEnded = true,
			WinnerIdx = playerIdx,
			CurrentScore = player.score,
			FinalScore = finalScore,
			EndReason = "Stop을 선언하여 라운드를 종료했습니다."
		};
	}
}