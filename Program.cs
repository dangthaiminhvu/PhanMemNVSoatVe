using System;
using System.Windows.Forms;
using PhanMemNVSoatVe;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.IO;

namespace PhanMemNVSoatVe
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void EnsureDatabase()
        {
            var rawConn = ConfigurationManager
                .ConnectionStrings["MyConnStr"]
                .ConnectionString;

            var builder = new MySqlConnectionStringBuilder(rawConn)
            {
                Database = ""
            };

            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                conn.Open();

                string script = File.ReadAllText(Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "schema.sql"));

                var sqlScript = new MySqlScript(conn, script);
                sqlScript.Execute();
            }
        }
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var login = new frmDangNhap())
            {
                if (login.ShowDialog() != DialogResult.OK)
                    return;

                Form mainForm;
                switch (login.ChucVuDaDangNhap)
                {
                    case "Quản Lý Thông Tin":
                        mainForm = new frmPhanMemNVQuanLyThongTin();
                        break;
                    case "Soát Vé":
                        mainForm = new frmPhanMemChoNVSoatVe();
                        break;
                    case "Quản Lý Khách Hàng":
                        mainForm = new frmPhanMemDanhChoNVQuanLyKhachHang();
                        break;
                    case "Quản Trị Viên":
                        mainForm = new frmPhanMemDanhChoNVQuanTriVien();
                        break;
                    default:
                        return;
                }

                Application.Run(mainForm);
            }
        }
    }
}