using System;
using UnityEngine;

public class BoardFactory
{
	public enum board_type { manual, ai}
	static BingoBoard board = null;
	public static BingoBoard CreateBoard(board_type bt)
	{
		switch (bt) {
		case board_type.manual:
			board = BingoBoard.getInstance ();
			break;
		case board_type.ai:
			board = new AiStrategy ();
			break;
		default:
			Debug.Log ("error");
			break;
		}
		return board;
	}
}