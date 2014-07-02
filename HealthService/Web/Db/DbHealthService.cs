using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using System.Collections;
using System.Data;
using System.Text;

namespace Web.Db
{
    public class DbHealthService :IDisposable
    {
        
        private OracleConnection conn;
        private const string OraceConnectionString = "User Id=xlfy;Password=xlfy;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=10.4.30.41)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ora11g)));";
        public DbHealthService()
        {
            open();
            
        }
        protected void open()
        {
            try
            {
                conn = new OracleConnection(OraceConnectionString);
                conn.Open();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
           
        }
        protected void close()
        {
            try
            {
                conn.Close();
                conn.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        public void  executeSql(String sql,Func<OracleDataReader,bool> readdr){
          
           OracleCommand cmd = conn.CreateCommand();
           cmd.CommandText = sql;
           OracleDataReader dr= cmd.ExecuteReader(System.Data.CommandBehavior.Default);
           readdr(dr);
           dr.Close();
           
        }
        public int executeSp(String sp_name, Parameter[] paramters)
        {
           object result = executeSp(sp_name, paramters, null);
           return (int)result;
        }

      
        public object executeSp(String sp_name, Parameter[] paramters,String return_Param_Name="v_rt")
        {
            OracleCommand cmd = conn.CreateCommand();
            try
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = sp_name;
                foreach (Parameter param in paramters)
                {
                    OracleParameter p = new OracleParameter(param.name, param.value);

                    p.Direction = param.getDirection();
                    p.OracleDbType = param.getOracleType();
                    cmd.Parameters.Add(p);
                }

                int result = cmd.ExecuteNonQuery();
                if (!String.IsNullOrEmpty(return_Param_Name))
                {
                    object obj = cmd.Parameters[return_Param_Name].Value;
                    return obj;
                }
                else
                {
                    return result;
                }
            }
            catch(Exception e)
            {

                return 0;
            }
            finally
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(String.Format("Execute [{0}] with parameters:\r\n",sp_name));
                foreach(var p in paramters)
                {
                    sb.Append(String.Format("\t{0}={1}\r\n",p.name,p.value.ToString()));
                }
                System.Diagnostics.Debug.WriteLine(sb.ToString());
                cmd.Dispose();
            }

            
            

        }
        public void executeSqlWithParameters(String sql, Parameter[] paramters, Func<OracleDataReader, bool> readdr)
        {
            OracleCommand cmd = conn.CreateCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                foreach (Parameter param in paramters)
                {
                    OracleParameter p = new OracleParameter(param.name, param.value);

                    p.Direction = param.getDirection();
                    p.OracleDbType = param.getOracleType();
                    cmd.Parameters.Add(p);
                }
               
                OracleDataReader dr = cmd.ExecuteReader(System.Data.CommandBehavior.Default);
                readdr(dr);
                dr.Close();
            }
            catch (Exception e)
            {
               
                
            }
            finally
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(String.Format("Execute [{0}] with parameters:\r\n", sql));
                foreach (var p in paramters)
                {
                    sb.Append(String.Format("\t{0}={1}\r\n", p.name, p.value.ToString()));
                }
                System.Diagnostics.Debug.WriteLine(sb.ToString());
                cmd.Dispose();
            }




        }
        public void Dispose()
        {
            close();
        }
        public static ArrayList datareaderToArrayList(OracleDataReader dr)
        {
            var list = new ArrayList();
            bool hasField = false;
            var fields = new ArrayList();
            while(dr.Read())
            {
                if (!hasField)
                {
                    var dt = dr.GetSchemaTable();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        fields.Add(dt.Rows[i].ItemArray[0]);
                    }
                    list.Add(fields);
                    hasField = true;

                }
                var fieldcount = dr.FieldCount;
                var items = new ArrayList();
                for (int i = 0; i < fieldcount; i++)
                {
                    items.Add(dr.GetValue(i).ToString());
                }
                list.Add(items);
                
            }
            return list;
        }
        public static ArrayList arrayListToDictionaryList(ArrayList results)
        {
            Dictionary<String, Object> dict = new Dictionary<string, object>();
            if (results.Count < 2)
            {
                return results;
            }
            ArrayList fields = results[0] as ArrayList;
            ArrayList temps = new ArrayList();
            for (int i = 1; i < results.Count; i++)
            {
                Dictionary<String, Object> item = new Dictionary<string, object>();
                for (int j = 0; j < fields.Count; j++)
                {
                    item.Add(fields[j].ToString(), (results[i] as ArrayList)[j]);
                }
                temps.Add(item);
            }
            return temps;  
        }
        public static ArrayList getTableToList(String tablename) { 
            return executeSqlToList(String.Format("select * from {0}",tablename));
        }
        public static ArrayList executeSqlToList(String sql)
        {
            ArrayList list=null;
            
            using (DbHealthService db = new DbHealthService())
            {
                db.executeSql(sql, (dr) =>
                {
                    list = datareaderToArrayList(dr);
                    return false;
                });
            }
            return list;
        }
        public static ArrayList ExecuteSql(String sql)
        {
            var results = executeSqlToList(sql);
            return arrayListToDictionaryList(results);
            
        }
        public static ArrayList ExecuteSqlWithParams(String sql, Parameter[] parameters)
        {
            var results = ExecuteSqlWithParamsToList(sql, parameters);
            return arrayListToDictionaryList(results);
        }
        public static int ExecuteSP(String sp_name, Parameter[] parameters,String return_Param_Name="v_rt") {
            int result;
            using (var db = new DbHealthService())
            {
                
                result = Convert.ToInt32(db.executeSp(sp_name, parameters,return_Param_Name).ToString());
            }
            return result;
        }
        public static ArrayList ExecuteSqlWithParamsToList(String sql, Parameter[] paramters)
        {
            ArrayList results=null;
            using (var db = new DbHealthService())
            {
                db.executeSqlWithParameters(sql, paramters, (dr) => {
                    results = datareaderToArrayList(dr);
                    return false;
                });
            }
            return results;
        }

    }
    public class Parameter
    {
        public String name;
        public Object value;
        public int length;
        public int direction;
        public int type;
        public Parameter()
        {
            direction = 1;
            type = 1;

        }
        public OracleDbType getOracleType()
        {
            OracleDbType result = OracleDbType.Varchar2;
            switch (type)
            { 
                case 2:
                    result = OracleDbType.Int32;
                    break;
                case 1:
                    result = OracleDbType.Varchar2;
                    break;
                case 3:
                    result = OracleDbType.Date;
                    break;
            }
            return result;
        }
        public ParameterDirection getDirection()
        {
            ParameterDirection result = ParameterDirection.ReturnValue;
            switch (direction)
            { 
                case 1:
                    result = ParameterDirection.Input;
                    break;
                case 2:
                    result = ParameterDirection.Output;
                    break;
                case 3:
                    result = ParameterDirection.InputOutput;
                    break;
               
            }
            return result;
        }

    }
}