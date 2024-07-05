using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using static MyTool;

/*
 * ����ʱ��ǰ�ӽǵ���ǰ������ ������
 * 
 * AI���ƶ��͹���Ҳ���Կ�ʼд��
 */


public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;


    private Vector3 offset; // �����������֮���ƫ����
    private float offdistance;

    public Transform cameraTransform;
    [SerializeField, Header("����ƶ��ٶ�")] private float MouseSensitivity = 10f;
    [SerializeField, Header("�ӽ������ƶ����Ƕ�")] private Vector2 MaxminAngle;
    [SerializeField, Header("վ���ӽ�Χ�Ƶ�")] private Transform StandCameraLook;


    [SerializeField, Header("�����ƶ��ٶ�")] private float Speed = 5f;

    [SerializeField, Header("����Ƿ��������")] private bool canLook = true;//����Ƿ��������



    [SerializeField, Header("�ڵؼ��뾶")] private float CheckRidus = 0.2f;//���뾶
    [SerializeField, Header("�Ƿ��ڵ���")] private bool isGround;
    public LayerMask layerMask;

    private Vector3 Velocity = Vector3.zero;

    [SerializeField, Header("����")] private float Gravity = -9.8f;
    [SerializeField, Header("��Ծ�߶�")] private float JumpHeight = 3f;

    [SerializeField, Header("�¶�״̬")] private bool isCrouch = false;
    //private float DownCamera = 1.28f;

    private float mX;
    private float mY;
    [SerializeField, Header("�Ƿ������ӽ�")] protected int islocked = 0;
    [SerializeField, Header("��ǰ����")] private Collider[] currentTarget = new Collider[1];

    private Animator anima;
    [SerializeField, Header("�����ƶ�����")] private float AnimationMoveMul = 10f;
    void Start()
    {
        islocked = 0;
        anima = GetComponent<Animator>();
        // ��ʼ��ƫ����
        offset = cameraTransform.position - StandCameraLook.position;
        offdistance = Vector3.Distance(cameraTransform.position, StandCameraLook.transform.position);

        anima.SetFloat("Locked", 0);

        controller = transform.GetComponent<CharacterController>();
        MaxminAngle.x = -60f;
        MaxminAngle.y = 60f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rotationSpeed = 500f;
        NowWeapon = Sword;

    }

    void Update()
    {
        //Debug.Log(Time.time+"Move:"+anima.CheckAnimationTag("Move", 0));
        //Debug.Log(Time.time + "Attack:" + anima.CheckAnimationTag("Attack", 0));
        if (anima.CheckAnimationTag("Move", 0))
        {
            MoveLocked(false);
        }
        else if (anima.CheckAnimationTag("Attack", 0) || anima.CheckAnimationTag("Dodge", 0))
        {
            MoveLocked(true);
        }
        MoveMent();
        // �жϵ�ǰ�����Ƿ񲥷����

    }

    private void LateUpdate()
    {

        //Debug.Log(anima.GetCurrentAnimatorStateInfo(0).IsName("Attack") ? "Attack" : "Unknown");
        DodgeAnimationMove();
    }

    private void MoveMent()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // �ͷ��������
            Cursor.lockState = CursorLockMode.None;
            // ��ʾ���
            Cursor.visible = true;
            // ����Ϊ�����Դ����������
            canLook = false;
        }
        else
        {
            isGround = Physics.CheckSphere(transform.position, CheckRidus, layerMask);
            //Debug.Log(isGround);
            if (isGround && Velocity.y < 0)
            {
                Velocity.y = 0;
            }
            Dodge();

            Velocity.y += Gravity * Time.deltaTime;
            controller.Move(Velocity * Time.deltaTime);

            if (isGround && Input.GetKeyDown(KeyCode.Space) && isCrouch == false)
            {
                anima.SetTrigger("Space");

                Velocity.y += Mathf.Sqrt(JumpHeight * -2 * Gravity);
            }

            anima.SetFloat("JumpHeight", transform.position.y);
            //Debug.Log(Velocity.y);

            LockEnemy();
            Move();
            //����ͷ�ƶ�
            CameraMove();


            //�׷�
            Crouch();

            ChangeWeapon();

        }


        if (Input.GetMouseButtonDown(0))
        {
            canLook = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }


    private void LockEnemy()
    {
        if (Input.GetMouseButtonDown(2))
        {

            DetectionTarget();
            islocked = islocked == 1 ? 0 : 1;
            if (islocked == 1) anima.SetFloat("Locked", 1);
            else anima.SetFloat("Locked", 0);
            //Debug.Log(Time.time + " " + islocked);
        }
    }


    private bool MoveLock = false;
    public void MoveLocked(bool IsMove)
    {
        MoveLock = IsMove;
    }

    private void Move()
    {
        if (MoveLock == false)
        {
            //����ж�
            Rush();

            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            anima.SetFloat("X", horizontal);
            anima.SetFloat("Y", vertical);

            if (horizontal != 0 || vertical != 0)
            {

                anima.SetBool("Moveing", true);

            }
            else
            {
                anima.SetFloat("Speed", 0);
                anima.SetBool("Moveing", false);
            }
            if (islocked == 0)//����ת��
                RotatePlayer(horizontal, vertical);
            else
            {
                var temp = transform.eulerAngles;
                temp.y = cameraTransform.eulerAngles.y;
                transform.eulerAngles = temp;
            }

            var move = new Vector3(horizontal, 0, vertical);

            move = cameraTransform.TransformDirection(move);


            move *= Speed * Time.deltaTime;
            //transform.forward* Speed *vertical * Time.deltaTime
            controller.Move(move);

        }
    }
    [SerializeField, Header("������ת�ٶ�")] private float rotationSpeed = 500f; // ��ת�ٶ�
    void RotatePlayer(float horizontalInput, float verticalInput)
    {
        Vector3 lookDirection = Vector3.zero;

        // ȷ��Ŀ�귽��
        if (verticalInput != 0)
        {
            lookDirection += (verticalInput > 0) ? cameraTransform.forward : -cameraTransform.forward;
        }
        if (horizontalInput != 0)
        {
            lookDirection += (horizontalInput > 0) ? cameraTransform.right : -cameraTransform.right;
        }

        // ������ˮƽ����
        lookDirection.y = 0;

        // ����Ŀ����ת
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }




    private void CameraMove()
    {
        if (islocked == 0)
        {
            mX += Input.GetAxis("Mouse X");
            mY -= Input.GetAxis("Mouse Y");

            mY = Mathf.Clamp(mY, MaxminAngle.x, MaxminAngle.y);

            Quaternion mRotation = Quaternion.Euler(mY, mX, 0);
            Vector3 mPostion = mRotation * offset + transform.position;

            cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, mRotation, Time.deltaTime * MouseSensitivity);
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, mPostion, Time.deltaTime * MouseSensitivity);
        }
        else
        {

            if (currentTarget == null) islocked = 0;
            else
            {

                Vector3 targetDirection = (currentTarget[0].transform.position - StandCameraLook.position).normalized;
                // ��������������֮ǰ�ľ��룬�õ�λ�õ�
                Vector3 newPosition = StandCameraLook.transform.position - targetDirection * offdistance;
                cameraTransform.position = Vector3.Lerp(cameraTransform.position, newPosition, Time.deltaTime * MouseSensitivity);

                Vector3 enemyCenter = currentTarget[0].transform.position;
                enemyCenter.y += 1.2f;
                cameraTransform.LookAt(enemyCenter);
                //Vector3 target1Direction = (enemyCenter - cameraTransform.position - offset).normalized;
                //Quaternion newRotation = Quaternion.LookRotation(target1Direction);
                //cameraTransform.rotation = newRotation;


            }
        }

    }


    //public Transform enemy;
    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(Vector3.zero, cameraTransform.position);
    //    Gizmos.DrawLine(Vector3.zero, transform.position);
    //    Gizmos.DrawLine(Vector3.zero, enemy.position);
    //    Gizmos.DrawLine(StandCameraLook.position, cameraTransform.position);
    //    Gizmos.DrawLine(enemy.position, StandCameraLook.position);
    //    Gizmos.DrawLine(enemy.position, cameraTransform.position);
    //    Gizmos.DrawLine(enemy.position, transform.position);

    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawLine(Vector3.zero, StandCameraLook.transform.position - (enemy.position - StandCameraLook.position).normalized * offdistance);
    //}


    public LayerMask layer;
    private void DetectionTarget()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, 10f, currentTarget, layer);
        if (count == 0) Debug.Log("�յ�");

    }

    private void Rush()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //move = new Vector3(0, 0, vertical);//ֻ����ǰ���ƶ�
            //move = transform.TransformDirection(move);
            //move *= Speed * Time.deltaTime;

            anima.SetFloat("Speed", 2);
            if (isCrouch == true)
            {
                Speed = 2.5f;
            }
            else
            {
                Speed = 6f;
            }

        }
        else
        {
            //move = new Vector3(horizontal, 0, vertical);
            //move = transform.TransformDirection(move);
            //move *= Speed * Time.deltaTime;

            anima.SetFloat("Speed", 1);
            if (isCrouch == true)
            {
                Speed = 1f;
            }
            else
            {
                Speed = 2.5f;
            }
        }
    }

    private void Dodge()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            anima.SetTrigger("Dodge");
        }
    }


    private void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.C) && isCrouch == false && NowWeapon == Sword)
        {
            ////����ͷ�����ƶ�
            ////Vector3 camepostion = cameraTransform.position;
            ////camepostion.y -= DownCamera;
            ////cameraTransform.position = camepostion;

            //�޸�charactercontroller����ײ���С����Ӧģ�����
            Vector3 temp = controller.center;
            temp.y = 0.55f;
            controller.center = temp;

            controller.height = 1.1f;

            isCrouch = true;
            anima.SetBool("Crouch", isCrouch);
            //Debug.Log(1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl) && isCrouch == true)
        {
            ////Vector3 camepostion = cameraTransform.position;
            ////camepostion.y += DownCamera;
            ////cameraTransform.position = camepostion;

            //�޸�charactercontroller����ײ���С����Ӧģ�����
            Vector3 temp = controller.center;
            temp.y = 1f;
            controller.center = temp;
            controller.height = 2f;

            isCrouch = false;
            anima.SetBool("Crouch", isCrouch);
            //Debug.Log(1);
        }
    }
    public GameObject NowWeapon;
    public GameObject Sword;
    public GameObject GS;
    public GameObject BackGS;

    private void ChangeWeapon()
    {
        if (Input.GetKeyDown(KeyCode.F) && isCrouch == false)
        {
            if (NowWeapon == Sword)
            {
                anima.SetFloat("Equip", 2);
            }
            else
            {
                anima.SetFloat("Equip", 1);
            }
            anima.SetTrigger("ChangeWeapon");
        }
    }

    public GameObject GetCurrentWeapon() => NowWeapon;

    private void HideWeapon(int Equip)
    {
        NowWeapon.gameObject.SetActive(false);
        if (Equip == 2)
        {
            NowWeapon = GS;
            BackGS.gameObject.SetActive(false);
        }
        else if (Equip == 1)
        {
            NowWeapon = Sword;
            BackGS.gameObject.SetActive(true);
        }
        NowWeapon.gameObject.SetActive(true);
    }



    private void DodgeAnimationMove()
    {
        if (anima.CheckAnimationTag("Dodge", 0))
        {
            var X = anima.GetFloat("X");
            var Y = anima.GetFloat("Y");


            if (Math.Abs(X) > Math.Abs(Y))
            {
                AnimationMovement(controller, transform.right, anima.GetFloat("DodgeMove") * AnimationMoveMul);

            }
            else if (Math.Abs(X) <= Math.Abs(Y))
            {
                AnimationMovement(controller, transform.forward, anima.GetFloat("DodgeMove") * AnimationMoveMul);

                //Debug.Log(1);
            }
            //else if (X == 0 && Y == 0)
            //{
            //    Debug.Log(1);
            //    AnimationMovement(controller, transform.forward, anima.GetFloat("DodgeMove") * AnimationMoveMul);
            //}
        }
    }


    public CharacterController GetController() => this.controller;

    //private void AnimationMovement(Vector3 moveDirection, float moveSpeed)
    //{

    //    //Debug.Log(Time.time + "����:" + moveDirection + "�ٶ�:" + moveSpeed);
    //    controller.Move((moveSpeed * Time.deltaTime) * moveDirection.normalized);
    //}


}

