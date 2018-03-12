using System;
using Syng;
using UnityEditor;
using UnityEngine;
using UnityEngine.CloudBuild;

public class BuildPlayer {

    [MenuItem("Syng/Build Android/All")]
    public static void AndroidAll() {
        AndroidDXT();
        AndroidPVRTC();
        AndroidATC();
        AndroidETC();
        AndroidETC2();
        AndroidASTC();
    }

    [MenuItem("Syng/Build Android/DXT")]
    public static void AndroidDXT() {
        Build("DXT", MobileTextureSubtarget.DXT);
    }

    [MenuItem("Syng/Build Android/PVRTC")]
    public static void AndroidPVRTC() {
        Build("PVRTC", MobileTextureSubtarget.PVRTC);
    }

    [MenuItem("Syng/Build Android/ATC")]
    public static void AndroidATC() {
        Build("ATC", MobileTextureSubtarget.ATC);
    }

    [MenuItem("Syng/Build Android/ETC")]
    public static void AndroidETC() {
        Build("ETC", MobileTextureSubtarget.ETC);
    }

    [MenuItem("Syng/Build Android/ETC2")]
    public static void AndroidETC2() {
        Build("ETC2", MobileTextureSubtarget.ETC2);
    }

    [MenuItem("Syng/Build Android/ASTC")]
    public static void AndroidASTC() {
        Build("ASTC", MobileTextureSubtarget.ASTC);
    }

    private static void Build(string textureType, MobileTextureSubtarget texture) {
        int buildNumber = GetBuildPrefix(texture);
        string prefix = buildNumber.ToString();
        int bundleVersion = Int32.Parse(PlayerSettings.bundleVersion);
        PlayerSettings.Android.bundleVersionCode = (buildNumber * 100) + bundleVersion;
        string playerName = "Android_" + textureType + "_" + prefix + "_" + bundleVersion + ".apk";
        BuildPipeline.BuildPlayer(new string[] {
                "Assets/Scenes/Preloader.unity",
                "Assets/Scenes/Main.unity",
            },
            "Builds/" + playerName,
            BuildTarget.Android,
            BuildOptions.Development);
    }

    private static int GetBuildPrefix(MobileTextureSubtarget texture) {
        switch ((int) texture) {
            case 0: // Generic. Not in use
                return 0;
            case 1: // DXT
                return 40010;
            case 2: // PVRTC
                return 50010;
            case 3: // ATC
                return 30010;
            case 4: // ETC
                return 10010;
            case 5: // ETC2
                return 20010;
            default: // ASTC
                return 60010;
        }
    }

    // Called by Unity Cloud Build

    private static void SetBundleVersion(int prefix, int buildNumber) {
        PlayerSettings.Android.bundleVersionCode = (prefix * 100) + buildNumber;
    }

    private static void SetBuildSubtarget(MobileTextureSubtarget texture) {
        EditorUserBuildSettings.androidBuildSubtarget = texture;
    }

    private static void Prebuild(MobileTextureSubtarget texture, BuildManifestObject manifest) {
        string buildNumber = manifest.GetValue<string>("buildNumber");
        SetBundleVersion(GetBuildPrefix(texture), Int32.Parse(buildNumber));
        SetBuildSubtarget(texture);
    }

    private static void SetVersion(BuildManifestObject manifest) {
        PlayerSettings.bundleVersion = manifest.GetValue<string>("buildNumber");
    }

    public static void BuildIOS_EN(BuildManifestObject manifest) {
        SetVersion(manifest);
        LocalizationEditorHelper.WriteDefaultLang(SystemLanguage.English);
        AssetDatabase.Refresh();
        ParallaxHelper.Reindex();
    }

    public static void BuildIOS_NO(BuildManifestObject manifest) {
        SetVersion(manifest);
        LocalizationEditorHelper.WriteDefaultLang(SystemLanguage.Norwegian);
        AssetDatabase.Refresh();
        ParallaxHelper.Reindex();
    }

    public static void SetDXT(BuildManifestObject manifest) {
        SetVersion(manifest);
        Prebuild(MobileTextureSubtarget.DXT, manifest);
        ParallaxHelper.Reindex();
    }

    public static void SetPVRTC(BuildManifestObject manifest) {
        SetVersion(manifest);
        Prebuild(MobileTextureSubtarget.PVRTC, manifest);
        ParallaxHelper.Reindex();
    }

    public static void SetATC(BuildManifestObject manifest) {
        SetVersion(manifest);
        Prebuild(MobileTextureSubtarget.ATC, manifest);
        ParallaxHelper.Reindex();
    }

    public static void SetETC(BuildManifestObject manifest) {
        SetVersion(manifest);
        Prebuild(MobileTextureSubtarget.ETC, manifest);
        ParallaxHelper.Reindex();
    }

    public static void SetETC2(BuildManifestObject manifest) {
        SetVersion(manifest);
        Prebuild(MobileTextureSubtarget.ETC2, manifest);
        ParallaxHelper.Reindex();
    }

    public static void SetASTC(BuildManifestObject manifest) {
        SetVersion(manifest);
        Prebuild(MobileTextureSubtarget.ASTC, manifest);
        ParallaxHelper.Reindex();
    }
}


