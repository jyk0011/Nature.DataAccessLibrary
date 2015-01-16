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
 * function: 创建DbConnection、DbCommand、DbDataAdapter、DbParameter实例的工厂
 *           封装一下，我可以更换适合的生成方式，或者是直接生成（不换数据库的情况下）。
 * history:  created by 金洋 
 * **********************************************
 */


using System.Configuration;
using System.Data.Common;


namespace Nature.Data
{
    /// <summary>
    /// 数据库的类型，对应 DataBaseType 
    /// </summary>
    /// user:jyk
    /// time:2013/1/12 11:05
    public enum DataType
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default = 0,

        /// <summary>
        /// Server SQL 2000 
        /// </summary>
        MsSql2000 = 1,

        /// <summary>
        /// Server SQL 2005+  
        /// </summary>
        MsSql2005 = 11,

        /// <summary>
        /// Access 各个版本 
        /// </summary>
        Access = 2


    }
    /// <summary>
    /// 创建DbConnection、DbCommand、DbDataAdapter、DbParameter实例的工厂
    /// </summary>
    public static class CommonFactory
    {
        #region 定义成员
        /// <summary>
        /// 默认的连接字符串。从webconfig获取。
        /// </summary>
        public static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        
        /// <summary>
        /// 默认的数据库驱动的名称。从webconfig里面的获取。  
        /// System.Data.SqlClient   可以连接 SQL Sever 2000 、SQL Server 2005等
        /// System.Data.OleDb       可以连接 Access、Excel等
        /// System.Data.Odbc        可以连接 Access、Excel等
        /// </summary>
        public static readonly string ProviderName = ConfigurationManager.ConnectionStrings["ConnectionString"].ProviderName;    //

        /// <summary>
        /// 数据库的类型  
        /// System.Data.SqlClient   可以连接 SQL Sever 2000 、SQL Server 2005等
        /// System.Data.OleDb       可以连接 Access、Excel等
        /// System.Data.Odbc        可以连接 Access、Excel等
        /// </summary>
        public static readonly DataType DataType = DataType.MsSql2000;// (DataType)int.Parse(ConfigurationManager.ConnectionStrings["DataBaseType"].ProviderName);    //

        /// <summary>
        /// 生成Connection 等的实例的工厂，ado.net2.0提供
        /// </summary>
        public static readonly  DbProviderFactory DbProvider = DbProviderFactories.GetFactory(ProviderName);

        #endregion

        ///// <summary>
        ///// 静态初始化，进行默认设置
        ///// </summary>
        //static CommonFactory()
        //{
        //}

        #region 创建实例
        #region Connection
        /// <summary>
        /// 根据数据库的连接方式创建一个Connection的实例，需要传递连接字符串。
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="dbProvider">DbProviderFactory</param>
        /// <returns></returns>
        public static DbConnection CreateConnection(string connectionString, DbProviderFactory dbProvider)
        {
            DbConnection cn = dbProvider.CreateConnection();
            cn.ConnectionString = connectionString;       //使用传递过来的连接字符串
            return cn;
             
        }
        #endregion

        #region Command
        /// <summary>
        /// 根据数据库的连接方式创建一个Command的实例，需要传递连接字符串。
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="dbProvider">DbProviderFactory</param>
        /// <returns></returns>
        public static DbCommand CreateCommand(string connectionString, DbProviderFactory dbProvider)
        {
            DbCommand cm = dbProvider.CreateCommand();
            cm.Connection = CreateConnection(connectionString, dbProvider);       //使用指定的连接字符串
            return cm;
        }
        #endregion

        #region DataAdapter
        /// <summary>
        /// 根据数据库的连接方式创建一个DataAdapter的实例，这个只生成实例，不做其他的设置了。
        /// </summary>
        /// <param name="cm">DbCommand</param>
        /// <param name="dbProvider">DbProviderFactory</param>
        /// <returns></returns>
        public static DbDataAdapter CreateDataAdapter(DbCommand cm, DbProviderFactory dbProvider)
        {
            DbDataAdapter da = dbProvider.CreateDataAdapter();
            da.SelectCommand = cm;
            return da; 
        }
        #endregion

        #region Parameter
        /// <summary>
        /// 根据数据库的连接方式创建一个Parameter的实例。
        /// </summary>
        /// <param name="parameterName">存储过程的参数的名称</param>
        /// <param name="dbProvider">DbProviderFactory</param>
        /// <returns></returns>
        public static DbParameter CreateParameter(string parameterName, DbProviderFactory dbProvider)
        {
            DbParameter parameter = dbProvider.CreateParameter();
            parameter.ParameterName = parameterName;
            return parameter;
        }
        #endregion
        #endregion
	
    }
}
