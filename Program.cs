using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

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

            using (var login = new frmDangNhap())
            {
                if (login.ShowDialog() != DialogResult.OK)
                    return;

                Form mainForm;
                switch (login.ChucVuDaDangNhap)
                {
                    case "Quản Lý Thông Tin":
                        mainForm = new frmPhanMemNVQuanLyKhachHang();
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
