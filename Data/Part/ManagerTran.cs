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
 * function: 数据访问类库的配件部分，对 ADO.NET的事务的封装。 
 * history:  created by 金洋 2010-05-26 11:20
 * **********************************************
 */

using System.Data.Common;

namespace Nature.Data.Part
{
    /// <summary>
    /// 关于事务的封装
    /// </summary>
    public class ManagerTran
    {
        /// <summary>
        /// 数据访问函数库的实例，主要是想操作Connection
        /// </summary>
        private readonly DataAccessLibrary _dal;

        /// <summary>
        /// 用于事务处理
        /// </summary>
        private DbTransaction _dbTran;

        /// <summary>
        /// 初始化，设置内部的数据访问实例，管理主体和扩展。
        /// </summary>
        /// <param name="dal">数据访问函数库的实例</param>
        public ManagerTran(DataAccessLibrary dal)
        {
            _dal = dal;
        }

        //事务日志
        #region 事务处理部分。并没有做太多的测试，有不合理的地方请多指教
        /// <summary>
        /// 打开连接，并且开始事务。
        /// </summary>
        public void TranBegin()
        {
            _dal.Command.Connection.Open();		                    //打开连接，直到回滚事务或者提交事务。
            _dbTran = _dal.Command.Connection.BeginTransaction();	//开始一个事务
            _dal.Command.Transaction = _dbTran;	                    //交给Command
            _dal.IsUseTrans = true;			                        //标记为启用事务

        }
        #endregion

        #region 提交事务，并关闭连接
        /// <summary>
        /// 提交事务，并关闭连接
        /// </summary>
        public void TranCommit()
        {
            if (_dal.IsUseTrans)
            {
                //启用了事务
                _dbTran.Commit();				        //提交事务
                _dal.Command.Connection.Close();	    //关闭连接
                _dal.IsUseTrans = false;				//修改事务标志。
                _dbTran.Dispose();
            }
            else
            {
                string str = "误操作。在没有启用事务，或者已经回滚，或者已经提交了事务的情况下再次提交事务。请注意查看程序流程！";
                _dal._errorMessage = str;

                //没有启用事务，或者已经回滚，或者已经提交了事务
                DataAccessLibrary.AddLogError(str);
            }

        }
        #endregion

        #region 回滚事务，并关闭连接。在程序出错的时候，自动调用。
        /// <summary>
        /// 回滚事务，并关闭连接。在程序出错的时候，自动调用。
        /// </summary>
        public void TranRollBack()
        {
            if (_dal.IsUseTrans)
            {
                _dbTran.Rollback();			            //回滚事务
                _dal.Command.Connection.Close();	    //关闭连接
                _dal.IsUseTrans = false;				//修改事务标志。
                //DbTran.Dispose();
            }
            else
            {
                //没有启用事务，或者已经回滚，或者已经提交了事务
                DataAccessLibrary.AddLogError("误操作。在没有启用事务，或者已经回滚，或者已经提交了事务的情况下再次回滚事务。请注意查看程序流程！");
            }
        }
        #endregion
    }
}
