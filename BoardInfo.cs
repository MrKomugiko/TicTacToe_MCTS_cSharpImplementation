public class BoardInfo
{
    public string[] currentBoardState = new string[9];
    public int currentPlayer;
    internal int? recentMovement = null;
    internal int currentState;
    public string playerMark
    {
        get
        {
            if (currentPlayer == 1)
                return "O";
            else
                return "X";
        }
    }
    public List<int> availableMoves => GetLegalMoves();

    public bool? isWin()
    {
        if (CheckWin() != null)
        {
            string winerMark = CheckWin();

            if (playerMark == winerMark)
                return true;
            else
                return false;
        }

        return null;
        throw new Exception("gra jeszcze trwa");
    }
    public bool isGameEnded()
    {
        bool result = false;

        if(CheckWin() != null) 
            result = true;

        if(availableMoves.Count == 0) 
            result = true;  

        return result;
    }
    public bool isDraw()
    {
        // no more moves available
        if (currentBoardState.Any(x => x == "") == false && CheckWin()==null)
            return true;

        return false;
    }
    public BoardInfo(BoardInfo board, int currentPlayer)
    {
        if (board != null)
        {
            int i = 0;
            foreach (var x in board.currentBoardState)
            {
                this.currentBoardState[i] = x;
                i++;
            }
        }
        else
        {
            // empty board;
            for (int i = 0; i < 9; i++) currentBoardState[i] = "";
        }
        this.currentPlayer = currentPlayer;
    }
    private List<int> GetLegalMoves()
    {
        List<int> moves = new();
        for (int i = 0; i < 9; i++)
        {
            if (currentBoardState[i] == "")
                moves.Add(i);
        }
        return moves;
    }
    public void PrintBoard()
    {
        Console.WriteLine(String.Join(",", currentBoardState));
    }
    public void DrawBoard()
    {
        Console.WriteLine("  {0}  |  {1}  |  {2}", currentBoardState[0].PadLeft(1), currentBoardState[1].PadLeft(1), currentBoardState[2].PadLeft(1));
        Console.WriteLine("_____|_____|_____ ");
        Console.WriteLine("  {0}  |  {1}  |  {2}", currentBoardState[3].PadLeft(1), currentBoardState[4].PadLeft(1), currentBoardState[5].PadLeft(1));
        Console.WriteLine("_____|_____|_____ ");
        Console.WriteLine("  {0}  |  {1}  |  {2}", currentBoardState[6].PadLeft(1), currentBoardState[7].PadLeft(1), currentBoardState[8].PadLeft(1));
        Console.WriteLine("\n");
    }
    private string CheckWin()
    {
        #region Horzontal Winning Condtion
        //Winning Condition For First Row
        if (currentBoardState[0] == currentBoardState[1] && currentBoardState[1] == currentBoardState[2] && currentBoardState[1] != "")
        {
            return currentBoardState[0];
        }
        //Winning Condition For Second Row
        else if (currentBoardState[3] == currentBoardState[4] && currentBoardState[4] == currentBoardState[5] && currentBoardState[4] != "")
        {
            return currentBoardState[3];
        }
        //Winning Condition For Third Row
        else if (currentBoardState[6] == currentBoardState[7] && currentBoardState[7] == currentBoardState[8] && currentBoardState[7] != "")
        {
            return currentBoardState[6];
        }
        #endregion
        #region vertical Winning Condtion
        //Winning Condition For First Column
        else if (currentBoardState[0] == currentBoardState[3] && currentBoardState[3] == currentBoardState[6] && currentBoardState[3] != "")
        {
            return currentBoardState[0];
        }
        //Winning Condition For Second Column
        else if (currentBoardState[1] == currentBoardState[4] && currentBoardState[4] == currentBoardState[7] && currentBoardState[4] != "")
        {
            return currentBoardState[1];
        }
        //Winning Condition For Third Column
        else if (currentBoardState[2] == currentBoardState[5] && currentBoardState[5] == currentBoardState[8] && currentBoardState[5] != "")
        {
            return currentBoardState[2];
        }
        #endregion
        #region Diagonal Winning Condition
        else if (currentBoardState[0] == currentBoardState[4] && currentBoardState[4] == currentBoardState[8] && currentBoardState[4] != "")
        {
            return currentBoardState[0];
        }
        else if (currentBoardState[2] == currentBoardState[4] && currentBoardState[4] == currentBoardState[6] && currentBoardState[4] != "")
        {
            return currentBoardState[2];
        }
        else return null;

        #endregion

    }

    public void SetMove(BoardInfo boardCopy, int movePosition, int playerId)
    {
        boardCopy.currentPlayer = playerId;
        boardCopy.currentBoardState[movePosition] = boardCopy.playerMark;
        boardCopy.recentMovement = movePosition;
    }
}