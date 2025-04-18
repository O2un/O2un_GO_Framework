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
            throw new NotImplementedException();
        }

        public override void Set()
        {
            throw new NotImplementedException();
        }
    }
}
