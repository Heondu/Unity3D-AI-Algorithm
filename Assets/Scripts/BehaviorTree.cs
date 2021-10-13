using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIAlgorithm.BT
{
    /// <summary>
    /// 행동 트리의 기본이 되는 클래스
    /// </summary>
    public class Node
    {
        public virtual bool Invoke()
        {
            return false;
        }
    }

    /// <summary>
    /// Node를 상속받아 자식 추가 기능을 구현한 클래스
    /// </summary>
    public class CompositNode : Node
    {
        private List<Node> children = new List<Node>();

        public void AddChild(Node node)
        {
            children.Add(node);
        }

        public List<Node> GetChildren()
        {
            return children;
        }
    }

    /// <summary>
    /// CompositNode를 상속하여 자식들의 조건을 검사하는 클래스
    /// 자식 중 하나라도 true를 리턴하면 true를 리턴한다.
    /// </summary>
    public class Selector : CompositNode
    {
        public override bool Invoke()
        {
            foreach (Node node in GetChildren())
            {
                if (node.Invoke())
                {
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// CompositNode를 상속하여 자식들의 조건을 검사하는 클래스
    /// 자식중 하나라도 false를 리턴하면 false를 리턴한다.
    /// </summary>
    public class Sequence : CompositNode
    {
        public override bool Invoke()
        {
            foreach (Node node in GetChildren())
            {
                if (!node.Invoke())
                {
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// 노드를 상속하여 구현한 조건 클래스
    /// </summary>
    public class IsPlayerDead : Node
    {
        private EnemyController enemyController;

        public void Init(EnemyController _enemyController)
        {
            enemyController = _enemyController;
        }

        public override bool Invoke()
        {
            return enemyController.IsPlayerDead();
        }
    }

    /// <summary>
    /// 노드를 상속하여 구현한 조건 클래스
    /// </summary>
    public class IsHerePlayer : Node
    {
        private EnemyController enemyController;

        public void Init(EnemyController _enemyController)
        {
            enemyController = _enemyController;
        }

        public override bool Invoke()
        {
            return enemyController.IsHerePlayer();
        }
    }

    /// <summary>
    /// 노드를 상속하여 구현한 조건 클래스
    /// </summary>
    public class IsPlayerInRange : Node
    {
        private EnemyController enemyController;

        public void Init(EnemyController _enemyController)
        {
            enemyController = _enemyController;
        }

        public override bool Invoke()
        {
            return enemyController.IsPlayerInRange();
        }
    }

    /// <summary>
    /// 노드를 상속하여 구현한 액션 클래스
    /// </summary>
    public class Attack : Node
    {
        private EnemyController enemyController;

        public void Init(EnemyController _enemyController)
        {
            enemyController = _enemyController;
        }

        public override bool Invoke()
        {
            enemyController.Attack();
            return true;
        }
    }

    /// <summary>
    /// 노드를 상속하여 구현한 액션 클래스
    /// </summary>
    public class Chase : Node
    {
        private EnemyController enemyController;

        public void Init(EnemyController _enemyController)
        {
            enemyController = _enemyController;
        }

        public override bool Invoke()
        {
            enemyController.Chase();
            return true;
        }
    }

    /// <summary>
    /// 노드를 상속하여 구현한 액션 클래스
    /// </summary>
    public class Patrol : Node
    {
        private EnemyController enemyController;

        public void Init(EnemyController _enemyController)
        {
            enemyController = _enemyController;
        }

        public override bool Invoke()
        {
            enemyController.Patrol();
            return true;
        }
    }

    /// <summary>
    /// 행동 트리를 초기화하는 클래스
    /// </summary>
    public class BehaviorTree : MonoBehaviour
    {
        private Sequence root = new Sequence();
        private Selector selector = new Selector();
        private Sequence seqAttack = new Sequence();
        private Sequence seqChase = new Sequence();
        private Sequence seqPatrol = new Sequence();

        private IsPlayerDead isPlayerDead = new IsPlayerDead();
        private IsHerePlayer isHerePlayer = new IsHerePlayer();
        private IsPlayerInRange isPlayerInRange = new IsPlayerInRange();
        private Attack attack = new Attack();
        private Chase chase = new Chase();
        private Patrol patrol = new Patrol();

        //조건과 액션에 연결할 함수를 가지고있는 컴포넌트
        private EnemyController enemyController;

        private void Start()
        {
            enemyController = FindObjectOfType<EnemyController>();
            enemyController.Init();

            //루트 시퀀스에 셀렉터와 조건을 추가한다.
            root.AddChild(selector);
            root.AddChild(isPlayerDead);

            //셀렉터에 공격 시퀀스와 추격 시퀀스, 순찰 시퀀스를 추가한다.
            selector.AddChild(seqAttack);
            selector.AddChild(seqChase);
            selector.AddChild(seqPatrol);

            //조건과 액션에 컴포넌트를 연결한다.
            isPlayerDead.Init(enemyController);
            isHerePlayer.Init(enemyController);
            isPlayerInRange.Init(enemyController);
            attack.Init(enemyController);
            chase.Init(enemyController);
            patrol.Init(enemyController);

            //공격 시퀀스에 조건과 액션을 추가한다.
            seqAttack.AddChild(isHerePlayer);
            seqAttack.AddChild(attack);

            //추격 시퀀스에 조건과 액션을 추가한다.
            seqChase.AddChild(isPlayerInRange);
            seqChase.AddChild(chase);

            //순찰 시퀀스에 액션을 추가한다.
            seqPatrol.AddChild(patrol);

            StartCoroutine("Loop");
        }

        private IEnumerator Loop()
        {
            //루트 시퀀스의 조건이 참일 경우 행동 트리를 종료한다.
            while (!root.Invoke())
            {
                yield return null;
            }
        }
    }
}
