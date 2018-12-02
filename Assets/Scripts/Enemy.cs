using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public GameObject target;
    public GameObject explosion;
    private UnityEngine.AI.NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        target = GameObject.FindWithTag("player");
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        if (GameManager.Instance.gameover)
        {
            return;
        }
        if (gameObject && target)
            agent.SetDestination(target.transform.position);

        Collider[] cols = Physics.OverlapSphere(transform.position, 0.6f);
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].transform.tag == "player")
            {
                animator.SetTrigger("attack");
                target.GetComponent<Animator>().SetBool("param_idletoko_big", true);
                GameManager.Instance.gameover = true;
                GameManager.Instance.isEnd = true;
                GameManager.Instance.animator.SetTrigger("end");
            }
        }
    }

    public void PushBack(Transform playerTrans)
    {
        GameObject ex1 =  Instantiate(explosion, transform);
        Vector3 offset = 5 * (transform.position - playerTrans.position);
        offset.y = transform.position.y;
        transform.Translate(offset);
        Destroy(ex1, 1f);
    }
}
