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
 * function: 数据访问类库的配件部分，对返回记录集的封装。 
 * history:  created by 金洋 2011-03-28 15:00
 * **********************************************
 */

using System;
using System.Data;
using System.Data.Common;

namespace Nature.Data.Part
{
    /// <summary>
    /// 返回记录集的配件
    /// </summary>
    public class SelectData
    {
        /// <summary>
        /// 数据访问函数库的实例，主要是想操作Connection
        /// </summary>
        private readonly DataAccessLibrary _dal;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="dal">数据访问函数库的实例</param>
        public SelectData(DataAccessLibrary dal)
        {
            _dal = dal;
        }

        #region ExecuteFillDataSet
        /// <summary>
        /// 运行SQL语句、参数化的SQL语句或者存储过程，返回DataSet。
        /// 可以传入多条查询语句，返回的DataSet里会有多个DataTable
        /// </summary>
        /// <param name="text">查询语句或者存储过程的名称。
        /// 比如select * from tableName1 select * from tableName2
        /// 或者 Proc_xxxGetDataSet
        /// </param>
        /// <returns>返回DataSet</returns>
        public virtual DataSet ExecuteFillDataSet(string text)
        {
            //设置command
            _dal.SetCommand(text);

            //创建一个DataAdapter，用于填充数据
            DbDataAdapter da = CommonFactory.CreateDataAdapter(_dal.Command, _dal.DatabaseProvider);

            //关联DataAdapter 和 Command
            da.SelectCommand = _dal.Command;
            try
            {
                var ds = new DataSet();
                da.Fill(ds);                //打开数据库，填充数据
                return ds;
            }
            catch (Exception ex)
            {
                _dal.SetError("ExecuteFillDataSet", text, ex.Message,_dal.ConnectionString);	//处理错误
                return null;
            }
            finally
            {
                //自动关闭了，不用手动关闭。
                da.Dispose();
            }
        }
        #endregion

        #region ExecuteFillDataTable
        /// <summary>
        /// 运行SQL语句、参数化的SQL语句或者存储过程，返回DataTable记录集
        /// </summary>
        /// <param name="text">查询语句或者存储过程的名称。
        /// 比如select * from tableName1 
        /// 或者 Proc_xxxGetDataTable
        /// </param>
        /// <returns></returns>
        public virtual DataTable ExecuteFillDataTable(string text)
        {
            _dal.SetCommand(text);		//设置command
            DbDataAdapter da = CommonFactory.CreateDataAdapter(_dal.Command, _dal.DatabaseProvider);
            try
            {
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                _dal.SetError("ExecuteFillDataTable", text, ex.Message + "<BR>ex:" + ex,_dal.ConnectionString);	//处理错误
                return null;
            }
            finally
            {
                //自动关闭了，不用手动关闭。
                da.Dispose();
            }
        }
        #endregion

        #region ExecuteStringsBySingleRow
        /// <summary>
        /// 运行SQl语句返回第一条记录的数组。返回字符串数组
        /// </summary>
        /// <param name="text">查询语句。比如select top 1 * from tableName</param>
        /// <returns></returns>
        public virtual string[] ExecuteStringsBySingleRow(string text)
        {
            //返回ID 传入查询语句，返回第一条记录的数组
            _dal.SetCommand(text);		//设置command
            DbDataReader r = null;
            try
            {
                _dal.ConnectionOpen();
                r = _dal.Command.ExecuteReader();
                string[] strValue = null;
                if (r.Read())
                {
                    int arrLength = r.FieldCount;
                    strValue = new string[arrLength];
                    for (int i = 0; i < arrLength; i++)
                        strValue[i] = r.GetValue(i).ToString();
                }
                return strValue;
            }
            catch (Exception ex)
            {
                _dal.SetError("ExecuteStringsBySingleRow", text, ex.Message, _dal.ConnectionString);	//处理错误
                return null;
            }
            finally
            {
                if (r != null) r.Close();

                if (!_dal.IsUseTrans)    //判断是否使用了事务，没有使用事务的情况下，才可以关闭连接
                    _dal.Command.Connection.Close();
            }
        }
        #endregion

        #region ExecuteStringsByColumns
        /// <summary>
        /// 运行SQl语句返回每一条记录的第一个字段的值。返回字符串数组
        /// </summary>
        /// <param name="text">查询语句。比如select myName from tableName</param>
        /// <returns></returns>
        public virtual string[] ExecuteStringsByColumns(string text)
        {
            //传入查询语句，返回每条记录的第一的字段的值
            _dal.SetCommand(text);		//设置command
            DbDataReader r = null;
            try
            {
                _dal.ConnectionOpen();

                r = _dal.Command.ExecuteReader();
                //int i = 0;
                System.Collections.IList li = new System.Collections.ArrayList();
                while (r.Read())
                    li.Add(r[0].ToString());

                var strValue = new string[li.Count];
                li.CopyTo(strValue, 0);
                return strValue;

            }
            catch (Exception ex)
            {
                _dal.SetError("ExecuteStringsByColumns", text, ex.Message, _dal.ConnectionString);	//处理错误
                return null;
            }
            finally
            {
                if (r != null) r.Close();

                if (!_dal.IsUseTrans)            //判断是否使用了事务，没有使用事务的情况下，才可以关闭连接
                    _dal.Command.Connection.Close();

            }
        }
        #endregion


    }
}
