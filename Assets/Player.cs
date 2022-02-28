
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Windows;
using Assets;
using Linearstar.Windows.RawInput;

public class Player : MonoBehaviour
{
    //退出
    private bool exit = false;

    //角色控制器组件
    CharacterController m_ch;

    //角色移动
    float tranSpeed = 0.3f;
    float maxR = 250.0f;
    float maxDist = 50.0f;
    static float rh = 0.0f;
    static float rv = 0.0f;
    static float mouseRot_x = 0.0f;
    static float mouseRot_y = 0.0f;
    static float mouseRot_x_ = 0.0f; //坐标系变换
    static float mouseRot_y_ = 0.0f;

    float transSense = 0.04f; //平移灵敏度
    float rotatSense = 0.6f; //转动灵敏度

    //平移方式
    static int transType = 0; //0距离，1速度

    //重力
    float m_gravity = 2.0f;

    //Transform
    public Transform m_transform;
    Transform m_camTransform;

    //摄像机旋转角度
    Vector3 m_camRot;
    //摄像机高度
    float m_camHeight = 1.4f;

    //鼠标对应类
    static MyMouse mouseRot, mouseTrans;
    static Dictionary<int, MyMouse> mouseMap = new Dictionary<int, MyMouse>();

    //记录位置变化次数
    static int count = 0;

    //获取窗口句柄，用于获得鼠标rawInput
    //[System.Runtime.InteropServices.DllImport("user32.dll")]
    //private static extern System.IntPtr GetForegroundWindow();

    //响应rawInput的线程开始函数
    public static void threadStartFunc()
    {
        Debug.Log("start new thread");

        //获取电脑连接的鼠标设备
        var devices = RawInputDevice.GetDevices();
        var mice = devices.OfType<Linearstar.Windows.RawInput.RawInputMouse>();
        foreach (var device in mice)
        {
            Console.WriteLine($"{device.DeviceType} {device.VendorId:X4}:{device.ProductId:X4}" +
                $" {device.ProductName}, {device.ManufacturerName}");
            Debug.Log($"{device.DeviceType} {device.VendorId:X4}:{device.ProductId:X4}" +
                $" {device.ProductName}, {device.ManufacturerName}");
            if (device.VendorId != 0 && device.ProductId != 0)
            {
                int id = (device.VendorId << 16) + device.ProductId;
                MyMouse mouse = new MyMouse();
                mouse.id = id; 
                if (device.VendorId == 0x046D && device.ProductId == 0xC534) //我的鼠标，平移
                {
                    mouse.type = 0;
                    mouseTrans = mouse;
                    Debug.Log($"mouseTrans: {id:X8}");
                }
                else if (device.VendorId == 0x413C && device.ProductId == 0x301A) //京东买的普通鼠标，平移
                {
                    mouse.type = 0;
                    mouseTrans = mouse;
                    Debug.Log($"mouseTrans: {id:X8}");
                }
                else if (device.VendorId == 0x062A && device.ProductId == 0x727A) //轨迹球鼠标，转动
                {
                    mouse.type = 1;
                    mouseRot = mouse;
                    Debug.Log($"mouseRot: {id:X8}");
                } 
                else // 其他鼠标模拟视角转动
                {
                    mouse.type = 1;
                    mouseRot = mouse;
                    Debug.Log($"mouseRot: {id:X8}");
                }
                mouseMap.Add(id, mouse);
            }
        }

        //获取电脑连接的键盘设备
        var keybords = devices.OfType<Linearstar.Windows.RawInput.RawInputKeyboard>();
        foreach (var device in keybords)
        {
            Console.WriteLine($"{device.DeviceType} {device.VendorId:X4}:{device.ProductId:X4}" +
                $" {device.ProductName}, {device.ManufacturerName}");
            Debug.Log($"{device.DeviceType} {device.VendorId:X4}:{device.ProductId:X4}" +
                $" {device.ProductName}, {device.ManufacturerName}");
        }

        var window = new RawInputReceiverWindow();

        //监听获取rawInput输入
        window.Input += (sender, e) =>
        {
            //Debug.Log("window.Input");
            var data = e.Data;
            Console.WriteLine(data);
            switch (data)
            {
                case RawInputMouseData mouseData:
                    Console.Write("WndProc ");
                    Console.WriteLine(mouseData.Mouse);
                    Debug.Log(mouseData.Mouse);
                    int id = (((RawInputData)data).Device.VendorId << 16) +
                    ((RawInputData)data).Device.ProductId;
                    MyMouse mouse = mouseMap[id];
                    /*if (((RawInputMouseData)data).Mouse.Buttons ==
                    Linearstar.Windows.RawInput.Native.RawMouseButtonFlags.RightButtonDown)
                    {
                        //按右键停止平移
                        mouseTrans.x = 0;
                        mouseTrans.y = 0;
                    }*/
                    if (((RawInputMouseData)data).Mouse.Buttons ==
                    Linearstar.Windows.RawInput.Native.RawMouseButtonFlags.None)
                    {

                        mouse.x += ((RawInputMouseData)data).Mouse.LastX;
                        mouse.y += ((RawInputMouseData)data).Mouse.LastY;
                    }
                    else if (transType == 0 &&
                    ((RawInputMouseData)data).Mouse.Buttons ==
                    Linearstar.Windows.RawInput.Native.RawMouseButtonFlags.MiddleButtonDown)
                    {
                        //换成速度控制平移
                        transType = 1;
                        mouseTrans.x = 0;
                        mouseTrans.y = 0;
                    }
                    else if (transType == 1 &&
                    ((RawInputMouseData)data).Mouse.Buttons ==
                    Linearstar.Windows.RawInput.Native.RawMouseButtonFlags.MiddleButtonUp)
                    {
                        //换成距离控制平移
                        transType = 0;
                        mouseTrans.x = 0;
                        mouseTrans.y = 0;
                        rh = 0.0f;
                        rv = 0.0f;
                    }
                    break;
                case RawInputKeyboardData keyboard:
                    Console.Write("WndProc ");
                    Console.WriteLine(keyboard.Keyboard);
                    if (keyboard.Keyboard.ScanCode == 1)
                    {
                        mouseTrans.x = 0;
                        mouseTrans.y = 0;
                        //按Esc退出
                        Debug.Log("RawInputDevice UnregisterDevice");
                        RawInputDevice.UnregisterDevice(HidUsageAndPage.Mouse);
                        RawInputDevice.UnregisterDevice(HidUsageAndPage.Keyboard);
                        System.Windows.Forms.Application.ExitThread();
                    }
                    break;
                case RawInputHidData hid:
                    Console.Write("WndProc ");
                    Console.WriteLine(hid.Hid);
                    break;
            }
        };

        try {
            Debug.Log($"window.Handle: {window.Handle}");
            RawInputDevice.RegisterDevice(HidUsageAndPage.Mouse, 
                RawInputDeviceFlags.ExInputSink | RawInputDeviceFlags.NoLegacy, window.Handle);
            RawInputDevice.RegisterDevice(HidUsageAndPage.Keyboard,
                RawInputDeviceFlags.ExInputSink | RawInputDeviceFlags.NoLegacy, window.Handle);
            System.Windows.Forms.Application.Run();
        }
        finally {
            Debug.Log("RawInputDevice UnregisterDevice");
            RawInputDevice.UnregisterDevice(HidUsageAndPage.Mouse);
            RawInputDevice.UnregisterDevice(HidUsageAndPage.Keyboard);
        }
    }


    //修改Start函数, 初始化摄像机的位置和旋转角度
    void Start()
    {
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
        Debug.Log("lock cursor");
        
        ThreadStart inputref = new ThreadStart(threadStartFunc);
        Console.WriteLine("In Main: Creating the moues thread");
        Thread  inputThread = new Thread(inputref);
        inputThread.Start();
        
    }

    void Update()
    {
        //RawInput线程还没获取设备
        //Debug.Log("update");
        Control();
    }

    void Control()
    {
        if (mouseTrans == null || mouseRot == null) return;
        else Debug.Log($"mouseTrans: ({mouseTrans.x}, {mouseTrans.y})");


        //定义3个值控制移动
        float xm = 0, ym = 0, zm = 0;

        //重力运动 ym -= m_gravity * Time.deltaTime;

        //前后左右平移
        if (transType == 0) //距离
        {
            float drh = mouseTrans.x * transSense - rh;
            float drv = -mouseTrans.y * transSense - rv;
            rh = mouseTrans.x * transSense;
            rv = -mouseTrans.y * transSense;
            if (drh > maxDist) xm += maxDist;
            else if (drh < -maxDist) xm += -maxDist;
            else xm += drh;
            if (drv > maxDist) zm += maxDist;
            else if (drv < -maxDist) zm += -maxDist;
            else zm += drv;
        }
        else //速度
        {
            rh = mouseTrans.x;
            rv = -mouseTrans.y;
            //二次曲线
            float alpha = 0.0005f;
            if (rh > maxR)
                xm += maxR * tranSpeed * Time.deltaTime * maxR * alpha;
            else if (rh < -maxR)
                xm += -maxR * tranSpeed * Time.deltaTime * maxR * alpha;
            else
                xm += rh * Math.Abs(rh) * tranSpeed * Time.deltaTime * alpha;

            if (rv > maxR)
                zm += maxR * tranSpeed * Time.deltaTime * maxR * alpha;
            else if (rv < -maxR)
                zm += -maxR * tranSpeed * Time.deltaTime * maxR * alpha;
            else
                zm += rv * Math.Abs(rv) * tranSpeed * Time.deltaTime * alpha;
        }

        if ((xm != 0 || zm != 0) && count % 5 == 0) Debug.Log($"trans type = {transType}, rh={rh}, rv={rv}, xm={xm}, zm={zm}");

        //使用角色控制器提供的Move函数进行移动
        m_ch.Move(m_transform.TransformDirection(new Vector3(xm, ym, zm)));


        //旋转摄像机
        float cos = 0.87128f;
        float sin = 0.49079f;
        /*float d_mouseRot_x_ = mouseRot.x * cos - mouseRot.y * sin - mouseRot_x_;
        float d_mouseRot_y_ = mouseRot.y * cos + mouseRot.x * sin - mouseRot_y_;
        mouseRot_x_ = mouseRot.x * cos - mouseRot.y * sin;
        mouseRot_y_ = mouseRot.y * cos + mouseRot.x * sin;*/
        float d_mouseRot_x = mouseRot.x - mouseRot_x;
        float d_mouseRot_y = mouseRot.y - mouseRot_y;
        mouseRot_x = mouseRot.x;
        mouseRot_y = mouseRot.y;
        m_camRot.x += d_mouseRot_y * rotatSense;
        m_camRot.y += d_mouseRot_x * rotatSense;
        if (m_camRot.x > 60) m_camRot.x = 40;
        if (m_camRot.x < -60) m_camRot.x = -20;
        //if (m_camRot.y > 190) m_camRot.y = 190;
        //if (m_camRot.y < -190) m_camRot.y = -190;

        if (m_camRot.x != 0 || m_camRot.y != 0 && count % 5 == 0)
            Debug.Log($"mouseRot.x= {mouseRot.x}, mouseRot.y={mouseRot.y}, \n" +
                $"mouseRot_x_={mouseRot_x_}, mouseRot_y_ = {mouseRot_y_}, \n" +
                $"m_camRot.x={m_camRot.x}, m_camRot.y={m_camRot.y}");
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

        if (count == 300) count = 0;
        count++;
    }
}
