using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3 : MonoBehaviour
{
    [SerializeField]
    private ArrayLayout boardLayout; 
    [Header("UI elements"), SerializeField]
    private Sprite[] pieces;
        
    [SerializeField]
    private RectTransform gameBoard;

    [Header("Prefabs"), SerializeField]
    private GameObject[] nodePiece;

    private Node[,] board;
    private int _width = 4;
    private int _height = 6;

    private float delay;
    private bool anim = false;
    List<NodePiece> update;
    List<FlippedPieces> flipped;
    System.Random random;
    void Start()
    {
        StartGame();
    }

    void Update()
    {
        List<NodePiece> finishedUpdating = new List<NodePiece>();
        for(int i = 0; i< update.Count; i++)
        {
            NodePiece piece = update[i];
            if(piece != null)
            if (!piece.UpdatePice()) finishedUpdating.Add(piece);
        }

        for(int i = 0;i < finishedUpdating.Count; i++)
        {
            NodePiece piece = finishedUpdating[i];
            FlippedPieces flip = getFlipped(piece);
            NodePiece flippedPiece = null;
            List<Point> connected = isConnected(piece.index, true);
            bool wasFlipped = (flip != null); 
            if(wasFlipped)
            {
                flippedPiece = flip.getOtherPiece(piece);
                AddPoints(ref connected, isConnected(flippedPiece.index, true));
            }

            foreach (Point pnt in connected) //Remove the node pieces connected
            {
                Node node = getNodeAtPoint(pnt);
                NodePiece nodePiece = node.getPiece();
                if (nodePiece != null)
                {
                    //nodePiece.gameObject.SetActive(false);
                    nodePiece.gameObject.GetComponent<Animator>().SetTrigger("Die");
                    StartCoroutine(DieAfterTime(nodePiece, node, flip, piece));
                }
                node.SetPiece(null);
            }
            if (!anim)
            {
                ApplyGravityToBoard();
                SortLayers();
                flipped.Remove(flip);// Remove after update
                update.Remove(piece);
            }
        }
    }

    IEnumerator DieAfterTime(NodePiece nodePiece , Node node , FlippedPieces flip , NodePiece piece)
    {
        anim = true;
        flipped.Remove(flip);// Remove after update
        update.Remove(piece);
        yield return new WaitForSeconds(0.7f);
        nodePiece.gameObject.SetActive(false);
        ApplyGravityToBoard();
        SortLayers();
      //  flipped.Remove(flip);// Remove after update
        //update.Remove(piece);
        anim = false;
    }
    void SortLayers() //Method to sort all gameobjects
    {
        for (int y = _height - 1; y >=0; y--)
        {
            for (int x = 0; x < _width; x++)
            {
                Point p = new Point(x, y);
                Node Np = getNodeAtPoint(p);
                NodePiece piece = Np.getPiece();
                if (piece != null)
                {
                    if (piece.GetGameobject() != null)
                        piece.GetGameobject().transform.SetAsLastSibling();
                }
            }
        }
    }
     
    void ApplyGravityToBoard()
    {
        for(int x = 0;x < _width; x++)
        {
            for(int y = (_height - 1); y>=0; y--)
            {
                Point p = new Point(x, y);
                Node node = getNodeAtPoint(p);
                int value = getValueAtPoint(p);
                if (value != 0)
                {
                    continue;
                }

                for (int ny = (y - 1); ny >= -1; ny--)
                {
                    Point next = new Point(x, ny);
                    int nextVal = getValueAtPoint(next);
                    if (nextVal == 0) continue;
                    if (nextVal != -1)
                    { //If we did not hit an end,but its not zero then fill the hole
                        Node got = getNodeAtPoint(next);
                        NodePiece piece = got.getPiece();

                        //set the hole
                        node.SetPiece(piece);
                        update.Add(piece);

                        //Replace the hole
                        got.SetPiece(null);
                    }
                    break;
                }
            }
        }
    }
    FlippedPieces getFlipped(NodePiece p)
    {
        FlippedPieces flip = null;
        for (int i = 0; i < flipped.Count; i++)
        {
            if (flipped[i].getOtherPiece(p) != null)
            {
                flip = flipped[i];
                break;
            }
        }
        return flip;
    }
    void StartGame()
    {
        update = new List<NodePiece>();
        flipped = new List<FlippedPieces>();
        InitializeBoard();
        InstantiateBoard();
    }

    void InitializeBoard()
    {
        board = new Node[_width, _height];
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                board[x, y] = new Node(boardLayout.rows[y].row[x], new Point(x, y));
            }
        }

    } 
    
    void InstantiateBoard()
    {
        for (int y = _height - 1; y >=0 ; y--)
        {
            for (int x = 0; x < _width; x++)
            {
                Node node = getNodeAtPoint(new Point(x, y));

                if (node.value <= 0) continue;
                GameObject p = Instantiate(nodePiece[node.value - 1 ], gameBoard);
                NodePiece piece = p.GetComponent<NodePiece>();
                RectTransform rect = p.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(128 + (176 * x), -128 - (178* y));
                piece.Initialize(node.value, new Point(x, y));
                node.SetPiece(piece);
            }
        }

    }

    public void ResetPiece(NodePiece piece)
    {
        piece.ResetPosition();
        update.Add(piece);
    }
    public void FlipPieces(Point one, Point two ,bool main)
    {
        if (getValueAtPoint(one) <= 0) return;
        Node nodeOne = getNodeAtPoint(one);
        NodePiece pieceOne = nodeOne.getPiece();
        if (getValueAtPoint(two) >= 0)
        {
            Node nodeTwo = getNodeAtPoint(two);
            NodePiece pieceTwo = nodeTwo.getPiece();
            nodeOne.SetPiece(pieceTwo);
            nodeTwo.SetPiece(pieceOne);

            if (main)  flipped.Add(new FlippedPieces(pieceOne, pieceTwo));
            
            update.Add(pieceOne);
            update.Add(pieceTwo);
        }
        else
        {
            ResetPiece(pieceOne);
        }
    }
    List<Point> isConnected(Point p, bool main)
    {
        List<Point> connected = new List<Point>();
        int val = getValueAtPoint(p);
        Point[] directions =
        {
            Point.up,
            Point.right,
            Point.down,
            Point.left
        };

        foreach (Point dir in directions) //Checking if there is 2 or more same shapes in the direction
        {
            List<Point> line = new List<Point>();
            int same = 0;

            for (int i = 1; i < 3; i++)
            {
                Point check = Point.add(p, Point.mult(dir, i));
                if (getValueAtPoint(check) == val)
                {            
                    line.Add(check);
                    same++;
                }
            }
            if (same > 1)//if there are more than 1 of the same shape in the direction then match
            {
                AddPoints(ref connected, line);//Add these points to the overarching connected list
            }
        }

        for (int i = 0; i < 2; i++) // Checking if we are between same shapes
        {
            List<Point> line = new List<Point>();

            int same = 0;
            Point[] toCheck = { Point.add(p, directions[i]), Point.add(p, directions[i + 2]) };

            foreach (Point next in toCheck)//Check both sides of the piece
            {
                if (getValueAtPoint(next) == val)
                {
                    line.Add(next);
                    same++;
                }
            }
            if (same > 1)
            {
                AddPoints(ref connected, line);
            }
        }
         

        if (main) //Checks for other matches
        {
            for (int i = 0; i < connected.Count; i++)
            {
                AddPoints(ref connected, isConnected(connected[i], false));
            }
        }

        if (connected.Count > 0)
        {
            connected.Add(p);
        }
        return connected;
    }

    void AddPoints(ref List<Point> points, List<Point> add)
    {
        foreach (Point p in add)
        {
            bool doAdd = true;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Equals(p))
                {
                    doAdd = false;
                    break;
                }
            }
            if (doAdd) points.Add(p);
        }
    }

    int getValueAtPoint(Point p)
    {
        if (p.x < 0 || p.x >= _width || p.y < 0 || p.y >= _height) return -1;
        return board[p.x, p.y].value;
    }

    Node getNodeAtPoint(Point p)
    {
        return board[p.x, p.y];
    }
    void setValueAtPoint(Point p, int v)
    {
        board[p.x, p.y].value = v;
    }

    public Vector2 getPositionFromPoint(Point p)
    {
        return new Vector2(128 + (176 * p.x), -128 - (178 * p.y));
    }
    [System.Serializable]

    public class Node
    {
        public int value;// 0 - blank, 1 - Fire, 2 - Water
        public Point index;
         NodePiece piece;
        public Node(int v, Point i)
        {
            value = v;
            index = i;
        }
        public void SetPiece (NodePiece p)
        {
            piece = p;
            value = (piece == null) ? 0 : piece.value;
            if (piece == null) return;
            piece.SetIndex(index);  
        }

        public NodePiece getPiece()
        {
            return piece;
        }
    }

    [System.Serializable]
    public class FlippedPieces
    {
        public NodePiece one;
        public NodePiece two;
        public FlippedPieces(NodePiece o, NodePiece t)
        {
            one = o;
            two = t;
        }

        public NodePiece getOtherPiece(NodePiece p)
        {
            if (p == one)
            {
                return two;
            }
            else if (p == two)
            {
                return one;
            }
            else
            {
                return null;
            }
        }
    }
}


