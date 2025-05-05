using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

//Bảng dữ liệu trong MySQL:
//    USE phanmemquanlybaiguixe;

//CREATE TABLE IF NOT EXISTS DuLieuXeVao (
//    ID INT AUTO_INCREMENT PRIMARY KEY,
//    BienSoXe VARCHAR(20),
//    LoaiVe VARCHAR(50),
//    SoVe VARCHAR(20),
//    ThoiGianVao DATETIME,
//    TrangThaiVe ENUM('ChuaTra', 'DaTra') DEFAULT 'ChuaTra',
//    ThoiGianRa DATETIME,
//    TienPhat DOUBLE DEFAULT 0

//);

namespace PhanMemNVSoatVe
{
    public partial class frmPhanMemChoNVSoatVe : Form
    {
        public frmPhanMemChoNVSoatVe()
        {
            InitializeComponent();
        }

        private bool barrierVaoDangMo = false;
        private bool barrierRaDangMo = false;

        private void CapNhatTrangThaiBarrier()
        {
            lblTrangThaiVao.BackColor = barrierVaoDangMo ? Color.LimeGreen : Color.Red;
            lblTrangThaiRa.BackColor = barrierRaDangMo ? Color.LimeGreen : Color.Red;
        }


        private void txtSoVeVao_Enter_1(object sender, EventArgs e)
        {
            // Nếu ô đã có giá trị thì nhân viên muốn gõ tay, bỏ qua
            if (!string.IsNullOrWhiteSpace(txtSoVeVao.Text))
                return;

            try
            {
                string chuoiKetNoi =
                    "server=localhost;database=PhanMemQuanLyBaiGuiXe;" +
                    "user=root;password=3010D@ngth@im1nhvu2005;";
                using (MySqlConnection conn = new MySqlConnection(chuoiKetNoi))
                {
                    conn.Open();

                    // 1) Tìm số vé đã trả nhỏ nhất mà hiện không có bản ghi ChuaTra
                    string sqlMinDaTra = @"
                        SELECT MIN(CAST(SoVe AS UNSIGNED))
                        FROM DuLieuXeVao
                        WHERE TrangThaiVe = 'DaTra'
                          AND SoVe NOT IN (
                              SELECT SoVe FROM DuLieuXeVao WHERE TrangThaiVe = 'ChuaTra'
                          )";
                    MySqlCommand cmdMin = new MySqlCommand(sqlMinDaTra, conn);
                    object kqMin = cmdMin.ExecuteScalar();

                    int soVeDefault;
                    if (kqMin != DBNull.Value && kqMin != null)
                    {
                        soVeDefault = Convert.ToInt32(kqMin);
                    }
                    else
                    {
                        // 2) Nếu không còn vé đã trả sẵn nào, cấp vé mới: MAX(SoVe)+1
                        string sqlMax = @"
                            SELECT COALESCE(MAX(CAST(SoVe AS UNSIGNED)), 0) + 1
                            FROM DuLieuXeVao";
                        MySqlCommand cmdMax = new MySqlCommand(sqlMax, conn);
                        soVeDefault = Convert.ToInt32(cmdMax.ExecuteScalar());
                    }

                    txtSoVeVao.Text = soVeDefault.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi khi lấy số vé xe vào mặc định: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void lblLanVao_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void cbxLoaiXe_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tmrThoiGianVao_Tick(object sender, EventArgs e)
        {
            lblThoiGianVao.Text = DateTime.Now.ToString("dd/MM/yyyy | H:mm:ss");
        }

        private void lblThoiGian_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblBienSoRa_Click(object sender, EventArgs e)
        {

        }

        private void txtBienSoVao_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSoVeVao_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSoVeRa_TextChanged(object sender, EventArgs e)
        {
            string soVeRa = txtSoVeRa.Text.Trim();

            // nếu ô trống thì xoá hết nhãn
            if (string.IsNullOrEmpty(soVeRa))
            {
                lblBienSoRa.Text = "";
                lblLoaiVeRa.Text = "";
                lblThoiGianRa.Text = "";
                lblPhatMuon.Text = "";
                return;
            }

            string connectionString = "server=localhost;database=PhanMemQuanLyBaiGuiXe;user=root;password=3010D@ngth@im1nhvu2005;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = @"
                    SELECT BienSoXe, LoaiVe, ThoiGianVao 
                    FROM DuLieuXeVao 
                    WHERE SoVe = @SoVe 
                      AND TrangThaiVe = 'ChuaTra' 
                    LIMIT 1";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SoVe", soVeRa);

                try
                {
                    conn.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // hiển thị thông tin cơ bản
                            DateTime thoiGianVao = Convert.ToDateTime(reader["ThoiGianVao"]);
                            lblBienSoRa.Text = reader["BienSoXe"].ToString();
                            lblLoaiVeRa.Text = reader["LoaiVe"].ToString();
                            lblThoiGianRa.Text = thoiGianVao.ToString("dd/MM/yyyy | H:mm:ss");

                            // tính tiền phạt
                            DateTime gio22cuaNgayVao = new DateTime(
                                thoiGianVao.Year,
                                thoiGianVao.Month,
                                thoiGianVao.Day,
                                22, 0, 0);
                            DateTime thoiGianRa = DateTime.Now;
                            TimeSpan chenLe = thoiGianRa - gio22cuaNgayVao;
                            double x = chenLe.TotalHours; 
                            double f0 = 5000;              
                            double y = 10000;            

                            double tienPhat = 0;
                            if (x > 0)
                            {
                                if (x < 1)
                                    tienPhat = f0;
                                else
                                    tienPhat = Math.Floor(x) * y;
                            }

                            if (tienPhat > 0)
                                lblPhatMuon.Text = $"{tienPhat:N0} VND | lấy xe muộn quá ({Math.Max(0, (int)Math.Floor(x))} giờ)";
                            else
                                lblPhatMuon.Text = "0 VND";
                        }
                        else
                        {
                            lblBienSoRa.Text = "";
                            lblLoaiVeRa.Text = "";
                            lblThoiGianRa.Text = "";
                            lblPhatMuon.Text = "";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lấy dữ liệu: " + ex.Message,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void lblLoaiVeRa_Click(object sender, EventArgs e)
        {

        }

        private void lblThoiGianRa_Click(object sender, EventArgs e)
        {

        }

        private void btnMoBarrier_Click_1(object sender, EventArgs e)
        {
            string bienSo = txtBienSoVao.Text;
            string loaiVe = cbxLoaiVeVao.SelectedItem?.ToString();
            string soVe = txtSoVeVao.Text;
            DateTime thoiGianVao = DateTime.Now;

            if (string.IsNullOrWhiteSpace(bienSo) ||
            string.IsNullOrWhiteSpace(soVe) ||
            string.IsNullOrWhiteSpace(loaiVe))
            {
                MessageBox.Show(
                    "Vui long nhap day du thong tin: Bien so, Loai ve, So ve.",
                    "Thong bao",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            string connectionString = "server=localhost;database=PhanMemQuanLyBaiGuiXe;user=root;password=3010D@ngth@im1nhvu2005;";

            if(barrierVaoDangMo)
            {
                MessageBox.Show(
                    "Vui lòng đóng barrier cổng vào trước khi cấp vé mới.",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // 1. Kiem tra xem da co ve dang 'ChuaTra' voi cung SoVe chua
                    string queryKiemTra = @"
                        SELECT COUNT(*) 
                        FROM DuLieuXeVao 
                        WHERE SoVe = @SoVe 
                          AND TrangThaiVe = 'ChuaTra'";
                    MySqlCommand cmdKiemTra = new MySqlCommand(queryKiemTra, conn);
                    cmdKiemTra.Parameters.AddWithValue("@SoVe", soVe);

                    int soLuongChuaTra = Convert.ToInt32(cmdKiemTra.ExecuteScalar());
                    if (soLuongChuaTra > 0)
                    {
                        MessageBox.Show(
                            "Ve so \"" + soVe + "\" van chua duoc tra. Khong the dung lai.",
                            "Canh bao",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return;
                    }

                    // 2. Neu chua co ve chua tra, thi cho phep them ban ghi moi
                    string queryInsert = @"
                INSERT INTO DuLieuXeVao 
                    (BienSoXe, LoaiVe, SoVe, ThoiGianVao) 
                VALUES 
                    (@BienSo, @LoaiVe, @SoVe, @ThoiGianVao)";
                    MySqlCommand cmdInsert = new MySqlCommand(queryInsert, conn);
                    cmdInsert.Parameters.AddWithValue("@BienSo", bienSo);
                    cmdInsert.Parameters.AddWithValue("@LoaiVe", loaiVe);
                    cmdInsert.Parameters.AddWithValue("@SoVe", soVe);
                    cmdInsert.Parameters.AddWithValue("@ThoiGianVao", thoiGianVao);

                    cmdInsert.ExecuteNonQuery();
                    barrierVaoDangMo = true;
                    CapNhatTrangThaiBarrier();

                    txtBienSoVao.Text = "";
                    cbxLoaiVeVao.SelectedIndex = -1;
                    txtSoVeVao.Text = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Loi khi xu ly du lieu: " + ex.Message,
                        "Loi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void btnDongBarrier_Click(object sender, EventArgs e)
        {
            barrierVaoDangMo = false;
            CapNhatTrangThaiBarrier();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string soVeRa = txtSoVeRa.Text.Trim();
            if (string.IsNullOrEmpty(soVeRa))
            {
                MessageBox.Show("Vui lòng nhập số vé và kiểm tra thông tin trước khi mở cổng.",
                                "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (barrierRaDangMo)
            {
                MessageBox.Show("Vui lòng đóng barrier cổng ra trước khi thực hiện lần kế tiếp.",
                                "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Lấy ThoiGianVao từ CSDL
            DateTime thoiGianVao;
            string cs = @"
                server=localhost;
                database=PhanMemQuanLyBaiGuiXe;
                user=root;
                password=3010D@ngth@im1nhvu2005;
                SslMode=None;
                AllowPublicKeyRetrieval=True;
            ";
            using (var conn = new MySqlConnection(cs))
            {
                conn.Open();
                var cmdSelect = new MySqlCommand(
                    "SELECT ThoiGianVao FROM DuLieuXeVao WHERE SoVe=@SoVe AND TrangThaiVe='ChuaTra' LIMIT 1",
                    conn);
                cmdSelect.Parameters.AddWithValue("@SoVe", soVeRa);
                object kq = cmdSelect.ExecuteScalar();
                if (kq == null)
                {
                    MessageBox.Show("Số vé không hợp lệ hoặc đã trả.", "Lỗi",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                thoiGianVao = Convert.ToDateTime(kq);

                // Tính tienPhat
                DateTime mốc22h = new DateTime(
                    thoiGianVao.Year, thoiGianVao.Month, thoiGianVao.Day, 22, 0, 0);
                DateTime thoiGianRa = DateTime.Now;
                double x = (thoiGianRa - mốc22h).TotalHours;
                double tienPhat = 0;
                double f0 = 5000, y = 10000;
                if (x > 0)
                {
                    tienPhat = x < 1 ? f0 : Math.Floor(x) * y;
                }

                // Cập nhật lại bản ghi
                var cmdUpdate = new MySqlCommand(@"
                    UPDATE DuLieuXeVao
                    SET ThoiGianRa = @ThoiGianRa,
                        TrangThaiVe = 'DaTra',
                        TienPhat   = @TienPhat
                    WHERE SoVe = @SoVe AND TrangThaiVe = 'ChuaTra'", conn);
                cmdUpdate.Parameters.AddWithValue("@ThoiGianRa", thoiGianRa);
                cmdUpdate.Parameters.AddWithValue("@TienPhat", tienPhat);
                cmdUpdate.Parameters.AddWithValue("@SoVe", soVeRa);
                int rows = cmdUpdate.ExecuteNonQuery();

                if (rows > 0)
                {
                    lblPhatMuon.Text = $"{tienPhat:N0} VND";
                    txtSoVeRa.Text = "";
                    lblBienSoRa.Text = "";
                    lblLoaiVeRa.Text = "";
                    lblThoiGianRa.Text = "";

                    barrierRaDangMo = true;
                    CapNhatTrangThaiBarrier();
                }
                else
                {
                    MessageBox.Show("Cập nhật không thành công.", "Lỗi",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            barrierRaDangMo = false;
            CapNhatTrangThaiBarrier();
        }

        private void lblTrangThaiVao_Click(object sender, EventArgs e)
        {
        }

        private void lblTrangThaiRa_Click(object sender, EventArgs e)
        {
        }

        private void lblPhatMuon_Click(object sender, EventArgs e)
        {

        }

    }
}
