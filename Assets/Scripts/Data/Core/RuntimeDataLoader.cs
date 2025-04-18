using System;
using System.Threading.Tasks;
using O2un.Core.Utils;
using UnityEngine;

namespace O2un.Data
{
    public static class RuntimeDataLoader
    {
        public static void LoadFromBinary()
        {
			TestStaticDataManager.Instance.Load(true);


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

        public static void LoadFromAddressable()
        {
			TestStaticDataManager.Instance.Load(true, true);


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

        private static async void Link()
        {
            // 데이터가 로드되는 타이밍에는 PlayerLoop가 없으므로 Unitask 대신 Task를 사용합니다.
            await Task.WhenAll(
				TestStaticDataManager.Instance.WaitForLoaded() 

            );

			TestStaticDataManager.Instance.Set();

			TestStaticDataManager.Instance.Link();

        }
    }
}
