using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;

/// <summary>
/// 用于通过命令行进行打包
/// </summary>
public class BuildInstall
{
	private const string OPT_VERSION = "-version";
	private static string version = "";
	private static BuildOptions buildOptions = BuildOptions.None;

	private static void UpdateBuildFlag()
	{
		string[] args = System.Environment.GetCommandLineArgs ();
		foreach (string oneArg in args) {
			if (oneArg != null && oneArg.Length > 0) {
				string tag = oneArg.ToLower ();
				if (tag.Contains (OPT_VERSION)) {
					string[] array = oneArg.Split ('@');
					version = array [1];
				} else if (tag.Contains ("-il2cpp")) {
					buildOptions |= BuildOptions.Il2CPP;
				} else if (tag.Contains ("-dev")) {
					buildOptions |= BuildOptions.Development;
				} else if (tag.Contains ("-profiler")) {
					buildOptions |= BuildOptions.Development;
					buildOptions |= BuildOptions.ConnectWithProfiler;
				}
			}
		}
	}

    [MenuItem("開啓血輪眼/Game/BuildApk", false, 200)]
    public static void BuildApk()
    {
		UpdateBuildFlag ();

		string path = "E:/1_ftp/2_apk/" + DateTime.Now.ToString("yyyyMMdd_HHmmss_") + version + ".apk";
		UnityEngine.Debug.Log (path);

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);

        //1、生成AssetBundle
//        NewExportAssetBundles.ExportAssetsAb(false);

        //2、设置打包参数
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "MSDK_ANDROID");

		string[] vers = version.Split ('.');
		string ver = vers [0] + vers [1] + vers [2];
		int versionCode = int.Parse (ver) * 1000;
		versionCode += int.Parse (vers [3]);
		PlayerSettings.bundleVersion = version;
		PlayerSettings.Android.bundleVersionCode = versionCode;

        PlayerSettings.Android.keystoreName = Application.dataPath + "/../user.keystore";
        PlayerSettings.Android.keystorePass = "umt123456";

        PlayerSettings.Android.keyaliasName = "com.tencent.freestyle";
        PlayerSettings.Android.keyaliasPass = "umt123456";

        //3、获取打包的场景
        List<string> levels = new List<string>();
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            var scene = EditorBuildSettings.scenes[i];
            levels.Add(scene.path);
        }

		System.Diagnostics.Stopwatch sp = System.Diagnostics.Stopwatch.StartNew ();
		//4、开始输出apk安装包
		BuildPipeline.BuildPlayer(levels.ToArray(), path, BuildTarget.Android, buildOptions);

		UnityEngine.Debug.LogFormat ("ok=>{0} time={1}s", path, sp.ElapsedMilliseconds / 1000);
    }
    [MenuItem("開啓血輪眼/Game/打IOS工程(自动CopyLua，压缩图集和导出ab包)", false, 300)]
    public static void BuildXcode()
    {
        EditorWindow.GetWindow<XcodeProjectHelper>(true, "IOS打包设置", true).Show();
    }
    public static void buildXcode(string gameVer,int build,bool withSDK,bool isToTencent,bool isMidasRelease,bool hotfix,bool env_debug,bool develop ,bool profiler)
    {
        string macros = "";

        string xcode ="../";// + DateTime.Now.ToString("yyyyMMddHHmm");
        string path = EditorUtility.SaveFilePanel("Save As",xcode,
            "XcocdeProject_Auto", "");
        if(string.IsNullOrEmpty(path))
        {
            UnityEngine.Debug.LogFormat("Cancel BuildXcode");
            return;
        }
        UnityEngine.Debug.LogFormat(path);
        if(EditorUserBuildSettings.activeBuildTarget!=BuildTarget.iOS)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);
        }
        //1、生成AssetBundle
//        if(hotfix)
//            NewExportAssetBundles.ExportAssetsAb(false);
//        else
//            CopyLuaAndProto.CopyLuaFilesToResource(false);
            
//        HelperTools.ReplaceEtc(false);
        if(withSDK)
        {
            macros +="MSDK_IOS";
        }
        if(isToTencent)
        {
            macros+=";TENCENT";
            if(isMidasRelease)
            {
                macros+=";MIDAS_RELEASE";
            }
        }

        //2、设置打包参数
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, macros);
        UnityEngine.Debug.LogFormat("DefineSymboles:"+PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS));
        //3、获取打包的场景
        List<string> levels = new List<string>();
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            var scene = EditorBuildSettings.scenes[i];
            if(!hotfix && scene.path.Contains("CheckVersion"))
            {
                continue;
            }
            levels.Add(scene.path);
        }

        //4、开始输出
        string fullpath = Path.GetFullPath(xcode);
        UnityEngine.Debug.Log(fullpath);
        string infoplistpath = Application.dataPath+"/Editor/XUPorter/Mods/iOS/Info.plist";

        if(!File.Exists(infoplistpath))
        {
            UnityEngine.Debug.LogError("文件"+infoplistpath+" 不存在"); 
            return;
        }
        string info = File.ReadAllText(infoplistpath);
        string newInfo = "";
        #region 版本号替换
        int VersionIdx;
        string versionString = "<key>CFBundleShortVersionString</key>\n\t<string>";
        VersionIdx = info.LastIndexOf(versionString)+versionString.Length;
        int versionLenth  = 5 ;
        string end = info.Substring(VersionIdx+versionLenth,9);
        if(end!="</string>")
        {
            UnityEngine.Debug.LogErrorFormat("ErrorFormat InfoPlist File!"+end);
            return;
        }
        string version = info.Substring(VersionIdx,5);
        string newVersion = gameVer;
        UnityEngine.Debug.Log("old gameVer "+version+",new gameVer "+newVersion);
        if(gameVer.Length!=5)
        {
            UnityEngine.Debug.LogErrorFormat("GameVersion Length Error!"+gameVer);
            return;
        }
//        info.Replace("<string>"+version+"</string>","<string>"+newVersion+"</string>");
        info = info.Replace("<key>CFBundleShortVersionString</key>\n\t<string>"+version+"</string>","<key>CFBundleShortVersionString</key>\n\t<string>"+newVersion+"</string>");
//        Debug.Log("next char is "+info.Substring(VersionIdx+5,1));
        #endregion 
        #region Build 替换
        int BuildIdx;
        string bundleString = "<key>CFBundleVersion</key>\n\t<string>";
        BuildIdx = info.LastIndexOf(bundleString)+bundleString.Length;
        int buildIdxEnd = info.IndexOf("</string>",BuildIdx);
//        Debug.Log("buildIdxEnd is "+buildIdxEnd+",BuildIdx "+BuildIdx);
        string Build = info.Substring(BuildIdx,buildIdxEnd-BuildIdx);
//        Debug.Log("build is "+Build);

        int newBuild = build;
        if(newBuild<0 || newBuild>999)
        {
            UnityEngine.Debug.LogError("Build 0~999 cur:"+build);
            return;
        }
        UnityEngine.Debug.Log("old build "+Build+",newBuild "+newBuild);
        info = info.Replace("<key>CFBundleVersion</key>\n\t<string>"+Build+"</string>","<key>CFBundleVersion</key>\n\t<string>"+newBuild+"</string>");
        #endregion
        #region 替换bid
        if(withSDK && isToTencent)
        {
            info = info.Replace("com.uminton.freestyle","com.tencent.ifreestyle");
        }
        else
        {
            info = info.Replace("com.tencent.ifreestyle","com.uminton.freestyle");
        }
        #endregion
        #region  msdk debug模式
        if(withSDK && !env_debug)
        {
            info = info.Replace("<key>MSDK_ENV</key>\n\t<string>test</string>","<key>MSDK_ENV</key>\n\t<string>release</string>");
        }
        else
        {
            info = info.Replace("<key>MSDK_ENV</key>\n\t<string>release</string>","<key>MSDK_ENV</key>\n\t<string>test</string>");
        }
        #endregion
//        Debug.Log(info);
        File.WriteAllText(infoplistpath,info);
        if(Directory.Exists(path))
            Directory.Delete(path,true);
        XCodePostProcess.toolBuild = true;
        BuildOptions options = BuildOptions.Il2CPP;
        if(develop)
            options |= BuildOptions.Development;
        if(profiler)
            options |= BuildOptions.ConnectWithProfiler;
        BuildPipeline.BuildPlayer(levels.ToArray(), path, BuildTarget.iOS, options);
        #region 有些人unity打包出错老是给我找麻烦
//        if(File.Exists(path+"/Classes/UnityAppController.h") && File.Exists(path+"/Classes/UnityAppController.mm"))
//        {
//            string appController_h = File.ReadAllText(Application.dataPath+"/Editor/XUPorter/Mods/iOS/Classes/UnityAppController.h");
//            string targetappController_h = File.ReadAllText(path+"/Classes/UnityAppController.h");
//            if(string.Compare(appController_h,targetappController_h)!=0)
//            {
//                File.WriteAllText(path+"/Classes/UnityAppController.h",appController_h);
//            }
//            string appController_mm = File.ReadAllText(Application.dataPath+"/Editor/XUPorter/Mods/iOS/Classes/UnityAppController.mm");
//            string targetappController_mm = File.ReadAllText(path+"/Classes/UnityAppController.mm");
//            if(string.Compare(appController_mm,targetappController_mm)!=0)
//            {
//                File.WriteAllText(path+"/Classes/UnityAppController.mm",appController_mm);
//            }
//        }
//        else
//        {
//            UnityEngine.Debug.LogError("打包检查失败！UnityAppController没有替换过去！");
//            return;
//        }
        #endregion
        UnityEngine.Debug.Log("打包完成，Xcode工程在："+path);
        File.Copy(Application.dataPath+"/Editor/pack.sh",path+"/pack.sh",true);
        File.Copy(Application.dataPath+"/Editor/dSYMUpload.sh",path+"/dSYMUpload.sh",true);
        if(!Directory.Exists(path+"/tmp"))
            Directory.CreateDirectory(path+"/tmp");
        ProcessStartInfo start = new ProcessStartInfo("/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal");
        start.Arguments =path+"/pack.sh -path "+path;
        start.CreateNoWindow = false;
        start.ErrorDialog = true;
        start.UseShellExecute = true;
        start.WorkingDirectory = path;
        if(start.UseShellExecute){
            start.RedirectStandardOutput = false;
            start.RedirectStandardError = false;
            start.RedirectStandardInput = false;
        } else{
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.RedirectStandardInput = true;
            start.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
            start.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
        }

        Process p = Process.Start(start);

        if(!start.UseShellExecute){
            UnityEngine.Debug.Log(p.StandardOutput.ToString());
            UnityEngine.Debug.Log(p.StandardError.ToString());
        }

//        p.WaitForExit();
//        p.Close();
    }
   
}
