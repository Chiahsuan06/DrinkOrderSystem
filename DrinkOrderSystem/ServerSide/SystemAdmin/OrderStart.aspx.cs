using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DrinkOrderSystem.ServerSide.SystemAdmin
{
    public partial class OrderStart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Imgbtn50Lan_Click(object sender, ImageClickEventArgs e)
        {
            string SupplierName = "50Lan";
            var dt = GetChooseDrinkList(SupplierName);

            ShopBtn(dt);
        }

        protected void ImgbtnWhiteAlley_Click(object sender, ImageClickEventArgs e)
        {
            string SupplierName = "WhiteAlley";
            var dt = GetChooseDrinkList(SupplierName);

            ShopBtn(dt);
        }

        protected void ImgbtnMilkshop_Click(object sender, ImageClickEventArgs e)
        {
            string SupplierName = "Milkshop";
            var dt = GetChooseDrinkList(SupplierName);

            ShopBtn(dt);
        }

        /// <summary>
        /// 商家的共用方法
        /// </summary>
        /// <param name="dt"></param>
        private void ShopBtn(DataTable dt)
        {
            if (dt.Rows.Count > 0) //check is empty data (大於0就做資料繫結)
            {
                var dtPaged = this.GetPagedDataTable(dt);

                this.gvChooseDrink.DataSource = dtPaged;
                this.gvChooseDrink.DataBind();
                this.ucPager.Visible = true;
            }
            else
            {
                this.gvChooseDrink.Visible = false;
                this.btnSent.Visible = false;
            }
        }
        /// <summary>
        /// GridView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvChooseDrink_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //if (string.Compare("ChooseDrink", e.CommandName, true) == 0)   //第一種自己寫的
            //{
            //    var DDL1 = ((Control)e.CommandSource).FindControl("dlChooseSugar") as DropDownList;
            //    this.txtChooseDrinkList.Text = $"{e.CommandArgument as string} + {DDL1.SelectedItem}";
            //}

            var item = e.CommandSource as Button;    //第二種 老師解的

            if (string.Compare("ChooseDrink", item.CommandName, true) == 0)
            {
                this.txtChooseDrinkList.Visible = true;

                // 找到 button 的容器，它會是 GridViewRow
                var container = item.NamingContainer;

                var DDL1 = container.FindControl("dlChooseSugar") as DropDownList;
                var DDL2 = container.FindControl("dlChooseIce") as DropDownList;
                var DDL3 = container.FindControl("dlChooseToppings") as DropDownList;

                this.txtChooseDrinkList.Text += $"{e.CommandArgument as string} {DDL1.SelectedItem} {DDL2.SelectedItem} {DDL3.SelectedItem}\r\n";

                this.btnDelete.Visible = true;
                this.btnSent.Visible = true;
            }

        }

        /// <summary>
        /// 清除選單內容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            this.txtChooseDrinkList.Text = string.Empty;
        }

        /// <summary>
        /// 下一頁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSent_Click(object sender, EventArgs e)
        {
            Response.Redirect("OrderConfirm.aspx");
        }




        /// <summary>
        /// 取得選取飲料店飲品資料
        /// </summary>
        /// <param name="SupplierName"></param>
        /// <returns></returns>
        public static DataTable GetChooseDrinkList(string SupplierName)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"SELECT ProductName,UnitPrice
                    FROM [Products]
                    WHERE SupplierName = @supplierName
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@supplierName", SupplierName));

            try
            {
                return DBHelper.ReadDataTable(connStr, dbcommand, list);
            }
            catch (Exception ex)
            {
                //Logger.WriteLog(ex);
                return null;
            }
        }


        public class DBHelper
        {
            public static string GetConnectionString()
            {
                string val = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                return val;
            }

            public static DataTable ReadDataTable(string connStr, string dbcommand, List<SqlParameter> list)
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    using (SqlCommand comm = new SqlCommand(dbcommand, conn))
                    {
                        comm.Parameters.AddRange(list.ToArray());

                        conn.Open();
                        var reader = comm.ExecuteReader();

                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        return dt;
                    }
                }
            }

            /// <summary>
            /// 讀取資料
            /// </summary>
            /// <param name="connStr"></param>
            /// <param name="dbcommand"></param>
            /// <param name="list"></param>
            /// <returns></returns>
            public static DataRow ReadDataRow(string connStr, string dbcommand, List<SqlParameter> list)
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    using (SqlCommand comm = new SqlCommand(dbcommand, conn))
                    {
                        comm.Parameters.AddRange(list.ToArray());


                        conn.Open();
                        var reader = comm.ExecuteReader();

                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        if (dt.Rows.Count == 0)
                            return null;

                        return dt.Rows[0];
                    }
                }
            }
        }

        public class DrinkInfoModel
        {
            //和DB裡的欄位一致
            public string ProductName { get; set; }
            public string SupplierID { get; set; }
            public string SupplierName { get; set; }
            public string UnitPrice { get; set; }
        }

        private int GetCurrentPage()
        {
            string pageText = Request.QueryString["Page"];

            if (string.IsNullOrWhiteSpace(pageText))
                return 1;

            int intPage;
            if (!int.TryParse(pageText, out intPage))
                return 1;

            if (intPage <= 0)
                return 1;

            return intPage;
        }
        private DataTable GetPagedDataTable(DataTable dt)
        {
            DataTable dtPaged = dt.Clone();
            //int pageSize = this.ucPager.PageSize;


            int startIndex = (this.GetCurrentPage() - 1) * 10;
            int endIndex = (this.GetCurrentPage()) * 10;

            if (endIndex > dt.Rows.Count)
                endIndex = dt.Rows.Count;

            for (var i = startIndex; i < endIndex; i++)
            {
                DataRow dr = dt.Rows[i];
                var drNew = dtPaged.NewRow();

                foreach (DataColumn dc in dt.Columns)
                {
                    drNew[dc.ColumnName] = dr[dc];
                }

                dtPaged.Rows.Add(drNew);
            }
            return dtPaged;
        }

    }
}