using System;
using System.Windows.Forms;
using PhanMemNVSoatVe;
using System.Configuration;
using System.IO;

namespace PhanMemNVSoatVe
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Database connection check as the first thing
            var connStr = ConnectionHelper.GetConnectionString();
            if (string.IsNullOrEmpty(connStr))
                return; // Exit if no valid connection

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