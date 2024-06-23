using UnityEngine;
using UnityEngine.AI;

public class ZCZombieManager : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1.5f;
    [SerializeField] Transform target;
    [SerializeField] Animator anim;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] SkinnedMeshRenderer colorBody;

    void Start()
    {
        agent.speed = moveSpeed;
        colorBody.material.color = ZCGameManager.Instance.Colors[Random.Range(0, ZCGameManager.Instance.Colors.Length)]; 
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        agent.SetDestination(target.position);
        anim.SetBool(StateAnimationZombie.Run.ToString(), true);
    }

    public Animator Anim
    {
        get
        {
            return anim;
        }
    }
    public NavMeshAgent Agent
    {
        get
        {
            return agent;
        }
    }
    public enum StateAnimationZombie { Walk, Run, Win }
}
