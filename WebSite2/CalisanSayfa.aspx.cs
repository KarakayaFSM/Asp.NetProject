using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

public partial class CalisanSayfaaspx : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    private bool isUserNameEmptyOrDigit(string input)
    {
        double dummy;
        if ((string.IsNullOrWhiteSpace(input) || (double.TryParse(input, out dummy))))
        {
            return true;
        }
        return false;
    }

    private void DataBindVideoNames(string username)
    {
        string conStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        using (SqlConnection con = new SqlConnection(conStr))
        {
            con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandText = "SELECT username,videoname FROM usersVideos WHERE username = @username";
                cmd.Parameters.AddWithValue("@username",username);
                DlVideoContainer.DataSource = cmd.ExecuteReader();
                DlVideoContainer.DataBind();
                con.Close();
            }
        }
    }


    protected void btnSorgula_Click(object sender, EventArgs e)
    {
        if (isUserNameEmptyOrDigit(tbusername.Text))
        {
            Debug.WriteLine("Lütfen geçerli bir kullanıcı adı giriniz");
            return;
        }
        DataBindVideoNames(tbusername.Text);
        DlVideoContainer.Visible = true;
    }
}