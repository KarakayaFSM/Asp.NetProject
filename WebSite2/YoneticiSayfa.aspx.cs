using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Web.UI.WebControls;

public partial class CS : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) // !PostBack yani Sayfa ilk defa açılıyosa verileri çek
        {
            PopulateList();
        }
    }
        
    private void PopulateList()
    {
        string conStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        using (SqlConnection con = new SqlConnection(conStr))
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = "SELECT Name FROM users";
                cmd.Connection = con;
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        ListItem li = new ListItem();
                        li.Text = sdr["Name"].ToString();
                        CheckBoxList1.Items.Add(li);
                    }
                }
                con.Close();
            }
        }
    }

    private bool isUserNameUnique(SqlConnection con, string username)
    {
        using (SqlCommand cmd = new SqlCommand("SELECT Name FROM users WHERE Name = @usrname", con))
        {
            cmd.Parameters.AddWithValue("@usrname", username);
            return cmd.ExecuteScalar() == null;
        }
    }

    private bool isUserNameEmptyOrDigit(string input)
    {
        double dummy;
        if ((string.IsNullOrWhiteSpace(input) || (double.TryParse(input, out dummy))))
        {
            return true;
        }

        /// KULLANICI ADI DÜZGÜN OLABİLMESİ İÇİN İKİ İFADENİN DE FALSE ÇIKMASI GEREKİR
        /// BU YÜZDEN İKİ SORGU ARASI || İŞARETİ KONULDU
        /// SAYI KONTROLÜ YAPAN TryParse için bir int değişken gerekiyordu
        /// oyüzden kullanıldı, başka özel bir anlamı yok
        return false;
        ///Ayrıca, Normelde işimiz bittiğinde Connection.Close() ile
        ///bağlantıyı kapatırdık.
        ///Ancak bu metodun çağırıldığı yerde bu Connection nesnesi
        ///Daha sonra da kullanılabileceğinden
        ///Burada bağlantıyı kapatmıyoruz
    }

    private int generateNewUserId(SqlConnection con)
    {
        ///EĞER PRIMARY KEY ALANLAR (users tablosundaki ID GİBİ)
        ///İLK DURUM OLARAK AUTO INCREMENT ÖZELLİĞİNE SAHİP OLMASAYDI
        ///BU ŞEKİLDE SON ELEMANIN ID DEĞERİNE 1 EKLEYEREK
        ///YENİ ID ÜRETEN BU METODU KULLANACAKTIK 
        using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 id FROM users ORDER BY id DESC"))
        {
            cmd.Connection = con;
            return ((int)cmd.ExecuteScalar() + 1);
        }
    }

    private void aliciEkle()
    {
        if (isUserNameEmptyOrDigit(tbAliciAdi.Text))
        {
            Debug.WriteLine("KULLANICI ADI BOŞ, SADECE BOŞLUK VEYA SAYILARDAN OLUŞAMAZ");
            Debug.WriteLine(tbAliciAdi.Text);
            ///EĞER GİRİLEN METİNDE BİR HATA VARSA SQL KISMINA HİÇ GİRME
            ///HATA MESAJI VER VE ÇIK
            return;
        }
        string sql = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string input = tbAliciAdi.Text;
        using (SqlConnection con = new SqlConnection(sql))
        {
            con.Open();
            if (!isUserNameUnique(con, input))
            {
                /// BU KULLANICI ADI ZATEN MEVCUT
                /// Burada ve altta aynı veritabanına bağlanıldığından
                ///  aynı connection nesnesi (con) kullanıldı.
                /// Böylece benzer işlemler için gereksiz yere 
                /// yeni bağlantı açılmamış oldu.
                Debug.WriteLine("BU KULLANICI ADI DAHA ÖNCE ALINMIŞ");
                con.Close();
                /// Program bu if bloğuna girdiyse
                /// Aşağı ilerlemeyecek ve Connection nesnesi kullanılmayacak demektir
                /// bu yüzden Connection.Close() diyerek
                /// bağlantıyı kapatıyoruz
                return;
            }
            using (SqlCommand cmd = new SqlCommand("INSERT INTO users (Name) VALUES (@name)", con))
            {
                /// CommandText ve Connection nesneleri sonradan da verilebilir
                /// ya da üstteki gibi cmd'nin içine de gömülebilir.
                cmd.Parameters.AddWithValue("@name", input);
                cmd.ExecuteNonQuery();
                con.Close();
                ///ID OLUŞTURMA VE OLUŞTURULAN ID İLE KAYIT YAPMAK İÇİN
                ///AYRI BİR METOT YAZILMIŞTI
                ///ANCAK C# ve MYSQL'de JAVADAN FARKLI OLARAK
                ///PRİMARY KEY ALANLARI AUTOıNCREMENT(KENDİ ARTAN) ŞEKİLDEDİR
                ///BU YÜZDEN ID KISMINI COMMAND KISMINA DAHİL ETMİYORUZ
                ///INSERT INTO İFADESİNDE SADECE (NAME) YAZMASI BUNDAN DOLAYIDIR.
            }
        }
        Debug.WriteLine("YENİ KULLANICI BAŞARIYLA EKLENDİ");
        Response.Redirect(Request.Url.AbsoluteUri,false); /// --> SAYFADAKİ SEÇİMLERİ, DEĞİŞİKLİKLERİ SİLER
        ///BURAYA YENİ KULLANICI BAŞARIYLA EKLENDİ MESAJI VERMEMİZ LAZIM
        ///ANCAK ÖNCE DE BELİRTTİĞİM GİBİ MESAJ VERDİRME KISMINI YAPAMADIM
        ///ANCAK BİR ŞEKİLDE JAVASCRIPT KULLANARAK BİR YOLU VAR GİBİ
        ///GÖRÜNÜYOR. YA C# KODU İÇERİSİNDE YA DA BENCE BÜYÜK İHTİMALLE
        ///ASPX SCRIPT KISMI İÇİNDE JS KODUYLA ALERT FONKSİYONU KULLANARAK OLABİLİR

        ///AYRICA EKLENEN KİŞİNİN LİSTEDE GÖZÜKMESİ İÇİN SAYFA YENİLENMELİ
        ///KENDİN KODLA SAYFAYI YENİLEYEBİLİR
        ///YA DA EKLENDİ MESAJINDAN SONRA F5'E BASABİLİRSİN

        ///YUKARIDAKİ Debug.WriteLine sadece Debug için
        ///Biz ise Ekrana bir uyarı çıkarmak istiyoruz
    }

    protected void btnAliciEkle_Click(object sender, EventArgs e)
    {
        aliciEkle();
    }

    protected void BtnGonder_Click(object sender, EventArgs e)
    {
        talepYonlendir();
        Response.Redirect(Request.Url.AbsoluteUri, false); /// Sayfadaki Seçimleri Siler.
    }

    private bool dosyaMevcutMu(string filename,SqlConnection con)
    {
        using (SqlCommand cmd = new SqlCommand("SELECT name FROM Videos WHERE name = @name",con))
        {
            cmd.Parameters.AddWithValue("@name", filename);
            return cmd.ExecuteScalar() != null;
        }
    }

    private bool talepYonlendir()
    {
        if (!FileUpload1.HasFile)
        {
            ///HİÇ BİR DOSYA EKLEMEDİNİZ
            ///HATA MESAJI VER VE ÇIK
            Debug.WriteLine("HİÇBİR DOSYA EKLEMEDİNİZ");
            return false;
        }
        if (!(System.IO.Path.GetExtension(FileUpload1.PostedFile.FileName) == ".mp4"))
        {
            ///GİRİLEN DOSYALARDAN BİRİ MP4 FORMATINDA DEĞİL
            ///HATA MESAJI VER VE ÇIK
            Debug.WriteLine("DOSYA BİÇİMİ GEÇERSİZ");
            return false;
        }

        string conStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string fileName = FileUpload1.PostedFile.FileName;
        using (SqlConnection con = new SqlConnection(conStr))
        {
            con.Open();
            return dosyaMevcutMu(fileName,con) ? mevcutVideoGonder(fileName,con) : yeniVideoGonder();
        }
    }

    private void KullanıcıyaMevcutVideoAta(string username,string filename, SqlConnection con)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Connection = con;
            cmd.CommandText = "INSERT INTO usersVideos (Id,username, videoname) VALUES (@Id,@username, @videoname)";
            cmd.Parameters.AddWithValue("@Id", username+filename);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@videoname", filename);
            cmd.ExecuteNonQuery();
        }
    }

    private bool mevcutVideoGonder(string filename, SqlConnection con)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Connection = con;
            foreach (ListItem item in CheckBoxList1.Items)
            {
                if (item.Selected)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "SELECT videoname FROM usersVideos WHERE username = @username AND videoname = @videoname";
                    string username = item.Text;
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@videoname", filename);
                    if (cmd.ExecuteScalar() == null)
                    {
                        KullanıcıyaMevcutVideoAta(username, filename, con);
                        ///program bu kısıma girdiyse kullanıcı dosyaya sahip değil demektir
                        ///Tablosuna dosya ismini ekle
                    }
                    else {
                        Debug.WriteLine(username + " Zaten "+filename +" Adlı Videoya Sahip");
                        continue;
                        ///program bu kısıma girdiyse kullanıcı dosyaya zaten sahip demektir
                        ///Uyarı ver ve devam et
                    }
                }
            }
            Debug.WriteLine(filename + " Başarıyla Gönderildi");
            con.Close();
        }
        return true;
    }

    private bool yeniVideoGonder()
    {
        if (CheckBoxList1.SelectedIndex == -1)
        {
            Debug.WriteLine("LÜTFEN LİSTEDEN BİR SEÇİM YAPINIZ");
            return false;
        }
        System.Web.HttpPostedFile VideoInput = FileUpload1.PostedFile;
        string conStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        using (SqlConnection con = new SqlConnection(conStr))
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                con.Open();
                cmd.Connection = con;
                    using (BinaryReader br = new BinaryReader(VideoInput.InputStream))
                    {
                        byte[] Inputbytes = br.ReadBytes((int)VideoInput.InputStream.Length);
                        cmd.CommandText = "INSERT INTO Videos (name, content) values (@name, @content)";
                        cmd.Parameters.AddWithValue("@name", Path.GetFileName(VideoInput.FileName));
                        cmd.Parameters.AddWithValue("@content", Inputbytes);
                        cmd.ExecuteNonQuery();
                    }                
            }

            foreach (ListItem item in CheckBoxList1.Items)
            {
                if (item.Selected)
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO usersVideos (Id,username, videoname) VALUES (@Id,@username, @videoname)";
                        cmd.Parameters.AddWithValue("@Id", item.Text+ VideoInput.FileName);
                        cmd.Parameters.AddWithValue("@username", item.Text);
                        cmd.Parameters.AddWithValue("@videoname",VideoInput.FileName);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            ///con.Close();
        }
        ///Response.Redirect(Request.Url.AbsoluteUri);
        Debug.WriteLine("VİDEO BAŞARIYLA KAYDEDİLDİ");
        return true;
    }
}









