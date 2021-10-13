using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIAlgorithm.AStar
{
    public class Node
    {
        public int x, y, G, H;
        public Node parentNode;

        public int F
        {
            get { return G + H; }
        }
        public Vector2Int Pos
        {
            get { return new Vector2Int(x, y); }
        }

        public Node(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }

    public class AStar : MonoBehaviour
    {
        [Header("Position")]
        [SerializeField] private Vector2Int startPos;
        [SerializeField] private Vector2Int endPos;

        [Header("Object")]
        [SerializeField] private Transform moveObject;

        [Header("Variable")]
        [SerializeField] private float moveSpeed = 1;

        private List<Node> openList, closedList, finalList;

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Space))
        //    {
        //        PathFinding();
        //    }
        //}

        public void StartFinding(Vector3 destination)
        {
            StopCoroutine("Move");

            startPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            endPos = new Vector2Int(Mathf.RoundToInt(destination.x), Mathf.RoundToInt(destination.z));

            PathFinding();
        }

        /// <summary>
        /// ������Ʈ�� �̵�ó���� �ϴ� �Լ�
        /// </summary>
        /// <returns></returns>
        private IEnumerator Move()
        {
            //transform.position = new Vector3(startPos.x, 0, startPos.y);

            for (int i = 0; i < finalList.Count; i++)
            {
                float percent = 0;
                float current = 0;

                Vector3 originPos, targetPos;
                originPos = transform.position;
                targetPos = new Vector3(finalList[i].x, originPos.y, finalList[i].y);

                while (current < 1)
                {
                    percent += Time.deltaTime;
                    current = percent * moveSpeed;

                    //�ε巴�� �̵��ϱ� ���� Lerp�Լ��� �̿��Ѵ�.
                    transform.position = Vector3.Lerp(originPos, targetPos, current);

                    yield return null;
                }
            }
        }

        /// <summary>
        /// ���� ã�� �Լ�
        /// </summary>
        private void PathFinding()
        {
            openList = new List<Node>();
            closedList = new List<Node>();
            finalList = new List<Node>();

            //���� ��Ͽ� ���� ��� �߰�
            openList.Add(new Node(startPos.x, startPos.y));

            //���� ��Ͽ� �ƹ��͵� ���ٸ� ���� ������ �ǹ�
            while (openList.Count > 0)
            {
                Node currentNode = openList[0];
                for (int i = 1; i < openList.Count; i++)
                {
                    //���� ��� �߿��� �̵� ����� ���� ���� ��带 ����
                    if ((openList[i].F <= currentNode.F) && openList[i].H < currentNode.H)
                    {
                        currentNode = openList[i];
                    }
                }
                //������ ��带 ���� ��Ͽ��� ����� ���� ��Ͽ� �߰�
                openList.Remove(currentNode);
                closedList.Add(currentNode);

                if (currentNode.Pos == endPos)
                {
                    //������ ��尡 ��ǥ ������ ���ٸ� ���� ������ �� ������ �θ� ���� �Ž��� �ö�
                    while (currentNode.Pos != startPos)
                    {
                        finalList.Add(currentNode);
                        currentNode = currentNode.parentNode;
                    }
                    finalList.Add(new Node(startPos.x, startPos.y));
                    //�Ųٷ� �߰� �Ǿ��� ������ Reverse�Լ��� ������ ��
                    finalList.Reverse();

                    StartCoroutine("Move");
                    return;
                }

                //������ ��� �� �� �� �ִ� ��带 ã�� ����
                AddNodeToOpenList(currentNode, new Node(currentNode.x + 1, currentNode.y));
                AddNodeToOpenList(currentNode, new Node(currentNode.x - 1, currentNode.y));
                AddNodeToOpenList(currentNode, new Node(currentNode.x, currentNode.y + 1));
                AddNodeToOpenList(currentNode, new Node(currentNode.x, currentNode.y - 1));
                AddNodeToOpenList(currentNode, new Node(currentNode.x + 1, currentNode.y + 1));
                AddNodeToOpenList(currentNode, new Node(currentNode.x - 1, currentNode.y + 1));
                AddNodeToOpenList(currentNode, new Node(currentNode.x + 1, currentNode.y - 1));
                AddNodeToOpenList(currentNode, new Node(currentNode.x - 1, currentNode.y - 1));
            }
        }

        /// <summary>
        /// ��� ���¸� Ȯ���ϰ� ���� ��Ͽ� �߰��ϴ� �Լ�
        /// </summary>
        /// <param name="currentNode">���� ���</param>
        /// <param name="newNode">���� ���</param>
        private void AddNodeToOpenList(Node currentNode, Node newNode)
        {
            //������ �˻�
            if (IsWall(newNode.x, newNode.y)) return;
            //���� ������ ��带 �밢������ ������������ �˻�
            if (IsWall(currentNode.x, newNode.y) || IsWall(newNode.x, currentNode.y)) return;
            //���� ��Ͽ� �̹� �����ϴ��� �˻�
            if (closedList.Contains(newNode)) return;

            int G;
            //����
            if (currentNode.x - newNode.x == 0 || currentNode.y - newNode.y == 0)
            {
                G = currentNode.G + 10;
            }
            //�밢��
            else
            {
                G = currentNode.G + 14;
            }

            //�̵� ����� �� ���ų� ���� ��Ͽ� ���ٸ� �̵� ����� ����ϰ� �θ� ��带 ���� ���� ����
            if ((G < newNode.G) || (!openList.Contains(newNode)))
            {
                newNode.G = G;

                newNode.H = (Mathf.Abs(newNode.x - endPos.x) + Mathf.Abs(newNode.y - endPos.y)) * 10;
                newNode.parentNode = currentNode;

                if (!openList.Contains(newNode))
                {
                    openList.Add(newNode);
                }
            }
        }

        /// <summary>
        /// �ش� ��ǥ�� ���� �ִ��� �˻��ϴ� �Լ�
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool IsWall(int x, int y)
        {
            Collider[] colliders = Physics.OverlapSphere(new Vector3(x, 0, y), 0.4f, 1 << LayerMask.NameToLayer("Wall"));
            if (colliders.Length > 0) return true;
            else return false;
        }

        //private void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.green;
        //    for (int x = 0; x < 10; x++)
        //    {
        //        for (int z = 0; z < 10; z++)
        //        {
        //            Gizmos.DrawWireCube(new Vector3(x, -0.9f, z), Vector3.one);
        //        }
        //    }
        //
        //    if (finalList != null)
        //    {
        //        Gizmos.color = Color.red;
        //        for (int i = 0; i < finalList.Count - 1; i++)
        //        {
        //            Gizmos.DrawWireCube(new Vector3(finalList[i].x, -0.9f, finalList[i].y), Vector3.one);
        //        }
        //    }
        //}
    }
}