using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

// Bảng dữ liệu trong MySQL:
//     USE phanmemquanlybaiguixe;
//
// CREATE TABLE IF NOT EXISTS DuLieuXeVao (
//     ID INT AUTO_INCREMENT PRIMARY KEY,
//     BienSoXe VARCHAR(20),
//     LoaiVe VARCHAR(50),
//     SoVe VARCHAR(20),
//     ThoiGianVao DATETIME,
//     GiaHan TINYINT(1) DEFAULT 0,
//     TrangThaiVe ENUM('ChuaTra', 'DaTra') DEFAULT 'ChuaTra',
//     ThoiGianRa DATETIME,
//     TienPhat DOUBLE DEFAULT 0
// );

namespace PhanMemNVSoatVe
{
    public partial class frmPhanMemChoNVSoatVe : Form
    {
        private readonly string _connectionString =
            "server=localhost;database=PhanMemQuanLyBaiGuiXe;user=root;password=3010D@ngth@im1nhvu2005;";

        private bool barrierVaoDangMo = false;

        private bool barrierRaDangMo = false;

        private bool ValidateBienSo(string bienSo)
        {
            const string pattern = @"^\d{2}[A-Za-z]-[A-Za-z0-9]{1,2} \d{3,4}(\.\d{2})?$";
            return Regex.IsMatch(bienSo, pattern);
        }

        public frmPhanMemChoNVSoatVe()
        {
            InitializeComponent();
            CapNhatTrangThaiBarrier();
            tmrThoiGianVao.Start();
        }

        private void CapNhatTrangThaiBarrier()
        {
            lblTrangThaiVao.BackColor = barrierVaoDangMo ? Color.LimeGreen : Color.Red;
            lblTrangThaiRa.BackColor = barrierRaDangMo ? Color.LimeGreen : Color.Red;
        }

        private MySqlConnection GetConnection()
            => new MySqlConnection(_connectionString);

        private void txtSoVeVao_Enter_1(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSoVeVao.Text)) return;

            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    // Tìm số vé tái sử dụng hoặc cấp mới
                    string sqlMin = @"
                        SELECT MIN(CAST(SoVe AS UNSIGNED))
                        FROM DuLieuXeVao
                        WHERE TrangThaiVe='DaTra'
                          AND SoVe NOT IN (
                              SELECT SoVe FROM DuLieuXeVao WHERE TrangThaiVe='ChuaTra'
                          )";
                    var cmdMin = new MySqlCommand(sqlMin, conn);
                    object resMin = cmdMin.ExecuteScalar();

                    int soVe = (resMin != DBNull.Value && resMin != null)
                        ? Convert.ToInt32(resMin)
                        : Convert.ToInt32(
                            new MySqlCommand(
                                "SELECT COALESCE(MAX(CAST(SoVe AS UNSIGNED)),0)+1 FROM DuLieuXeVao", conn
                            ).ExecuteScalar()
                        );

                    txtSoVeVao.Text = soVe.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy số vé xe vào mặc định: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tmrThoiGianVao_Tick(object sender, EventArgs e)
            => lblThoiGianVao.Text = DateTime.Now.ToString("dd/MM/yyyy | H:mm:ss");

        private void btnMoBarrier_Click_1(object sender, EventArgs e)
        {
            string bienSo = txtBienSoVao.Text.Trim();
            string loaiVe = cbxLoaiVeVao.SelectedItem?.ToString();
            string soVe = txtSoVeVao.Text.Trim();
            DateTime now = DateTime.Now;

            if (string.IsNullOrWhiteSpace(bienSo)
                || string.IsNullOrWhiteSpace(loaiVe)
                || string.IsNullOrWhiteSpace(soVe))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin: Biển số, Loại vé, Số vé.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateBienSo(bienSo))
            {
                MessageBox.Show($"Biển số '{bienSo}' không hợp lệ.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra định dạng biển số hợp lệ
            if (barrierVaoDangMo)
            {
                MessageBox.Show("Vui lòng đóng barrier cổng vào trước khi cấp vé mới.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra biển số xe chưa có vé chưa trả
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    "SELECT COUNT(*) FROM DuLieuXeVao WHERE BienSoXe=@BienSo AND TrangThaiVe='ChuaTra'", conn);
                cmd.Parameters.AddWithValue("@BienSo", bienSo);
                if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                {
                    MessageBox.Show($"Xe biển số {bienSo} đang còn trong bãi (chưa trả vé).", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Kiểm tra giờ nhận xe (đóng cửa sau 22:00)
            if (now.TimeOfDay >= TimeSpan.FromHours(22))
            {
                MessageBox.Show("Đã quá thời gian mở cửa (22:00). Không nhận xe mới.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Thêm bản ghi vé mới
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    var cmdExist = new MySqlCommand(
                        "SELECT COUNT(*) FROM DuLieuXeVao WHERE SoVe=@SoVe AND TrangThaiVe='ChuaTra'", conn);
                    cmdExist.Parameters.AddWithValue("@SoVe", soVe);
                    if (Convert.ToInt32(cmdExist.ExecuteScalar()) > 0)
                    {
                        MessageBox.Show($"Vé số {soVe} vẫn chưa được trả. Không thể cấp lại.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var cmdInsert = new MySqlCommand(@"
                        INSERT INTO DuLieuXeVao (BienSoXe, LoaiVe, SoVe, ThoiGianVao)
                        VALUES (@BienSo, @LoaiVe, @SoVe, @ThoiGianVao)", conn);
                    cmdInsert.Parameters.AddWithValue("@BienSo", bienSo);
                    cmdInsert.Parameters.AddWithValue("@LoaiVe", loaiVe);
                    cmdInsert.Parameters.AddWithValue("@SoVe", soVe);
                    cmdInsert.Parameters.AddWithValue("@ThoiGianVao", now);
                    cmdInsert.ExecuteNonQuery();

                    barrierVaoDangMo = true;
                    CapNhatTrangThaiBarrier();

                    txtBienSoVao.Clear();
                    cbxLoaiVeVao.SelectedIndex = -1;
                    txtSoVeVao.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xử lý dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSoVeRa_TextChanged(object sender, EventArgs e)
        {
            string soVe = txtSoVeRa.Text.Trim();
            if (string.IsNullOrEmpty(soVe))
            {
                lblBienSoRa.Text = lblLoaiVeRa.Text = lblThoiGianRa.Text = lblPhatMuon.Text = string.Empty;
                return;
            }

            using (var conn = GetConnection())
            {
                var cmd = new MySqlCommand(@"
                    SELECT BienSoXe, LoaiVe, ThoiGianVao, GiaHan
                    FROM DuLieuXeVao
                    WHERE SoVe=@SoVe AND TrangThaiVe='ChuaTra' LIMIT 1", conn);
                cmd.Parameters.AddWithValue("@SoVe", soVe);

                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return;

                        DateTime tgVao = reader.GetDateTime("ThoiGianVao");
                        bool coGiaHan = reader.GetBoolean("GiaHan");

                        lblBienSoRa.Text = reader.GetString("BienSoXe");
                        lblLoaiVeRa.Text = reader.GetString("LoaiVe");
                        lblThoiGianRa.Text = tgVao.ToString("dd/MM/yyyy | H:mm:ss");

                        DateTime m22 = new DateTime(tgVao.Year, tgVao.Month, tgVao.Day, 22, 0, 0);
                        double gioTre = (DateTime.Now - m22).TotalHours;
                        double phat = 0;

                        if (coGiaHan)
                        {
                            if (gioTre > 1) phat = Math.Floor(gioTre) * 10000;
                        }
                        else if (gioTre > 0)
                        {
                            phat = gioTre < 1 ? 5000 : Math.Floor(gioTre) * 10000;
                        }

                        lblPhatMuon.Text = phat > 0
                            ? $"{phat:N0} VND | muộn {(int)Math.Floor(gioTre)} giờ"
                            : "0 VND";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi lấy dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void lblLanVao_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void cbxLoaiXe_SelectedIndexChanged(object sender, EventArgs e) { }
        private void lblThoiGian_Click(object sender, EventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void lblBienSoRa_Click(object sender, EventArgs e) { }
        private void txtBienSoVao_TextChanged(object sender, EventArgs e) { }
        private void txtSoVeVao_TextChanged(object sender, EventArgs e) { }
        private void lblLoaiVeRa_Click(object sender, EventArgs e) { }
        private void lblThoiGianRa_Click(object sender, EventArgs e) { }
        private void btnDongBarrier_Click(object sender, EventArgs e)
        {
            barrierVaoDangMo = false;
            CapNhatTrangThaiBarrier();
        }
        private void button2_Click(object sender, EventArgs e) { }
        private void button1_Click(object sender, EventArgs e)
        {
            barrierRaDangMo = false;
            CapNhatTrangThaiBarrier();
        }
        private void lblTrangThaiVao_Click(object sender, EventArgs e) { }
        private void lblTrangThaiRa_Click(object sender, EventArgs e) { }
        private void lblPhatMuon_Click(object sender, EventArgs e) { }
    }
}
