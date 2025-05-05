using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace PhanMemNVSoatVe
{
    public partial class frmPhanMemNVQuanLy : Form

    {
        private MySqlDataAdapter adapter;
        private DataTable dt;

        public frmPhanMemNVQuanLy()
        {
            InitializeComponent();
            this.Load += frmPhanMemNVQuanLyKhachHang_Load;

            // Cấu hình hiển thị ngày giờ cho DateTimePicker ở phần thêm mới
            dtpNhapThoiGianVao.Format = DateTimePickerFormat.Custom;
            dtpNhapThoiGianVao.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpNhapThoiGianVao.ShowUpDown = true;

            dtpNhapThoiGianRa.Format = DateTimePickerFormat.Custom;
            dtpNhapThoiGianRa.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpNhapThoiGianRa.ShowUpDown = true;

            // Cấu hình hiển thị ngày giờ cho DateTimePicker ở phần chỉnh sửa
            dtpChinhSuaThoiGianVao.Format = DateTimePickerFormat.Custom;
            dtpChinhSuaThoiGianVao.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpChinhSuaThoiGianVao.ShowUpDown = true;

            dtpChinhSuaThoiGianRa.Format = DateTimePickerFormat.Custom;
            dtpChinhSuaThoiGianRa.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpChinhSuaThoiGianRa.ShowUpDown = true;

            btnTimKiem.Click += (s, e) => ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (dt == null) return;

            var filters = new List<string>();

            // 1. Biển số xe
            var bs = txtBienSoXe.Text.Trim().Replace("'", "''");
            if (!string.IsNullOrEmpty(bs))
                filters.Add($"BienSoXe LIKE '%{bs}%'");

            // 2. Loại vé
            if (cbxLoaiVe.SelectedIndex >= 0)
            {
                var lv = cbxLoaiVe.SelectedItem.ToString().Replace("'", "''");
                filters.Add($"LoaiVe = '{lv}'");
            }

            // 3. Số vé
            var sv = txtSoVe.Text.Trim();
            if (!string.IsNullOrEmpty(sv) && int.TryParse(sv, out _))
                filters.Add($"SoVe = {sv}");

            // 4. Thời gian vào (nhập dạng yyyy-MM-dd hoặc bất cứ định dạng nào DateTime.TryParse chấp nhận)
            var vaoText = txtThoiGianVao.Text.Trim();
            if (!string.IsNullOrEmpty(vaoText)
                && DateTime.TryParse(vaoText, out var vaoDt))
            {
                // Lọc theo cả ngày của ThoiGianVao
                var d = vaoDt.Date;
                var next = d.AddDays(1);
                filters.Add($"ThoiGianVao >= #{d:MM/dd/yyyy}# AND ThoiGianVao < #{next:MM/dd/yyyy}#");
            }

            // 5. Thời gian ra
            var raText = txtThoiGianRa.Text.Trim();
            if (!string.IsNullOrEmpty(raText)
                && DateTime.TryParse(raText, out var raDt))
            {
                var d2 = raDt.Date;
                var next2 = d2.AddDays(1);
                filters.Add($"ThoiGianRa >= #{d2:MM/dd/yyyy}# AND ThoiGianRa < #{next2:MM/dd/yyyy}#");
            }

            // 6. Trạng thái Vé
            if (chkTimKiemDaTra.Checked && !chkTimKiemChuaTra.Checked)
                filters.Add("TrangThaiVe = 'DaTra'");
            else if (!chkTimKiemDaTra.Checked && chkTimKiemChuaTra.Checked)
                filters.Add("TrangThaiVe = 'ChuaTra'");

            // Áp dụng filter — nếu không có điều kiện nào, RowFilter = "" để hiện toàn bộ
            dt.DefaultView.RowFilter = filters.Count > 0
                ? string.Join(" AND ", filters)
                : string.Empty;
        }



        private void frmPhanMemNVQuanLyKhachHang_Load(object sender, EventArgs e)
        {
            string cs = @"server=localhost;database=Phanmemquanlybaiguixe;
                     uid=root;pwd=3010D@ngth@im1nhvu2005;";
            string query = "SELECT * FROM dulieuxevao";

            try
            {
                using (var conn = new MySqlConnection(cs))
                using (var cmd = new MySqlCommand(query, conn))
                {
                    adapter = new MySqlDataAdapter(cmd);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    grdThongTinKhachHang.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi load dữ liệu: " + ex.Message);
            }
        }

        private void mySqlDataAdapter1_RowUpdated(object sender, MySqlRowUpdatedEventArgs e)
        {

        }

        private void mySqlDataAdapter1_RowUpdating(object sender, MySqlRowUpdatingEventArgs e)
        {

        }

        private void grd_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtBienSoXe_TextChanged(object sender, EventArgs e)
        {

        }

        private void cbxLoaiVe_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtSoVe_TextChanged(object sender, EventArgs e)
        {

        }

        private void chkTimKiemDaTra_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkTimKiemChuaTra_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void txtThoiGianVao_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtThoiGianRa_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtNhapBienSo_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtNhapSoVe_TextChanged(object sender, EventArgs e)
        {

        }
                
        private void cbsNhapLoaiVe_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtNhapThoiGianVao_TextChanged(object sender, EventArgs e)
        {

        }

        private void cbxNhapTrangThaiVe_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtNhapThoiGianRa_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtNhapTienPhat_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnThemDuLieu_Click(object sender, EventArgs e)
        {
            string bienSo = txtNhapBienSo.Text.Trim();
            string loaiVe = cbxNhapLoaiVe.SelectedItem?.ToString();
            string soVe = txtNhapSoVe.Text.Trim();
            DateTime thoiGianVao = dtpNhapThoiGianVao.Value;
            string trangThaiVe = cbxNhapTrangThaiVe.SelectedItem?.ToString();
            DateTime? thoiGianRa = null;
            if (trangThaiVe == "DaTra")
            {
                thoiGianRa = dtpNhapThoiGianRa.Value;
            }
            decimal tienPhat = 0;
            if (!string.IsNullOrWhiteSpace(txtNhapTienPhat.Text) && !decimal.TryParse(txtNhapTienPhat.Text.Trim(), out tienPhat))
            {
                MessageBox.Show("Tien phat khong dung dinh dang!");
                return;
            }

            // Kiểm tra dữ liệu bắt buộc
            if (string.IsNullOrWhiteSpace(bienSo) ||
                string.IsNullOrWhiteSpace(loaiVe) ||
                string.IsNullOrWhiteSpace(soVe))
            {
                MessageBox.Show("Vui long nhap day du thong tin!");
                return;
            }
            if (trangThaiVe == "DaTra" && thoiGianRa == null)
            {
                MessageBox.Show("Trang thai ve la 'DaTra' thi phai nhap Thoi Gian Ra!");
                return;
            }
            if (string.IsNullOrWhiteSpace(trangThaiVe))
                trangThaiVe = "ChuaTra";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"INSERT INTO DuLieuXeVao
                        (BienSoXe, LoaiVe, SoVe, ThoiGianVao, TrangThaiVe, ThoiGianRa, TienPhat)
                        VALUES
                        (@BienSoXe, @LoaiVe, @SoVe, @ThoiGianVao, @TrangThaiVe, @ThoiGianRa, @TienPhat)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@BienSoXe", bienSo);
                        cmd.Parameters.AddWithValue("@LoaiVe", loaiVe);
                        cmd.Parameters.AddWithValue("@SoVe", soVe);
                        cmd.Parameters.AddWithValue("@ThoiGianVao", thoiGianVao);
                        cmd.Parameters.AddWithValue("@TrangThaiVe", trangThaiVe);
                        if (thoiGianRa.HasValue)
                            cmd.Parameters.AddWithValue("@ThoiGianRa", thoiGianRa.Value);
                        else
                            cmd.Parameters.AddWithValue("@ThoiGianRa", DBNull.Value);
                        cmd.Parameters.AddWithValue("@TienPhat", tienPhat);

                        int result = cmd.ExecuteNonQuery();
                        MessageBox.Show(result > 0 ? "Them du lieu thanh cong!" : "Them du lieu that bai!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Loi: " + ex.Message);
                }
            }
        }


        private void btnReset_Click(object sender, EventArgs e)
        {
            txtNhapBienSo.Clear();
            txtNhapSoVe.Clear();
            cbxNhapLoaiVe.SelectedIndex = -1;
            cbxNhapTrangThaiVe.SelectedIndex = -1;
            dtpNhapThoiGianVao.Value = DateTime.Now;
            dtpNhapThoiGianRa.Value = DateTime.Now;
            txtNhapTienPhat.Clear();
        }

        private void txtNhapSoVe_Enter_1(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtNhapSoVe.Text))
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

                    txtNhapSoVe.Text = soVeDefault.ToString();
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

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void XoaThongTinHienThi()
        {
            lblThonTinBienSo.Text = string.Empty;
            lblThongTinLoaiVe.Text = string.Empty;
            lblThongTinSoVe.Text = string.Empty;
            lblThongTinThoiGianVao.Text = string.Empty;
            lblThongTinTrangThaiVe.Text = string.Empty;
            lblThongTinThoiGianRa.Text = string.Empty;
            lblThongTinTienPhat.Text = string.Empty;

            txtChinhSuaBienSo.Clear();
            txtChinhSuaSoVe.Clear();
            cbxChinhSuaLoaiVe.SelectedIndex = -1;
            cbxChinhSuaTrangThaiVe.SelectedIndex = -1;
            dtpChinhSuaThoiGianVao.Value = DateTime.Now;
            dtpChinhSuaThoiGianRa.Value = DateTime.Now;
            txtChinhSuaTienPhat.Clear();
        }

        private string connectionString = "Server=localhost;Database=phanmemquanlybaiguixe;Uid=root;Pwd=3010D@ngth@im1nhvu2005;";

        private void txtNhapID_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNhapID.Text))
            {
                XoaThongTinHienThi();
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM DuLieuXeVao WHERE ID = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtNhapID.Text.Trim());
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblThonTinBienSo.Text = reader["BienSoXe"].ToString();
                                lblThongTinLoaiVe.Text = reader["LoaiVe"].ToString();
                                lblThongTinSoVe.Text = reader["SoVe"].ToString();
                                lblThongTinThoiGianVao.Text = Convert.ToDateTime(reader["ThoiGianVao"]).ToString("G");
                                lblThongTinTrangThaiVe.Text = reader["TrangThaiVe"].ToString();
                                lblThongTinThoiGianRa.Text = reader["ThoiGianRa"] == DBNull.Value ? "" : Convert.ToDateTime(reader["ThoiGianRa"]).ToString("G");
                                lblThongTinTienPhat.Text = reader["TienPhat"].ToString();

                                txtChinhSuaBienSo.Text = reader["BienSoXe"].ToString();
                                txtChinhSuaSoVe.Text = reader["SoVe"].ToString();
                                cbxChinhSuaLoaiVe.Text = reader["LoaiVe"].ToString();
                                cbxChinhSuaTrangThaiVe.Text = reader["TrangThaiVe"].ToString();
                                dtpChinhSuaThoiGianVao.Value = Convert.ToDateTime(reader["ThoiGianVao"]);
                                if (reader["ThoiGianRa"] != DBNull.Value)
                                    dtpChinhSuaThoiGianRa.Value = Convert.ToDateTime(reader["ThoiGianRa"]);
                                txtChinhSuaTienPhat.Text = reader["TienPhat"].ToString();
                            }
                            else
                            {
                                XoaThongTinHienThi();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi khi lay du lieu: " + ex.Message);
            }
        }

        private void lblThonTinBienSo_Click(object sender, EventArgs e)
        {

        }

        private void lblThongTinLoaiVe_Click(object sender, EventArgs e)
        {

        }

        private void lblThongTinSoVe_Click(object sender, EventArgs e)
        {

        }

        private void lblThongTinThoiGianVao_Click(object sender, EventArgs e)
        {

        }

        private void lblThongTinThoiGianRa_Click(object sender, EventArgs e)
        {

        }

        private void lblThongTinTienPhat_Click(object sender, EventArgs e)
        {

        }

        private void btnXoaThongTin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNhapID.Text))
            {
                MessageBox.Show("Vui long nhap ID de xoa.");
                return;
            }

            if (MessageBox.Show("Ban co chac muon xoa thong tin nay?",
                                "Xac nhan",
                                MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        // Xoá bản ghi
                        using (var del = new MySqlCommand(
                            "DELETE FROM DuLieuXeVao WHERE ID = @id", conn, tran))
                        {
                            del.Parameters.AddWithValue("@id", txtNhapID.Text.Trim());
                            if (del.ExecuteNonQuery() == 0)
                                throw new Exception("Khong tim thay ban ghi de xoa.");
                        }

                        // Tái đánh lại ID bằng bảng tạm
                        using (var reseed = new MySqlCommand(@"
                            CREATE TEMPORARY TABLE tmp AS
                              SELECT ID AS oldID,
                                     (@count := @count + 1) AS newID
                              FROM DuLieuXeVao
                              CROSS JOIN (SELECT @count := 0) AS vars
                              ORDER BY ID;
                            UPDATE DuLieuXeVao AS d
                            JOIN tmp AS t ON d.ID = t.oldID
                            SET d.ID = t.newID;
                            DROP TEMPORARY TABLE tmp;
                        ", conn, tran))
                        {
                            reseed.ExecuteNonQuery();
                        }

                        // Reset AUTO_INCREMENT ve 1
                        using (var ai = new MySqlCommand(
                            "ALTER TABLE DuLieuXeVao AUTO_INCREMENT = 1", conn, tran))
                        {
                            ai.ExecuteNonQuery();
                        }

                        tran.Commit();
                        MessageBox.Show("Xoa thanh cong va cap nhat lai ID.");

                        txtNhapID.Clear();
                        XoaThongTinHienThi();
                    }
                }
            }
            catch (MySqlException mex)
            {
                MessageBox.Show($"Loi MySQL ({mex.Number}): {mex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi khi xoa du lieu: " + ex.Message);
            }
        }


        private void btnLuuThonTin_Click(object sender, EventArgs e)
        {
            // Kiem tra thong tin khong de trong
            if (string.IsNullOrWhiteSpace(txtNhapID.Text) ||
                string.IsNullOrWhiteSpace(txtChinhSuaBienSo.Text) ||
                string.IsNullOrWhiteSpace(txtChinhSuaSoVe.Text) ||
                string.IsNullOrWhiteSpace(cbxChinhSuaLoaiVe.Text) ||
                string.IsNullOrWhiteSpace(cbxChinhSuaTrangThaiVe.Text))
            {
                MessageBox.Show("Vui long khong de thong tin nao trong khi luu.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string updateQuery = @"
                        UPDATE DuLieuXeVao SET 
                            BienSoXe = @bienSo,
                            LoaiVe = @loaiVe,
                            SoVe = @soVe,
                            ThoiGianVao = @tgVao,
                            TrangThaiVe = @trangThai,
                            ThoiGianRa = @tgRa,
                            TienPhat = @tienPhat
                        WHERE ID = @id";

                    using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@bienSo", txtChinhSuaBienSo.Text.Trim());
                        cmd.Parameters.AddWithValue("@loaiVe", cbxChinhSuaLoaiVe.Text);
                        cmd.Parameters.AddWithValue("@soVe", txtChinhSuaSoVe.Text.Trim());
                        cmd.Parameters.AddWithValue("@tgVao", dtpChinhSuaThoiGianVao.Value);
                        cmd.Parameters.AddWithValue("@trangThai", cbxChinhSuaTrangThaiVe.Text);
                        cmd.Parameters.AddWithValue("@tgRa", dtpChinhSuaThoiGianRa.Value);
                        cmd.Parameters.AddWithValue("@tienPhat", Convert.ToDouble(txtChinhSuaTienPhat.Text.Trim()));
                        cmd.Parameters.AddWithValue("@id", txtNhapID.Text.Trim());

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("Cap nhat thanh cong.");
                            txtNhapID_TextChanged(null, null);
                        }
                        else
                        {
                            MessageBox.Show("Cap nhat khong thanh cong. Vui long kiem tra lai ID.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi khi luu du lieu: " + ex.Message);
            }
        }

        private void cbxChinhSuaLoaiVe_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void LayThongTinTuCSDL()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM DuLieuXeVao WHERE ID = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtNhapID.Text.Trim());
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblThonTinBienSo.Text = reader["BienSoXe"].ToString();
                                lblThongTinLoaiVe.Text = reader["LoaiVe"].ToString();
                                lblThongTinSoVe.Text = reader["SoVe"].ToString();
                                lblThongTinThoiGianVao.Text = Convert.ToDateTime(reader["ThoiGianVao"]).ToString("G");
                                lblThongTinTrangThaiVe.Text = reader["TrangThaiVe"].ToString();
                                lblThongTinThoiGianRa.Text = reader["ThoiGianRa"] == DBNull.Value ? "" : Convert.ToDateTime(reader["ThoiGianRa"]).ToString("G");
                                lblThongTinTienPhat.Text = reader["TienPhat"].ToString();

                                txtChinhSuaBienSo.Text = reader["BienSoXe"].ToString();
                                txtChinhSuaSoVe.Text = reader["SoVe"].ToString();
                                cbxChinhSuaLoaiVe.Text = reader["LoaiVe"].ToString();
                                cbxChinhSuaTrangThaiVe.Text = reader["TrangThaiVe"].ToString();
                                dtpChinhSuaThoiGianVao.Value = Convert.ToDateTime(reader["ThoiGianVao"]);
                                if (reader["ThoiGianRa"] != DBNull.Value)
                                    dtpChinhSuaThoiGianRa.Value = Convert.ToDateTime(reader["ThoiGianRa"]);
                                txtChinhSuaTienPhat.Text = reader["TienPhat"].ToString();
                            }
                            else
                            {
                                XoaThongTinHienThi();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi khi lay du lieu: " + ex.Message);
            }
        }

        private void btnVietLai_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNhapID.Text))
            {
                MessageBox.Show("Vui long nhap ID truoc khi reset.");
                return;
            }

            LayThongTinTuCSDL();
        }
    }
}
