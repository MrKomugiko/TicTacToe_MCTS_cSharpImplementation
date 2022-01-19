class Program
{
    static int Player_1 = 1; // 1szy = "O"
    static int Player_2 = 2; // 2gi = "X"
    
    static async Task Main(string[] args)
    {

            MCTS _mcts = new MCTS();
            Console.WriteLine("Start");
            var _board = new BoardInfo(null, Player_1==1?Player_1:Player_2);
            _board.DrawBoard();
           
            while (true)
            {
                if(Player_1 == 1)
                {
                    //// ruch gracza
                    // Console.WriteLine("player 1 ['O']");
                    // int playerMove = Int32.Parse(Console.ReadLine());
                    // _board.SetMove(_board, playerMove, HumanId);
                    // _board.DrawBoard();
                    // if(GameOver(_board)) break;
                    Console.WriteLine("bot 1 ['O'] = BOT");
                    _mcts.AIBot_Id = Player_1;
                    _mcts.EnemyID = Player_2;
                    Node root = new Node(null, _board, Player_1); 
                    //Console.WriteLine("Właczenie mcts");

                    var searching = await _mcts.SearchAsync(root.board, _timeout:200);
                    
                    //Console.WriteLine("trwa szukanie odpowiedniego ruchu");
                    int botMove = (int)searching.board.recentMovement;
                  //  wykonanie ruchu przez bota
                    _board.SetMove(_board, botMove, Player_1);
                    _board.DrawBoard();
                    //Console.ReadKey();
                    if(GameOver(_board)) break;
                

                    Console.WriteLine("bot 2 ['X'] = BOT");
                    // create working copy of current board
                    _mcts.AIBot_Id = Player_2;
                    _mcts.EnemyID = Player_1;
                    searching = await _mcts.SearchAsync(_board, _timeout:200);
                    botMove = (int)searching.board.recentMovement;
                    // wykonanie ruchu przez bota
                    _board.SetMove(_board, botMove, Player_2);
                    _board.DrawBoard();
                //Console.ReadKey();
                    if(GameOver(_board)) break;
                    
                }
                else
                {
                    // Console.WriteLine("bot 1 ['O'] = BOT");
                    // _mcts.AIBot_Id = Player_1;
                    // _mcts.EnemyID = Player_2;
                    // Node root = new Node(null, _board, Player_1);
                    // var newBotMove = await _mcts.Search(root.board);
                    // int botMove =(int)newBotMove.board.recentMovement;
                    // _board.SetMove(_board, botMove, Player_2);
                    // _board.DrawBoard();
                    // if(GameOver(_board)) break;

                    // Console.WriteLine("player 2 ['X'] = Human \n");
                    // int playerMove = Int32.Parse(Console.ReadLine());
                    // _board.SetMove(_board, playerMove, Player_1);
                    // _board.DrawBoard();
                    // if(GameOver(_board)) break;
                }
            }
       
        Console.ReadKey();
    }

    private static bool GameOver(BoardInfo _board)
    {
        if (_board.isDraw())
        {
            Console.WriteLine("DRAW.");
            return true; ;
        }
        if (_board.isGameEnded() == true)
        {
            Console.WriteLine($"Winner: {_board.currentPlayer}['{_board.playerMark}'] ");
            return true;
        }
        return false;
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
