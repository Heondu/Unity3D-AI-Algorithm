using System.Collections.Generic;
using UnityEngine;

namespace AIAlgorithm.Formation
{
    public class FormationController : MonoBehaviour
    {
        private bool isDragBegin = false;
        private Vector2 beginPos; //마우스 시작 위치
        private Vector2 currentPos;
        private Vector2 posMin; //rect
        private Vector2 posMax;

        private Agent[] agents; //유닛을 담을 배열
        private List<Agent> selectedAgents; //선택한 유닛을 관리할 리스트

        private void Start()
        {
            //씬에 있는 모드 유닛을 가져온다.
            agents = FindObjectsOfType<Agent>();
            selectedAgents = new List<Agent>();
        }

        private void Update()
        {
            Drag();

            Move();
        }

        /// <summary>
        /// 드래그 처리를 하는 함수
        /// </summary>
        private void Drag()
        {
            if (Input.GetMouseButton(0))
            {
                isDragBegin = true;
            }
            if (!isDragBegin) return;

            currentPos = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                beginPos = currentPos;
            }

            //드래그 범위 계산
            posMin.x = Mathf.Min(currentPos.x, beginPos.x);
            posMin.y = Mathf.Min(currentPos.y, beginPos.y);
            posMax.x = Mathf.Max(currentPos.x, beginPos.x);
            posMax.y = Mathf.Max(currentPos.y, beginPos.y);

            if (Input.GetMouseButtonUp(0))
            {
                Select();
                isDragBegin = false;
            }
        }

        /// <summary>
        /// 유닛 선택 함수
        /// </summary>
        private void Select()
        {
            bool flag = false;

            //컨트롤 키를 누리지 않았다면 선택한 유닛 목록 초기화
            if (!Input.GetKey(KeyCode.LeftControl))
            {
                DeselectAllAgents();
            }

            flag = DragSelect();

            //드래그 처리에서 아무런 유닛도 선택되지 않았다며 클릭 처리로 전환
            if (flag == false)
            {
                flag = ClickSelect();
            }
            
            //드래그와 클릭 모두 아무런 유닛도 선택하지 못했고 커트롤키도 누르고 있지 않다면 선택한 유닛 목록 초기화
            if (flag == false && !Input.GetKey(KeyCode.LeftControl))
            {
                DeselectAllAgents();
            }
        }

        /// <summary>
        /// 드래그 선택 처리 함수
        /// </summary>
        /// <returns></returns>
        private bool DragSelect()
        {
            bool flag = false;
            foreach (Agent agent in agents)
            {
                //유닛의 좌표를 월드 좌표에서 스크린 좌표로 변환한다.
                Vector3 agentPos = Camera.main.WorldToScreenPoint(agent.transform.position);

                //유닛이 드래그 범위 안에 있는지 확인한다.
                if (agentPos.x >= posMin.x && agentPos.y >= posMin.y &&
                    agentPos.x <= posMax.x && agentPos.y <= posMax.y)
                {
                    flag = true;
                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        //컨트롤 키를 누르고 있을 때 선택한 유닛이 선택한 유닛 목록에 있다면 선택 해제
                        if (selectedAgents.Contains(agent))
                        {
                            DeselectAgent(agent);
                        }
                        //목록에 없다며 선택
                        else
                        {
                            SelectAgent(agent);
                        }
                    }
                    else
                    {
                        SelectAgent(agent);
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// 클릭 선택 처리 함수
        /// </summary>
        /// <returns></returns>
        private bool ClickSelect()
        {
            bool flag = false;
            //마우스 좌표에 맞춰 레이를 쏜다.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Agent")))
            {
                flag = true;
                Agent agent = hit.collider.GetComponent<Agent>();
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    if (selectedAgents.Contains(agent))
                    {
                        DeselectAgent(agent);
                    }
                    else
                    {
                        SelectAgent(agent);
                    }
                }
                else
                {
                    SelectAgent(agent);
                }
            }
            return flag;
        }

        private void SelectAgent(Agent agent)
        {
            agent.IsSelected = true;
            agent.ToggleSelectUI();
            selectedAgents.Add(agent);
        }

        private void DeselectAgent(Agent agent)
        {
            agent.IsSelected = false;
            agent.ToggleSelectUI();
            selectedAgents.Remove(agent);
        }

        private void DeselectAllAgents()
        {
            foreach (Agent agent in selectedAgents)
            {
                agent.IsSelected = false;
                agent.ToggleSelectUI();
            }

            selectedAgents.Clear();
        }

        private void Move()
        {
            if (Input.GetMouseButtonUp(1))
            {
                //카메라와 월드상의 거리를 구해 z값에 넣어준다. 그래야 좌표가 정상적으로 나온다.
                Vector3 destination = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Sqrt(Mathf.Pow(10, 2) + Mathf.Pow(-10, 2))));

                Vector3 targetPos = Vector3.zero;
                int count = 0;
                float xOffset = 2f;
                float zOffset = 2f;
                float sqrt = Mathf.Sqrt(selectedAgents.Count);
                float startX = targetPos.x;

                for (int i = 0; i < selectedAgents.Count; i++)
                {
                    targetPos = new Vector3(targetPos.x, 0, targetPos.z);

                    //열 바꿈
                    if (count == Mathf.RoundToInt(sqrt))
                    {
                        count = 0;
                        targetPos.x = startX;
                        targetPos.z += zOffset;
                    }

                    selectedAgents[i].MoveTo(destination + targetPos);

                    count++;
                    targetPos.x += xOffset;
                }
            }
        }

        private void OnGUI()
        {
            if (!isDragBegin) return;
            Rect rect = new Rect();
            rect.min = new Vector2(posMin.x, Screen.height - posMin.y);
            rect.max = new Vector2(posMax.x, Screen.height - posMax.y);

            GUI.Box(rect, "");
        }
    }
}