public class TurnResult
{
	public bool IsValidTurn { get; set; }

	public bool IsJjok { get; set; }
	public bool IsPpuk { get; set; }
	public bool IsDdadak { get; set; }
	public bool IsPanSweep { get; set; }

	public bool NeedNineYeolChoice { get; set; }
	public bool CanGoStop { get; set; }
	public bool IsNagari { get; set; }

	public int CurrentScore { get; set; }

	public bool RoundEnded { get; set; }
	public int WinnerIdx { get; set; } = -1;
	public int FinalScore { get; set; }

	public string EndReason { get; set; } = "";
}