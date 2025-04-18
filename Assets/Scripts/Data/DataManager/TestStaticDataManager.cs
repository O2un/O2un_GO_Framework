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
            // NOTE 이 데이터 내에 캐싱이 필요한 데이터 셋
            // 예) 타입별로 데이터를 찾을 수 있는 Dictionary를 만들어서 캐싱
        }

        protected override void LinkXXX()
        {
            // NOTE 다른 데이터와 함께 연결되는 데이터 검증 및 연결
            // 예) 타입별로 다른 데이터와 연결이 필요한 컨테이너가 필요할경우 캐싱 등
        }
    }
}