class Program
{
    static string[] arr = new string[9];
    static int HumanId = 1; // 1szy = "O"
    static int BotId = 2; // 2gi = "X"
    static void Main(string[] args)
    {
        for (int i = 0; i < 9; i++) arr[i] = "";

        // Console.WriteLine("Start - empty board");
        // var _board = new BoardInfo(null, HumanId==1?HumanId:BotId);
        // _board.DrawBoard();

        // while (true)
        // {
        //     if(HumanId == 1)
        //     {
        //         Console.WriteLine("player 1 ['O']");
        //         int playerMove = Int32.Parse(Console.ReadLine());
        //         _board.SetMove(_board, playerMove, HumanId);
        //         _board.DrawBoard();
        //         if (_board.isGameEnded() == true)
        //         {
        //             Console.WriteLine($"Winner: {_board.currentPlayer}['{_board.playerMark}'] ");
        //             break;
        //         }
        //         if (_board.isDraw())
        //         {
        //             Console.WriteLine("DRAW.");
        //             break;
        //         }

        //         Console.WriteLine("bot 2 ['X'] = BOT \n");
        //         int botMove = GetNewMove(_board, BotId);
        //         _board.SetMove(_board, botMove, BotId);
        //         _board.DrawBoard();
        //         if (_board.isGameEnded() == true)
        //         {
        //             Console.WriteLine($"Winner: {_board.currentPlayer}['{_board.playerMark}'] ");
        //             break;
        //         }
        //         if (_board.isDraw())
        //         {
        //             Console.WriteLine("DRAW.");
        //             break;
        //         }
        //     }
        //     else
        //     {
        //          Console.WriteLine("bot 1 ['O']");
        //         int playerMove = Int32.Parse(Console.ReadLine());
        //         _board.SetMove(_board, playerMove, BotId);
        //         _board.DrawBoard();
        //         if (_board.isGameEnded() == true)
        //         {
        //             Console.WriteLine($"Winner: {_board.currentPlayer}['{_board.playerMark}'] ");
        //             break;
        //         }
        //         if (_board.isDraw())
        //         {
        //             Console.WriteLine("DRAW.");
        //             break;
        //         }

        //         Console.WriteLine("player 2 ['X'] = BOT \n");
        //         int botMove = GetNewMove(_board, HumanId);
        //         _board.SetMove(_board, botMove, HumanId);
        //         _board.DrawBoard();
        //         if (_board.isGameEnded() == true)
        //         {
        //             Console.WriteLine($"Winner: {_board.currentPlayer}['{_board.playerMark}'] ");
        //             break;
        //         }
        //         if (_board.isDraw())
        //         {
        //             Console.WriteLine("DRAW.");
        //             break;
        //         }
        //     }
        // }
        var rand = new Random();
        var _board2 = new BoardInfo(null, HumanId == 1 ? HumanId : BotId);
        _board2.SetMove(_board2, 0, HumanId);
        _board2.SetMove(_board2, 3, BotId);
        _board2.SetMove(_board2, 2, HumanId);

        MCTS _mcts = new MCTS();
        _mcts.BotId = BotId;
        _mcts.HumanId = HumanId;

        Node root = new Node(null, _board2, HumanId);
        root.board.DrawBoard();

        var newBotMove = _mcts.Search(root.board, HumanId);
        Console.WriteLine("odpowiedz bota = pozycja nr."+newBotMove.board.recentMovement);
        
        // _mcts.Expand(root);
        // _mcts.Expand(root);
        // _mcts.Expand(root);
        // _mcts.Expand(root);
        // Console.WriteLine(String.Join(",",root.childrens.Select(x=>x.board.recentMovement).ToList()));

        // _mcts.Expand(root.childrens.First());
        // _mcts.Expand(root.childrens.First());

        Console.WriteLine("end.");
        Console.ReadKey();
    }

    private static int Simulate(BoardInfo _board, int firstPlayer)
    {
        BoardInfo boardCopy = new BoardInfo(_board, firstPlayer);
        int secondPlayer = firstPlayer == 1 ? 2 : 1;

        while (true)
        {
            //random move
            int movePosition = GetNewMove(boardCopy, firstPlayer);
            boardCopy.SetMove(boardCopy, movePosition, firstPlayer);

            boardCopy.PrintBoard();
            if (boardCopy.isGameEnded() == true)
            {
                Console.WriteLine($"Winner: {_board.currentPlayer}['{_board.playerMark}'] ");
                break;
            }
            if (boardCopy.isDraw())
            {
                Console.WriteLine("DRAW.");
                break;
            }

            //--------------------------------------------------------------------------------------------

            //random move
            movePosition = GetNewMove(boardCopy, secondPlayer);
            boardCopy.SetMove(boardCopy, movePosition, secondPlayer);

            boardCopy.PrintBoard();
            if (boardCopy.isGameEnded() == true)
            {
                Console.WriteLine($"Winner: {_board.currentPlayer}['{_board.playerMark}'] ");
                break;
            }
            if (boardCopy.isDraw())
            {
                Console.WriteLine("DRAW.");
                break;
            }
        }

        // Console.WriteLine("Perspektywa gracza ["+firstPlayer+$" = '{boardCopy.playerMark}']");
        // Console.WriteLine("Draw: "+boardCopy.isDraw());

        // _board.currentPlayer = secondPlayer;
        // Console.WriteLine("Perspektywa gracza ["+secondPlayer+$" = '{_board.playerMark}']");
        // Console.WriteLine("Win: "+_board.isWin());
        // Console.WriteLine("Draw: "+_board.isDraw());

        // w odniesieni do 1 gracza zaczynajacego runde gracza nr.1 'O'
        boardCopy.currentPlayer = _board.currentPlayer;

        if (boardCopy.isDraw())
            return 0;

        if ((bool)boardCopy.isWin())
            return 1;
        else
            return -1;
        // Console.WriteLine("Game ended: "+_board.isGameEnded());
    }

    private static int GetNewMove(BoardInfo board, int firstPlayer)
    {
        // miejsce na algorytm
        // MCTS _MCTS = new MCTS();
        // var rootNode = new MonteNode(null,board);
        // MonteNode bestMove = _MCTS.Search(board);


        // test make copy
        // var newBoard = new BoardInfo(board,playerId);
        // newBoard.currentBoardState[0] = "X";
        // newBoard.currentBoardState[1] = "O";
        // newBoard.currentBoardState[2] = "X";

        var rand = new Random();
        board.currentPlayer = firstPlayer;
        return board.availableMoves[rand.Next(0, board.availableMoves.Count)];
    }
}
