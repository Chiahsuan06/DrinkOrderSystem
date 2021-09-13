using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DrinkOrderSystem.ClientSide
{
    public partial class Feedback : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 清除文字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btndelete_Click(object sender, EventArgs e)
        {
            this.txtboxName.Text = string.Empty;
            this.txtboxMain.Text = string.Empty;
            this.txtboxMail.Text = string.Empty;
            this.TextBox1.Text = string.Empty;
        }

        protected void btnsend_Click(object sender, EventArgs e)
        {
            
        }
    }
}