using CommonComponents.Interfaces.Utilities;
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Xml;

namespace CommonComponents.Concrete.Utilities
{
    internal class DataAccess : IDataAccess
    {
        string _connstr = null;

        public string ConnectionString
        {
            get { return (_connstr == null) ? ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString : _connstr; }
            set { _connstr = value; }
        }
        
        private Hashtable _outputValue = new Hashtable();
        private ArrayList _parameterNames = new ArrayList();

        /// <summary>
        /// Stores the values for the output parameters.
        /// </summary>
        private void AddOutputValue(string name, int value)
        {
            _outputValue[name] = value;
        }
        /// <summary>
        /// Stores the values for the output parameters.
        /// </summary>
        private void AddOutputValue(string name, string value)
        {
            _outputValue[name] = value;
        }
        /// <summary>
        /// Retrieves the output value from the parameter name.
        /// </summary>
        public int GetOutputValue(string id)
        {
            return (int)_outputValue[id];
        }
        /// <summary>
        /// Retrieves the output value from the parameter name.
        /// </summary>
        public string GetStringOutputValue(string id)
        {
            bool isnull =
                _outputValue.ContainsKey(id) == false
                || _outputValue[id] == null
                || Convert.IsDBNull(_outputValue[id]);
            return isnull ? null : _outputValue[id].ToString();
        }

        private ArrayList ParameterNames
        {
            get { return _parameterNames; }
            set { _parameterNames = value; }
        }

        public void CloseReader(IDataReader dr)
        {
            SqlUtil.CloseReader((SqlDataReader)dr);
        }
        public void ExecuteQuery(string procName, params object[] allParams)
        {
            SqlConnection conn = null;
            SqlCommand cmd;

            try
            {
                conn = SqlUtil.CreateConnection(ConnectionString);
                cmd = SqlUtil.CreateCommand(conn);
                cmd.CommandText = procName.Trim();
                cmd.CommandType = CommandType.StoredProcedure;

                if (allParams != null)
                {
                    foreach (object obj in allParams)
                    {
                        if (obj is IList)
                        {
                            foreach (DBParameter db in (IList)obj)
                            {
                                AddDBParam(cmd, (DBParameter)db);
                            }
                        }
                        else
                            AddDBParam(cmd, (DBParameter)obj);
                    }
                }

                SqlUtil.ExecuteCommand(cmd, ConnectionString);

                #region Output parameters
                SetOutputParameter(cmd);
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                SqlUtil.ReleaseConnection(conn);
            }
        }

        public T ExecuteQuery<T>(string procName, params object[] allParams)
        {
            SqlConnection conn = null;
            SqlCommand cmd;
            T rtn;
            try
            {
                conn = SqlUtil.CreateConnection(ConnectionString);
                cmd = SqlUtil.CreateCommand(conn);
                cmd.CommandText = procName.Trim();
                cmd.CommandType = CommandType.StoredProcedure;

                if (allParams != null)
                {
                    foreach (object obj in allParams)
                    {
                        if (obj is IList)
                        {
                            foreach (DBParameter db in (IList)obj)
                            {
                                AddDBParam(cmd, (DBParameter)db);
                            }
                        }
                        else
                            AddDBParam(cmd, (DBParameter)obj);
                    }
                }

                rtn = (T)SqlUtil.ExecuteScalarCommand(cmd, ConnectionString);

                #region Output parameters
                SetOutputParameter(cmd);
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                SqlUtil.ReleaseConnection(conn);
            }
            return rtn;
        }

        public int GetNoOfRowsAffected(string procName, params object[] allParams)
        {
            SqlConnection conn = null;
            SqlCommand cmd;
            int rtn;
            try
            {
                conn = SqlUtil.CreateConnection(ConnectionString);
                cmd = SqlUtil.CreateCommand(conn);
                cmd.CommandText = procName.Trim();
                cmd.CommandType = CommandType.StoredProcedure;

                if (allParams != null)
                {
                    foreach (object obj in allParams)
                    {
                        if (obj is IList)
                        {
                            foreach (DBParameter db in (IList)obj)
                            {
                                AddDBParam(cmd, (DBParameter)db);
                            }
                        }
                        else
                            AddDBParam(cmd, (DBParameter)obj);
                    }
                }

                rtn = SqlUtil.GetNoOfRowsAffected(cmd, ConnectionString);

                #region Output parameters
                SetOutputParameter(cmd);
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                SqlUtil.ReleaseConnection(conn);
            }
            return rtn;
        }

        public IDataReader GetReader(string procName, params object[] allParams)
        {
            SqlConnection conn = null;
            SqlCommand cmd;

            try
            {
                conn = SqlUtil.CreateConnection(ConnectionString);
                cmd = SqlUtil.CreateCommand(conn);
                cmd.CommandText = procName;
                cmd.CommandType = CommandType.StoredProcedure;

                if (allParams != null)
                {
                    foreach (object obj in allParams)
                    {
                        if (obj is IList)
                        {
                            foreach (DBParameter db in (IList)obj)
                            {
                                AddDBParam(cmd, (DBParameter)db);
                            }
                        }
                        else
                            AddDBParam(cmd, (DBParameter)obj);
                    }
                }

                return SqlUtil.GetDataReader(cmd, ConnectionString);
            }
            finally
            {
                SqlUtil.ReleaseConnection(conn);
            }
        }

        public IDataReader GetReaderByQuery(string query)
        {
            SqlConnection conn = null;
            SqlCommand cmd;

            try
            {
                conn = SqlUtil.CreateConnection(ConnectionString);
                cmd = SqlUtil.CreateCommand(conn);
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;

                return SqlUtil.GetDataReader(cmd, ConnectionString);
            }
            finally
            {
                SqlUtil.ReleaseConnection(conn);
            }
        }

        public DataSet GetDataSetByQuery(string query)
        {
            DataSet retVal = new DataSet();
            SqlConnection conn = null;
            SqlCommand cmd;
            SqlDataAdapter da;

            try
            {
                conn = SqlUtil.CreateConnection(ConnectionString);
                cmd = SqlUtil.CreateCommand(conn);
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;

                da = new SqlDataAdapter(cmd);
                SqlUtil.StartTimer();
                da.Fill(retVal);
                SqlUtil.StopTimer(cmd);
                return retVal;
            }
            finally
            {
                SqlUtil.ReleaseConnection(conn);
            }
        }

        public DataSet GetDataSetByQuery(string query, params object[] allParams)
        {
            DataSet retVal = new DataSet();
            SqlConnection conn = null;
            SqlCommand cmd;
            SqlDataAdapter da;

            try
            {
                conn = SqlUtil.CreateConnection(ConnectionString);
                cmd = SqlUtil.CreateCommand(conn);
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;

                if (allParams != null)
                {
                    foreach (object obj in allParams)
                    {
                        if (obj is IList)
                        {
                            foreach (DBParameter db in (IList)obj)
                            {
                                AddDBParam(cmd, (DBParameter)db);
                            }
                        }
                        else
                            AddDBParam(cmd, (DBParameter)obj);
                    }
                }

                da = new SqlDataAdapter(cmd);
                SqlUtil.StartTimer();
                da.Fill(retVal);
                SqlUtil.StopTimer(cmd);
                return retVal;
            }
            finally
            {
                SqlUtil.ReleaseConnection(conn);
            }
        }

        public DataSet GetDataSet(string procName, params object[] allParams)
        {
            DataSet retVal = new DataSet();
            SqlConnection conn = null;
            SqlCommand cmd;
            SqlDataAdapter da;

            try
            {
                conn = SqlUtil.CreateConnection(ConnectionString);
                cmd = SqlUtil.CreateCommand(conn);
                cmd.CommandText = procName;
                cmd.CommandType = CommandType.StoredProcedure;

                if (allParams != null)
                {
                    foreach (object obj in allParams)
                    {
                        if (obj is IList)
                        {
                            foreach (DBParameter db in (IList)obj)
                            {
                                AddDBParam(cmd, (DBParameter)db);
                            }
                        }
                        else
                            AddDBParam(cmd, (DBParameter)obj);
                    }
                }

                da = new SqlDataAdapter(cmd);
                SqlUtil.StartTimer();
                da.Fill(retVal);
                SqlUtil.StopTimer(cmd);

                #region Output parameters
                SetOutputParameter(cmd);
                #endregion

                return retVal;
            }
            finally
            {
                SqlUtil.ReleaseConnection(conn);
            }
        }
        public DataSet GetDataSetWithPaging(string procName, int recNumToStart, int recNumToShow, params object[] allParams)
        {
            DataSet retVal = new DataSet();
            SqlConnection conn = null;
            SqlCommand cmd;
            SqlDataAdapter da;
            DataTable dt = new DataTable();
            try
            {
                conn = SqlUtil.CreateConnection(ConnectionString);
                cmd = SqlUtil.CreateCommand(conn);
                cmd.CommandText = procName;
                cmd.CommandType = CommandType.StoredProcedure;

                if (allParams != null)
                {
                    foreach (object obj in allParams)
                    {
                        if (obj is IList)
                        {
                            foreach (DBParameter db in (IList)obj)
                            {
                                AddDBParam(cmd, (DBParameter)db);
                            }
                        }
                        else
                            AddDBParam(cmd, (DBParameter)obj);
                    }
                }

                da = new SqlDataAdapter(cmd);
                //da.Fill(retVal);
                SqlUtil.StartTimer();
                da.Fill(recNumToStart, recNumToShow, dt);
                SqlUtil.StopTimer(cmd);
                retVal.Tables.Add(dt);


                if (ParameterNames.Count != 0)
                {
                    foreach (string s in ParameterNames)
                    {
                        /****set output parameter*****/
                        AddOutputValue(s, (int)cmd.Parameters[s].Value);

                    }
                }

                return retVal;

            }
            finally
            {
                SqlUtil.ReleaseConnection(conn);
            }
        }

        public void UpdateDataSet(string updateStatement, string deleteStatement, DataSet ds, bool queryTypeUpdateCommand, bool queryTypeDeleteCommand, params object[] allParams)
        {
            SqlConnection conn = null;
            SqlCommand updCmd;
            SqlCommand delCmd;
            SqlCommand cmd = null;
            SqlDataAdapter da;
            try
            {
                conn = SqlUtil.CreateConnection(ConnectionString);
                updCmd = SqlUtil.CreateCommand(conn);
                updCmd.CommandText = updateStatement;
                if (queryTypeUpdateCommand)
                    updCmd.CommandType = CommandType.Text;
                else
                    updCmd.CommandType = CommandType.StoredProcedure;

                delCmd = SqlUtil.CreateCommand(conn);
                delCmd.CommandText = deleteStatement;
                if (queryTypeDeleteCommand)
                    delCmd.CommandType = CommandType.Text;
                else
                    delCmd.CommandType = CommandType.StoredProcedure;

                if (allParams != null)
                {
                    for (int iCount = 0; iCount < allParams.Length; iCount++)
                    {
                        if (iCount == 0)
                            cmd = updCmd;
                        else if (iCount == 1)
                            cmd = delCmd;
                        object obj = allParams[iCount];
                        if (obj == null)
                            continue;

                        if (obj is IList)
                        {
                            foreach (DBParameter db in (IList)obj)
                            {
                                if (db.ParamValue == null)
                                    AddDBParamNoValue(cmd, (DBParameter)db);
                                else
                                    AddDBParam(cmd, (DBParameter)db);
                            }
                        }
                        else
                            if (((DBParameter)obj).ParamValue == null)
                            AddDBParamNoValue(cmd, (DBParameter)obj);
                        else
                            AddDBParam(cmd, (DBParameter)obj);
                    }
                }

                da = new SqlDataAdapter();
                da.UpdateCommand = updCmd;
                da.InsertCommand = updCmd;
                da.DeleteCommand = delCmd;
                //da.UpdateBatchSize = 10;
                da.Update(ds);
            }
            finally
            {
                SqlUtil.ReleaseConnection(conn);
            }
        }

        private void AddDBParam(SqlCommand cmd, DBParameter dbParam)
        {
            if (dbParam.ParamValue == null)
            {
                AddDBParamNoValue(cmd, dbParam);
                return;
            }
            try
            {
                DBParameterType paramType = dbParam.ParamType;
                switch (paramType)
                {
                    case DBParameterType.BoolParam:
                        SqlUtil.AddParam(cmd, dbParam.ParamName, (bool)dbParam.ParamValue); break;
                    case DBParameterType.DateTimeParam:
                        SqlUtil.AddParam(cmd, dbParam.ParamName, (DateTime)dbParam.ParamValue); break;
                    case DBParameterType.DoubleParam:
                        SqlUtil.AddParam(cmd, dbParam.ParamName, (double)dbParam.ParamValue); break;
                    case DBParameterType.IntParam:
                        SqlUtil.AddParam(cmd, dbParam.ParamName, (int)dbParam.ParamValue); break;
                    case DBParameterType.NullableIntParam:
                        SqlUtil.AddParam(cmd, dbParam.ParamName, (int?)dbParam.ParamValue); break;
                    case DBParameterType.StringParam:
                        SqlUtil.AddParam(cmd, dbParam.ParamName, (string)dbParam.ParamValue); break;
                    case DBParameterType.TextParam:
                        SqlUtil.AddParam(cmd, dbParam.ParamName, (string)dbParam.ParamValue, true); break;
                    case DBParameterType.XmlParam:
                        SqlUtil.AddParam(cmd, dbParam.ParamName, (string)dbParam.ParamValue, false, true); break;
                    case DBParameterType.BytesParam:
                        SqlUtil.AddParam(cmd, dbParam.ParamName, (byte[])dbParam.ParamValue); break;
                    case DBParameterType.NcharParam:
                        SqlUtil.AddParam(cmd, dbParam.ParamName, dbParam.ParamValue == null ? new char[0] : ((string)dbParam.ParamValue).ToCharArray()); break;
                    case DBParameterType.IntParamOutput:
                        SqlUtil.AddOutputParam(cmd, dbParam.ParamName, paramType);
                        ParameterNames.Add(dbParam.ParamName);
                        break;
                    case DBParameterType.StringParamOutput:
                        SqlUtil.AddOutputParam(cmd, dbParam.ParamName, paramType);
                        ParameterNames.Add(dbParam.ParamName);
                        break;
                    case DBParameterType.ByteParam:
                        SqlUtil.AddParam(cmd, dbParam.ParamName, (byte)dbParam.ParamValue); break;
                }
            }
#pragma warning disable 168
            catch (System.InvalidCastException whoops)
            {
                // bad data, treat as null...
                AddDBParamNoValue(cmd, dbParam);
            }
#pragma warning restore 168
        }

        private void SetOutputParameter(SqlCommand cmd)
        {
            if (ParameterNames.Count != 0)
            {
                foreach (string s in ParameterNames)
                {
                    /****set output parameter*****/
                    if ((cmd.Parameters[s].DbType == DbType.String) ||
                        (cmd.Parameters[s].DbType == DbType.AnsiString))
                    {
                        if (cmd.Parameters[s] != null
                                && cmd.Parameters[s].Value != DBNull.Value)
                        {
                            AddOutputValue(s, (string)cmd.Parameters[s].Value);
                        }
                        else
                        {
                            AddOutputValue(s, string.Empty);
                        }
                    }
                    else
                    {
                        if (cmd.Parameters[s].Value != null
                                && cmd.Parameters[s].Value != DBNull.Value
                                && String.Compare(cmd.Parameters[s].Value.ToString(), String.Empty) != 0)
                        {
                            AddOutputValue(s, (int)cmd.Parameters[s].Value);
                        }
                        else
                        {
                            AddOutputValue(s, 0);
                        }
                    }
                }
            }
        }

        private void AddDBParamNoValue(SqlCommand cmd, DBParameter dbParam)
        {
            SqlParameter param = new SqlParameter();
            param.IsNullable = true;
            param.Value = DBNull.Value;
            param.ParameterName = dbParam.ParamName;
            param.SourceColumn = dbParam.SourceColumn;
            cmd.Parameters.Add(param);

            switch (dbParam.ParamType)
            {
                case DBParameterType.BoolParam:
                    param.SqlDbType = SqlDbType.Bit;
                    break;
                case DBParameterType.DateTimeParam:
                    param.SqlDbType = SqlDbType.DateTime;
                    break;
                case DBParameterType.DoubleParam:
                    param.SqlDbType = SqlDbType.Float;
                    break;
                case DBParameterType.IntParam:
                    param.SqlDbType = SqlDbType.Int;
                    break;
                case DBParameterType.StringParam:
                    param.SqlDbType = SqlDbType.VarChar;
                    break;
                case DBParameterType.TextParam:
                    param.SqlDbType = SqlDbType.Text;
                    break;
                case DBParameterType.BytesParam:
                    param.SqlDbType = SqlDbType.Image;
                    break;
                case DBParameterType.NcharParam:
                    param.SqlDbType = SqlDbType.NChar;
                    break;
                case DBParameterType.IntParamOutput:
                    param.SqlDbType = SqlDbType.Int;
                    param.Direction = ParameterDirection.Output;
                    ParameterNames.Add(param.ParameterName);
                    break;
                case DBParameterType.StringParamOutput:
                    param.SqlDbType = SqlDbType.VarChar;
                    param.Direction = ParameterDirection.Output;
                    ParameterNames.Add(param.ParameterName);
                    break;
            }

        }

        public string GetXmlFromArray(Array array)
        {
            return SqlUtil.GetXmlFromArray(array);
        }
    }

    internal class SqlUtil
    {
        public static int commandTimeOut;
        private static bool m_db_enable_audit = false;

        private static System.Diagnostics.Stopwatch _stopwatch;

        public static void StartTimer()
        {
            _stopwatch = new System.Diagnostics.Stopwatch();
            _stopwatch.Start();
        }

        private static int _slowQuerythreshold = 5;

        public static void StopTimer(SqlCommand cmd)
        {
            if (null != _stopwatch)
            {
                _stopwatch.Stop();
                if (_stopwatch.ElapsedMilliseconds > _slowQuerythreshold * 1000)
                {
                    ReportError(cmd, _stopwatch.ElapsedMilliseconds);
                }
            }
        }

        static SqlUtil()
        {
            string commandTimeOutStr = ConfigurationManager.AppSettings["commandTimeOut"];
            if (commandTimeOutStr != null)
            {
                try
                {
                    commandTimeOut = Int32.Parse(commandTimeOutStr);
                }
                catch
                { commandTimeOut = 30; }
            }
            else
                commandTimeOut = 30;	//default to 30

            if (!int.TryParse(ConfigurationManager.AppSettings["DB_SlowQueryThreshold"], out _slowQuerythreshold))
            {
                _slowQuerythreshold = 5;
            }

            string db_Enable_Audit = ConfigurationManager.AppSettings["DB_Enable_Audit"];
            if (!string.IsNullOrEmpty(db_Enable_Audit))
            {
                db_Enable_Audit = db_Enable_Audit.ToUpper();
                m_db_enable_audit = db_Enable_Audit == "1" || db_Enable_Audit == "Y" || db_Enable_Audit == "TRUE";
            }

        }

        public static void SetupAudit(DbConnection cn, DbCommand cmd, int autingUserId)
        {
            if (m_db_enable_audit == false ||
                cmd.CommandText.Contains("procGet") ||
                cmd.CommandText.Contains("proc_Get") ||
                cmd.CommandText.Contains("spGet") ||
                cmd.CommandText.Contains("sp_Get") ||
                cmd.CommandText.StartsWith("SELECT ", StringComparison.InvariantCultureIgnoreCase))
            {
                // don't need audit those "read" operations
                return;
            }

            int userId = autingUserId;
            string sql = string.Format(@"IF CONTEXT_INFO() IS NULL
                                         BEGIN
                                            DECLARE @contextInfo binary(128)
									        SET @contextInfo = CAST({0} AS binary(128))
									        SET CONTEXT_INFO @contextInfo
                                         END", userId);

            DbCommand cmd2 = cn.CreateCommand();
            cmd2.Connection = cn;
            cmd2.CommandText = sql;
            cmd2.CommandType = CommandType.Text;
            cmd2.ExecuteNonQuery();

            System.Diagnostics.Debug.WriteLine("**** GenericDataAccessObjects::SqlUtil -- EnableAudit **** " + cmd.CommandText);
        }

        public static string GetXmlFromArray(Array array)
        {
            using (MemoryStream s = new MemoryStream())
            {
                XmlTextWriter writer = new XmlTextWriter(s, Encoding.UTF8);

                // Starts a new document (standalone)
                // writer.WriteStartDocument(true);
                //
                // Note:
                //   WriteStartDocument() emits xml: <?xml version="1.0" encoding="UTF-8"?>
                //   which causes sql server xml parser fail: 
                //          error message: XML parsing: line xx, character xx, unable to switch the encoding
                //   therefore, i commented the call of WriteStartDocument() to fix the problem

                // Start of the array
                writer.WriteStartElement("Array");

                for (int i = 0; i < array.Length; i++)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteAttributeString("Value", array.GetValue(i).ToString());
                    writer.WriteEndElement();

                    //writer.WriteElementString("Item", array.GetValue(i).ToString());
                }

                // end of the array
                writer.WriteEndElement();

                // Ends the document
                //writer.WriteEndDocument();

                // must to call writer.Flush() before calling s.ToArray()
                writer.Flush();

                // convert to string
                byte[] data = s.ToArray();
                int dataLength = (int)s.Length;
                string xml = Encoding.UTF8.GetString(data, 0, dataLength);

                return xml;
            }
        }

        public static SqlConnection CreateConnection(string connstr)
        {
            SqlConnection conn;
            conn = new SqlConnection(connstr);
            conn.Open();
            return conn;
        }

        public static SqlCommand CreateCommand(SqlConnection conn)
        {
            SqlCommand retVal = conn.CreateCommand();
            retVal.CommandTimeout = commandTimeOut;
            return retVal;
        }

        public static void ReleaseConnection(SqlConnection conn)
        {
            if (conn != null)
            {
                try { conn.Close(); }
                catch { }
                conn.Dispose();
            }
        }

        public static void CloseReader(SqlDataReader dr)
        {
            if (dr != null)
            {
                try { dr.Close(); }
                catch { }
                dr.Dispose();
            }
        }

        public static void ExecuteCommand(SqlCommand cmd, string connstr, int audituserId = 0)
        {
            SqlConnection conn = null;
            try
            {
                conn = CreateConnection(connstr);

                SetupAudit(conn, cmd, audituserId);

                cmd.Connection = conn;
                SqlUtil.StartTimer();
                cmd.ExecuteNonQuery();
                SqlUtil.StopTimer(cmd);
            }
            finally
            {
                ReleaseConnection(conn);
            }
        }

        public static int GetNoOfRowsAffected(SqlCommand cmd, string connstr, int audituserId = 0)
        {
            SqlConnection conn = null;
            int rtnNoOfRows = 0;
            try
            {
                conn = CreateConnection(connstr);

                SetupAudit(conn, cmd, audituserId);

                cmd.Connection = conn;
                SqlUtil.StartTimer();
                rtnNoOfRows = cmd.ExecuteNonQuery();
                SqlUtil.StopTimer(cmd);
                return rtnNoOfRows;
            }
            finally
            {
                ReleaseConnection(conn);
            }
        }

        public static object ExecuteScalarCommand(SqlCommand cmd, string connstr)
        {
            SqlConnection conn = null;

            try
            {
                conn = CreateConnection(connstr);
                cmd.Connection = conn;
                SqlUtil.StartTimer();
                object rtn = cmd.ExecuteScalar();
                SqlUtil.StopTimer(cmd);
                return rtn;
            }
            finally
            {
                ReleaseConnection(conn);
            }
        }

        public static SqlDataReader GetDataReader(SqlCommand cmd, string connstr)
        {
            SqlConnection conn = null;
            SqlDataReader retVal = null;

            try
            {
                conn = CreateConnection(connstr);
                cmd.Connection = conn;

                SqlUtil.StartTimer();
                retVal = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                SqlUtil.StopTimer(cmd);

                return retVal;
            }
            catch (System.Exception e)
            {
                //just in case!
                CloseReader(retVal);
                ReleaseConnection(conn);
                throw e;
            }
            finally
            {
            }
        }


        public static void AddParam(SqlCommand cmd, string strName, byte[] bytes)
        {
            SqlParameter p;
            p = new SqlParameter(strName, SqlDbType.Image);
            p.Value = bytes;
            cmd.Parameters.Add(p);
        }

        public static void AddParam(SqlCommand cmd, string strName, byte value)
        {
            SqlParameter p;
            p = new SqlParameter(strName, SqlDbType.TinyInt);
            p.Value = value;
            cmd.Parameters.Add(p);
        }


        public static void AddParam(SqlCommand cmd, string strName, string strValue)
        {
            AddParam(cmd, strName, strValue, false);
        }

        public static void AddParam(SqlCommand cmd, string strName, string strValue, bool isText, bool isXml = false)
        {
            SqlParameter p;

            if (isText)
                p = new SqlParameter(strName, SqlDbType.Text);
            else if (isXml)
                p = new SqlParameter(strName, SqlDbType.Xml);
            else
                p = new SqlParameter(strName, SqlDbType.VarChar);

            p.Value = strValue;
            cmd.Parameters.Add(p);
        }

        public static void AddParam(SqlCommand cmd, string strName, int iValue)
        {
            SqlParameter p;

            p = new SqlParameter(strName, SqlDbType.Int);
            p.Value = iValue;
            cmd.Parameters.Add(p);
        }

        public static void AddParam(SqlCommand cmd, string strName, int? iValue)
        {
            SqlParameter p;

            p = new SqlParameter(strName, SqlDbType.Int);
            p.Value = iValue;
            cmd.Parameters.Add(p);
        }

        public static void AddParam(SqlCommand cmd, string strName, DateTime? dtValue)
        {
            SqlParameter p;

            p = new SqlParameter(strName, SqlDbType.DateTime);
            p.Value = dtValue;
            cmd.Parameters.Add(p);
        }

        public static void AddParam(SqlCommand cmd, string strName, double dblValue)
        {
            SqlParameter p;

            p = new SqlParameter(strName, SqlDbType.Decimal);
            //p.Scale = 2;
            p.Value = dblValue;
            cmd.Parameters.Add(p);
        }

        public static void AddParam(SqlCommand cmd, string strName, bool bValue)
        {
            SqlParameter p;

            p = new SqlParameter(strName, SqlDbType.Bit);
            p.Value = bValue;
            cmd.Parameters.Add(p);
        }

        public static void AddParam(SqlCommand cmd, string strName, char[] arrNchar)
        {
            SqlParameter p;

            p = new SqlParameter(strName, SqlDbType.NChar, arrNchar == null ? 0 : arrNchar.Length);
            p.Value = arrNchar;
            cmd.Parameters.Add(p);
        }
        public static void AddOutputParam(SqlCommand cmd, string strName, DBParameterType paramType)
        {
            SqlParameter p = null;

            switch (paramType)
            {
                case DBParameterType.IntParamOutput:
                    p = new SqlParameter(strName, SqlDbType.Int);
                    break;
                case DBParameterType.StringParamOutput:
                    p = new SqlParameter(strName, SqlDbType.VarChar, 8000);
                    break;
            }


            p.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(p);
        }
        public static void AddIdentityParam(SqlCommand cmd, string returnName)
        {
            SqlParameter p;

            p = new SqlParameter(returnName, SqlDbType.Int);
            p.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(p);
        }

        public static void ReportError(SqlCommand cmd, long time)
        {
            string serverInfo = "N/A";
            try
            {
                serverInfo = System.Environment.MachineName;
            }
            catch
            {
                // ignore any error
            }

            try
            {
                //Write the command that is taking longer
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("The following command took {0}ms to Execute", time);
                sb.Append(cmd.CommandText);
                sb.Append(" ");
                foreach (SqlParameter p in cmd.Parameters)
                {
                    switch (p.DbType)
                    {
                        case DbType.Int16:
                        case DbType.Int32:
                        case DbType.Int64:
                        case DbType.Decimal:
                        case DbType.Double:
                            sb.AppendFormat("{0},", p.Value.ToString());
                            break;
                        default:
                            sb.AppendFormat("'{0}',", p.Value.ToString());
                            break;
                    }
                }

                SqlCommand logcommand = SqlUtil.CreateCommand(CreateConnection(ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString));
                logcommand.CommandText = "Logging.procSaveErrorLog";
                logcommand.CommandType = CommandType.StoredProcedure;
                AddParam(logcommand, "@serverInfo", serverInfo);
                AddParam(logcommand, "@sourceClass", "DBExecute");
                AddParam(logcommand, "@sourceFunction", "DBExecute");
                AddParam(logcommand, "@userId", 0);
                AddParam(logcommand, "@errorMessage", sb.ToString());

                logcommand.ExecuteNonQuery();
            }
            catch { return; }
        }
    }
}
