<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;

public class Handler : IHttpHandler {

    string username = "";

    public void ProcessRequest (HttpContext context) {

        username = context.Request.QueryString["username"];
        byte[] bytes;
        string conStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        using (SqlConnection con = new SqlConnection(conStr))
        {
            con.Open();
            List<string> videoNames = getVideoNames(con);
            if (videoNames.Count != 0)
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT name,content FROM Videos where "+getCommandSuffix(videoNames.Count);

                    for (int i = 1; i <= videoNames.Count; i++)
                    {
                        cmd.Parameters.AddWithValue("@name"+i,videoNames[i-1]);
                    }

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            bytes = (byte[])sdr["content"];
                            context.Response.Clear();
                            context.Response.Buffer = true;
                            context.Response.AppendHeader("Content-Disposition", "attachment; filename=" + sdr["name"]);
                            context.Response.ContentType = "video/mp4";
                            context.Response.BinaryWrite(bytes);
                            context.Response.End();
                        }
                        con.Close();
                    }
                }
            }
        }
    }

    string getCommandSuffix(int count)
    {
        string suffix = "";
        for (int i = 1; i <= count; i++)
        {
            suffix += "name = @name" + i+" OR ";
        }
        return suffix.Substring(0, suffix.Length - 4);
    }

    private List<string> getVideoNames(SqlConnection con)
    {
        List<string> videoNames = new List<string>();
        string conStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Connection = con;
            cmd.CommandText = "SELECT videoname FROM usersVideos WHERE username = @username";
            cmd.Parameters.AddWithValue("@username",username);
            using(SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    videoNames.Add(sdr["videoname"].ToString());
                }
            }
        }
        return videoNames;
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}