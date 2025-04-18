using System.Collections.Generic;
using System.Collections.Immutable;
using GameCommonTypes;
using O2un.Core.Excel;
using O2un.Core.Utils;

namespace O2un.Data
{
    public class TestStaticDataManager : StaticDataManager<TestStaticDataManager, TestStaticData>
    {
        protected override void LoadFromExcel()
        {
#if UNITY_EDITOR
            Dictionary<UniqueKey64, TestStaticData> loadDictionary = new();
            if (false == Excel.Load<TestStaticData>(out var result))
            {
                LogHelper.Dev("TestStaticData 데이터 로드 실패", LogHelper.LogLevel.Error);
            }

            result.ForEach((d) =>
            {
                loadDictionary.TryAdd(d.Key, d);
            });
            DataList = loadDictionary.ToImmutableDictionary();
#endif
        }

        protected override void SetXXX()
        {
            throw new System.NotImplementedException();
        }

        protected override void LinkXXX()
        {
            throw new System.NotImplementedException();
        }
    }
}