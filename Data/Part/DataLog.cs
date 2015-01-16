using System;
using System.Collections.Generic;
using System.Text;

namespace Nature.Data.Part
{
    /// <summary>
    /// 数据修改日志的属性
    /// </summary>
    public class DataLog
    {
        /// <summary>
        /// 模块ID
        /// </summary>
        public string ModuleID { get; set; }
        /// <summary>
        /// 依据哪个视图做的操作
        /// </summary>
        public string PageVIewID { get; set; }
        /// <summary>
        /// 哪个按钮触发的
        /// </summary>
        public string ButtonID { get; set; }
        /// <summary>
        /// 操作的记录ID
        /// </summary>
        public string DataID { get; set; }
        /// <summary>
        /// 谁修改的
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// IP
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 申请的URL
        /// </summary>
        public string DataUrl { get; set; }

    }
}
