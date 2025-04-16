#if UNITY_EDITOR 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using GameCommonTypes;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using O2un.Core.Utils;
using O2un.Data;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace O2un.Core.Excel
{
    public static class Excel
    {
        // 폴더내 모든 엑셀 파일을 읽음 하위 디렉토리 포함
        public static IEnumerable<string> GetAllFileFromPath(string path)
        {
            return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).
                    Where(s => (s.ToLower().EndsWith(".xlsx") || s.ToLower().EndsWith(".xls") || s.ToLower().EndsWith(".xlsm"))
                            && !Path.GetFileName(s).StartsWith("~$")
                            && !Path.GetFileName(s).StartsWith(COMMENTPREFIX)
                            && !Path.GetFileName(s).Contains("복사본"));
        }

        public static bool DataTypeParser(string str, out Type type)
        {
            type = null;

            if (str.Equals("int", StringComparison.OrdinalIgnoreCase))
            {
                type = typeof(int);

                return true;
            }
            else if (str.Equals("float", StringComparison.OrdinalIgnoreCase))
            {
                type = typeof(float);

                return true;
            }
            else if (str.Equals("string", StringComparison.OrdinalIgnoreCase))
            {
                type = typeof(string);

                return true;
            }
            else if (str.Equals("bool", StringComparison.OrdinalIgnoreCase))
            {
                type = typeof(bool);

                return true;
            }
            else if (str.Equals("Vector2", StringComparison.OrdinalIgnoreCase))
            {
                type = typeof(Vector2);
                return true;
            }
            else if (str.Equals("Vector3", StringComparison.OrdinalIgnoreCase))
            {
                type = typeof(Vector3);
                return true;
            }
            else if (str.Equals("Color", StringComparison.OrdinalIgnoreCase))
            {
                type = typeof(Color);
                return true;
            }
            else if (str.Equals("key", StringComparison.OrdinalIgnoreCase))
            {
                type = typeof(UniqueKey64);
                return true;
            }
            else if (str.Equals("date", StringComparison.OrdinalIgnoreCase))
            {
                type = typeof(DateTime);
                return true;
            }
            else if (str.Equals("VariableType", StringComparison.OrdinalIgnoreCase))
            {
                type = typeof(object);
                return true;
            }
            else
            {
                type = Type.GetType($"GameCommonTypes.{str}", false, true);
                if (null != type)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool LoadHeader(ISheet sheet, out SheetInfo sheetInfo, out List<ColumnInfo> columnInfo)
        {
            sheetInfo = new();
            sheetInfo._keyIndex = new();

            columnInfo = new();

            var nameRow = sheet.GetRow(0);
            var typeRow = sheet.GetRow(1);

            int keyCount = 0;
            var len = nameRow.Cells.Count;
            for (int jj = 0; jj < len; ++jj)
            {
                ColumnInfo newData = new();

                var nameStr = nameRow.GetCell(jj)?.StringCellValue;
                var typeStr = typeRow.GetCell(jj)?.StringCellValue;

                // #은 주석
                if (nameStr.StartsWith(COMMENTPREFIX))
                {
                    continue;
                }

                if (nameStr.StartsWith(KEYPREFIX))
                {
                    ++keyCount;
                    sheetInfo._keyIndex.Add(jj);

                    continue;
                }
                else
                {
                    if (false == DataTypeParser(typeStr, out var type))
                    {
                        LogHelper.Dev($"{sheet.SheetName} 시트의 {nameStr} 열의 데이터타입{typeStr} 이 비정상 입니다.", LogHelper.LogLevel.Error);
                        return false;
                    }

                    bool isList = nameStr.StartsWith(LISTPREFIX);

                    newData._isList = isList;
                    newData._dataType = type;
                    newData._propertyName = isList ? nameStr.Remove(0, 1) : nameStr;
                    newData._cellIndex = jj;
                }

                columnInfo.Add(newData);
            }

            if (0 == keyCount)
            {
                LogHelper.Dev($"{sheet.SheetName} 데이터는 Key 가 없습니다 확인해주세요", LogHelper.LogLevel.Error);
                return false;
            }
            else if (1 == keyCount)
            {
                sheetInfo._isPairKey = false;
            }
            else if (2 == keyCount)
            {
                sheetInfo._isPairKey = true;
            }
            else
            {
                LogHelper.Dev($"{sheet.SheetName} 데이터는 Key 가 세개 이상입니다 확인해주세요", LogHelper.LogLevel.Error);
                return false;
            }

            return true;
        }

        static IWorkbook LoadBook(string excelPath)
        {
            using (FileStream stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (Path.GetExtension(excelPath) == ".xls") return new HSSFWorkbook(stream);
                else return new XSSFWorkbook(stream);
            }
        }

        private static TextAsset _dataScriptTemplate;
        static TextAsset DataScriptTemplate
        {
            get
            {
                if (null == _dataScriptTemplate)
                {
                    var dat = AssetDatabase.LoadAssetAtPath<TextAsset>(StaticData.TemplatePath);
                    if (null == dat)
                    {
                        LogHelper.Dev("스크립트 템플릿 로드 실패", LogHelper.LogLevel.Error);
                        return null;
                    }
                    _dataScriptTemplate = dat;
                }

                return _dataScriptTemplate;
            }
        }
        private static TextAsset _dataManagerTemplate;
        static TextAsset DataManagerTemplate
        {
            get
            {
                if (null == _dataManagerTemplate)
                {
                    var dat = AssetDatabase.LoadAssetAtPath<TextAsset>(IStaticDataManager.TemplatePath);
                    if (null == dat)
                    {
                        LogHelper.Dev("스크립트 템플릿 로드 실패", LogHelper.LogLevel.Error);
                        return null;
                    }
                    _dataManagerTemplate = dat;
                }

                return _dataManagerTemplate;
            }
        }

        //private static readonly string DATAPATH = "Assets/DataTable/.RawData";
        private static readonly char COMMENTPREFIX = '#';
        private static readonly char KEYPREFIX = '^';
        private static readonly char LISTPREFIX = '!';
        public static void CreateScriptAllData()
        {
            // 임시 파일 제외 xls파일과 xlsx 파일모두
            var paths = GetAllFileFromPath(DataConfig.Instance._dataPath);
            paths.ForEach((s) =>
            {
                CreateScriptForData(s);
                LogHelper.Dev($"{s}데이터 생성 완료");
            });

            RefreshExcelList(true);
        }

        public static void RefreshExcelList(bool force = false)
        {
            if (false == force)
            {
                if (0 != DataConfig.Instance.DataInfoList.Count)
                {
                    return;
                }
            }

            DataConfig.Instance.Clear();

            var paths = GetAllFileFromPath(DataConfig.Instance._dataPath);
            paths.ForEach((s) =>
            {
                RefreshExcelList(s);
            });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void RefreshExcelList(string path)
        {
            var workbook = LoadBook(path);

            var len = workbook.NumberOfSheets;
            for (int ii = 0; ii < len; ++ii)
            {
                var sheet = workbook.GetSheetAt(ii);
                if (sheet.SheetName.StartsWith(COMMENTPREFIX))
                {
                    // 주석시트
                    ++_generateCount;
                    continue;
                }

                // 헤더, 타입 인포
                string className = $"{sheet.SheetName}{StaticData.SUFFIX}";
                DataConfig.Instance.Add(new(className, path, ii));
            }
        }

        // 엑셀 파일 하나의 모든 시트를 StaticData.cs로 생성해줌
        // 로딩바 같은거 만들때 쓸것
        private static int _generateCount = 0;
        private static void CreateScriptForData(string path)
        {
            _generateCount = 0;

            var workbook = LoadBook(path);

            var len = workbook.NumberOfSheets;
            for (int ii = 0; ii < len; ++ii)
            {
                var sheet = workbook.GetSheetAt(ii);
                if (sheet.SheetName.StartsWith(COMMENTPREFIX))
                {
                    // 주석시트
                    ++_generateCount;
                    continue;
                }

                // 헤더, 타입 인포
                string className = $"{sheet.SheetName}{StaticData.SUFFIX}";
                string savepath = $"{StaticData.ScriptPath}/{className}.cs";
                bool isReplace = File.Exists(savepath);

                if (LoadHeader(sheet, out var sheetInfo, out var columnInfo))
                {
                    CreateStaticDataScript(columnInfo, className, savepath, isReplace);
                }
            }
        }

        private static void CreateStaticDataScript(List<ColumnInfo> columnInfo, string className, string path, bool isReplace)
        {
            string template = DataScriptTemplate.text;
            if (isReplace)
            {
                using (StreamReader sr = new(path))
                {
                    template = sr.ReadToEnd();
                }
            }
            else
            {
                template = template.Replace(NewScriptTool.CLASSNAME, className);
            }

            StringBuilder values = new();
            values.AppendLine();
            HashSet<string> listName = new();
            foreach (var st in columnInfo)
            {
                // 키는 기본 파일에 정의됩니다.
                if (st._isList)
                {
                    // 리스트명 같은이름끼리는 리스트로 처리함
                    if (true == listName.Add(st._propertyName))
                    {
                        values.AppendLine($"\t\tprivate List<{st._dataType.Name}> {st.ValueName} = new();");
                        values.AppendLine($"\t\tpublic List<{st._dataType.Name}> {st._propertyName} => {st.ValueName};");
                    }
                }
                else
                {
                    values.AppendLine($"\t\tprivate {st._dataType.Name} {st.ValueName};");
                    values.AppendLine($"\t\tpublic {st._dataType.Name} {st._propertyName} => {st.ValueName};");
                }
            }
            //values.Append($"\t\tpublic {className}(uint upperKey, uint lowerKey) : base(upperKey,lowerKey) {{}}");

            var valueStart = template.IndexOfEnd(StaticData.VALUEPREFIX);
            var valueEnd = template.IndexOf(StaticData.VALUESUFFIX);

            template = template.Remove(valueStart, valueEnd - valueStart - 2);
            template = template.Insert(valueStart, values.ToString());

            using (StreamWriter sw = new(path, false))
            {
                sw.Write(template);
                if (false == isReplace)
                {
                    CreateDataManagerScript(className);
                }
                else
                {
                    ++_generateCount;
                }
            }
        }

        private static void CreateDataManagerScript(string className)
        {
            string managerName = className + IStaticDataManager.SUFFIX;
            string template = DataManagerTemplate.text;
            template = template.Replace(NewScriptTool.CLASSNAME, managerName);
            template = template.Replace(IStaticDataManager.DATATYPE, className);

            string savepath = $"{IStaticDataManager.ScriptPath}/{managerName}.cs";
            using (StreamWriter sw = new(savepath, false))
            {
                sw.Write(template);
            }
        }

        private static string GetCellValueString(ICell cell)
        {
            if (cell == null) return null;

            switch (cell.CellType)
            {
                case CellType.String:
                    return cell.StringCellValue.Trim(); // 문자열이면 Trim() 적용

                case CellType.Numeric:
                    return cell.NumericCellValue.ToString(); // 숫자는 문자열 변환

                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString(); // 불리언 변환

                case CellType.Formula:
                    return GetFormulaCellValue(cell); // 수식 결과값 가져오기

                default:
                    return null; // 빈 값 처리
            }
        }

        private static string GetFormulaCellValue(ICell cell)
        {
            try
            {
                if (cell == null || cell.CellType != CellType.Formula)
                    return null;

                IFormulaEvaluator evaluator = WorkbookFactory.CreateFormulaEvaluator(cell.Sheet.Workbook);
                var evaluatedCell = evaluator.Evaluate(cell);

                if (evaluatedCell == null) return null; // 수식 평가 결과가 없으면 빈 값 처리

                switch (evaluatedCell.CellType)
                {
                    case CellType.String:
                        return evaluatedCell.StringValue.Trim();
                    case CellType.Numeric:
                        return evaluatedCell.NumberValue.ToString();
                    case CellType.Boolean:
                        return evaluatedCell.BooleanValue.ToString();
                    case CellType.Blank:
                        return null; // ⬅️ 수식 결과가 빈 값인 경우 처리
                    default:
                        return null;
                }
            }
            catch
            {
                return null;
            }
        }

        private static bool IsRowNotEmpty(IRow row)
        {
            foreach (var cell in row.Cells)
            {
                if (cell.CellType != CellType.Blank && false == string.IsNullOrWhiteSpace(GetCellValueString(cell)))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool TryGetValidRow(ISheet sheet, int index, out IRow row)
        {
            row = sheet.GetRow(index);
            if (null != row && IsRowNotEmpty(row))
            {
                return true;
            }

            return false;
        }

        public static bool Load<T>(out List<T> result) where T : StaticData, new()
        {
            result = new();

            var info = DataConfig.Instance.GetPath(typeof(T).Name);
            if (string.IsNullOrEmpty(info.ExcelPath))
            {
                LogHelper.Dev($"데이터 리스트 갱신이 필요합니다. {typeof(T).Name} 의 엑셀 파일 경로를 알 수 없습니다.", LogHelper.LogLevel.Error);

                return false;
            }

            var wb = LoadBook(info.ExcelPath);
            var sheet = wb.GetSheetAt(info.SheetIndex);

            LoadHeader(sheet, out var sheetInfo, out var columnInfo);

            // 프로그레스바 필요할경우 처리하기위해 일반 for사용
            // 0 이름 로우 1 데이터타입로우는 넘김
            // 엑셀 카운트는 0 부터시작이어서 길이와 비교는 <= 입니다
            HashSet<UniqueKey64> checkDuplication = new();
            var len = sheet.LastRowNum;
            for (int ii = 2; ii <= len; ++ii)
            {
                if (false == TryGetValidRow(sheet, ii, out var row))
                {
                    LogHelper.Dev($"{info.ExcelPath} : {sheet.SheetName} , 시트에 공백열(빈값 함수 등)이 있습니다 문제 없는지 확인 필요", LogHelper.LogLevel.Warning);
                    continue;
                }
                if (row.IsComment())
                {
                    continue;
                }

                T newData = new();
                if (false == sheetInfo._isPairKey)
                {
                    newData.SetKey(0, (uint)row.GetCell(sheetInfo._keyIndex[0]).NumericCellValue);
                }
                else
                {
                    newData.SetKey((uint)row.GetCell(sheetInfo._keyIndex[0]).NumericCellValue, (uint)row.GetCell(sheetInfo._keyIndex[1]).NumericCellValue);
                }

                if (false == checkDuplication.Add(newData.Key))
                {
                    LogHelper.Dev($"{typeof(T).Name} 데이터의 Key{newData.Key}가 중복됐습니다 확인해주세요", LogHelper.LogLevel.Fatal);
                    throw new Exception();
                }

                columnInfo.ForEach((h) =>
                {
                    var fieldInfo = typeof(T).GetField(h.ValueName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var obj = CellToFieldObject(row.GetCell(h._cellIndex), fieldInfo, out bool isEmpty);
                    if (isEmpty)
                    {
                        LogHelper.Dev($"{sheet.SheetName} 시트 {ii}열 {h.ValueName} 값이 빈값입니다 정상적인 값인지 확인해주세요", LogHelper.LogLevel.Error);
                    }
                    // 데이터에 <null> 를 명시할경우 빈값으로 넘어갑니다
                    if (obj is string && NULL.Equals(obj))
                    {
                        return;
                    }

                    if (h._isList)
                    {
                        object list = fieldInfo.GetValue(newData);
                        Type listType = list.GetType();
                        MethodInfo addMethod = listType.GetMethod("Add");

                        if (null != addMethod)
                        {
                            addMethod.Invoke(list, new object[] { obj });
                        }
                    }
                    else
                    {
                        fieldInfo.SetValue(newData, obj);
                    }
                });

                result.Add(newData);
            }

            return true;
        }

        static bool IsComment(this IRow row)
        {
            var cell = row.GetCell(0);
            if (CellType.String != cell.CellType)
            {
                return false;
            }

            return cell.StringCellValue.StartsWith(COMMENTPREFIX);
        }

        private static readonly string NULL = "<null>";
        static object CellToFieldObject(ICell cell, FieldInfo fieldInfo, out bool isEmpty, bool isFormulaEvalute = false)
        {
            isEmpty = false;
            if (null == cell)
            {
                isEmpty = true;
                return default;
            }

            var fieldType = fieldInfo.FieldType;
            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                fieldType = fieldType.GetGenericArguments()[0];
            }

            var type = isFormulaEvalute ? cell.CachedFormulaResultType : cell.CellType;
            try
            {
                switch (type)
                {
                    case CellType.String:
                        if (fieldType.IsEnum)
                            return CommonUtils.TryParserOrDefault(fieldType, cell.StringCellValue);
                        // if (fieldType == typeof(StringKey)) return new StringKey() { _key = cell.StringCellValue };
                        else return cell.StringCellValue;
                    case CellType.Boolean:
                        return cell.BooleanCellValue;
                    case CellType.Numeric:
                        if (fieldType == typeof(UniqueKey64)) return new UniqueKey64((ulong)cell.NumericCellValue);
                        return Convert.ChangeType(cell.NumericCellValue, fieldType);
                    case CellType.Formula:
                        if (isFormulaEvalute) return null;
                        return CellToFieldObject(cell, fieldInfo, out isEmpty, true);
                    default:
                        if (fieldType.IsValueType)
                        {
                            return Activator.CreateInstance(fieldType);
                        }

                        return null;
                }
            }
            catch (Exception e)
            {
                // 오류 발생 시, Sheet 이름과 Cell 위치 출력
                string sheetName = cell.Sheet.SheetName;
                int rowIndex = cell.RowIndex + 1;  // 엑셀 기준 1부터 시작
                int colIndex = cell.ColumnIndex + 1;  // 엑셀 기준 1부터 시작
                LogHelper.Dev($"Error in Sheet '{sheetName}', Cell ({rowIndex}, {colIndex}): {e.Message}", LogHelper.LogLevel.Error);
                return null;
            }
        }

        // FieldInfo.SetValue 가 Struct에서 동작하지 않아서 Class 제약이 있음
        public static bool LoadCustomClass<T>(string excelPath, string sheetName, out List<T> result) where T : class, new()
        {
            result = new();

            var book = LoadBook(excelPath);
            if (null == book)
            {
                LogHelper.Dev($"LoadCustomClass : {excelPath} 엑셀 파일이 존재 하지 않습니다.");
                return false;
            }

            var sheet = book.GetSheet(sheetName);
            if (null == sheet)
            {
                LogHelper.Dev($"LoadCustomClass : {excelPath} 엑셀 파일에 {sheetName} 시트는 존재 하지 않습니다.");

                return false;
            }

            result = LoadCustomClassSheet<T>(sheet);

            return true;
        }

        public static bool LoadCustomClass<T>(string excelPath, string sheetName, out List<T> result, out List<int> indexes) where T : class, new()
        {
            result = new();
            indexes = new();

            var book = LoadBook(excelPath);
            if (null == book)
            {
                LogHelper.Dev($"LoadCustomClass : {excelPath} 엑셀 파일이 존재 하지 않습니다.");
                return false;
            }

            var sheet = book.GetSheet(sheetName);
            if (null == sheet)
            {
                LogHelper.Dev($"LoadCustomClass : {excelPath} 엑셀 파일에 {sheetName} 시트는 존재 하지 않습니다.");

                return false;
            }

            result = LoadCustomClassSheet<T>(sheet, out indexes);

            return true;
        }

        public static bool LoadCustomClasses<T>(string path, out Dictionary<string, List<T>> result) where T : class, new()
        {
            result = new();

            var book = LoadBook(path);
            if (null == book)
            {
                LogHelper.Dev($"LoadCustomClasses {path} 엑셀 파일이 존재 하지 않습니다.");
                return false;
            }

            var len = book.NumberOfSheets;
            for (int ii = 0; ii < len; ++ii)
            {
                var sheet = book.GetSheetAt(ii);
                if (null != sheet)
                {
                    if (sheet.SheetName.StartsWith(COMMENTPREFIX))
                    {
                        // 주석시트
                        continue;
                    }

                    var list = LoadCustomClassSheet<T>(sheet);
                    if (false == result.TryAdd(sheet.SheetName, list))
                    {
                        LogHelper.Dev($"LoadCustomClasses {path} / {sheet.SheetName} 시트 로드 실패", LogHelper.LogLevel.Error);
                    }
                }
            }

            return true;
        }

        public static bool LoadCustomClasses<T>(string path, out Dictionary<string, List<T>> result, out Dictionary<string, List<int>> indexes) where T : class, new()
        {
            result = new();
            indexes = new();

            var book = LoadBook(path);
            if (null == book)
            {
                LogHelper.Dev($"LoadCustomClasses {path} 엑셀 파일이 존재 하지 않습니다.");
                return false;
            }

            var len = book.NumberOfSheets;
            for (int ii = 0; ii < len; ++ii)
            {
                var sheet = book.GetSheetAt(ii);
                if (null != sheet)
                {
                    if (sheet.SheetName.StartsWith(COMMENTPREFIX))
                    {
                        // 주석시트
                        continue;
                    }

                    var list = LoadCustomClassSheet<T>(sheet, out var indexList);
                    if (false == result.TryAdd(sheet.SheetName, list))
                    {
                        LogHelper.Dev($"LoadCustomClasses {path} / {sheet.SheetName} 시트 로드 실패", LogHelper.LogLevel.Error);
                    }
                    if (false == indexes.TryAdd(sheet.SheetName, indexList))
                    {
                        LogHelper.Dev($"LoadCustomClasses {path} / {sheet.SheetName} 시트 로드 실패", LogHelper.LogLevel.Error);
                    }
                }
            }

            return true;
        }

        private static List<T> LoadCustomClassSheet<T>(NPOI.SS.UserModel.ISheet sheet) where T : class, new()
        {
            List<T> result = new();

            LoadHeader(sheet, out var sheetInfo, out var rowInfo);

            // 프로그레스바 필요할경우 처리하기위해 일반 for사용
            // 0 이름 로우 1 데이터타입로우는 넘김
            // 엑셀 카운트는 0 부터시작이어서 길이와 비교는 <= 입니다
            var len = sheet.LastRowNum;
            for (int ii = 2; ii <= len; ++ii)
            {
                if (false == TryGetValidRow(sheet, ii, out var row))
                {
                    LogHelper.Dev($"{sheet.SheetName} , 시트에 공백열이 있습니다 데이터 확인해주세요", LogHelper.LogLevel.Warning);
                    continue;
                }


                if (row.IsComment())
                {
                    continue;
                }

                T newData = new();

                rowInfo.ForEach((h) =>
                {
                    var fieldInfo = typeof(T).GetField(h.ValueName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var obj = CellToFieldObject(row.GetCell(h._cellIndex), fieldInfo, out bool isEmpty);
                    if (isEmpty)
                    {
                        LogHelper.Dev($"{sheet.SheetName} 시트 {ii}열 {h.ValueName} 값이 빈값입니다 정상적인 값인지 확인해주세요", LogHelper.LogLevel.Error);
                    }
                    fieldInfo.SetValue(newData, obj);
                });

                result.Add(newData);
            }

            return result;
        }

        private static List<T> LoadCustomClassSheet<T>(NPOI.SS.UserModel.ISheet sheet, out List<int> indexes) where T : class, new()
        {
            List<T> result = new();
            indexes = new();

            LoadHeader(sheet, out var sheetInfo, out var rowInfo);

            // 프로그레스바 필요할경우 처리하기위해 일반 for사용
            // 0 이름 로우 1 데이터타입로우는 넘김
            // 엑셀 카운트는 0 부터시작이어서 길이와 비교는 <= 입니다
            var len = sheet.LastRowNum;
            for (int ii = 2; ii <= len; ++ii)
            {
                if (false == TryGetValidRow(sheet, ii, out var row))
                {
                    LogHelper.Dev($"{sheet.SheetName} , 시트에 공백열이 있습니다 데이터 확인해주세요", LogHelper.LogLevel.Warning);
                    continue;
                }

                if (row.IsComment())
                {
                    continue;
                }

                indexes.Add((int)row.GetCell(sheetInfo._keyIndex[0]).NumericCellValue);

                T newData = new();

                rowInfo.ForEach((h) =>
                {
                    var fieldInfo = typeof(T).GetField(h.ValueName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var obj = CellToFieldObject(row.GetCell(h._cellIndex), fieldInfo, out bool isEmpty);
                    if (isEmpty)
                    {
                        LogHelper.Dev($"{sheet.SheetName} 시트 {ii}열 {h.ValueName} 값이 빈값입니다 정상적인 값인지 확인해주세요", LogHelper.LogLevel.Error);
                    }
                    fieldInfo.SetValue(newData, obj);
                });

                result.Add(newData);
            }

            return result;
        }
    }
}
#endif
