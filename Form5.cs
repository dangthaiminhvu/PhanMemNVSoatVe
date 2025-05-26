using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace PhanMemNVSoatVe
{
    public partial class frmDangNhap : Form
    {
        private string connectionString;

        public frmDangNhap()
        {
            InitializeComponent();
            txtNhapMatKhau.UseSystemPasswordChar = true;

            connectionString = ConfigurationManager.ConnectionStrings["MyDb"]?.ConnectionString;

            foreach (var ctrl in new Control[] { txtTenDangNhap, txtMaNhanVien, txtNhapMatKhau })
            {
                ctrl.Enter += (s, e) => ctrl.BackColor = Color.LightBlue;
                ctrl.Leave += (s, e) => ctrl.BackColor = SystemColors.Window;
            }
        }

        // Hàm băm SHA-256
        private string MaHoaSha256(string input)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = sha.ComputeHash(bytes);
                var sb = new StringBuilder();
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        private void txtNhapMatKhau_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtTenDangNhap_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtMaNhanVien_TextChanged(object sender, EventArgs e)
        {
        }

        public string ChucVuDaDangNhap { get; private set; }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string tenDangNhap = txtTenDangNhap.Text.Trim();
            string maNV = txtMaNhanVien.Text.Trim();
            string matKhauRaw = txtNhapMatKhau.Text.Trim();

            if (tenDangNhap == "" || maNV == "" || matKhauRaw == "")
            {
                MessageBox.Show(
                    "Vui long dien day du Ten NV, Ma NV va Mat khau.",
                    "Thieu thong tin",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            string matKhauHash = MaHoaSha256(matKhauRaw);

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = @"
                        SELECT MatKhau, ChucVu, TrangThai
                          FROM dulieunhanvien
                         WHERE TenNhanVien = @ten
                           AND IDNhanVien  = @id";
                    cmd.Parameters.AddWithValue("@ten", tenDangNhap);
                    cmd.Parameters.AddWithValue("@id", maNV);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            MessageBox.Show(
                                "Sai Ten NV hoac Ma NV.",
                                "Dang nhap that bai",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            return;
                        }

                        string pwStored = reader.GetString("MatKhau");
                        string chucVu = reader.GetString("ChucVu");
                        string trangThai = reader.GetString("TrangThai");

                        var choPhep = new[] { "Đang Làm", "Thử Việc", "Học Việc", "Đào Tạo" };
                        if (!choPhep.Contains(trangThai))
                        {
                            MessageBox.Show(
                                $"Tài khoản đang ở trạng thái '{trangThai}' nên không được phép sử dụng hệ thống.",
                                "Không đủ quyền",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                        }

                        if (!string.Equals(matKhauHash, pwStored, StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBox.Show(
                                "Sai mat khau.",
                                "Dang nhap that bai",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            return;
                        }

                        this.ChucVuDaDangNhap = chucVu;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Loi khi ket noi hoac truy van CSDL: " + ex.Message,
                    "Loi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnNhapLai_Click(object sender, EventArgs e)
        {
            txtTenDangNhap.Clear();
            txtMaNhanVien.Clear();
            txtNhapMatKhau.Clear();
            txtTenDangNhap.Focus();
        }

        private void frmDangNhap_Load(object sender, EventArgs e)
        {

        }
    }
}
