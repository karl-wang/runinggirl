using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleManager : MonoBehaviour {
    public GameObject model;

    //位置状态，1为地面，2为空中
    int location_state = 1;

    //动作状态，1默认，2跑步，3跳跃，4技能，5攻击，6死亡
    int animation_state = 1;

    //是否死亡
    bool isDeath = false;

    //死亡动画持续时间
    public float deathAnimationDuration = 1;

    //死亡动画倒计时
    public float deathRemainingTime = 0;

    //实际移动速度
    Vector3 actualSpeed = new Vector3(0, 0, 0);

    //重力加速度
    Vector3 gAcceleration = new Vector3(0, -9.8f, 0);

    //跳跃初速度
    public float jumpV0 = 5;

    //最大下降速度
    public float minSpeedY = -30;
    //最大速度
    public float maxSpeed = 30;

    public CharacterController controller;
    public CollisionFlags collisionFlags;
    public float lastLeaveGrounded = 0;
    public float lastLeaveAir = 0;
    public float baseMoveSpeed = 5;
    public float realMoveSpeed = 0;
    public float gravity = 1;

    public Dictionary<int, bool> curBuffList = new Dictionary<int, bool>();
    Dictionary<int, float> BuffCountDown = new Dictionary<int, float>();

    float baseSpeedRate = 2;
    public float speedRate = 1.0f;
    

    // Use this for initialization
    void Start () {
        animation_state = 1;
        controller = GetComponent<CharacterController>();
        model.GetComponent<Animation>()["Run"].speed = speedRate;
    }
	
	// Update is called once per frame
	void Update () {
		if (!isDeath)
        {
            float v = 1.0f;
            float h = Input.GetAxisRaw("Horizontal");
            if (location_state == 1)
            {
                Vector3 movedirection = new Vector3(h, 0, v);

                //保证斜走的速度和直走的速度一致(向量单位化)
                movedirection = movedirection.normalized;

                //模型根据按键情况转向
                if (v != 0 || h != 0)
                {
                    model.transform.rotation = Quaternion.LookRotation(transform.rotation * movedirection);
                    animation_state = 2;
                }
                else
                {
                    model.transform.rotation = Quaternion.LookRotation(transform.rotation * Vector3.forward);
                    animation_state = 1;
                }

                //真实地面移动的速度
                realMoveSpeed = baseMoveSpeed * speedRate;

                //真实三维的移动速度
                actualSpeed = transform.rotation * movedirection * realMoveSpeed;

                //判断跳跃键
                if (Input.GetAxisRaw("Jump") > 0 && location_state == 1)
                {
                    animation_state = 3;
                    ApplyJump();
                }
            }
            calcBuffList();
            AddGravity();   //增加重力加速度
            UpdateLocation();   //根据真实三维速度计算位置
            UpdateState();      //更新状态
        }
        else
        {
            deathRemainingTime -= Time.deltaTime;
            if (deathRemainingTime <= 0)
            {   //死亡动画时间结束
                //main.GameOver ();
                Time.timeScale = 0;
            }
        }
        UpdateAnimation();	//更新角色模型骨骼动画
    }

    void calcBuffList()
    {
        speedRate = baseSpeedRate;
        foreach (KeyValuePair<int, bool> kvp in curBuffList)
        {
            if (kvp.Value)
            {
                if (kvp.Key == 1)
                {
                    speedRate *= 1.5f;
                }

                if (kvp.Key == 2)
                {
                    speedRate *= 2.0f;
                }

                if (kvp.Key == 3)
                {
                    speedRate *= 0.5f;
                }
            }
        }
    }

    void ApplyJump ()
    {
        lastLeaveGrounded = Time.time;
        location_state = 2;
        actualSpeed.y += jumpV0;
    }

    void AddGravity()
    {
        actualSpeed += gAcceleration * Time.deltaTime;
    }

    void UpdateLocation()
    {
        //使用Move有返回值，可用来判断是否到达地面，而SimpleMove没有返回值
        collisionFlags = controller.Move(actualSpeed * Time.deltaTime);
    }

    void UpdateState()
    {
        var is_grounded = IsGrounded();
        if (location_state == 1 && !is_grounded && lastLeaveAir + 0.5 < Time.time)
        {
            lastLeaveGrounded = Time.time;
            location_state = 2;
        }

        if (location_state == 2 && is_grounded && lastLeaveGrounded + 0.5 < Time.time)
        {
            lastLeaveAir = Time.time;
            location_state = 1;
        }

        if (location_state == 1)
        {
            actualSpeed.y = 0;
        }
    }

    void UpdateAnimation()
    {
        if (animation_state == 1)
        {
            model.GetComponent< Animation > ().CrossFade("Idle");
        }
        else if (animation_state == 2)
        {
            model.GetComponent<Animation>()["Run"].speed = speedRate;
            model.GetComponent< Animation > ().CrossFade("Run");
        }
        else if (animation_state == 3)
        {
            model.GetComponent< Animation > ().CrossFade("jump");
        }
        else if (animation_state == 4)
        {
            model.GetComponent< Animation > ().CrossFade("Skill");
        }
        else if (animation_state == 5)
        {
            model.GetComponent< Animation > ().CrossFade("Attack", 0.2f);
        }
        else if (animation_state == 6)
        {
            model.GetComponent< Animation > ().CrossFade("Death");
        }
    }

    bool IsGrounded()
    {
        return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
    }

    public void addBuff(int buffId)
    {
        curBuffList.Add(buffId, true);
    }

    public void removeBuff(int buffId)
    {
        curBuffList.Remove(buffId);
    }
}
