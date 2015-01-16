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
 * Nature.DataAccessLibrary is free software. You are allowed to download, modify and distribute 
 * the source code in accordance with LGPL 2.1 license, however if you want to use 
 * Nature.DataAccessLibrary on your site or include it in your commercial software, you must  be registered.
 * http://www.natureFW.com/registered
 * Nature.DataAccessLibrary:自然框架之数据访问类库
 */

/* ***********************************************
 * author :  金洋（金色海洋jyk）
 * email  :  jyk0011@live.cn  
 * function: 数据访问类库的主体部分
 * history:  created by 金洋 
 * **********************************************
 */

using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text;
using System.Web.Configuration;
using Nature.Common;
using Nature.Data.Part;

namespace Nature.Data
{
    /// <summary>
    /// 数据访问函数库的主体部分。属性、初始化、析构、内部共用函数。
    /// </summary>
    public abstract partial class DataAccessLibrary : IDisposable
    {
        #region 属性 
        //下划线开始的是类的内部成员，小写字母开头的是函数内部成员
        /// <summary>
        /// 一个DbCommand， 数据访问类库的核心
        /// </summary>
        public DbCommand Command { get; set; }

        #region 设置连接字符串
        /// <summary>
        /// 当不使用web.config里面设置的连接字符串的时候使用
        /// </summary>
        private string _connectionString = string.Empty;
        /// <summary>
        /// 获取或者修改连接字符串
        /// </summary>
        public string ConnectionString
        {
            set { _connectionString = value; }
            get
            {
                if (_connectionString == string.Empty)      //如果没有设置连接字符串,那么使用默认的连接字符串
                    _connectionString = CommonFactory.ConnectionString ;

                return _connectionString;
            }
        }
        #endregion

        #region 设置连接数据库的驱动名称
        private string _providerName = string.Empty;
        /// <summary>
        /// 读取或者设置连接数据库的驱动名称
        /// </summary>
        public string ProviderName
        {
            set
            {
                //设置了连接数据库的驱动名称，表示不使用Webconfig里面的默认设置，所以要根据这个单独设立一个“工厂”
                _providerName = value;
                _dbProvider = DbProviderFactories.GetFactory(value);
            }
            get
            {
                if (_providerName == string.Empty)
                    //没有设置单独的名称，使用默认的名称
                    _providerName = CommonFactory.ProviderName;
                
                return _providerName;
            }
        }
        #endregion

        #region 设置数据库类型
        private DataType _dataType = DataType.Default ;
        /// <summary>
        /// 读取或者设置连接数据库的驱动名称
        /// </summary>
        public DataType DataType
        {
            set
            {
                //设置了连接数据库的驱动名称，表示不使用Webconfig里面的默认设置，所以要根据这个单独设立一个“工厂”
                _dataType =  value;
            }
            get
            {
                string tmpType = "1" ;     

                if (WebConfigurationManager.AppSettings["DataBaseType"] != null)
                {
                    tmpType = WebConfigurationManager.AppSettings["DataBaseType"];
                    if (!Functions.IsInt(tmpType))
                    {
                        tmpType = "1";
                    }
                }

                if (_dataType == DataType.Default)
                    _dataType = (DataType) (Int32.Parse(tmpType));

                return _dataType;
            }
        }
        #endregion

        #region DbProvider。
        /// <summary>
        /// 如果不使用Webconfig里面的数据库驱动的话，需要使用这个。
        /// </summary>
        private DbProviderFactory _dbProvider;
        /// <summary>
        /// 获取DbProvider
        /// </summary>
        public DbProviderFactory DatabaseProvider
        {
            get { return _dbProvider; }
        }
        #endregion

        #region ErrorMessage。记录出错信息，只读。
        /// <summary>
        /// 出现异常时，记录出错信息，包括访问的网页、SQL语句、错误原因、访问时间等
        /// 对于外部（调用者）来说是只读的，而对于内部或者子类来说是可以写入的，所以protected
        /// </summary>
        internal string _errorMessage;

        /// <summary>
        /// 获取出错信息，没有错误的话返回string.Empty 
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
        }
        #endregion

        #region ExecuteRowCount。记录执行sql影响的行数
        /// <summary>
        /// 获取执行SQL查询语句后影响的行数。select语句无效。
        /// 对于外部（调用者）来说是只读的，而对于内部或者子类来说是可以写入的，所以protected
        /// </summary>
        protected int _executeRowCount;
        /// <summary>
        /// 获取执行SQL语句影响的行数，select语句无效。
        /// </summary>
        public int ExecuteRowCount
        {
            get { return _executeRowCount; }
        }
        #endregion

        #region IsUseTrans。标记是否已经启用了ado.net 事务

        /// <summary>
        /// 标记是否已经启用了ado.net 事务
        /// </summary>
        public bool IsUseTrans { get; set; }

        #endregion

        //加载辅助管理的实例

        #region 加载存储过程参数的管理部分
        private ManagerTran _managerTran;
        /// <summary>
        /// 加载处理存储过程参数的实例。用这个实例来添加存储过程的参数
        /// </summary>
        public ManagerTran ManagerTran
        {
            get { return _managerTran ?? (_managerTran = new ManagerTran(this)); }
            set { _managerTran = value; }
        }
        #endregion

        #region 加载返回记录集的部分
        private SelectData _selectData;
        /// <summary>
        /// 返回记录集的部分，返回DataTable，string[]等
        /// </summary>
        public SelectData SelectData
        {
            get { return _selectData ?? (_selectData = new SelectData(this)); }
            set { _selectData = value; }
        }
        #endregion

        #region 加载返回Json形式记录集的管理部分
        private ManagerJson _managerJson;
        /// <summary>
        /// 加载 返回Json形式记录集 的实例。用这个实例来返回Json形式记录集
        /// </summary>
        public ManagerJson ManagerJson
        {
            get { return _managerJson ?? (_managerJson = new ManagerJson(this)); }
            set { _managerJson = value; }
        }
        #endregion

        #region 加载固定实体类（WebList）的管理部分
        private ManagerWebModel _managerWebModel;
        /// <summary>
        /// 加载页面列表实体类（WebList）的管理部分。用这个实例来填充页面列表实体类（WebList）。
        /// </summary>
        public ManagerWebModel ManagerWebModel
        {
            get { return _managerWebModel ?? (_managerWebModel = new ManagerWebModel(this)); }
            set { _managerWebModel = value; }
        }
        #endregion

        #region 加载存储过程参数的管理部分
        private ManagerParameter _managerParameter;
        /// <summary>
        /// 加载处理存储过程参数管理的实例。用这个实例来添加存储过程的参数
        /// </summary>
        public ManagerParameter ManagerParameter
        {
            get { return _managerParameter ?? (_managerParameter = new ManagerParameter(this)); }
            set { _managerParameter = value; }
        }
        #endregion

        #region 加载添加、修改的管理部分
        private ModifyData _modifyData;
        /// <summary>
        /// 处理添加、修改的管理的实例。用这个实例来实现添加、修改数据。
        /// </summary>
        public ModifyData ModifyData
        {
            get { return _modifyData ?? (_modifyData = new ModifyData(this)); }
            set { _modifyData = value; }
        }
        #endregion


        //自动保存数据的修改记录的属性
        #region 设置是否保存数据的修改记录
        private bool? _isLogData ;
        /// <summary>
        /// 读取或者设置连接数据库的驱动名称
        /// </summary>
        public bool IsLogData
        {
            get
            {
                if (_isLogData != null)
                    return (bool)_isLogData;

                string tmpType = "1";

                if (WebConfigurationManager.AppSettings["IsLogData"] != null)
                    tmpType = WebConfigurationManager.AppSettings["IsLogData"];

                _isLogData = (tmpType == "1");

                return (bool)_isLogData;
            }
        }
        #endregion
        #endregion
        
        //===============================

        #region 无参初始化
        /// <summary>
        /// 初始化。使用web.config里的配置。
        /// </summary>
        protected DataAccessLibrary()
        {
            //在创建实例的时候会自动设置默认的连接字符串
            //信息来至web.config文件
            _dbProvider = CommonFactory.DbProvider;   //使用默认的工厂
            Command = CommonFactory.CreateCommand(ConnectionString, _dbProvider);
        }
        #endregion

        #region 有参初始化
        /// <summary>
        /// 不使用默认的数据库类型的时候使用
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="providerName">数据库驱动的名称。System.Data.SqlClient、System.Data.OleDb 、System.Data.Odbc、System.Data.OracleClient等</param>
        protected DataAccessLibrary(string connectionString, string providerName)
        {
            //修改数据库类型，然后才能创建实例
            ConnectionString = connectionString;
            ProviderName = providerName;       //在这里同时设置_DbProvider。

            Command = CommonFactory.CreateCommand(ConnectionString, _dbProvider);
        }
        #endregion

        #region 释放资源Dispose
        /// <summary>
        /// 释放资源。据说如果没有非托管的资源，是不用写析构函数的。所以，是不是不需要这个函数？
        /// </summary>
        public void Dispose()
        {
            if (_managerWebModel != null)
                _managerWebModel = null;
            if (_managerTran != null)
                _managerTran = null;
            if (_managerParameter != null)
                _managerParameter = null;

            _errorMessage = null;

            Command.Parameters.Clear();

            /* 有的时候close会报错，所以就注释掉了。
             * 内部 .net framework 数据提供程序错误 1。
            if (Command.Connection != null)
                if (Command.Connection.State != ConnectionState.Closed)
                    Command.Connection.Close();
            */
            Command.Dispose();

        }

        #endregion

        #region 基础的函数

        #region 设置Command的CommandText和CommandType
        /// <summary>
        /// 设置Command的CommandText和CommandType
        /// </summary>
        /// <param name="commandText"></param>
        internal void SetCommand(string commandText )
        {
            _errorMessage = string.Empty;      //清空出错信息
            _executeRowCount = 0;              //重置影响的行数

            //这种处理方式不太理想，但是目前还没有找到更好的方式
            string state = commandText.Substring(0, 2).ToLower();
            switch (state)
            {
                case "p_":
                    Command.CommandType = CommandType.StoredProcedure;
                    break;
                default:
                    state = commandText.Substring(0, 3).ToLower();

                    switch (state)
                    {
                        case "pro":
                        case "sp_":
                        case "dt_":
                        case "ms_":
                        case "xp_":
                            Command.CommandType = CommandType.StoredProcedure;
                            break;

                        default:
                            Command.CommandType = CommandType.Text;
                            break ;
                    }
                    break;
            }

            Command.CommandText = commandText;
        }
        #endregion

        #region 打开连接
        /// <summary>
        /// 在使用DataReader或者开始事务的时候，需要手动打开连接。
        /// 注意：DataReader使用完毕之后必须手动关闭连接。
        /// </summary>
        public void ConnectionOpen()
        {
            if (Command.Connection.State == ConnectionState.Broken || Command.Connection.State == ConnectionState.Closed)
                Command.Connection.Open();
        }
        #endregion

        #region 关闭连接
        /// <summary>
        /// 在未开启事务的情况下，关闭连接。
        /// 注意：DataReader使用完毕之后必须手动关闭连接。
        /// </summary>
        public void ConnectionClose()
        {
            if (!IsUseTrans)            
                //判断是否使用了事务，没有使用事务的情况下，才可以关闭连接
                Command.Connection.Close();
        }
        #endregion

        #region 保存出错信息
        //设置出错信息
        /// <summary>
        /// 当发生异常时，所做的处理。
        /// </summary>
        /// <param name="functionName">函数名称</param>
        /// <param name="commandText">查询语句或者存储过程</param>
        /// <param name="message">错误信息</param>
        /// <param name="cn">连接字符串</param>
        public void SetError(string functionName, string commandText, string message,string cn)
        {
            //设置返回给调用者的错误信息
            _errorMessage = functionName + "函数出现异常。访问者IP："+ System.Web.HttpContext.Current.Request.UserHostAddress  +"\r\n错误信息：" + message
                + "\r\n查询语句：" + commandText;

            if (message.Substring(0, 2) == "在与")
            {
                string[] cns = cn.Split(';');
                if (cns.Length >=2)
                    _errorMessage += "\r\ncn：" + cns[0] + ";" + cns[1];
            }
            if (IsUseTrans)
                _managerTran.TranRollBack();			    //事务模式下：自动回滚事务，不用调用者回滚

            IsUseTrans = false;

            Command.Connection.Close();		//关闭连接
            AddLogError(_errorMessage);	    //记录到错误日志
        }

        #endregion 

        #region 记录错误日志
        /// <summary>
        /// 把错误描述信息记录在文本文件里。
        /// </summary>
        /// <param name="errorDescribe">描述信息，即保存到日志文件里的信息</param>
        public static void AddLogError(string errorDescribe)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath("~/log/");

            //判断是否建立了log文件夹
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            
            //记录到错误日志
            string filePath = path + "/" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            var str = new StringBuilder();

            str.Append(DateTime.Now.ToString(CultureInfo.InvariantCulture));    //访问的时间
            str.Append("\t");
            str.Append(System.Web.HttpContext.Current.Request.Url.PathAndQuery); //访问的网页和URL参数
            str.Append("\r\n");
            str.Append(errorDescribe);              //填写正文
            str.Append("\r\n\r\n");

            System.IO.StreamWriter sw = null;
            try
            {
                sw = new System.IO.StreamWriter(filePath, true, Encoding.Unicode);
                sw.Write(str.ToString());
            }
            catch (Exception ex)
            {
                Functions.MsgBox("没有访问日志文件的权限！或日志文件只读!" + ex.Message,false );
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }
        }
        #endregion

        #endregion

    }
}
