using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public bool isWall;
    public Node parentNode;

    public int x, y, G, H;

    public int F
    {
        get
        {
            return G + H;
        }
    }

    public Node(bool _isWall, int _x, int _y)
    {
        isWall = _isWall;
        x = _x;
        y = _y;
    }
}

public class AStar : MonoBehaviour
{
    public Vector2Int bottomLeft, topRight, startPos, targetPos;
    public List<Node> FinalNodeList;
    public bool allowDiagonal, dontCrossCorner;

    int sizeX, sizeY;

    Node[,] NodeArray;

    Node StartNode, TargetNode, CurNode;

    List<Node> OpenList, ClosedList;

    public GameObject MoveObject;

    bool isFind = false;

    private void FixedUpdate()
    {
        Go();
    }

    IEnumerator MoveGameObject()
    {
        for (int i = 0; i < FinalNodeList.Count; i++)
        {
            MoveObject.transform.position = new Vector3(FinalNodeList[i].x, 0, FinalNodeList[i].y);
            yield return new WaitForSeconds(0.1f);
        }
    }

    void Go()
    {
        if (isFind)
        {
            isFind = false;
            StartCoroutine("MoveGameObject");
        }
    }

    [ContextMenu("PathFinding")]
    public void PathFinding()
    {
        isFind = true;

        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;

        NodeArray = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false;

                foreach (Collider col in Physics.OverlapSphere(new Vector3(i + bottomLeft.x, 0, j + bottomLeft.y), 0.4f))
                {
                    if (col.gameObject.layer == LayerMask.NameToLayer("Wall"))
                    {
                        isWall = true;
                    }
                }
                NodeArray[i, j] = new Node(isWall, i + bottomLeft.x, j + bottomLeft.y);
            }
        }

        StartNode = NodeArray[startPos.x - bottomLeft.x, startPos.y - bottomLeft.y];
        TargetNode = NodeArray[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y];

        OpenList = new List<Node>();
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();

        OpenList.Add(StartNode);

        while (OpenList.Count > 0)
        {
            CurNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
            {
                if ((OpenList[i].F <= CurNode.F) && (OpenList[i].H < CurNode.H))
                {
                    CurNode = OpenList[i];
                }
            }

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);

            if (CurNode == TargetNode)
            {
                Node TargetCurNode = TargetNode;

                while (TargetCurNode != StartNode)
                {
                    FinalNodeList.Add(TargetCurNode);
                    TargetCurNode = TargetCurNode.parentNode;
                }

                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();

                for (int i = 0; i < FinalNodeList.Count; i++)
                {
                    Debug.Log($"{i} ¹øÂ°´Â {FinalNodeList[i].x}, {FinalNodeList[i].y}");
                }

                return;
            }

            if (allowDiagonal)
            {
                OpenListAdd(CurNode.x + 1, CurNode.y + 1);
                OpenListAdd(CurNode.x - 1, CurNode.y + 1);
                OpenListAdd(CurNode.x - 1, CurNode.y - 1);
                OpenListAdd(CurNode.x + 1, CurNode.y - 1);
            }

            OpenListAdd(CurNode.x, CurNode.y + 1);
            OpenListAdd(CurNode.x + 1, CurNode.y);
            OpenListAdd(CurNode.x, CurNode.y - 1);
            OpenListAdd(CurNode.x - 1, CurNode.y);
        }
    }

    void OpenListAdd(int checkX, int checkY)
    {
        if ((checkX >= bottomLeft.x) && (checkX < topRight.x + 1) && (checkY >= bottomLeft.y) && (checkY < topRight.y + 1)
            && (!NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isWall)
            && (!ClosedList.Contains(NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y])))
        {
            if (allowDiagonal)
            {
                if ((NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall)
                    && (NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall)) return;
            }

            if (dontCrossCorner)
            {
                if ((NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall)
                    || (NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall)) return;
            }

            Node NeighborNode = NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y];

            int MoveCost;

            if (CurNode.x - checkX == 0 || CurNode.y - checkY == 0)
            {
                MoveCost = CurNode.G + 10;
            }
            else
            {
                MoveCost = CurNode.G + 14;
            }

            if ((MoveCost < NeighborNode.G) || (!OpenList.Contains(NeighborNode)))
            {
                NeighborNode.G = MoveCost;

                NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.y - TargetNode.y)) * 10;
                NeighborNode.parentNode = CurNode;

                OpenList.Add(NeighborNode);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (FinalNodeList.Count != 0)
        {
            for (int i = 0; i < FinalNodeList.Count - 1; i++)
            {
                Gizmos.DrawLine(new Vector2(FinalNodeList[i].x, FinalNodeList[i].y), new Vector2(FinalNodeList[i + 1].x, FinalNodeList[i + 1].y));
            }
        }
    }
}
