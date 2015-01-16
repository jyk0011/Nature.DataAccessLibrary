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
 * function: 根据数据驱动类型，建立数据访问函数库的工厂
 * history:  created by 金洋 
 * **********************************************
 */


namespace Nature.Data
{
    /// <summary>
    /// 根据数据驱动类型，建立数据访问函数库的工厂
    /// </summary>
    public static class DalFactory
	{
        /// <summary>
        /// 根据web.config里的默认值实例化
        /// </summary>
        /// <returns></returns>
        public static DataAccessLibrary CreateDal()
        {
            DataAccessLibrary myDal ;

            switch (CommonFactory.ProviderName)
            {
                case "System.Data.SqlClient":     //SqlClient "System.Data.SqlClient"
                    myDal = new SqlClientDal();
                    break;

                default :     //OleDb
                    myDal = new OleDbDal();
                    break;

                case "System.Data.Odbc":     //Odbc
                    myDal = new OdbcDal();
                    break;

                case "System.Data.OracleClient":
                    myDal = new OracleClientDal();
                    break;
            }

            return myDal;
        }

        /// <summary>
        /// 根据参数创建实例。
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="providerName">数据库驱动的名称。System.Data.SqlClient、System.Data.OleDb 、System.Data.Odbc、System.Data.OracleClient等</param>
        /// <returns></returns>
        public static DataAccessLibrary CreateDal(string connectionString, string providerName)
        {
            DataAccessLibrary myDal;

            switch (providerName)
            {
                case "System.Data.SqlClient":     //SqlClient "System.Data.SqlClient"
                    myDal = new SqlClientDal(connectionString, providerName);
                    break;

                default:     //OleDb
                    myDal = new OleDbDal(connectionString, providerName);
                    break;

                case "System.Data.Odbc":     //Odbc
                    myDal = new OdbcDal(connectionString, providerName);
                    break;

                case "System.Data.OracleClient":
                    myDal = new OracleClientDal(connectionString, providerName);
                    break;
            }

            return myDal;
        }
	}
}
