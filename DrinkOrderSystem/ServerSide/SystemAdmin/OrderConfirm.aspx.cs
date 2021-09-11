using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DrinkOrderSystem.ServerSide.SystemAdmin
{
    public partial class OrderConfirm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (Session["Row"] != null)
                {
                    //Fetch the GridView Row from Session.
                    GridViewRow row = Session["Row"] as GridViewRow;

                    //Fetch and display the Cell values.
                    Label1.Text = row.Cells[0].Text;
                    Label2.Text = row.Cells[2].Text;
                    Label3.Text = row.Cells[3].Text;
                    Label4.Text = row.Cells[4].Text;
                    Label5.Text = row.Cells[5].Text;
                    Label6.Text = row.Cells[1].Text;
                }
            }
        }
    }
}