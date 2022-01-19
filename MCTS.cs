using System.Diagnostics;

public class MCTS
{
    public int AIBot_Id {get;set;}
    public int EnemyID { get; internal set; }
    public int iterations = 1_000_000;
    public Random rand = new Random();
    static int simulationsCount = 0;
    public async Task<Node> SearchAsync(BoardInfo _initialGameData, int _timeout)
    {     
        Console.WriteLine("Searching started...");
       
        // timeout settings
        var _tokenSource = new CancellationTokenSource();
        var token = _tokenSource.Token;
        _tokenSource.CancelAfter(_timeout);
       
        simulationsCount = 0;
        // create root node
        Node root = new Node(_parent:null, _initialGameData, AIBot_Id==1?2:1);


        // search iteration
        for (int i = 0; i < iterations; i++)
        {
            if(token.IsCancellationRequested){
                //Console.WriteLine("Szukanie aanulowane. Zwracanie aktualnego wygenerowanego wyniku.");
                break;
            } 
            // select node
            Node node =  SelectNode(root); 

            // rollout
            float score = Rollout(node.board);

            // backpropagation
            Backpropagate(node,score);
        }
       
        // DEBUG show statistics
        foreach(var child in root.childrens)
        {
            Console.WriteLine($"Value: {child.value}\tVisits: {child.visits}\tUCB1: {child.UCB1Score}");
            //child.board.DrawBoard();
        }

        return GetBestMove(root,0);
        
    }
    private Node GetRoot(Node child){
        if(child.parent != null){
            return GetRoot(child.parent);
        }
        else
            return child;
    }
    // select best node basing on UCB1 formula ( explorationConst )
    private Node GetBestMove(Node _node, int exploration)
    {
        float bestScore = float.NegativeInfinity; // -oo
        List<Node> bestMoves = new();

        double lnOftotalVisits = Math.Log(GetRoot(_node).visits);
        double explorationConst = exploration;
        foreach(var child in _node.childrens)
        {
            double averageScorePerVisitCurrentNode = (child.value) / (double)child.visits;
            double UCBScore = averageScorePerVisitCurrentNode + (explorationConst * (Math.Sqrt(lnOftotalVisits/(double)child.visits)));
            child.UCB1Score = (float)UCBScore;

            if((float)UCBScore > bestScore)
            {
                bestScore = (float)UCBScore;
                bestMoves = new() {child};
            }
            else if(UCBScore == bestScore)
            {
                bestMoves.Add(child);
            }
        }
        return bestMoves[rand.Next(0,bestMoves.Count)];
    }

    private Node SelectNode(Node _node)
    {
        while(!_node.IsTerminated)
        {
            if(_node.isFullyExpanded)
                _node = GetBestMove(_node, 2);
            else
               return Expand(_node)??_node;
        }

        return _node;
    }

    public Node Expand(Node parent)
    {
        List<int> possibleMovements = parent.board.availableMoves;
        List<int> usedMovesInChilds = parent.childrens.Select(x => (int)x.board.recentMovement).ToList();
        usedMovesInChilds.ForEach(x => possibleMovements.Remove(x));

        if(possibleMovements.Count == 0){
            parent.isFullyExpanded = true;
            return null;
        }

        
        Node newNode = new Node(parent, parent.board, parent.IdMakesMove == AIBot_Id ? EnemyID : AIBot_Id);

        parent.childrens.Add(newNode);
        int randomUniqueMovealongChildrens = possibleMovements[rand.Next(0, possibleMovements.Count)];
        newNode.board.SetMove(newNode.board, randomUniqueMovealongChildrens, newNode.IdMakesMove);
        if(newNode.board.isGameEnded())
        {
            if(newNode.board.isWin() != null)
            {
                if(newNode.board.currentPlayer != AIBot_Id )
                {
                    //Console.WriteLine("Przegrałbys tak czy siak, przeciwnik moze wygrac w nastepnym ruchu na 100%");
                    newNode.parent.IsTerminated = true;
                }
            }
        }
        return newNode;
    }

    private void Backpropagate(Node node, float score)
    {
        while(node.parent != null)
        {
            node.visits +=1;
            node.value += (int)score;
            node.victoriesCount +=(int)score==1?1:0;
            node.drawsCount += (int)score==0?1:0;
            node.losesCount +=(int)score==-1?1:0;

            node = node.parent;
        }
        node.visits +=1;
        node.value += (int)score;
    }

    private float Rollout(BoardInfo _board)
    {
        //SYMULACJA+
        BoardInfo boardCopy = new BoardInfo(_board, _board.currentPlayer);
        int firstPlayer = _board.currentPlayer==1?2:1;
        int secondPlayer = firstPlayer==1?2:1;

        //Console.WriteLine("current case node:");
        //boardCopy.DrawBoard();
        //Console.WriteLine("Start simulation: next move by "+(firstPlayer==BotId?"Bot":"Human"));
        while(true)
        {
            //random move
            if(boardCopy.availableMoves.Count==0) break;

            int movePosition = GetRandomMove(boardCopy,firstPlayer);
            boardCopy.SetMove(boardCopy,movePosition,firstPlayer);

            //boardCopy.DrawBoard();
            if(boardCopy.isGameEnded() == true || boardCopy.isDraw()) 
                break;

            //--------------------------------------------------------------------------------------------
            
            //random move
            movePosition = GetRandomMove(boardCopy,secondPlayer);
            boardCopy.SetMove(boardCopy,movePosition,secondPlayer);

            //boardCopy.DrawBoard();
            if(boardCopy.isGameEnded() == true || boardCopy.isDraw()) 
                break;
        }

        MCTS.simulationsCount++;

        boardCopy.currentPlayer = AIBot_Id;
        //boardCopy.DrawBoard();
       if(boardCopy.isWin() != null)
       {
            if((bool)boardCopy.isWin())
                {
                    //Console.WriteLine("Bot wygrał +1");
                    return 1;
                }

            else
            {
                    //Console.WriteLine("Bot przegrał -1");
                return -1;
            }
       }

        if(boardCopy.isDraw())
        {
                    //Console.WriteLine("Remis 0");
            return 0;
        }

        throw new Exception("nie powinno sie zdażyc");
    }

    private int GetRandomMove(BoardInfo board, int firstPlayer)
    {
        var rand = new Random();
        board.currentPlayer = firstPlayer;
        return board.availableMoves[rand.Next(0, board.availableMoves.Count)];
    }
}