/**
 * 自然框架之元数据
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
 * Nature Framework MetaData is free software. You are allowed to download, modify and distribute 
 * the source code in accordance with LGPL 2.1 license, however if you want to use 
 * Nature Framework MetaData on your site or include it in your commercial software, you must  be registered.
 * http://www.natureFW.com/registered
 */

/* ***********************************************
 * author :  金洋（金色海洋jyk）
 * email  :  jyk0011@live.cn  
 * function: 数据访问函数库实例的集合
 * history:  created by 金洋 2012-9-13 10:18:59
 *           
 * **********************************************
 */


namespace Nature.Data
{
    /// <summary>
    /// 数据访问函数库实例的集合
    /// </summary>
    /// user:jyk
    /// time:2012/9/13 14:08
    public class DalCollection
    {
        #region 访问数据库的实例，四个

        /// <summary>
        /// 用于访问客户项目的数据库
        /// </summary>
        /// <value>
        /// 访问数据库的实例
        /// </value>
        /// user:jyk
        /// time:2012/9/5 10:52
        public DataAccessLibrary DalCustomer { set; get; }

        /// <summary>
        /// 用于访问存放元数据（配置信息）的数据库
        /// </summary>
        /// <value>
        /// 访问数据库的实例
        /// </value>
        /// user:jyk
        /// time:2012/9/5 10:52
        public DataAccessLibrary DalMetadata { set; get; }

        /// <summary>
        /// 用于访问存放角色信息、角色分配信息的数据库
        /// </summary>
        /// <value>
        /// 访问数据库的实例
        /// </value>
        /// user:jyk
        /// time:2012/9/5 10:52
        public DataAccessLibrary DalRole { set; get; }

        /// <summary>
        /// 用于访问登录用户信息的数据库
        /// </summary>
        /// <value>
        /// 访问数据库的实例
        /// </value>
        /// user:jyk
        /// time:2012/9/5 10:52
        public DataAccessLibrary DalUser { set; get; }

        #endregion
 
    }
}
