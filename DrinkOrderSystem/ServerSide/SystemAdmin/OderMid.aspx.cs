using DBORM;
using DrinkOrderSystem.Models;
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
    public partial class OderMid : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GetDrinkShopSession();
        }
        private void GetDrinkShopSession() 
        {
            this.Session["DrinkShop"] = "WhiteAlley";   //先寫死
            string SupplierName = this.Session["DrinkShop"] as string;
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
            var item = e.CommandSource as Button;
            var container = item.NamingContainer;

            this.txtChooseDrinkList.Visible = true;
            this.lbText.Visible = true;
            this.lbTotalAmount.Visible = true;
            this.btnDelete.Visible = true;
            this.btnSent.Visible = true;

            var DDLQuantity = container.FindControl("dlQuantity") as DropDownList;
            var DDlSugar = container.FindControl("dlChooseSugar") as DropDownList;
            var DDLIce = container.FindControl("dlChooseIce") as DropDownList;
            var DDLToppings = container.FindControl("dlChooseToppings") as DropDownList;

            if (DDLQuantity.SelectedIndex == 0)
            {
                this.txtChooseDrinkList.Text = "杯數選擇錯誤，請重新選擇";
                return;
            }
            if (DDlSugar.SelectedIndex == 0)
            {
                this.txtChooseDrinkList.Text = "糖量選擇錯誤，請重新選擇";
                return;
            }
            if (DDLIce.SelectedIndex == 0)
            {
                this.txtChooseDrinkList.Text = "冰塊選擇錯誤，請重新選擇";
                return;
            }
            if (DDLToppings.SelectedIndex == 0)
            {
                this.txtChooseDrinkList.Text = "加料選擇錯誤，請重新選擇";
                return;
            }

            if (string.Compare("ChooseDrink", item.CommandName, true) == 0)  //(e.CommandName == "ChooseDrink")
            {
                string argu = (e.CommandArgument) as string;
                var supplierName = this.Session["DrinkShop"].ToString();
                var orderNumber = "001";
                var account = "BpbMa";

                this.txtChooseDrinkList.Text += $"{e.CommandArgument as string} {DDLQuantity.SelectedItem}杯 {DDlSugar.SelectedItem} {DDLIce.SelectedItem} {DDLToppings.SelectedItem} {GetUnitPrice(e.CommandArgument as string)}元/杯 \r\n";

                var list = this.txtChooseDrinkList.Text;

                List<DBORM.OrderDetail> sourcedetaillist = GetOrderDetailList(supplierName);

                var DrinkList = sourcedetaillist.Where(obj => obj.ProductName == argu).FirstOrDefault();

                if (DrinkList != null)
                {
                    if (this.Session["SelectedItems"] == null)
                        this.Session["SelectedItems"] = new List<DrinkRedirectModel>();
                }
                var orderdetaillist = new DrinkRedirectModel()
                {
                    OrderDetailsID = Guid.NewGuid(),
                    OrderNumber = orderNumber,
                    Account = account,
                    OrderTime = DateTime.Now.ToString(),
                    OrderEndTime = DateTime.Now.ToString(),
                    RequiredTime = DateTime.Now.ToString(),
                    ProductName = e.CommandArgument as string,
                    Quantity = Convert.ToInt32(DDLQuantity.SelectedItem.Value),
                    UnitPrice = GetUnitPrice(e.CommandArgument as string),
                    Suger = DDlSugar.SelectedItem.ToString(),
                    Ice = DDlSugar.SelectedItem.ToString(),
                    Toppings = DDLToppings.SelectedItem.ToString(),
                    SupplierName = supplierName,
                    OtherRequest = null,
                };

                var sessionLList = this.Session["SelectedItems"] as List<DrinkRedirectModel>; //將Session轉成List，再做總和
                sessionLList.Add(orderdetaillist);

                decimal totalAmount = 0;
                foreach (var item2 in sessionLList)
                {
                    totalAmount += item2.SubtotalAmount;
                }

                this.lbTotalAmount.Text = totalAmount.ToString();

            }
        }

        public static List<DBORM.OrderDetail> GetOrderDetailList(string supplierName)
        {
            try
            {
                using (DBContextModel context = new DBContextModel())
                {
                    var query =
                        (from item in context.OrderDetails
                         where item.SupplierName == supplierName
                         select item
                        );

                    var list = query.ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex);
                return null;
            }
        }

        public static decimal GetUnitPrice(string ProductName)
        {
            try
            {
                using (DBContextModel context = new DBContextModel())
                {
                    var price = context.Products
                        .Select(obj => obj.UnitPrice);
                    var UnitPrice = price.FirstOrDefault();
                    return UnitPrice;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex);
                return decimal.Zero;
            }
        }

        /// <summary>
        /// 清除選單內容、SelectedItems
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            this.txtChooseDrinkList.Text = string.Empty;
            Session.Remove("SelectedItems");  //選擇要的Session做刪除
        }

        /// <summary>
        /// 直接儲存到資料庫
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSent_Click(object sender, EventArgs e)
        {
            //var writSession = this.Session["SelectedItems"] as List<DrinkRedirectModel>;

            //foreach (var sub in writSession)
            //{
            //    DrinkRedirectModel.AddGroup(sub);
            //}
        }
        //public static void AddGroup(DrinkRedirectModel orderDetail)
        //{
        //    try
        //    {
        //        using (DBContextModel context = new DBContextModel())
        //        {

        //            OrderDetail neworderDetail = new OrderDetail();
        //            //加入
        //            neworderDetail.OrderDetailsID = orderDetail.OrderDetailsID;
        //            neworderDetail.OrderNumber = orderDetail.OrderNumber;
        //            neworderDetail.Account = orderDetail.Account;
        //            neworderDetail.OrderTime = orderDetail.OrderTime;
        //            neworderDetail.OrderEndTime = orderDetail.OrderEndTime;
        //            neworderDetail.RequiredTime = orderDetail.RequiredTime;
        //            neworderDetail.ProductName = orderDetail.ProductName;
        //            neworderDetail.Quantity = orderDetail.Quantity;
        //            neworderDetail.UnitPrice = orderDetail.UnitPrice;
        //            neworderDetail.Suger = orderDetail.Suger;
        //            neworderDetail.Ice = orderDetail.Ice;
        //            neworderDetail.Toppings = orderDetail.Toppings;
        //            neworderDetail.SupplierName = orderDetail.SupplierName;
        //            neworderDetail.OtherRequest = orderDetail.OtherRequest;

        //            context.OrderDetails.Add(neworderDetail);
        //            context.SaveChanges();

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.WriteLog(ex);
        //        return;
        //    }
        //}

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
                Logger.WriteLog(ex);
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