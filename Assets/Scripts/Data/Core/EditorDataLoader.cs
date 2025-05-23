#if UNITY_EDITOR
using System;
using O2un.Core.Utils;
using UnityEditor;

namespace O2un.Data
{
    public static class EditorDataLoader
    {
        public static void LoadFromExcel()
        {
			TestStaticDataManager.Instance.Load();


            try
            {
                Link();
            }
            catch (Exception e)
            {
                LogHelper.Log(LogHelper.LogLevel.Error, $"데이터 에러 : {e}");
                CommonUtils.Exit();
            }
        }

        public static void SaveToBinary()
        {
            LoadFromExcel();

			TestStaticDataManager.Instance.SaveToBinary();


            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void Link()
        {
			TestStaticDataManager.Instance.Set();

			TestStaticDataManager.Instance.Link();

        }
    }
}
#endif
