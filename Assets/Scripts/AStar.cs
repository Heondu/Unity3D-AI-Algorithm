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
        /// 오브젝트의 이동처리를 하는 함수
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

                    //부드럽게 이동하기 위해 Lerp함수를 이용한다.
                    transform.position = Vector3.Lerp(originPos, targetPos, current);

                    yield return null;
                }
            }
        }

        /// <summary>
        /// 길을 찾는 함수
        /// </summary>
        private void PathFinding()
        {
            openList = new List<Node>();
            closedList = new List<Node>();
            finalList = new List<Node>();

            //열린 목록에 시작 노드 추가
            openList.Add(new Node(startPos.x, startPos.y));

            //열린 목록에 아무것도 없다면 길이 없음을 의미
            while (openList.Count > 0)
            {
                Node currentNode = openList[0];
                for (int i = 1; i < openList.Count; i++)
                {
                    //열린 목록 중에서 이동 비용이 가장 낮은 노드를 선택
                    if ((openList[i].F <= currentNode.F) && openList[i].H < currentNode.H)
                    {
                        currentNode = openList[i];
                    }
                }
                //선택한 노드를 열린 목록에서 지우고 닫힌 목록에 추가
                openList.Remove(currentNode);
                closedList.Add(currentNode);

                if (currentNode.Pos == endPos)
                {
                    //선택한 노드가 목표 지점과 같다면 시작 지점이 될 때까지 부모 노드로 거슬러 올라감
                    while (currentNode.Pos != startPos)
                    {
                        finalList.Add(currentNode);
                        currentNode = currentNode.parentNode;
                    }
                    finalList.Add(new Node(startPos.x, startPos.y));
                    //거꾸로 추가 되었기 때문에 Reverse함수로 뒤집어 줌
                    finalList.Reverse();

                    StartCoroutine("Move");
                    return;
                }

                //인접한 노드 중 갈 수 있는 노드를 찾는 과정
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
        /// 노드 상태를 확인하고 열린 목록에 추가하는 함수
        /// </summary>
        /// <param name="currentNode">현재 노드</param>
        /// <param name="newNode">인접 노드</param>
        private void AddNodeToOpenList(Node currentNode, Node newNode)
        {
            //벽인지 검사
            if (IsWall(newNode.x, newNode.y)) return;
            //벽에 인접한 노드를 대각선으로 가로지르는지 검사
            if (IsWall(currentNode.x, newNode.y) || IsWall(newNode.x, currentNode.y)) return;
            //닫힌 목록에 이미 존재하는지 검사
            if (closedList.Contains(newNode)) return;

            int G;
            //직선
            if (currentNode.x - newNode.x == 0 || currentNode.y - newNode.y == 0)
            {
                G = currentNode.G + 10;
            }
            //대각선
            else
            {
                G = currentNode.G + 14;
            }

            //이동 비용이 더 낫거나 열린 목록에 없다면 이동 비용을 계산하고 부모 노드를 현재 노드로 지정
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
        /// 해당 좌표에 벽이 있는지 검사하는 함수
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