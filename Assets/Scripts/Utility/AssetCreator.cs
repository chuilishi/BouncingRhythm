#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utility
{
    public static class AssetCreator
    {
        public static void SafelyCreateAsset(ScriptableObject scriptableObject, string path)
        {
            string directory = Path.GetDirectoryName(path);
            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            string result = Path.Combine(directory,filenameWithoutExtension+".asset");
            int counter = 1;
            result = EnsureFolderExist(result);
            while (AssetDatabase.LoadAssetAtPath<Object>(result) != null)
            {
                result = Path.Combine(directory,filenameWithoutExtension+counter+".asset");
                result = result.Replace("\\", "/");
                counter++;
            }
            AssetDatabase.CreateAsset(scriptableObject, result);
            AssetDatabase.SaveAssets();
        }

        private static string EnsureFolderExist(string path)
        {
            var fileName = Path.GetFileName(path);
            path = Path.GetDirectoryName(path);
            path = path.Replace("\\", "/");
            if (!AssetDatabase.IsValidFolder(path))
            {
                string[] splitPath = path.Split("/");
                string currentPath = "Assets";

                for (int i = 1; i < splitPath.Length; i++)
                {
                    string folder = splitPath[i];
                    string newPath = currentPath+ "/" + folder;
                    if (!AssetDatabase.IsValidFolder(newPath))
                    {
                        AssetDatabase.CreateFolder(currentPath, folder);
                    }
                    currentPath = newPath;
                }

                return currentPath + "/" + fileName;
            }
            else
            {
                return path + "/" + fileName;
            }
        }
    }
}
#endif