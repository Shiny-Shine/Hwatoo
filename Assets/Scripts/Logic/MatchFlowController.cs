using System.Collections.Generic;

public enum TurnFlowState
{
    WaitingPlayCard,
    WaitingNineYeolChoice,
    WaitingGoStop,
    RoundEnded
}

public class MatchFlowController
{
    private readonly HwatooEngine engine = new HwatooEngine();

    private int currentPlayerIdx = 0;
    public int CurrentPlayerIdx => currentPlayerIdx;
    private int pendingNineYeolPlayerIdx = -1;

    public TurnFlowState State { get; private set; } = TurnFlowState.WaitingPlayCard;
    public TurnResult LastResult { get; private set; }
    

    public bool StartRound()
    {
        bool ok = engine.StartRound();
        State = ok ? TurnFlowState.WaitingPlayCard : TurnFlowState.RoundEnded;
        currentPlayerIdx = 0;
        pendingNineYeolPlayerIdx = -1;
        return ok;
    }
    
    public List<Card> GetHandSnapshot(int playerIdx)
    {
        return engine.GetPlayerHandSnapshot(playerIdx);
    }

    public List<Card>[] GetFloorSlotsSnapshot()
    {
        return engine.GetFloorSlotsSnapshot();
    }
    
    public List<Card>[] GetFieldSnapshot(int index)
    {
        return engine.GetPlayerFieldSnapshot(index);
    }

    public int GetScore(int playerIdx)
    {
        return engine.GetPlayerScore(playerIdx);
    }

    public int GetDeckCount()
    {
        return engine.GetDeckCount();
    }

    public TurnResult PlayCard(Card selectedCard)
    {
        if (State != TurnFlowState.WaitingPlayCard)
            return Invalid("플레이 단계가 아닙니다.");

        LastResult = engine.ProcessTurn(selectedCard, currentPlayerIdx);
        return RouteAfterTurn(LastResult, currentPlayerIdx);
    }

    public TurnResult ChooseNineYeol(bool asSsangP)
    {
        if (State != TurnFlowState.WaitingNineYeolChoice || pendingNineYeolPlayerIdx < 0)
            return Invalid("9월 열끗 선택 대기 상태가 아닙니다.");

        LastResult = engine.SelectNineYeolChoice(pendingNineYeolPlayerIdx, asSsangP);

        if (!LastResult.IsValidTurn)
        {
            if (LastResult.RoundEnded) State = TurnFlowState.RoundEnded;
            // 실패 시에는 대기 상태 유지
            return LastResult;
        }
        
        pendingNineYeolPlayerIdx = -1;

        if (LastResult.RoundEnded)
        {
            State = TurnFlowState.RoundEnded;
            return LastResult;
        }

        if (LastResult.NeedNineYeolChoice)
        {
            State = TurnFlowState.WaitingNineYeolChoice;
            return LastResult;
        }

        if (LastResult.CanGoStop)
        {
            State = TurnFlowState.WaitingGoStop;
            return LastResult;
        }

        AdvanceTurn();
        State = TurnFlowState.WaitingPlayCard;
        return LastResult;
    }

    public TurnResult ChooseGoStop(bool go)
    {
        if (State != TurnFlowState.WaitingGoStop)
            return Invalid("고스톱 선택 단계가 아닙니다.");

        LastResult = engine.GoStopDecision(currentPlayerIdx, go);

        if (!LastResult.IsValidTurn || LastResult.RoundEnded)
        {
            if (LastResult.RoundEnded) State = TurnFlowState.RoundEnded;
            return LastResult;
        }

        AdvanceTurn();
        State = TurnFlowState.WaitingPlayCard;
        return LastResult;
    }

    private TurnResult RouteAfterTurn(TurnResult result, int playerIdx)
    {
        if (!result.IsValidTurn || result.RoundEnded)
        {
            if (result.RoundEnded) State = TurnFlowState.RoundEnded;
            return result;
        }

        if (result.NeedNineYeolChoice)
        {
            pendingNineYeolPlayerIdx = playerIdx;
            State = TurnFlowState.WaitingNineYeolChoice;
            return result;
        }

        if (result.CanGoStop)
        {
            State = TurnFlowState.WaitingGoStop;
            return result;
        }

        AdvanceTurn();
        State = TurnFlowState.WaitingPlayCard;
        return result;
    }

    private void AdvanceTurn()
    {
        currentPlayerIdx = (currentPlayerIdx + 1) % 2;
    }

    private TurnResult Invalid(string reason)
    {
        return new TurnResult
        {
            IsValidTurn = false,
            EndReason = reason
        };
    }
}