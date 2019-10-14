using GongBei.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using TeriusCommon.Helpers;

namespace GongBei.DB
{
    public class SQLAction
    {
        string SavectConclusionResultData_SQL = "";
        string SavectConclusionResultData_HasResultMask_SQL = "";
        readonly string StationID = System.Configuration.ConfigurationManager.AppSettings["StationID"];
        public int SavectConclusionResultData(ctConclusionResult info)
        {
            try
            {
                CreateConclusionResultSql(info.Body.ResultMask);
                var M_Result = info.Body.JudgeResult == "R" ? "1" : "0";
                IList<SqlParameter> sqlparams = new List<SqlParameter>();
                sqlparams.Add(new SqlParameter("@OPT_ID", info.Body.UserId));
                sqlparams.Add(new SqlParameter("@M_Result", M_Result));
                sqlparams.Add(new SqlParameter("@OPT_Time", info.Body.JudgeTime));
                sqlparams.Add(new SqlParameter("@VOYAGE_NO", info.Body.BillNo));
                sqlparams.Add(new SqlParameter("@BILL_NO_1", info.Body.Barcode));
                if (info.Body.ResultMask != null)
                {
                    sqlparams.Add(new SqlParameter("@ResultMask", info.Body.ResultMask.Value));
                    return DbHelperSQL.ExecuteSql(SavectConclusionResultData_HasResultMask_SQL, sqlparams);
                }
                else
                {
                    return DbHelperSQL.ExecuteSql(SavectConclusionResultData_SQL, sqlparams);
                }

             
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

        private void CreateConclusionResultSql(int? resultMask)
        {

            if (resultMask != null)
            {
                if (SavectConclusionResultData_HasResultMask_SQL == "")
                {
                    StringBuilder sb = new StringBuilder("update EHS_ENTRY_TMP set ");
                    sb.Append("OPT_ID=@OPT_ID,");
                    sb.Append("M_Result=@M_Result,");
                    sb.Append("OPT_Time=@OPT_Time, ");
                    sb.Append("ResultMask=@ResultMask ");
                    sb.Append(" where VOYAGE_NO=@VOYAGE_NO and  BILL_NO_1=@BILL_NO_1");
                    SavectConclusionResultData_HasResultMask_SQL = sb.ToString();
                }
            }
            else
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

        string sqlSaveHead = "";
        public int SaveHead(Hashtable ht)
        {
            CreateSaveHeadSql(ht);
            IList<SqlParameter> paramList = new List<SqlParameter>();
            foreach (DictionaryEntry de in ht)
            {
                paramList.Add(new SqlParameter("@" + de.Key, de.Value));
            }
            return DbHelperSQL.ExecuteSql(sqlSaveHead, paramList);
        }

        private void CreateSaveHeadSql(Hashtable ht)
        {
            if (sqlSaveHead == "")
            {
                StringBuilder sb1 = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();
                string sql = "insert into EHS_ENTRY_HEAD({0}) values({1})";
                foreach (DictionaryEntry de in ht)
                {
                    sb1.Append(de.Key + ",");
                    sb2.Append("@" + de.Key + ",");
                }
                sb1.Remove(sb1.Length - 1, 1);
                sb2.Remove(sb2.Length - 1, 1);
                sqlSaveHead = string.Format(sql, sb1.ToString(), sb2.ToString())
;
            }
        }


        public bool CheckVoyageExist(string voyageNo)
        {
            string sql = "select count(1) from EHS_ENTRY_HEAD where VOYAGE_NO=@VOYAGE_NO";
            SqlParameter param = new SqlParameter("@VOYAGE_NO", voyageNo);
            return DbHelperSQL.Exists(sql, param);
        }

        //public void test()
        //{
        //    string sql = "select top 1 * from EHS_ENTRY_HEAD";
        //    var ds = DbHelperSQL.Query(sql);
        //    foreach (DataColumn col in ds.Tables[0].Columns)
        //    {
        //        if (col.ColumnName== "TOTAL_VALUE")
        //        {

        //        }
        //    }
        //}
    }
}
