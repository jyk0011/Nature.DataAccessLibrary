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
 * function: 数据访问类库的配件部分，列表实体类的管理
 * history:  created by 金洋 
 * **********************************************
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Nature.Common;
using Nature.Data.Model;

namespace Nature.Data.Part
{
    /// <summary>
    /// 管理网页里用的几个实体类，填充实体类的几个函数
    /// </summary>
    public class ManagerWebModel
    {
        /// <summary>
        /// 数据访问函数库的实例，主要是借用ado.net2.0里的 DbCommand
        /// </summary>
        private readonly DataAccessLibrary _dal;
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="dal">数据访问函数库的实例</param>
        public ManagerWebModel(DataAccessLibrary dal)
        {
            _dal = dal;
        }

        #region ExecuteFillWebList1，不指定数组大小，自动统计记录数
        /// <summary>
        /// 运行SQl语句，返回IList WebList1  集合
        /// </summary>
        /// <param name="sql">查询语句。比如select myName from tableName</param>
        /// <returns>返回WebList的集合。ID，URL，标题</returns>
        public IList<WebList1> ExecuteFillWebList1(string sql)
        {

            //返回ID 传入查询语句，返回第一条记录的第一的字段的值
            _dal.SetCommand(sql);		//设置command
            DbDataReader dr = null;
            try
            {
                IList<WebList1> returnList = new List<WebList1>();

                _dal.ConnectionOpen();
                dr = _dal.Command.ExecuteReader(CommandBehavior.CloseConnection);
                //判断SQL语句里有哪些字段
                IList<string> hasColumns = GetColumnsByWebList1(dr);

                while (dr.Read())
                {
                    var list = new WebList1();
                    #region 填充属性值
                    foreach (string col in hasColumns)
                    {
                        //SQL里有指定的字段的话，设置对应的属性。可以使用字段别名
                        switch (col)
                        {
                            case "id"://0 主键
                                list.ID = dr["id"].ToString();
                                break;

                            case "url"://1 网址
                                list.URL = dr["url"].ToString();
                                break;

                            case "title"://2 判断截取字符数
                                list.Title = dr["title"].ToString();
                                break;
                        }
                    }
                    #endregion

                    returnList.Add(list);
                }

                return returnList;
            }
            catch (Exception ex)
            {
                _dal.SetError("ExecuteFillWebList1", sql, ex.Message,_dal.ConnectionString);	//处理错误
                return null;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
                _dal.ConnectionClose();
            }
        }
        #endregion


        #region ExecuteFillWebList2，
        /// <summary>
        /// 运行SQl语句返回 IList WebList2 集合
        /// </summary>
        /// <param name="sql">查询语句。比如select myName from tableName</param>
        /// <param name="lstFormat">标题的最大字符数、内容简介的最大字符数，一个汉字按照两个字符计算。传入“0”则表示不截取标题。发表时间的格式化。</param>
        /// <returns>返回WebList2结构的集合。URL，标题，时间，人气，图片名</returns>
        public IList<WebList2> ExecuteFillWebList2(string sql, WebList2Format lstFormat)
        {

            //返回ID 传入查询语句，返回第一条记录的第一的字段的值
            _dal.SetCommand(sql);		//设置command
            DbDataReader dr = null;

            try
            {
                IList<WebList2> returnList = new List<WebList2>();

                _dal.ConnectionOpen();

                dr = _dal.Command.ExecuteReader(CommandBehavior.CloseConnection );

                //检查SQL语句里有哪些字段
                IList<string> hasColumns = GetColumnsByWebList2(dr);

                while (dr.Read())
                {
                    var list = new WebList2();

                    #region 填充属性值
                    foreach (string col in hasColumns)
                    {
                        //SQL里有指定的字段的话，设置对应的属性。可以使用字段别名
                        switch (col)
                        {
                            case "id"://0 主键
                                list.ID = dr["id"].ToString();
                                break;

                            case "url"://1 网址
                                list.URL = dr["url"].ToString();
                                break;

                            case "img"://6 图片名称
                                list.Img = dr["img"].ToString();
                                break;

                            case "kind"://7 分类
                                list.Kind = dr["kind"].ToString();
                                break;

                            case "spare"://8 备用
                                list.Spare = dr["spare"].ToString();
                                break;

                            case "title"://2 判断截取字符数
                                list.FullTitle = dr["title"].ToString();
                                if (lstFormat.TitleMaxCount == 0)
                                    list.Title = dr["title"].ToString();
                                else
                                    list.Title = Functions.StringCut(dr["title"].ToString(), lstFormat.TitleMaxCount);
                                break;

                            case "addeddate"://3 判断时间
                                if (lstFormat.DateFormat.Length == 0)
                                    list.AddedDate = dr["AddedDate"].ToString();
                                else
                                    list.AddedDate = dr.GetDateTime(dr.GetOrdinal("AddedDate")).ToString(lstFormat.DateFormat);
                                break;

                            case "intro"://4 内容简介
                                if (lstFormat.IntroMaxCount == 0)
                                    list.Introduction = dr["Intro"].ToString();
                                else
                                    list.Introduction = Functions.StringCut(dr["Intro"].ToString(), lstFormat.IntroMaxCount);
                                break;

                            case "hits"://5 人气
                                list.Hits = (int)dr["hits"];
                                break;
                        }
                    }

                    #endregion

                    returnList.Add(list);
                }

                return returnList;
            }
            catch (Exception ex)
            {
                _dal.SetError("ExecuteFillWebList2", sql, ex.Message, _dal.ConnectionString);	//处理错误
                return null;
            }
            finally
            {
                if (dr != null) dr.Close();
                _dal.ConnectionClose();
            }
        }
        #endregion

        #region 判断DataReader 里有哪些规定的字段（WebList1），把包含的字段放到字典里面
        private static IList<string> GetColumnsByWebList1( DbDataReader dr)
        {
            IList<string> re = new List<string>();

            string tmp;
            for (int i = 0; i < dr.FieldCount; i++)
            {
                tmp = dr.GetName(i).ToLower();
                switch (tmp)
                {
                    case "id":
                    case "url":
                    case "title":
                        re.Add(tmp);
                        break;
                }
            }

            return re;
        }
        #endregion

        #region 判断DataReader 里有哪些规定的字段（WebList2），把包含的字段放到字典里面
        private IList<string> GetColumnsByWebList2( DbDataReader dr)
        {
            IList<string> re = new List<string>();

            string tmp;
            for (int i=0;i<dr.FieldCount ;i++)
            {
                tmp = dr.GetName(i).ToLower();
                switch (tmp)
                {
                    case "id":
                    case "url":
                    case "title":
                    case "addeddate":
                    case "intro":
                    case "hits":
                    case "img":
                    case "spare":
                    case "kind":
                        re.Add(tmp);
                        break;
                }
            }

            return re;
        }
        #endregion
    }
}
