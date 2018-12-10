using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(CharacterController))]
public class MoveController : MonoBehaviour {

    Transform m_transform, m_camera;//人物自己以及相机的对象
    CharacterController controller;//Charactor Controller组件
    public float moveSpeed = 5.0f;//移动的速度

    private AudioSource audioSource;
    private List<AudioClip> hits = new List<AudioClip>();

    private string hit1Path = "Music/hit01";
    private string hit2Path = "Music/hit02";
    private Vector3 target;
    private int hitFlag = 0;
    private Animator animator;

    void Start()
    {
        m_transform = gameObject.transform;
        m_camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        hits.Add((AudioClip)Resources.Load(hit1Path, typeof(AudioClip)));
        hits.Add((AudioClip)Resources.Load(hit2Path, typeof(AudioClip)));
        target = new Vector3(10f, 0f, 40f);
    }
    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.gameover)
            return;
        if ((transform.position - target).sqrMagnitude < 1)
        {
            GameManager.Instance.isEnd = true;
            GameManager.Instance.gameover = true;
            GameManager.Instance.win = true;
            GameManager.Instance.animator.SetTrigger("end");
        }
        if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.A)) || (Input.GetKey(KeyCode.D)))
        {
            animator.SetBool("param_idletowalk", true);
            if (Input.GetKey(KeyCode.W))
            {
                //根据主相机的朝向决定人物的移动方向，下同
                controller.transform.eulerAngles = new Vector3(0, m_camera.transform.eulerAngles.y, 0);
            }

            if (Input.GetKey(KeyCode.S))
            {
                controller.transform.eulerAngles = new Vector3(0, m_camera.transform.eulerAngles.y + 180f, 0);
            }

            if (Input.GetKey(KeyCode.A))
            {
                controller.transform.eulerAngles = new Vector3(0, m_camera.transform.eulerAngles.y + 270f, 0);
            }

            if (Input.GetKey(KeyCode.D))
            {
                controller.transform.eulerAngles = new Vector3(0, m_camera.transform.eulerAngles.y + 90f, 0);
            }

            controller.Move(m_transform.forward * Time.deltaTime * moveSpeed);
        }
        else
        {
            //静止状态
            animator.SetBool("param_idletowalk", false);
            animator.SetBool("param_toidle", true);
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            animator.SetTrigger("attack");
            if (hitFlag == 0)
                hitFlag += 1;
            else
                hitFlag -= 1;
            audioSource.clip = hits[hitFlag];
            audioSource.Play();
            Collider[] cols = Physics.OverlapSphere(transform.position, 1.5f);

            if (cols.Length > 0)
            {
                for (int i = 0; i < cols.Length; i++)
                {
                    if (cols[i].tag == "enemy")
                    {

                        cols[i].gameObject.GetComponent<Enemy>().PushBack(transform);
                        GameManager.Instance.score += 1;
                    }
                }
            }
        }

        if(!controller.isGrounded)
        {
            controller.Move(new Vector3(0, -10f * Time.deltaTime, 0));
        }
    }


}
