using GongBei.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TeriusCommon.Helpers;

namespace InputForm
{
    public partial class Form1 : Form
    {
        SQLAction db = new SQLAction();
        public Form1()
        {
            InitializeComponent();

        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                string voyageNo = txtVoyage.Text.Trim();
                if (string.IsNullOrWhiteSpace(voyageNo))
                {
                    MessageBox.Show("请先输入总运单号");
                    return;
                }
                AddMessage("开始读取数据...总运单号：" + voyageNo);
                string userName = txtUserName.Text.Trim();
                string pwd = txtPwd.Text.Trim();
                string code = txtCode.Text.Trim();
                DataSet ds =  ServiceHelper.GetData(voyageNo, userName, pwd, code);
                //  ds.ReadXml("d:\\20181201.xml");
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    dgvHead.DataSource = ds.Tables[0];
                    dgvList.DataSource = ds.Tables[1];
                    AddMessage(string.Format("读取数据成功!表头数据:{0}条,表体数据{1}条", ds.Tables[0].Rows.Count, ds.Tables[1].Rows.Count));

                    if (!db.CheckVoyageExist(voyageNo))
                    {
                        int rs = AddData(ds.Tables[0]);
                        AddMessage("成功插入" + rs + "条数据");
                    }
                    else
                    {
                        AddMessage("总运单号：" + voyageNo +"在数据库中已存在！");
                    }

                }
                else
                {
                    AddMessage("未读取到数据");
                }
            }
            catch (Exception ex)
            {
                FileHelper.WriteLog(ex.ToString());
                AddMessage("执行错误，错误信息：" + ex.Message);
            }

        }

        private int AddData(DataTable dt)
        {
            int rs = 0;
            Hashtable ht = new Hashtable();
            foreach (DataRow row in dt.Rows)
            {
                ht.Clear();
                ht["SHIP_ID"] = row["SHIP_ID"].ToString();
                ht["VOYAGE_NO"] = row["VOYAGE_NO"].ToString();
                ht["BILL_NO"] = row["BILL_NO"].ToString();
                ht["I_E_FLAG"] = row["I_E_FLAG"].ToString();
                ht["I_E_PORT"] = row["I_E_PORT"].ToString();
                ht["L_D_PORT"] = row["L_D_PORT"].ToString();
                ht["TRAF_NAME"] = row["TRAF_NAME"].ToString();
                ht["SEND_NAME"] = row["SEND_NAME"].ToString();
                ht["OWNER_NAME"] = row["OWNER_NAME"].ToString();
                ht["SEND_COUNTRY"] = row["SEND_COUNTRY"].ToString();
                ht["SEND_CITY"] = row["SEND_CITY"].ToString();
                //  ht["SEND_ID"] = row["SEND_ID"].ToString();
                ht["TRADE_CODE"] = row["TRADE_CO"].ToString();
                ht["TRADE_NAME"] = row["TRADE_NAME"].ToString();
                ht["MAIN_G_NAME"] = row["MAIN_G_NAME"].ToString();
                ht["PACK_NO"] = ConvertHelper.ToSafeNullabledecimal(row["PACK_NO"]);
                ht["GROSS_WT"] = ConvertHelper.ToSafeNullabledecimal(row["GROSS_WT"]);
                ht["TOTAL_VALUE"] = ConvertHelper.ToSafeNullabledecimal(row["TOTAL_VALUE"]);
                ht["CURR_CODE"] = row["CURR_CODE"].ToString();
                ht["I_E_DATE"] = ConvertHelper.ToSafeNullableDateTime(row["I_E_DATE"]);
                ht["D_DATE"] = ConvertHelper.ToSafeNullableDateTime(row["D_DATE"]);
                ht["ENTRY_TYPE"] = row["ENTRY_TYPE"].ToString();
                ht["DEC_TYPE"] = row["DEC_TYPE"].ToString();
                ht["DEC_ER"] = row["DEC_ER"].ToString();
                ht["DEC_DATE"] = ConvertHelper.ToSafeNullableDateTime(row["DEC_DATE"]);
                ht["READ_DATE"] = ConvertHelper.ToSafeNullableDateTime(row["READ_DATE"]);
                //   ht["READ_FLAG"] = row["READ_FLAG"].ToString();
                // ht["NOTE"] = row["NOTE"].ToString();
                //   ht["M_CHECKH"] = row["M_CHECKH"].ToString();
                int result = db.SaveHead(ht);
                rs += result;
            }
            return rs;
        }

        private void AddMessage(string msg)
        {
            this.lbMessage.Items.Add(msg);
            lbMessage.TopIndex = lbMessage.Items.Count - (int)(lbMessage.Height / lbMessage.ItemHeight);
            Application.DoEvents();
        }

        private void dgvHead_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private void dgvList_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }
    }
}
