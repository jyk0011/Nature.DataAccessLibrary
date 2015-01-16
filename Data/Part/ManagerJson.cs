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
 * function: 数据访问类库的配件部分，把记录集转换成Json的格式。
 * history:  created by 金洋 2011-03-28 15:28
 * **********************************************
 */

using System;
using System.Data;
using System.Data.Common;
using System.Text;
using Nature.Common;

namespace Nature.Data.Part
{
    /// <summary>
    /// 把记录集转换成Json的格式的配件
    /// </summary>
    public class ManagerJson
    {
        private string _jsonName = "data";
        /// <summary>
        /// json的名称，默认data
        /// </summary>
        /// <value>
        /// json的名称
        /// </value>
        /// user:jyk
        /// time:2012/10/23 9:36
        public string JsonName
        {
            get { return _jsonName; }
            set { _jsonName = value; }
        }
        /// <summary>
        /// 数据访问函数库的实例，主要是想操作Connection
        /// </summary>
        private readonly DataAccessLibrary _dal;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="dal">数据访问函数库的实例</param>
        public ManagerJson(DataAccessLibrary dal)
        {
            _dal = dal;
        }

        #region ExecuteFillJsonByColName
        /// <summary>
        /// 运行SQL语句、参数化的SQL语句或者存储过程，返回json格式的记录集。
        /// <para>数组形式，字段名作为key，字段值作为value。</para>
        /// <example>data:[{colName1:value1,colName2:value2},{colName1:value1,colName2:value2}]</example>
        /// </summary>
        /// <param name="text">查询语句或者存储过程的名称。
        /// 比如select * from tableName1 
        /// 或者 Proc_xxxGetDataTable
        /// </param>
        /// <returns></returns>
        public virtual string ExecuteFillJsonByColName(string text)
        {
            _dal.SetCommand(text);		//设置command

            var sb = new StringBuilder(2000);

            try
            {
                _dal.ConnectionOpen();
                DbDataReader reader = _dal.Command.ExecuteReader(CommandBehavior.CloseConnection);
                 
                sb.Append("\"");
                sb.Append(_jsonName);
                sb.Append("\":[ ");

                while (reader.Read())
                {
                    sb.Append("{ ");
                    for (int rowIndex = 0; rowIndex < reader.FieldCount;rowIndex++ )
                    {
                        sb.Append("\"");
                        sb.Append(reader.GetName(rowIndex));
                        sb.Append("\":");

                        if (reader[rowIndex] is DBNull)
                            Json.ObjectToJson("", sb);
                        else
                            Json.ObjectToJson(reader[rowIndex], sb);
                        sb.Append(",");
                        
                    }
                    //把最后一个 , 号 换成 } 。
                    sb[sb.Length - 1] = '}';
                    
                    sb.Append(",");
                    
                }

                if (sb.Length > _jsonName.Length +6)
                {
                    //把最后一个 , 号 换成 ] 。
                    sb[sb.Length - 1] = ']';
                }
                else
                {
                    sb.Append("]");
                    //sb[sb.Length - 1] = '\"';
                    //sb.Append("null\"");
                }
                return sb.ToString();

            }
            catch (Exception ex)
            {
                _dal.SetError("ExecuteFillJsonByColName", text, ex.Message + "<BR>ex:" + ex, _dal.ConnectionString);	//处理错误
                return null;
            }
            finally
            {
                //返回DataReader，不能关闭连接，需要调用者手动关闭连接
                if (!_dal.IsUseTrans) //判断是否使用了事务，没有使用事务的情况下，才可以关闭连接
                    _dal.Command.Connection.Close();

            }

        }
        #endregion

        #region ExecuteFillJsonByColNameKey
        /// <summary>
        /// 运行SQL语句、参数化的SQL语句或者存储过程，返回json格式的记录集。
        /// <para>第一个字段值作为key，字段名、字段值作为value。</para>
        /// <example>data:{value1:{colName1:value1,colName2:value2},value2:{colName1:value1,colName2:value2}}</example>
        /// </summary>
        /// <param name="text">查询语句或者存储过程的名称。
        /// 比如select * from tableName1 
        /// 或者 Proc_xxxGetDataTable
        /// </param>
        /// <returns></returns>
        public virtual string ExecuteFillJsonByColNameKey(string text)
        {
            _dal.SetCommand(text);		//设置command

            var sb = new StringBuilder(2000);

            try
            {
                _dal.ConnectionOpen();

                DbDataReader reader = _dal.Command.ExecuteReader(CommandBehavior.CloseConnection);

                //int i = 0;

                var keys = new StringBuilder(1000);
                keys.Append("\"");
                keys.Append(_jsonName);
                keys.Append("keys\":[");

                sb.Append("\"");
                sb.Append(_jsonName);
                sb.Append("\":{");

                while (reader.Read())
                {
                    Json.ObjectToJson(reader[0], keys);
                    keys.Append(",");

                    //第一个字段值最为key
                    sb.Append("\"");
                    sb.Append(reader[0]);
                    sb.Append("\":{");
                    for (int rowIndex = 0; rowIndex < reader.FieldCount; rowIndex++)
                    {
                        sb.Append("\"");
                        sb.Append(reader.GetName(rowIndex));
                        sb.Append("\":");

                        if (reader[rowIndex] is DBNull)
                            Json.ObjectToJson("", sb);
                        else
                            Json.ObjectToJson(reader[rowIndex], sb);

                        sb.Append(",");
                    }

                    //把最后一个 , 号 换成 } 
                    sb[sb.Length - 1] = '}';
                    sb.Append(",");
                }

                if (sb.Length > _jsonName.Length + 6)
                {
                    //把最后一个 , 号 换成 ] 
                    keys[keys.Length - 1] = ']';

                    //把最后一个 , 号 换成 ] 
                    sb[sb.Length - 1] = '}';
                }
                else
                {
                    sb.Append("}");
                    keys.Append("]");
                }

                sb.Append(",");
                sb.Append(keys);    
               
                return sb.ToString();

            }
            catch (Exception ex)
            {
                _dal.SetError("ExecuteFillJsonByColNameKey", text, ex.Message + "<BR>ex:" + ex, _dal.ConnectionString);	//处理错误
                return null;
            }
            finally
            {
                //返回DataReader，不能关闭连接，需要调用者手动关闭连接
                if (!_dal.IsUseTrans) //判断是否使用了事务，没有使用事务的情况下，才可以关闭连接
                    _dal.Command.Connection.Close();

            }

        }
        #endregion

        #region ExecuteFillJsonByRoleArrorID
        /// <summary>
        /// 运行SQL语句、参数化的SQL语句或者存储过程，返回json格式的记录集。
        /// 字段名作为key，字段值作为value。
        /// </summary>
        /// <param name="text">查询语句或者存储过程的名称。
        /// 比如select * from tableName1 
        /// 或者 Proc_xxxGetDataTable
        /// </param>
        /// <returns></returns>
        public virtual string ExecuteFillJsonByRoleArrorID(string text)
        {
            _dal.SetCommand(text);		//设置command

            var sb = new StringBuilder(2000);

            try
            {
                _dal.ConnectionOpen();
                DbDataReader reader = _dal.Command.ExecuteReader(CommandBehavior.CloseConnection);
                var keys = new StringBuilder(1000);
                sb.Append("\"");
                sb.Append(_jsonName);
                sb.Append("keys\":[");

                while (reader.Read())
                {
                    sb.Append(reader[0]);
                    sb.Append(",");
                }

                if (sb.Length > _jsonName.Length + 6)
                {
                    //把最后一个 , 号 换成 ] 
                    sb[keys.Length - 1] = ']';
                }
                else
                {
                    sb.Append("]");
                }

                return sb.ToString();

            }
            catch (Exception ex)
            {
                _dal.SetError("ExecuteFillJsonByRoleArrorID", text, ex.Message + "<BR>ex:" + ex, _dal.ConnectionString);	//处理错误
                return null;
            }
            finally
            {
                //返回DataReader，不能关闭连接，需要调用者手动关闭连接
                if (!_dal.IsUseTrans) //判断是否使用了事务，没有使用事务的情况下，才可以关闭连接
                    _dal.Command.Connection.Close();

            }

        }
        #endregion

    }
}
