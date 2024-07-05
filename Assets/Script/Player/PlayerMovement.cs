using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using static MyTool;

/*
 * 攻击时向当前视角的正前方攻击 包括大剑
 * 
 * AI的移动和攻击也可以开始写了
 */


public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;


    private Vector3 offset; // 摄像机与人物之间的偏移量
    private float offdistance;

    public Transform cameraTransform;
    [SerializeField, Header("鼠标移动速度")] private float MouseSensitivity = 10f;
    [SerializeField, Header("视角上下移动最大角度")] private Vector2 MaxminAngle;
    [SerializeField, Header("站立视角围绕点")] private Transform StandCameraLook;


    [SerializeField, Header("人物移动速度")] private float Speed = 5f;

    [SerializeField, Header("鼠标是否可以输入")] private bool canLook = true;//鼠标是否可以输入



    [SerializeField, Header("在地检测半径")] private float CheckRidus = 0.2f;//检测半径
    [SerializeField, Header("是否在地面")] private bool isGround;
    public LayerMask layerMask;

    private Vector3 Velocity = Vector3.zero;

    [SerializeField, Header("重力")] private float Gravity = -9.8f;
    [SerializeField, Header("跳跃高度")] private float JumpHeight = 3f;

    [SerializeField, Header("下蹲状态")] private bool isCrouch = false;
    //private float DownCamera = 1.28f;

    private float mX;
    private float mY;
    [SerializeField, Header("是否锁定视角")] protected int islocked = 0;
    [SerializeField, Header("当前索敌")] private Collider[] currentTarget = new Collider[1];

    private Animator anima;
    [SerializeField, Header("动画移动倍率")] private float AnimationMoveMul = 10f;
    void Start()
    {
        islocked = 0;
        anima = GetComponent<Animator>();
        // 初始化偏移量
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
        // 判断当前动画是否播放完毕

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
            // 释放鼠标锁定
            Cursor.lockState = CursorLockMode.None;
            // 显示鼠标
            Cursor.visible = true;
            // 设置为不可以处理鼠标输入
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
            //摄像头移动
            CameraMove();


            //蹲伏
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
            //冲刺判断
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
            if (islocked == 0)//人物转向
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
    [SerializeField, Header("人物旋转速度")] private float rotationSpeed = 500f; // 旋转速度
    void RotatePlayer(float horizontalInput, float verticalInput)
    {
        Vector3 lookDirection = Vector3.zero;

        // 确定目标方向
        if (verticalInput != 0)
        {
            lookDirection += (verticalInput > 0) ? cameraTransform.forward : -cameraTransform.forward;
        }
        if (horizontalInput != 0)
        {
            lookDirection += (horizontalInput > 0) ? cameraTransform.right : -cameraTransform.right;
        }

        // 保持在水平面上
        lookDirection.y = 0;

        // 计算目标旋转
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
                // 将方向向量乘以之前的距离，得到位置点
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
        if (count == 0) Debug.Log("空的");

    }

    private void Rush()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //move = new Vector3(0, 0, vertical);//只接受前后移动
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
            ////摄像头向下移动
            ////Vector3 camepostion = cameraTransform.position;
            ////camepostion.y -= DownCamera;
            ////cameraTransform.position = camepostion;

            //修改charactercontroller的碰撞体大小，适应模型体积
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

            //修改charactercontroller的碰撞体大小，适应模型体积
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

    //    //Debug.Log(Time.time + "方向:" + moveDirection + "速度:" + moveSpeed);
    //    controller.Move((moveSpeed * Time.deltaTime) * moveDirection.normalized);
    //}


}

