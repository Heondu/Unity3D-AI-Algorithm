using System.Collections.Generic;
using UnityEngine;

namespace AIAlgorithm.Formation
{
    public class FormationController : MonoBehaviour
    {
        private bool isDragBegin = false;
        private Vector2 beginPos; //���콺 ���� ��ġ
        private Vector2 currentPos;
        private Vector2 posMin; //rect
        private Vector2 posMax;

        private Agent[] agents; //������ ���� �迭
        private List<Agent> selectedAgents; //������ ������ ������ ����Ʈ

        private void Start()
        {
            //���� �ִ� ��� ������ �����´�.
            agents = FindObjectsOfType<Agent>();
            selectedAgents = new List<Agent>();
        }

        private void Update()
        {
            Drag();

            Move();
        }

        /// <summary>
        /// �巡�� ó���� �ϴ� �Լ�
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

            //�巡�� ���� ���
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
        /// ���� ���� �Լ�
        /// </summary>
        private void Select()
        {
            bool flag = false;

            //��Ʈ�� Ű�� ������ �ʾҴٸ� ������ ���� ��� �ʱ�ȭ
            if (!Input.GetKey(KeyCode.LeftControl))
            {
                DeselectAllAgents();
            }

            flag = DragSelect();

            //�巡�� ó������ �ƹ��� ���ֵ� ���õ��� �ʾҴٸ� Ŭ�� ó���� ��ȯ
            if (flag == false)
            {
                flag = ClickSelect();
            }
            
            //�巡�׿� Ŭ�� ��� �ƹ��� ���ֵ� �������� ���߰� ĿƮ��Ű�� ������ ���� �ʴٸ� ������ ���� ��� �ʱ�ȭ
            if (flag == false && !Input.GetKey(KeyCode.LeftControl))
            {
                DeselectAllAgents();
            }
        }

        /// <summary>
        /// �巡�� ���� ó�� �Լ�
        /// </summary>
        /// <returns></returns>
        private bool DragSelect()
        {
            bool flag = false;
            foreach (Agent agent in agents)
            {
                //������ ��ǥ�� ���� ��ǥ���� ��ũ�� ��ǥ�� ��ȯ�Ѵ�.
                Vector3 agentPos = Camera.main.WorldToScreenPoint(agent.transform.position);

                //������ �巡�� ���� �ȿ� �ִ��� Ȯ���Ѵ�.
                if (agentPos.x >= posMin.x && agentPos.y >= posMin.y &&
                    agentPos.x <= posMax.x && agentPos.y <= posMax.y)
                {
                    flag = true;
                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        //��Ʈ�� Ű�� ������ ���� �� ������ ������ ������ ���� ��Ͽ� �ִٸ� ���� ����
                        if (selectedAgents.Contains(agent))
                        {
                            DeselectAgent(agent);
                        }
                        //��Ͽ� ���ٸ� ����
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
        /// Ŭ�� ���� ó�� �Լ�
        /// </summary>
        /// <returns></returns>
        private bool ClickSelect()
        {
            bool flag = false;
            //���콺 ��ǥ�� ���� ���̸� ���.
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
                //ī�޶�� ������� �Ÿ��� ���� z���� �־��ش�. �׷��� ��ǥ�� ���������� ���´�.
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

                    //�� �ٲ�
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