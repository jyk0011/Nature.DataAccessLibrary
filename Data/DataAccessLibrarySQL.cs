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
 * function: 数据访问类库的主体部分，实现基本操作。
 *           传递SQL语句、存储过程、存储过程的参数。接收记录集存放在指定的容器里。
 * history:  created by 金洋 
 *           2010.08.27 增加 ExecuteScalar<T> 
 * **********************************************
 */

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using Nature.Data.Model;

namespace Nature.Data
{
    /// <summary>
    /// 数据访问函数库，基础功能部分
    /// </summary>
    public partial class DataAccessLibrary : IDal
    {

        #region 钩子
        #region 设置存储过程的参数名称的前缀
        /// <summary>
        /// 设置存储过程的参数名称的前缀。
        /// </summary>
        /// <returns></returns>
        public virtual string ParameterPrefix()
        {
            return "@";
        }
        #endregion

        #region 设置字段名左面的符号
        /// <summary>
        /// 设置字段名左面的符号。
        /// </summary>
        /// <returns></returns>
        public virtual string ColumnLeft()
        {
            return "[";
        }
        #endregion

        #region 设置字段名右面的符号
        /// <summary>
        /// 设置字段名右面的符号。
        /// </summary>
        /// <returns></returns>
        public virtual string ColumnRight()
        {
            return "]";
        }
        #endregion

        #endregion

        //返回记录集部分
        #region ExecuteReader
        /// <summary>
        /// 运行SQL语句、参数化的SQL语句或者存储过程，返回ExecuteReader。使用完毕后，需要手动关闭连接。
        /// </summary>
        /// <param name="text">查询语句或者存储过程的名称。
        /// 比如select * from tableName1 
        /// 或者 Proc_xxxGetDataSet
        /// </param>
        /// <returns></returns>
        public virtual DbDataReader ExecuteReader(string text)
        {
            SetCommand(text);		//设置command
            
            try
            {
                ConnectionOpen();
                DbDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }
            catch (Exception ex)
            {
                SetError("ExecuteReader", text, ex.Message, _connectionString );	//处理错误
                return null;
            }
            //finally
            //{
                //返回DataReader，不能关闭连接，需要调用者手动关闭连接
                //if (!IsUseTrans)            //判断是否使用了事务，没有使用事务的情况下，才可以关闭连接
                //    Command.Connection.Close();

            //}

        }
        #endregion

        #region ExecuteString
        /// <summary>
        /// 运行SQl语句返回第一条记录的第一个字段的值。
        /// </summary>
        /// <param name="text">查询语句。
        /// 比如select top 1 userName from tableName where ID=3。返回userName字段的内容。
        /// 适合字符串类型的字段</param>
        /// <returns></returns>
        public virtual string ExecuteString(string text)
        {
            return ExecuteString(text, "ExecuteString");
        }

        /// <summary>
        /// 运行SQl语句返回第一条记录的第一个字段的值。区分内部函数调用的情况
        /// </summary>
        /// <param name="text">SQL语句，或者参数化的SQL语句</param>
        /// <param name="functionName">函数名称。日志里需要这个名称，用于调试</param>
        /// <returns></returns>
        internal virtual string ExecuteString(string text, string functionName)
        {
            SetCommand(text);		//设置command
            DbDataReader r = null;
            try
            {
                ConnectionOpen();
                r = Command.ExecuteReader(CommandBehavior.SingleRow);
                string re = r.Read() ? r.GetValue(0).ToString() : null;

                return re;

            }
            catch (Exception ex)
            {
                SetError(functionName, text, ex.Message,_connectionString);	//处理错误
                return null;
            }
            finally
            {
                if (r != null) r.Close();

                if (!IsUseTrans)        //判断是否使用了事务，没有使用事务的情况下，才可以关闭连接
                    Command.Connection.Close();
            }
        }
        #endregion

        #region ExecuteScalar<T> 2010.08.27 增加的一个函数
        /// <summary>
        /// 运行SQl语句返回第一条记录的第一个字段的值。采用泛型作为返回值类型。
        /// </summary>
        /// <param name="text">SQL语句，或者参数化的SQL语句</param>
        /// <returns></returns>
        public virtual T ExecuteScalar<T>(string text)
        {
            SetCommand(text);		//设置command
            try
            {
                ConnectionOpen();
                var re = (T)Convert.ChangeType(Command.ExecuteScalar(), typeof(T));
                return re;

            }
            catch (Exception ex)
            {
                SetError("ExecuteScalar<T>", text, ex.Message,_connectionString);	//处理错误
                return default(T);
            }
            finally
            {
                if (!IsUseTrans)        //判断是否使用了事务，没有使用事务的情况下，才可以关闭连接
                    Command.Connection.Close();
            }
        }
        #endregion

        //不返回记录集的
        //运行查询语句不返回记录集（无返回记录、检查持否存在指定的记录）
        #region ExecuteNonQuery 
        /// <summary>
        /// 运行SQL查询语句，不返回记录集。用于添加、修改、删除等操作
        /// </summary>
        /// <param name="sql">查询语句。比如insert into tableName 、update tableName...</param>
        /// <returns></returns>
        public virtual void ExecuteNonQuery(string sql)
        {
            ExecuteNonQuery(sql, "ExecuteNonQuery" );
        }

        /// <summary>
        /// 运行SQL语句，不返回记录集。用于添加、修改、删除等操作。 
        /// </summary>
        /// <param name="sql">查询语句。比如insert into tableName 、update tableName...、delete from 等</param>
        /// <param name="functionName">函数名称，需要知道确切的调用的函数名称</param>
        /// <returns></returns>
        internal virtual void ExecuteNonQuery(string sql, string functionName )
        {
            SetCommand(sql);		//设置command
             
            try
            {
                ConnectionOpen();
                _executeRowCount = Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                SetError(functionName, sql, ex.Message,_connectionString );	//处理错误
            }
            finally
            {
                if (!IsUseTrans)            //判断是否使用了事务，没有使用事务的情况下，才可以关闭连接
                    Command.Connection.Close();
            }
        }
        #endregion

        #region ExecuteExists
        /// <summary>
        /// 执行一条SQL语句，看是否能查到记录 有：返回true；没有返回false，用于判断是否重名
        /// </summary>
        /// <param name="sql">查询语句。比如select ID from tableName where userName='aa'</param>
        /// <returns></returns>
        public virtual bool ExecuteExists(string sql)
        {
            const string tmpSql = " select 1 where (exists({0}))";

            SetCommand(string.Format(tmpSql, sql));		//设置command

            DbDataReader r = null;
            try
            {
                ConnectionOpen();
                
                r = Command.ExecuteReader();
                bool re = r.HasRows;

                return re;
            }
            catch (Exception ex)
            {
                SetError("ExecuteExists", sql, ex.Message, _connectionString );	//处理错误
                return true;
            }
            finally
            {
                if (r != null) r.Close();

                if (!IsUseTrans)        //判断是否使用了事务，没有使用事务的情况下，才可以关闭连接
                    Command.Connection.Close();
            }
        }
        #endregion
       
        #region 操作日志的功能。
        /*
        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="EmplID">登录人ID ，即员工ID</param>
        /// <param name="DeptID">部门ID</param>
        /// <param name="FunctionID">操作的节点的ID</param>
        /// <param name="TableName">操作的表或者视图</param>
        /// <param name="DataID">操作的数据的ID</param>
        /// <param name="isTrue">操作是否成功</param>
        /// <param name="Content">说明</param>
        /// <param name="Kind">操作类型。1：添加；2：修改；3：删除</param>
        /// <returns>返回日志的ID</returns>
        public string AddSystemLog(string EmplID, string DeptID, string FunctionID, string TableName, string DataID, string isTrue, string Content, string Kind)
        {
            string[] str1 = new string[9];
            str1[0] = "EmplID";			//登录人ID
            str1[1] = "DeptID";			//部门ID
            str1[2] = "FunctionID";		//操作的节点的ID
            str1[3] = "TableName";		//操作的表或者视图
            str1[4] = "DataID";			//操作的数据的ID
            str1[5] = "isTrue";			//操作是否成功
            str1[6] = "Content";		//说明
            str1[7] = "Kind";			//操作类型。1：添加；2：修改；3：删除
            str1[8] = "IP";				//登录人的IP

            string[] str = new string[9];
            str[0] = EmplID;			//登录人ID
            str[1] = DeptID;			//部门ID
            str[2] = FunctionID;		//操作的节点的ID
            str[3] = TableName;			//操作的表或者视图
            str[4] = DataID;			//操作的数据的ID
            str[5] = isTrue;			//操作是否成功
            str[6] = Content;			//说明
            str[7] = Kind;				//操作类型。1：添加；2：修改；3：删除
            str[8] = System.Web.HttpContext.Current.Request.UserHostAddress;		//登录人的IP

            return this.InsertData("Base_SysLog", str1, str);
        }
         */
        #endregion

        #region 实现接口
        /// <summary>
        /// 运行SQl语句返回 实体类WebList2集合
        /// </summary>
        /// <param name="sql">查询语句。比如select myName from tableName</param>
        /// <param name="lstFormat">标题的最大字符数、内容简介的最大字符数，一个汉字按照两个字符计算。传入“0”则表示不截取标题。发表时间的格式化。</param>
        /// <returns>返回WebList2结构的集合。URL，标题，时间，人气，图片名</returns>
        public IList<WebList2> ExecuteFillWebList2(string sql, WebList2Format lstFormat)
        {
            return ManagerWebModel.ExecuteFillWebList2(sql, lstFormat);
        }
        

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
            return SelectData.ExecuteFillDataSet(text);
        }

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
            return SelectData.ExecuteFillDataTable(text);
        }

        /// <summary>
        /// 运行SQl语句返回每一条记录的第一个字段的值。返回字符串数组
        /// </summary>
        /// <param name="text">查询语句。比如select myName from tableName</param>
        /// <returns></returns>
        public virtual string[] ExecuteStringsByColumns(string text)
        {
            return SelectData.ExecuteStringsByColumns(text);
        }

        #endregion

        #region 向下兼容 便于调用
        /// <summary>
        /// 运行SQl语句返回第一条记录的数组。返回字符串数组
        /// </summary>
        /// <param name="text">查询语句。比如select top 1 * from tableName</param>
        /// <returns></returns>
        public virtual string[] ExecuteStringsBySingleRow(string text)
        {
            return SelectData.ExecuteStringsBySingleRow(text);
        }

        #endregion

    }
}
