using System;
using System.Data;

namespace CommonComponents.Interfaces.Utilities
{
    public interface IDataAccess
    {
        string ConnectionString { get; set; }

        void CloseReader(IDataReader dr);
        void ExecuteQuery(string procName, params object[] allParams);
        T ExecuteQuery<T>(string procName, params object[] allParams);
        DataSet GetDataSet(string procName, params object[] allParams);
        DataSet GetDataSetByQuery(string query);
        DataSet GetDataSetByQuery(string query, params object[] allParams);
        DataSet GetDataSetWithPaging(string procName, int recNumToStart, int recNumToShow, params object[] allParams);
        int GetNoOfRowsAffected(string procName, params object[] allParams);
        int GetOutputValue(string id);
        IDataReader GetReader(string procName, params object[] allParams);
        IDataReader GetReaderByQuery(string query);
        string GetStringOutputValue(string id);
        string GetXmlFromArray(Array array);
        void UpdateDataSet(string updateStatement, string deleteStatement, DataSet ds, bool queryTypeUpdateCommand, bool queryTypeDeleteCommand, params object[] allParams);
    }

    public struct DBParameter
    {
        private string _paramName;
        private object _paramValue;
        private DBParameterType _paramType;
        private string _sourceColumn;

        public DBParameter(string name, object value, DBParameterType paramType)
        {
            _paramName = name;
            _paramValue = value;
            _paramType = paramType;
            _sourceColumn = string.Empty;
        }
        public DBParameter(string name, object value, DBParameterType paramType, string sourceColumn)
        {
            _paramName = name;
            _paramValue = value;
            _paramType = paramType;
            _sourceColumn = sourceColumn;
        }
        public string ParamName
        {
            get { return _paramName; }
            set { _paramName = value; }
        }

        public object ParamValue
        {
            get { return _paramValue; }
            set { _paramValue = value; }
        }

        public DBParameterType ParamType
        {
            get { return _paramType; }
            set { _paramType = value; }
        }
        public string SourceColumn
        {
            get { return _sourceColumn; }
            set { _sourceColumn = value; }
        }
    }

    public enum DBParameterType
    {
        StringParam,
        IntParam,
        DateTimeParam,
        DoubleParam,
        BoolParam,
        TextParam,
        BytesParam,
        NcharParam,
        XmlParam,
        IntParamOutput,
        StringParamOutput,
        NullableIntParam,
        MoneyParam,
        ByteParam,
    }
}
