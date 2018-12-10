using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public GameObject target;
    public GameObject explosion;
    public List<Vector3> targets = new List<Vector3>();
    private UnityEngine.AI.NavMeshAgent agent;
    private Animator animator;
    private bool arrival = false;
    private bool find = false;
    private int arrFlag = 0;

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
        if ((transform.position - targets[arrFlag]).sqrMagnitude < 1)
        {
            arrival = true;
        }
        if (gameObject && target && !find)
        {
            if (arrFlag == 0 && arrival)
            {
                arrFlag += 1;
                arrival = false;
            }
            else if (arrFlag == 1 && arrival)
            {
                arrFlag -= 1;
                arrival = false;
            }
            agent.SetDestination(targets[arrFlag]);
        }
        else if (find)
        {
            agent.SetDestination(target.transform.position);
        }
            
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
        cols = Physics.OverlapSphere(transform.position, 3f);
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].transform.tag == "player")
            {
                find = true;
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
