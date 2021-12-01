using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RawMouseDriver;
using RawInputSharp;

public class Player : MonoBehaviour
{
    //多鼠标
    private RawMouseDriver.RawMouseDriver mousedriver;
    private RawMouse mouse1;
    private RawMouse mouse2;

    public Transform m_transform;
    //角色控制器组件
    CharacterController m_ch;
    //角色移动速度
    float m_rotSpeed = 100.0f;
    float m_movSpeed = 0.1f;
    float maxR = 30.0f;
    float rh = 0.0f;
    float rv = 0.0f;
    //重力
    float m_gravity = 2.0f;
    //摄像机Transform
    Transform m_camTransform;
    //摄像机旋转角度
    Vector3 m_camRot;
    //摄像机高度
    float m_camHeight = 1.4f;
    //修改Start函数, 初始化摄像机的位置和旋转角度
    void Start()
    {
        //多鼠标
        mousedriver = new RawMouseDriver.RawMouseDriver();

        m_transform = this.transform;
        //获取角色控制器组件
        m_ch = this.GetComponent<CharacterController>();
        //获取摄像机
        m_camTransform = Camera.main.transform;
        Vector3 pos = m_transform.position;
        pos.y += m_camHeight;
        m_camTransform.position = pos;
        //设置摄像机的旋转方向与主角一致
        m_camTransform.rotation = m_transform.rotation;
        m_camRot = m_camTransform.eulerAngles;
        //锁定鼠标
        Screen.lockCursor = true;
    }
    void Update()
    {
        Control();
    }
    void Control()
    {
        //多鼠标
        mousedriver.GetMouse(0, ref mouse1);
        mousedriver.GetMouse(1, ref mouse1);
        mousedriver.GetMouse(0, ref mouse2);
        mousedriver.GetMouse(1, ref mouse2);

        //定义3个值控制移动
        float xm = 0, ym = 0, zm = 0;
        //重力运动
        //ym -= m_gravity * Time.deltaTime;
        //获取鼠标移动距离
        //float drh = Input.GetAxis("Mouse X");
        //float drv = Input.GetAxis("Mouse Y");
        float drh = mouse1.XDelta;
        float drv = mouse1.YDelta;
        Debug.Log(drh);
        Debug.Log(drv);
        rh += drh;
        rv += drv;
        if(Input.GetMouseButtonDown(0))
        {
            rh = 0;
            rv = 0;
        }
        //前后左右移动
        if (rh > maxR)
            xm += maxR * m_movSpeed * Time.deltaTime;
        else if (rh < -maxR)
            xm += -maxR * m_movSpeed * Time.deltaTime;
        else
            xm += rh * m_movSpeed * Time.deltaTime;
        if (rv > maxR)
            zm += maxR * m_movSpeed * Time.deltaTime;
        else if (rv < -maxR)
            zm += -maxR * m_movSpeed * Time.deltaTime;
        else
            zm += rv * m_movSpeed * Time.deltaTime;
        //使用角色控制器提供的Move函数进行移动
        m_ch.Move(m_transform.TransformDirection(new Vector3(xm, ym, zm)));
        /*if (Input.GetKey(KeyCode.W))
        {
            zm += m_movSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            zm -= m_movSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            xm -= m_movSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            xm += m_movSpeed * Time.deltaTime;
        }*/

        //旋转摄像机
        //m_camRot.x -= rv;
        //m_camRot.y += rh; 
        float drh2 = mouse2.XDelta;
        float drv2 = mouse2.YDelta;
        m_camRot.x -= drv2;
        m_camRot.y += drh2;
        Debug.Log(drh2);
        Debug.Log(drv2);
        /*if (Input.GetKey(KeyCode.W))
        {
            m_camRot.x -= m_rotSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            m_camRot.x += m_rotSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            m_camRot.y -= m_rotSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            m_camRot.y += m_rotSpeed * Time.deltaTime;
        }*/
        m_camTransform.eulerAngles = m_camRot;

        //使角色的面向方向与摄像机一致
        Vector3 camrot = m_camTransform.eulerAngles;
        camrot.x = 0; camrot.z = 0;
        m_transform.eulerAngles = camrot;
        //操作角色移动代码
        //使摄像机位置与角色一致
        Vector3 pos = m_transform.position;
        pos.y += m_camHeight;
        m_camTransform.position = pos;
    }
}
