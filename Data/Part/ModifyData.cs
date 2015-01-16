/**
 * 自然框架之数据访问类库
 * http://www.natureFW.com/
 *
 * @author
 * 金洋（金色海洋jyk）
 * 
 * @copyright
 * Copyright (C) 2005-2013 金洋.
 *
 * Licensed under a GNU Lesser General Public License.
 * http://creativecommons.org/licenses/LGPL/2.1/
 *
 * 自然框架之数据访问类库 is free software. You are allowed to download, modify and distribute 
 * the source code in accordance with LGPL 2.1 license, however if you want to use 
 * 自然框架之数据访问类库 on your site or include it in your commercial software, you must  be registered.
 * http://www.natureFW.com/registered
 */

/* ***********************************************
 * author :  金洋（金色海洋jyk）
 * email  :  jyk0011@live.cn  
 * function: 数据访问类库的配件部分，对SQL语句的insert into、update 的封装。 
 * history:  created by 金洋 2010-05-26 11:20
 *           2011-4-14 出现异常时把参数值记录到错误日志里面，以便于修改错误。
 * **********************************************
 */


using System.Text;
using System.Data.Common;

namespace Nature.Data.Part
{
    /// <summary>
    /// 处理数据的添加、修改
    /// </summary>
    public class ModifyData
    {
        /// <summary>
        /// 数据访问函数库的实例，主要是想操作Connection
        /// </summary>
        private readonly DataAccessLibrary _dal;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="dal">数据访问函数库的实例</param>
        public ModifyData(DataAccessLibrary dal)
        {
            _dal = dal;
        }

        //SQL语句的方式添加、修改数据

        #region 使用拼接SQL语句的方式添加记录
        /// <summary>
        /// 添加记录。传入表名，字段数组，值数组，返回新生成记录的ID(仅限于MS SQL数据库，且表里有自增字段)
        /// </summary>
        /// <param name="tableName">要添加记录的表的名称</param>
        /// <param name="columnName">字段名数组</param>
        /// <param name="columnValue">字段对应的值的数组</param>
        /// <returns></returns>
        public virtual string InsertData(string tableName, string[] columnName, string[] columnValue)
        {
            //添加数据，Oledb\Odbc都在这里实现，其他的（SqlClient等）在子类里面实现，
            var sql = new StringBuilder(800);
            sql.Append("insert into ");					//insert into 
            sql.Append(tableName);
            sql.Append(" ([");
            int i;

            #region 字段
            for (i = 0; i < columnName.Length; i++)		//字段
            {
                if (columnValue[i] != null)
                {
                    sql.Append(columnName[i]);
                    sql.Append("],[");
                }
            }
            sql = sql.Remove(sql.Length - 2, 2);
            #endregion

            sql.Append(")  values ('");

            #region 值
            for (i = 0; i < columnName.Length; i++)     //字段对应的值
            {
                if (columnValue[i] != null)
                {
                    sql.Append(columnValue[i]);
                    sql.Append("','");
                }
            }
            sql = sql.Remove(sql.Length - 2, 2);
            #endregion
            sql.Append(") ");

            sql.Append(" select scope_identity() as newID ");

            string re = _dal.ExecuteString(sql.ToString(), "InsertData_SQL");
            sql.Length = 0;
            return re;

        }
        #endregion

        #region 根据存储过程的参数的信息，自动拼接参数化的SQL语句，用于添加数据
        /// <summary>
        /// 根据存储过程的参数的信息，自动拼接参数化的SQL语句，用于添加数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public virtual string InsertData(string tableName)
        {
            if (_dal.Command.Parameters.Count == 0)
            {
                _dal.SetError("InsertData_Parameters", tableName, "没有参数，无法自动拼接参数化的SQL语句","");
                return "没有参数，无法自动拼接参数化的SQL语句";
            }

            //添加数据	返回新添加的ID，在子类里实现
            var sql = new StringBuilder(1000);
            sql.Append("insert into ");					//insert into 
            sql.Append(tableName);
            sql.Append(" ([");
            int i;

            #region 字段

            for (i = 0; i < _dal.Command.Parameters.Count; i++)		//字段
            {
                if (_dal.Command.Parameters[i].Value != null)
                {
                    string tmp = _dal.Command.Parameters[i].ParameterName;
                    tmp = tmp.Substring(1);
                    sql.Append(tmp);
                    sql.Append("],[");
                }
            }
            sql = sql.Remove(sql.Length - 2, 2);
            #endregion

            sql.Append(")  values (");

            #region 值
            for (i = 0; i < _dal.Command.Parameters.Count; i++)     //字段对应的值
            {
                if (_dal.Command.Parameters[i].Value != null)
                {
                    sql.Append(_dal.Command.Parameters[i].ParameterName);
                    sql.Append(",");
                }
            }
            sql = sql.Remove(sql.Length - 1, 1);

            #endregion

            //获取新添加的记录的主键值，自增字段有效
            sql.Append(")  select scope_identity() as newID ");
            
            string re = _dal.ExecuteString(sql.ToString(), "InsertData_Parameters");
            sql.Length = 0;

            if (_dal.ErrorMessage.Length > 2)
            {
                //出错了，需要把参数值也记录到错误日志里面，以便于修改错误。
                //获取参数值，以便于查找错误
                _dal.SetError("InsertData_Parameters_Value", tableName + GetParamerterValue(_dal.Command.Parameters), "添加数据时出现异常！", _dal.ConnectionString);
                return "添加数据时出现异常！";
            }

            return re;

        }
        #endregion

        #region SQL语句的方式修改数据
        /// <summary>
        /// 修改记录。传入表名，字段数组，值数组 ，修改条件
        /// </summary>
        /// <param name="tableName">要修改记录的表的名称</param>
        /// <param name="columnName">字段名数组</param>
        /// <param name="columnValue">字段对应的值的数组</param>
        /// <param name="query">条件 ，加在where 后面的语句</param>
        /// <returns></returns>
        public virtual bool UpdateData(string tableName, string[] columnName, string[] columnValue, string query)
        {
            //修改记录的方式是一样的，就在基类里面实现了。
            var sql = new StringBuilder(500);
            sql.Append("update ");					//update
            sql.Append(tableName);
            sql.Append(" set ");
            int i;

            #region 字段 = 值
            for (i = 0; i < columnName.Length; i++)
            {
                if (columnValue[i] != null)
                {
                    sql.Append("[");
                    sql.Append(columnName[i]);					//update
                    sql.Append("]='");
                    sql.Append(columnValue[i]);
                    sql.Append("',");
                }
                //else
                //{
                //    SQL.Append("]='',");
                //}

            }
            sql = sql.Remove(sql.Length - 1, 1);	//去掉最后一个 ","
            #endregion

            sql.Append(" where ");
            sql.Append(query);

            _dal.ExecuteNonQuery(sql.ToString(), "UpdateData_SQL" );
            sql.Length = 0;
            return true;

        }
        #endregion

        #region 使用参数化SQL语句的方式修改记录
        /// <summary>
        /// 修改记录。传入表名 ，修改条件
        /// </summary>
        /// <param name="tableName">要修改记录的表的名称</param>
        /// <param name="query">条件 ，加在where 后面的语句</param>
        /// <returns></returns>
        public virtual string UpdateData(string tableName, string query)
        {
            //修改记录的方式是一样的，就在基类里面实现了。
            var sql = new StringBuilder(500);
            sql.Append("update ");					//update
            sql.Append(tableName);
            sql.Append(" set ");
            int i;

            #region 字段 = 值

            for (i = 0; i < _dal.Command.Parameters.Count; i++)
            {
                string tmp = _dal.Command.Parameters[i].ParameterName;
                tmp = tmp.Substring(1);

                sql.Append("[");
                sql.Append(tmp);					//update

                sql.Append("]=");
                sql.Append(_dal.Command.Parameters[i].ParameterName);
                sql.Append(",");

            }
            sql = sql.Remove(sql.Length - 1, 1);	//去掉最后一个 ","
            #endregion

            sql.Append(" where ");
            sql.Append(query);

            _dal.ExecuteNonQuery(sql.ToString(), "UpdateData_Parameters" );
            sql.Length = 0;

            if (_dal.ErrorMessage.Length > 0)
            {
                //出错了，需要把参数值也记录到错误日志里面，以便于修改错误。
                _dal.SetError("UpdateData_Parameters", tableName, "修改数据时出现异常！参数值：" + GetParamerterValue(_dal.Command.Parameters), _dal.ConnectionString);
                return "修改数据时出现异常！";
            }

            return "";

        }
        #endregion

        #region 获取参数值
        private string GetParamerterValue(DbParameterCollection parms)
        {
            //
            var sb = new StringBuilder(parms.Count * 500);
            sb.Append("/n");
            foreach (DbParameter parm in parms)
            {
                //遍历参数
                sb.Append(parm.ParameterName);
                sb.Append(":");
                sb.Append(parm.Value);
                sb.Append(",");
                
            }

            return sb.ToString();

        }
        #endregion
    }
}
