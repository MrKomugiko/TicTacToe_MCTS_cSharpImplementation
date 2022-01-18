    public class Node
    {
        public int IdMakesMove; // kto wykonał ostatni ruch 
        public BoardInfo board { get; set; }
        public Node parent { get; set; }
        public List<Node> childrens = new List<Node>();
        public int level { get; set; } = 0;
        public int value { get; set; } = 0;

        public int victoriesCount {get;set;} = 0;
        public int drawsCount {get;set;} = 0;
        public int losesCount {get;set;} = 0;


        public int visits { get; set; } = 0;
    private bool loseGuaranteed;

    public bool isFullyExpanded { get; set; } // no more available moves to add new
    public float UCB1Score { get; internal set; }
    public bool IsTerminated 
    { 
        get {
            if(loseGuaranteed == true)
                return true;
                
            if (board.isWin() ?? false || board.isDraw())
                {
                    return true;
                }
                return false;
        }
        set => loseGuaranteed = value; 
    }

    public Node(Node _parent, BoardInfo _board, int _whoMakeTurnId)
        {
            this.IdMakesMove = _whoMakeTurnId;
            this.board = new BoardInfo(_board, _whoMakeTurnId);
            if (_parent != null)
            {
                this.parent = _parent;
                this.level = parent.level + 1;
            }
        }


    }