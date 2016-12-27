using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
public class XcodeProjectHelper : EditorWindow {

    string gameVersion = "0.9.4";
    string build = "0";
    bool isToTencent = false;
    bool isMidasRelease = false;
    bool withSDK = false;
    bool isHotFix = false;
    bool msdkENV_DEBUG = true;//debug
    bool inited = false;
    bool profiler = false;
    bool develop = true;
    void OnGUI ()
    {
        if(!inited)
        {
            inited = true;
            withSDK = PlayerPrefs.GetInt("xcode_withSDK",0) >0;
            gameVersion = PlayerPrefs.GetString("xcode_gameVersion",gameVersion);
            build = PlayerPrefs.GetString("xcode_build",build);
            isToTencent = PlayerPrefs.GetInt("xcode_isToTencent",0) >0;
            isMidasRelease = PlayerPrefs.GetInt("xcode_isMidasRelease",0) >0;
            isHotFix = PlayerPrefs.GetInt("xcode_isHotFix",0) >0;
            msdkENV_DEBUG = PlayerPrefs.GetInt("xcode_msdkENV_DEBUG",0) >0;
            isHotFix = PlayerPrefs.GetInt("xcode_isHotFix",0) >0;
            develop = PlayerPrefs.GetInt("xcode_develop",0) >0;
            profiler = PlayerPrefs.GetInt("xcode_profiler",0) >0;
        }

        EditorGUIUtility.labelWidth = 80f;
        GUILayout.Space(20f);
//        StartHorizontal();
        DrawLabel("                \t\t有名堂iOS工程傻瓜打包机            ");
//        EndHorizontal();
        GUILayout.Space(3f);
        StartHorizontal();
        DrawLabel("版本号设置  {如：gameVersion:0.9.4,Build:10}");
        EndHorizontal();
        StartVertical();
        StartHorizontal();
        DrawLabel("GameVersion:");
        gameVersion = EditorGUILayout.TextField(gameVersion);
        EndHorizontal();
        StartHorizontal();
        DrawLabel("Build(请输入0~999的数字):");
        build = EditorGUILayout.TextField(build);
        EndHorizontal();
        EndVertical();
        StartVertical();
        StartHorizontal();
        withSDK = EditorGUILayout.Toggle("打SDK包",withSDK);
        GUILayout.Space(10f);
        isHotFix = EditorGUILayout.Toggle("热更新包",isHotFix);
        DrawLabel("（请自己勾选CheckVersion场景）");
        EndHorizontal();
        StartHorizontal();
        DrawLabel("Development Build");
        develop = EditorGUILayout.Toggle("",develop, GUILayout.MinWidth(10f));
        profiler = EditorGUILayout.Toggle("开启性能分析",profiler, GUILayout.MinWidth(10f));
        EndHorizontal();
        if(withSDK)
        {
            DrawLabel("选择打包方式：");
            StartHorizontal();
            isToTencent = EditorGUILayout.Toggle("腾讯证书包",isToTencent, GUILayout.MinWidth(20f));
            DrawLabel("(不选择使用公司证书)");
            EndHorizontal();
            StartHorizontal();
            DrawLabel("MSDK DEBUG环境");
            msdkENV_DEBUG = EditorGUILayout.Toggle("",msdkENV_DEBUG, GUILayout.MinWidth(20f));
            isMidasRelease = !EditorGUILayout.Toggle("支付沙箱环境",!isMidasRelease, GUILayout.MinWidth(20f));
            EndHorizontal();
        }
        else
        {
            GUILayout.Space(25f);
            DrawLabel("注意：当前打包方式登录不需要QQ或者微信登录，无法测试包括支付\n、登录和分享等SDK相关内容。");
            GUILayout.Space(25f);
        }
        EndVertical();
        GUILayout.Space(25f);
        if(GUILayout.Button("开始打包"))
        {
            startBuild();
//            return;
        }
        GUILayout.Space(25f);
    }
    void DrawLabel(string text,float fontSize = 25)
    {
        GUILayout.Label(text);
    }
    void StartHorizontal()
    {
        GUILayout.Space(6f);
        GUILayout.BeginHorizontal();
    }
    void EndHorizontal()
    {
        GUILayout.Space(6f);
        GUILayout.EndHorizontal();
    }
    void StartVertical()
    {
        GUILayout.BeginVertical();
    }
    void EndVertical()
    {
        GUILayout.Space(3f);
        GUILayout.EndVertical();
    }
    void startBuild()
    {
//        GameObject.DestroyImmediate(this);
        int buildVer = 0;
        if (!int.TryParse(build,out buildVer))
        {
            Debug.LogError("Error build "+build+"(必须是个0~999的数字)");
        }
        PlayerPrefs.SetInt("xcode_withSDK",withSDK?1:0);
        PlayerPrefs.SetInt("xcode_isToTencent",isToTencent?1:0);
        PlayerPrefs.SetInt("xcode_isMidasRelease",isMidasRelease?1:0);
        PlayerPrefs.SetInt("xcode_isHotFix",isHotFix?1:0);
        PlayerPrefs.SetInt("xcode_msdkENV_DEBUG",msdkENV_DEBUG?1:0);
        PlayerPrefs.SetString("xcode_gameVersion",gameVersion);
        PlayerPrefs.SetString("xcode_build",build);
        PlayerPrefs.SetInt("xcode_develop",develop?1:0);
        PlayerPrefs.SetInt("xcode_profiler",profiler?1:0);
        PlayerPrefs.Save();
        BuildInstall.buildXcode(gameVersion,buildVer,withSDK,isToTencent,isMidasRelease,isHotFix,msdkENV_DEBUG,develop,profiler);
        Close();

    }
   
}
