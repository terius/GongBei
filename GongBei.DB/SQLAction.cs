using GongBei.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using TeriusCommon.Helpers;

namespace GongBei.DB
{
    public class SQLAction
    {
        string SavectConclusionResultData_SQL = "";
        readonly string StationID = System.Configuration.ConfigurationManager.AppSettings["StationID"];
        public int SavectConclusionResultData(ctConclusionResult info)
        {
            try
            {
                CreateConclusionResultSql();
                var M_Result = info.Body.JudgeResult == "R" ? "1" : "0";
                IList<SqlParameter> sqlparams = new List<SqlParameter>();
                sqlparams.Add(new SqlParameter("@OPT_ID", info.Body.UserId));
                sqlparams.Add(new SqlParameter("@M_Result", M_Result));
                sqlparams.Add(new SqlParameter("@OPT_Time", info.Body.JudgeTime));
                sqlparams.Add(new SqlParameter("@VOYAGE_NO", info.Body.BillNo));
                sqlparams.Add(new SqlParameter("@BILL_NO_1", info.Body.Barcode));
                return DbHelperSQL.ExecuteSql(SavectConclusionResultData_SQL, sqlparams);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                FileHelper.WriteLog(ex.ToString());
            }
            return 0;

         
        }

        public int SavectDeviceStatusData(ctDeviceStatus info)
        {
            try
            {
                string sql = "insert into EHS_ENTRY_DEVICE(StationID,MessageTime,Status) values(@StationID,@MessageTime,@Status)";
                IList<SqlParameter> sqlparams = new List<SqlParameter>();
                sqlparams.Add(new SqlParameter("@StationID", StationID));
                sqlparams.Add(new SqlParameter("@MessageTime", info.Header.MessageTime));
                sqlparams.Add(new SqlParameter("@Status", info.Body.Status));
                return DbHelperSQL.ExecuteSql(sql, sqlparams);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                FileHelper.WriteLog(ex.ToString());
            }
            return 0;
        }

        private void CreateConclusionResultSql()
        {


            if (SavectConclusionResultData_SQL == "")
            {
                StringBuilder sb = new StringBuilder("update EHS_ENTRY_TMP set ");
                sb.Append("OPT_ID=@OPT_ID,");
                sb.Append("M_Result=@M_Result,");
                sb.Append("OPT_Time=@OPT_Time ");
                sb.Append(" where VOYAGE_NO=@VOYAGE_NO and  BILL_NO_1=@BILL_NO_1");
                SavectConclusionResultData_SQL = sb.ToString();
            }
        }
    }
}
