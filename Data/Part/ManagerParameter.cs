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
 * function: 数据访问类库的配件部分，对存储过程参数的封装。 
 * history:  created by 金洋 
 * **********************************************
 */

using System;
using System.Data;
using System.Data.Common;

namespace Nature.Data.Part
{
    /// <summary>
    /// 处理存储过程的参数
    /// </summary>
    public class ManagerParameter 
    {
        /// <summary>
        /// 数据访问函数库的实例。
        /// </summary>
        private readonly DataAccessLibrary _dal;

        /// <summary>
        /// 初始化，设置数据访问实例，把主体和扩展关联起来。
        /// </summary>
        /// <param name="dal">数据访问函数库的实例</param>
        public ManagerParameter(DataAccessLibrary dal)
        {
            _dal = dal;
        }

        //int：		tinyint、smallint
        //bigint：
        //bool：	bit
        //double：	float、real
        //string：	char、nchar、varchar、nvarchar、uniqueidentifier、smalldatetime、datetime
        //string：	ntext、text

        //decimal：从 -10^38 +1 到 10^38 –1 的固定精度和小数位的数字数据。
        //numeric：功能上等同于 decimal。
        //decimal：	smallmoney、money

        //二进制
        //			binary、varbinary、image

        #region 索引器
        /// <summary>
        /// 对存储过程的参数的一种封装
        /// </summary>
        /// <param name="parameterName">存储过程的参数名称。比如 UserName。注意：参数名称不需要加前缀！</param>
        /// <returns></returns>
        public DbParameter this[string parameterName]
        {
            get { return _dal.Command.Parameters[ _dal.ParameterPrefix() + parameterName]; }
            set { _dal.Command.Parameters[ _dal.ParameterPrefix() + parameterName] = value; }
        }

        /// <summary>
        /// 对存储过程的参数的一种封装
        /// </summary>
        /// <param name="parameterIndex">存储过程的参数的序号</param>
        /// <returns></returns>
        public  DbParameter this[int parameterIndex]
        {
            get { return _dal.Command.Parameters[parameterIndex]; }
            set { _dal.Command.Parameters[parameterIndex] = value; }
        }
        #endregion

        #region 参数的数量
        /// <summary>
        /// 返回参数的数量
        /// </summary>
        /// 2013-5-3 添加by金洋
        public int Count
        {
            get { return _dal.Command.Parameters.Count; }
        }
        #endregion

        #region 清除参数
        /// <summary>
        /// 清除Command的存储过程的参数。
        /// </summary>
        public virtual void ClearParameter()
        {
            _dal.Command.Parameters.Clear();
        }
        #endregion

        #region //以前的，不用了 输入型的参数 int 、double、decimal、nvarChar、、、
        #region image
        ///// <summary>
        ///// 添加二进制数组型的参数。方向是输入（input）
        ///// </summary>
        ///// <param name="ParameterName">参数名称。比如 @UserName</param>
        ///// <param name="ParameterValue">参数值</param>
        //public virtual void AddNewInParameter(string ParameterName, Byte[] ParameterValue)
        //{
        //    DbParameter par = DataBaseFactory.CreateParameter(ParameterName,this.dal.ProviderName );
        //    ((System.Data.SqlClient.SqlParameter)par).SqlDbType = SqlDbType.Image;
        //    par.Value = ParameterValue;

        //    //添加存储过程的参数
        //    dal.myCommand.Parameters.Add(par);
        //}
        #endregion 
    
        #endregion

        #region 泛型的方式添加参数
        /*
        /// <summary>
        /// 泛型的方式添加参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameterName">存储过程的参数名称。比如 UserName。注意：参数名称不需要加前缀！</param>
        /// <param name="parameterValue">存储过程的参数值。</param>
        public virtual void AddNewInParameter<T>(string parameterName, T parameterValue)
        {
            DbParameter par = CommonFactory.CreateParameter(this.dal.ParameterPrefix() + parameterName, this.dal.DatabaseProvider);
            par.Direction = ParameterDirection.Input ;

            switch (typeof(T).ToString())
            {
                case "Int64":
                    //1
                    par.DbType = DbType.Int64; break;

                case "Int32":
                    //2
                    par.DbType = DbType.Int32; break;

                case "String":               //ntext、text类型的，不能指定长度
                    //4
                    par.DbType = DbType.String; break;

                case "DateTime":

                    //日期和时间
                    par.DbType = DbType.DateTime; break;

                case "Decimal":
                    //添加金额参数
                    par.DbType = DbType.Decimal; break;



                case "Boolean":
                    par.DbType = DbType.Boolean; break;

                case "Double":               //小数
                    par.DbType = DbType.Single; break;

                case "Single":
                    par.DbType = DbType.Single; break;

                case "Byte[]":                //SQL Server 里面的Image类型
                    ((System.Data.SqlClient.SqlParameter)par).SqlDbType = SqlDbType.Image; break;

            
            }

            par.Value = parameterValue;

            //添加存储过程的参数
            dal.Command.Parameters.Add(par);
         * 

        }*/
        #endregion

        #region 输入型的参数
        /// <summary>
        /// 添加输入型的参数，不用指定参数大小的情况。只支持常用的几个参数类型，如果需要可以扩充。
        /// </summary>
        /// <param name="parameterName">参数名称。比如 UserName。注意：参数名称不需要加前缀！</param>
        /// <param name="parameterValue">参数值</param>
        public virtual void AddNewInParameter(string parameterName, object parameterValue)
        {
            DbParameter par = CommonFactory.CreateParameter( _dal.ParameterPrefix() + parameterName, _dal.DatabaseProvider);
            par.Direction = ParameterDirection.Input;

             
            if (parameterValue is Int64)
                par.DbType = DbType.Int64;

            else if (parameterValue is Int32)
                par.DbType = DbType.Int32;

            else if (parameterValue is DateTime)             //日期和时间
                par.DbType = DbType.DateTime;


            else if (parameterValue is Decimal)              //添加金额参数
                par.DbType = DbType.Decimal;
               
            else if (parameterValue is String)               //ntext、text类型的，不能指定长度
                par.DbType = DbType.String;

            else if (parameterValue is Boolean)
                par.DbType = DbType.Boolean;

            else if (parameterValue is Double)              //小数
                par.DbType = DbType.Single;
            else if (parameterValue is Single)
                par.DbType = DbType.Single;

            else if (parameterValue is Byte[])               //SQL Server 里面的Image类型
                ((System.Data.SqlClient.SqlParameter)par).SqlDbType = SqlDbType.Image;

            else
                par.DbType = DbType.String;                 //不在上面的判断范围内的，都定义成string的类型

            par.Value = parameterValue;

            //添加存储过程的参数
            _dal.Command.Parameters.Add(par);

        }

        /// <summary>
        /// 添加输入型的参数，nvarchar、carchar、nchar、cahr需要设置参数大小的情况。
        /// </summary>
        /// <param name="parameterName">参数名称。比如 UserName。注意：参数名称不需要加前缀！</param>
        /// <param name="parameterValue">参数值</param>
        /// <param name="parameterSize">参数的大小</param>
        public virtual void AddNewInParameter(string parameterName, object parameterValue, int parameterSize)
        {
            DbParameter parameter = CommonFactory.CreateParameter( _dal.ParameterPrefix() + parameterName,  _dal.DatabaseProvider);
            parameter.Direction = ParameterDirection.Input;

            parameter.DbType = DbType.String;
            parameter.Size = parameterSize;
            parameter.Value = parameterValue;

            //添加存储过程的参数
            _dal.Command.Parameters.Add(parameter);

        }

        #endregion

        #region 输出型的参数
        /// <summary>
        /// 添加输出型的参数。只支持常用的几个参数类型，如果需要可以扩充。
        /// </summary>
        /// <param name="parameterName">参数名称。比如 UserName。注意：参数名称不需要加前缀！</param>
        /// <param name="dbType">参数的类型</param>
        public virtual void AddNewOutParameter(string parameterName, DbType dbType)
        {
            DbParameter parameters = CommonFactory.CreateParameter( _dal.ParameterPrefix() + parameterName,  _dal.DatabaseProvider);
            parameters.Direction = ParameterDirection.InputOutput;
            parameters.DbType = dbType;
            
            //添加存储过程的参数
            _dal.Command.Parameters.Add(parameters);
        
        }

        #endregion

        #region 存储过程的参数部分——取参数的返回值
        /// <summary>
        /// 按序号返回参数值，一般在执行完存储过程后使用
        /// </summary>
        /// <param name="parameterIndex">序号</param>
        /// <returns>返回参数的内容</returns>
        public T GetParameter<T>(int parameterIndex)
        {
            return (T)Convert.ChangeType(_dal.Command.Parameters[parameterIndex].Value, typeof(T));

            //return _dal.Command.Parameters[parameterIndex].Value.ToString();
        }

        /// <summary>
        /// 按名称返回参数值，一般在执行完存储过程后使用
        /// </summary>
        /// <param name="parameterName">参数名称。比如 UserName。注意：参数名称不需要加前缀（比如@）！</param>
        /// <returns>返回参数的内容</returns>
        public T GetParameter<T>(string parameterName)
        {
            return (T)Convert.ChangeType(_dal.Command.Parameters[_dal.ParameterPrefix() + parameterName].Value, typeof(T));
            //return _dal.Command.Parameters[_dal.ParameterPrefix() + parameterName].Value.ToString();
        }
        #endregion

        #region 存储过程的参数部分——修改参数值
        /// <summary>
        /// 按序号修改参数值，一般在一次添加多条记录时用。
        /// </summary>
        /// <param name="parameterIndex">序号</param>
        /// <param name="parameterValue">值</param>
        public void SetParameter<T>(int parameterIndex, T parameterValue)
        {
            _dal.Command.Parameters[parameterIndex].Value = parameterValue;
        }

        /// <summary>
        /// 按名称修改参数值，一般在一次添加多条记录时用
        /// </summary>
        /// <param name="parameterName">参数名称。比如 UserName。注意：参数名称不需要加前缀！</param>
        /// <param name="parameterValue">值</param>
        public void SetParameter<T>(string parameterName, T parameterValue)
        {
            _dal.Command.Parameters[ _dal.ParameterPrefix() + parameterName].Value = parameterValue;
        }
        #endregion

         
    } 
}
