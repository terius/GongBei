using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WebApplication1
{
    /// <summary>
    /// WebService1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strVOYAGE_NO">总运单号</param>
        /// <param name="strUserID">关区登录用户		——海关技术处生成并提供</param>
        /// <param name="strUserPWD">关区登录密码		——海关技术处生成并提供</param>
        /// <param name="strVisitorNO">来访关员代码		——不可为空(暂未启用)</param>
        /// <param name="outMessage">异常返回错误信息</param>
        /// <returns></returns>
        [WebMethod]
        public DataSet GBCustoms_ServiceEntrance(string strVOYAGE_NO, string strUserID, string strUserPWD, string strVisitorNO, out string[][] outMessage)
        {
            outMessage = new string[][] { };
            DataTable EHS_ENTRY_HEAD = new DataTable();
            EHS_ENTRY_HEAD.Columns.Add("SHIP_ID");
            EHS_ENTRY_HEAD.Columns.Add("VOYAGE_ID");
            EHS_ENTRY_HEAD.Columns.Add("BILL_ID");
            EHS_ENTRY_HEAD.Columns.Add("I_E_FLAG");
            EHS_ENTRY_HEAD.Columns.Add("I_E_PORT");
            return null;
        }
    }
}
