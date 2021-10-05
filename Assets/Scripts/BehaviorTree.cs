using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIAlgorithm.BT
{
    /// <summary>
    /// �ൿ Ʈ���� �⺻�� �Ǵ� Ŭ����
    /// </summary>
    public class Node
    {
        public virtual bool Invoke()
        {
            return false;
        }
    }

    /// <summary>
    /// Node�� ��ӹ޾� �ڽ� �߰� ����� ������ Ŭ����
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
    /// CompositNode�� ����Ͽ� �ڽĵ��� ������ �˻��ϴ� Ŭ����
    /// �ڽ� �� �ϳ��� true�� �����ϸ� true�� �����Ѵ�.
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
    /// CompositNode�� ����Ͽ� �ڽĵ��� ������ �˻��ϴ� Ŭ����
    /// �ڽ��� �ϳ��� false�� �����ϸ� false�� �����Ѵ�.
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
    /// ��带 ����Ͽ� ������ ���� Ŭ����
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
    /// ��带 ����Ͽ� ������ ���� Ŭ����
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
    /// ��带 ����Ͽ� ������ ���� Ŭ����
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
    /// ��带 ����Ͽ� ������ �׼� Ŭ����
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
    /// ��带 ����Ͽ� ������ �׼� Ŭ����
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
    /// ��带 ����Ͽ� ������ �׼� Ŭ����
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
    /// �ൿ Ʈ���� �ʱ�ȭ�ϴ� Ŭ����
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

        //���ǰ� �׼ǿ� ������ �Լ��� �������ִ� ������Ʈ
        private EnemyController enemyController;

        private void Start()
        {
            enemyController = FindObjectOfType<EnemyController>();
            enemyController.Init();

            //��Ʈ �������� �����Ϳ� ������ �߰��Ѵ�.
            root.AddChild(selector);
            root.AddChild(isPlayerDead);

            //�����Ϳ� ���� �������� �߰� ������, ���� �������� �߰��Ѵ�.
            selector.AddChild(seqAttack);
            selector.AddChild(seqChase);
            selector.AddChild(seqPatrol);

            //���ǰ� �׼ǿ� ������Ʈ�� �����Ѵ�.
            isPlayerDead.Init(enemyController);
            isHerePlayer.Init(enemyController);
            isPlayerInRange.Init(enemyController);
            attack.Init(enemyController);
            chase.Init(enemyController);
            patrol.Init(enemyController);

            //���� �������� ���ǰ� �׼��� �߰��Ѵ�.
            seqAttack.AddChild(isHerePlayer);
            seqAttack.AddChild(attack);

            //�߰� �������� ���ǰ� �׼��� �߰��Ѵ�.
            seqChase.AddChild(isPlayerInRange);
            seqChase.AddChild(chase);

            //���� �������� �׼��� �߰��Ѵ�.
            seqPatrol.AddChild(patrol);

            StartCoroutine("Loop");
        }

        private IEnumerator Loop()
        {
            //��Ʈ �������� ������ ���� ��� �ൿ Ʈ���� �����Ѵ�.
            while (!root.Invoke())
            {
                yield return null;
            }
        }
    }
}
