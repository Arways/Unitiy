using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;
// 0 :Hip
// 1 :RHip
// 2 :RKnee
// 3 :RFoot
// 4 :LHip
// 5 :LKnee
// 6 :LFoot
// 7 :Spine
// 8 :Thorax
// 9 :Neck/Nose
// 10:Head
// 11:LShoulder
// 12:LElbow
// 13:LWrist
// 14:RShoulder
// 15:RElbow
// 16:RWrist

public class Pos_txt_Reader1 : MonoBehaviour
{
    float scale_ratio = 0.001f;  
    float heal_position = 0.00f; 
    float head_angle = 25f; 

    public string[] str = new string[52];
    
    public Boolean debug_cube; 
    
    Transform[] bone_t; 
    Transform[] cube_t; 
    Vector3 init_position; 
    Quaternion[] init_rot; 
    Quaternion[] init_inv; 
    
    int[] bones = new int[10] { 1, 2, 4, 5, 7, 8, 11, 12, 14, 15 }; 
    int[] child_bones = new int[10] { 2, 3, 5, 6, 8, 10, 12, 13, 15, 16 }; 
    static int bone_num = 17;
    Animator anim;
    string data = null;
    int count = 1;
    int _port = 5005;
    int bytesRec = 0;
    byte[] buffer = new Byte[1024];
    IPEndPoint localEndPoint = null;
    Socket listener = null;
    Socket socket = null;
    static Vector3[] now_pos = new Vector3[bone_num];
    
    void GetInitInfo()
    {
        bone_t = new Transform[bone_num];
        init_rot = new Quaternion[bone_num];
        init_inv = new Quaternion[bone_num];

        bone_t[0] = anim.GetBoneTransform(HumanBodyBones.Hips);
        bone_t[1] = anim.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        bone_t[2] = anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        bone_t[3] = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        bone_t[4] = anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        bone_t[5] = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        bone_t[6] = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        bone_t[7] = anim.GetBoneTransform(HumanBodyBones.Spine);
        bone_t[8] = anim.GetBoneTransform(HumanBodyBones.Neck);
        bone_t[10] = anim.GetBoneTransform(HumanBodyBones.Head);
        bone_t[11] = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        bone_t[12] = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        bone_t[13] = anim.GetBoneTransform(HumanBodyBones.LeftHand);
        bone_t[14] = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
        bone_t[15] = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
        bone_t[16] = anim.GetBoneTransform(HumanBodyBones.RightHand);

        
        Vector3 init_forward = TriangleNormal(bone_t[7].position, bone_t[4].position, bone_t[1].position);
        init_inv[0] = Quaternion.Inverse(Quaternion.LookRotation(init_forward));

        init_position = bone_t[0].position;
        init_rot[0] = bone_t[0].rotation;
        for (int i = 0; i < bones.Length; i++)
        {
            int b = bones[i];
            int cb = child_bones[i];

            init_rot[b] = bone_t[b].rotation;
            init_inv[b] = Quaternion.Inverse(Quaternion.LookRotation(bone_t[b].position - bone_t[cb].position, init_forward));
        }
    }

    Vector3 TriangleNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 d1 = a - b;
        Vector3 d2 = a - c;

        Vector3 dd = Vector3.Cross(d1, d2);
        dd.Normalize();

        return dd;
    }

    void UpdateCube()
    {
        if (cube_t == null)
        {
            cube_t = new Transform[bone_num];

            for (int i = 0; i < bone_num; i++)
            {
                Transform t = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                t.transform.parent = this.transform;
                t.localPosition = now_pos[i] * scale_ratio;
                t.name = i.ToString();
                t.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                cube_t[i] = t;

                Destroy(t.GetComponent<BoxCollider>());
            }
        }
        else
        {
            Vector3 offset = new Vector3(1.2f, 0, 0);

            for (int i = 0; i < bone_num; i++)
            {
                cube_t[i].localPosition = now_pos[i] * scale_ratio + new Vector3(0, heal_position, 0) + offset;
            }
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();

        GetInitInfo();

        localEndPoint = new IPEndPoint(IPAddress.Parse("192.168.55.90"), _port);
        listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true); //?
        listener.Connect(localEndPoint);
       
    }

    void Update()
    {
        
        //listener.Listen(10);
        Debug.Log(count);
        //socket = listener.Accept();

        //int bytesRec = socket.Receive(buffer);
        int bytesRec = listener.Receive(buffer);
        
        data = Encoding.ASCII.GetString(buffer, 0, bytesRec);
        

        if (data != "" && data!=null)
        {
            count++;
            //Debug.Log(data);

            //byte[] msg = Encoding.ASCII.GetBytes(data);
            //socket.Send(msg);
            //socket.Shutdown(SocketShutdown.Both);
            //socket.Close();

            //socket = null;


            str = data.Split(' ');
            Debug.Log(data);
            Debug.Log(str);
            now_pos = new Vector3[bone_num];
            for (int i = 0; i < str.Length; i += 3)
            {
                now_pos[i / 3] = new Vector3(-float.Parse(str[i]), float.Parse(str[i + 2]), -float.Parse(str[i + 1]));
            }
            
            //play_time += Time.deltaTime;

            //int frame = s_frame + (int)(play_time * 30.0f);  //30fps
            //if (frame > e_frame)
            //{
            //    play_time = 0;  // 繰り返す
            //    frame = s_frame;
            //}

            if (debug_cube)
            {
                UpdateCube();
            }

            Vector3 pos_forward = TriangleNormal(now_pos[7], now_pos[4], now_pos[1]);
            bone_t[0].position = now_pos[0] * scale_ratio + new Vector3(init_position.x, heal_position, init_position.z);
            bone_t[0].rotation = Quaternion.LookRotation(pos_forward) * init_rot[0];

            for (int i = 0; i < bones.Length; i++)
            {
                int b = bones[i];
                int cb = child_bones[i];
                bone_t[b].rotation = Quaternion.LookRotation(now_pos[b] - now_pos[cb], pos_forward) * init_inv[b] * init_rot[b];
            }

            bone_t[8].rotation = Quaternion.AngleAxis(head_angle, bone_t[11].position - bone_t[14].position) * bone_t[8].rotation;

            buffer = new Byte[1024];
            data = null;
            bytesRec = 0;
        }
    }
}
