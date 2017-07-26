﻿using System.IO;
using UnityEditor;
using UnityEngine;

/*
 * https://github.com/5minlab/minamo/blob/master/UnityProject/Assets/Minamo/Editor/EntryPoint.cs
 */

namespace Assets.Sagiri.Editor {
    class EntryPoint {
        public static void ExportPackage() {
            string output;
            if (EnvironmentReader.TryRead("EXPORT_PATH", out output)) {
                Debug.LogFormat("export package : {0}", output);
                var b = new PackageBuilder();
                b.Build(output);
            }
        }

        /// <summary>
        /// 최종 빌드에서 sagiri를 지우고 싶을때 사용
        /// PostProcessBuildAttribute에 연결시키면된다
        /// </summary>
        public static void RemoveSteamingAssets(BuildTarget target, string pathToBuiltProject) {
            // https://docs.unity3d.com/ScriptReference/Callbacks.PostProcessBuildAttribute.html
            // http://answers.unity3d.com/questions/984854/is-it-possible-to-excluding-streamingassets-depend.html
            string streamingAssetsPath = null;

            switch (target) {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal: {
                        // windows and linux use "_Data" folder
                        string root = Path.Combine(Path.GetDirectoryName(pathToBuiltProject), Path.GetFileNameWithoutExtension(pathToBuiltProject) + "_Data");
                        streamingAssetsPath = Path.Combine(root, "StreamingAssets");
                    }
                    break;
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal: {
                        streamingAssetsPath = Path.Combine(pathToBuiltProject, "Contents");
                        streamingAssetsPath = Path.Combine(streamingAssetsPath, "Resources");
                        streamingAssetsPath = Path.Combine(streamingAssetsPath, "Data");
                        streamingAssetsPath = Path.Combine(streamingAssetsPath, "StreamingAssets");
                    }
                    break;
            }

            if (streamingAssetsPath == null || !Directory.Exists(streamingAssetsPath))
                return;

            Directory.Delete(streamingAssetsPath, true);
        }
    }
}