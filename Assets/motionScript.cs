// Requrirements
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class motionScript : MonoBehaviour
{
    public TextAsset motion;
    // Animate the position and color of the GameObject
    public void Start()
    {
        Animation anim = GetComponent<Animation>();
        AnimationCurve curve;

        // create a new AnimationClip
        AnimationClip clip = new AnimationClip();
        clip.legacy = true;

        // Read file
        // var lines = System.IO.File.ReadAllLines("text_input/test_motion_22.txt");
        var value_lines = motion.text.Split('\n');
        var joint_lines = System.IO.File.ReadAllLines("text_input/joint_name_upper.txt");

        // Get macro
        int num_frame = Int32.Parse(value_lines[0]);
        int num_joint = Int32.Parse(value_lines[1]);
        float sec_per_frame = float.Parse(value_lines[2]);
        int axis_per_joint = 3;
        int value_line_start = 3;

        Debug.Log("Num of frame: " + num_frame);
        Debug.Log("Num of joint: " + num_joint);

        // Mem for keys
        // Load key value for every frame from text, store in a 2D-array
        Keyframe[][][] keys = new Keyframe[num_joint][][];

        // dim 1: joint; dim 2: x, y, z; dim 3: frame
        for (int i_joint=0; i_joint<num_joint; i_joint++) {
            keys[i_joint] = new Keyframe[axis_per_joint][];

            for (int i_axis=0; i_axis<axis_per_joint; i_axis++) {
                keys[i_joint][i_axis] = new Keyframe[num_frame];
                string[] value_line = value_lines[i_joint * 3 + i_axis + value_line_start].Split(' ');

                for (int i_frame=0; i_frame<num_frame; i_frame++) {
                    // Conver to left hand rule
                    float deg = 0.0f;
                    if (i_axis == 0 || i_axis == 1) {
                        deg = - float.Parse(value_line[i_frame]);
                    } else {
                        deg = float.Parse(value_line[i_frame]);
                    }

                    keys[i_joint][i_axis][i_frame] = new Keyframe(sec_per_frame * i_frame, deg);
                }
            }
        }

        // Set curve for axis of every joint
        for (int i_joint=0; i_joint<num_joint; i_joint++) {
            string joint_name = joint_lines[i_joint];
            Debug.Log("joint name:" + joint_name);

            if (string.Compare("root", joint_name) == 0) {
                curve = new AnimationCurve(keys[i_joint][0]);
                clip.SetCurve("", typeof(Transform), "localEulerAngles.x", curve);
                curve = new AnimationCurve(keys[i_joint][1]);
                clip.SetCurve("", typeof(Transform), "localEulerAngles.y", curve);
                curve = new AnimationCurve(keys[i_joint][2]);
                clip.SetCurve("", typeof(Transform), "localEulerAngles.z", curve);

            } else {
                // Debug.Log("value:" + keys[i_joint][0][0].value);
                curve = new AnimationCurve(keys[i_joint][0]);
                clip.SetCurve(joint_name, typeof(Transform), "localEulerAngles.x", curve);
                curve = new AnimationCurve(keys[i_joint][1]);
                clip.SetCurve(joint_name, typeof(Transform), "localEulerAngles.y", curve);
                curve = new AnimationCurve(keys[i_joint][2]);
                clip.SetCurve(joint_name, typeof(Transform), "localEulerAngles.z", curve);

            }
        }

        // now animate the GameObject
        anim.AddClip(clip, clip.name);
        anim.Play(clip.name);
    }
}
