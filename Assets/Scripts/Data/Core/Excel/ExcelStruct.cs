using System;
using System.Collections.Generic;

namespace O2un.Core.Excel 
{
    public struct SheetInfo
    {
        public bool _isPairKey;
        public List<int> _keyIndex;
    }
    
    public struct ColumnInfo
    {
        public string _propertyName;
        public Type _dataType;
        public int _cellIndex;
        public bool _isList;

        public string ValueName => $"_{_propertyName.ToLower()}";
    }

    public class EnumInfo
    {
        public string _name;
        public string _desc;
    }
    
    public class ConstantInfo
    {
        public string _name;
        public string _datatype;
        public string _value;
    }
}