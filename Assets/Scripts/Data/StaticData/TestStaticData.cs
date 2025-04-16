using System;
using O2un.Data;
using UnityEngine;

public class TestStaticData : StaticData
{
    //#VALUESTART#
		private String _name;
		public String Name => _name;
  //#VALUESEND#

    public override void Link()
    {
        throw new System.NotImplementedException();
    }

    public override void Set()
    {
        throw new System.NotImplementedException();
    }
}
