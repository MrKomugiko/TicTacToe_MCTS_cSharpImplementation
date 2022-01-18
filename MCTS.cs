public class MCTS
{
    public int BotId {get;set;}
    public int HumanId { get; internal set; }
    public int iterations = 1_000;

    public Node Search(BoardInfo _initialGameData, int botId)
    {
        // create root node
        Node root = new Node(_parent:null, _initialGameData, botId);

        // search iteration
        for (int i = 0; i < iterations; i++)
        {
            // select node
            Node node =  SelectNode(root); 

            // rollout
            float score = Rollout(node.board);

            // backpropagation
            Backpropagate(node,score); 
        }


        // debug show statistics
        foreach(var child in root.childrens)
        {
            Console.WriteLine($"Value: {child.value}\tVisits: {child.visits}\tUCB1: {child.UCB1Score}");
            child.board.DrawBoard();
        }

        // get best score

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
        var rand = new Random();
        // define best score & best moves
        float bestScore = float.NegativeInfinity; // -oo
        List<Node> bestMoves = new();

        // loop over child nodes
        foreach(var child in _node.childrens)
        {
            //define current player
            // int currentPlayer = 0;
            // if(_node.IdMakesMove == BotId)
            //     currentPlayer = -1;
            // else
            //     currentPlayer = 1;

            // // get move score using UCT formula
            //float moveScore = currentPlayer * child.value / child.visits + exploration *(float)Math.Sqrt(Math.Log(_node.visits/child.visits));
            
            // averageScorePerVisitCurrentNode + constantC * sqrt(lnOftotalVisits / currentnodevisits )
            // double UCBScore = ((double)(currentPlayer * child.value) / (double)child.visits) + (exploration * Math.Sqrt(Math.Log((double)_node.visits / (double)child.visits)));
            // child.UCB1Score = (float)UCBScore;

            Node x = GetRoot(_node);
            int totalVisits = x.visits;
            double averageScorePerVisitCurrentNode = (child.value) / (double)child.visits;
            double explorationConst = exploration;
            double lnOftotalVisits = Math.Log(totalVisits);
            double UCBScore = averageScorePerVisitCurrentNode + (explorationConst * Math.Sqrt(lnOftotalVisits/(double)child.visits));

            // var UCBScore = ((child.victoriesCount+child.drawsCount/2.0f) / (float)child.visits) + 2 * Math.Sqrt(Math.Log((float)child.parent.visits) / (float)child.visits);
            child.UCB1Score = (float)UCBScore;

            if((float)UCBScore > bestScore)
            {
            // better move has been found
                bestScore = (float)UCBScore;
                bestMoves = new() {child};
            }
            else if(UCBScore == bestScore)
            {
            // found as good move as already available
                bestMoves.Add(child);
            }
        }
        // return one of the best moves randomly
        return bestMoves[rand.Next(0,bestMoves.Count)];
    }

    // select most promising node
    private Node SelectNode(Node _node)
    {
        // make sure that we're dealing with non-terminal nodes
        while(!_node.IsTerminated)
        {
            // case where the node is fully expanded
            if(_node.isFullyExpanded)
                _node = GetBestMove(_node, 2);
            // case where node is not fully expanded
            else
                // otherwise expand the node
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
            //Console.WriteLine("node fully expanded");
            parent.isFullyExpanded = true;
            return null;
        }

        var rand = new Random();
        Node newNode = new Node(parent, parent.board, parent.IdMakesMove == BotId ? HumanId : BotId);
        parent.childrens.Add(newNode);
        //make random move in child nodes 
        int randomUniqueMovealongChildrens = possibleMovements[rand.Next(0, possibleMovements.Count)];
        newNode.board.SetMove(newNode.board, randomUniqueMovealongChildrens, newNode.IdMakesMove);
        //Console.WriteLine("new node");
       // newNode.board.DrawBoard();

        if(newNode.board.isGameEnded())
        {
            //Console.WriteLine("node zze skończoną grą!");
            // sprawdzenie czy wygrał bot czy człowiek
            //Console.WriteLine("czy wygrana: "+newNode.board.isWin()??"brak zwyciezcy / remis");
            if(newNode.board.isWin() != null)
            {
                //Console.WriteLine("zwyciezca jest: "+newNode.board.currentPlayer+ $" [{newNode.board.playerMark}]");
                if(newNode.board.currentPlayer != BotId)
                {
                    // Gwarantowana przegrana w nastepnym ruchuu gracza, wiec ten parent node ma miec wartosc -oo nie ma sensu do niego wracac xd
                   // newNode.parent.value = -1_000_000;
                    newNode.parent.IsTerminated = true;
                }
            }
        }
        return newNode;
    }

    //backpropagatte the number of visits and score up to the root node
    private void Backpropagate(Node node, float score)
    {
        // update node;s up to root node
        while(node.parent != null)
        {
            // update node's visits
            node.visits +=1;

            // update node score
            node.value += (int)score;
            
            node.victoriesCount +=(int)score==1?1:0;
            node.drawsCount += (int)score==0?1:0;
            node.losesCount +=(int)score==-1?1:0;

            // set node to parent
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
        var rand = new Random();

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

        boardCopy.currentPlayer = BotId;
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