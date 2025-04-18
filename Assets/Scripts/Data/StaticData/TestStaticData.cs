using System;
using UnityEngine;

namespace O2un.Data
{
    [Serializable]    
    public class TestStaticData : StaticData
    {
//#VALUESTART#
		private String _name;
		public String Name => _name;

//#VALUESEND#

        public override void Link()
        {
            // NOTE 다른 데이터와 함께 연결되는 데이터 검증 및 연결
            // 예) 아이템을 사용하는 데이터 일경우 그 아이템 키가 실제로 존재하는지 체크 또는 캐싱
        }

        public override void Set()
        {
            // NOTE 이 데이터 내에 캐싱이 필요한 데이터 셋
            // 예) 하나의 컬럼에서 여러개의 데이터를 사용한다고 했을때 String으로 받은 데이터를 리스트로 변경 등
        }
    }
}
