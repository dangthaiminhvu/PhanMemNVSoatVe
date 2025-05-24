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
using System.Text.RegularExpressions;


namespace PhanMemNVSoatVe
{
    public partial class frmPhanMemNVQuanLyKhachHang : Form

    {
        private readonly string _connectionString =
            "server=localhost;database=PhanMemQuanLyBaiGuiXe;uid=root;pwd=3010D@ngth@im1nhvu2005;";

        private MySqlConnection GetConnection() => new MySqlConnection(_connectionString);

        private MySqlDataAdapter adapter;
        private DataTable dt;

        public frmPhanMemNVQuanLyKhachHang()
        {
            InitializeComponent();
            this.Load += frmPhanMemNVQuanLyKhachHang_Load;
            tab1.SelectedIndexChanged += tab1_SelectedIndexChanged;

            // Định dạng DateTimePicker
            foreach (var dtp in new[] { dtpNhapThoiGianVao, dtpNhapThoiGianRa,
                                         dtpChinhSuaThoiGianVao, dtpChinhSuaThoiGianRa })
            {
                dtp.Format = DateTimePickerFormat.Custom;
                dtp.CustomFormat = "dd/MM/yyyy HH:mm:ss";
                dtp.ShowUpDown = true;
            }

            // Sự kiện nút
            btnTimKiem.Click += (s, e) => ApplyFilter();
            btnNhapLaiTimKiem.Click += btnNhapLaiTimKiem_Click;
            btnThemDuLieu.Click += btnThemDuLieu_Click;
            ConfigureGrid();
        }

        private void tab1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tab1.SelectedTab != null && tab1.SelectedTab.Name == "tabtrang1")
            {
                LoadGridData();
            }
        }

        private void ConfigureGrid()
        {
            grdThongTinKhachHang.ReadOnly = true;
            grdThongTinKhachHang.AllowUserToAddRows = false;
            grdThongTinKhachHang.AllowUserToDeleteRows = false;
            grdThongTinKhachHang.AllowUserToOrderColumns = false;
            grdThongTinKhachHang.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdThongTinKhachHang.MultiSelect = false;
            grdThongTinKhachHang.EditMode = DataGridViewEditMode.EditProgrammatically;
        }

        private void ApplyFilter()
        {
            if (dt == null) return;
            var filters = new List<string>();

            // Biển số
            var bs = txtBienSoXe.Text.Trim().Replace("'", "''");
            if (!string.IsNullOrEmpty(bs))
                filters.Add($"BienSoXe LIKE '%{bs}%'");

            // Loại vé
            if (cbxLoaiVe.SelectedIndex >= 0)
                filters.Add($"LoaiVe = '{cbxLoaiVe.Text.Replace("'", "''")}'");

            // Số vé
            if (int.TryParse(txtSoVe.Text.Trim(), out var soVe))
                filters.Add($"SoVe = {soVe}");

            // Thời gian
            AddDateFilter(txtThoiGianVao, "ThoiGianVao", filters);
            AddDateFilter(txtThoiGianRa, "ThoiGianRa", filters);

            // Trạng thái
            if (chkDaTra.Checked ^ chkChuaTra.Checked)
                filters.Add(chkDaTra.Checked ? "TrangThaiVe = 'DaTra'" : "TrangThaiVe = 'ChuaTra'");

            dt.DefaultView.RowFilter = filters.Any() ? string.Join(" AND ", filters) : string.Empty;
        }

        private void AddDateFilter(TextBox txt, string column, List<string> filters)
        {
            if (DateTime.TryParse(txt.Text.Trim(), out var dtp))
            {
                var d = dtp.Date;
                filters.Add($"{column} >= '{d:yyyy-MM-dd}' AND {column} < '{d.AddDays(1):yyyy-MM-dd}'");
            }
        }

        private void frmPhanMemNVQuanLyKhachHang_Load(object sender, EventArgs e)
        {
            LoadGridData();
        }

        private void LoadGridData()
        {
            const string query = "SELECT * FROM DuLieuXeVao";
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var cmd = new MySqlCommand(query, conn))
                using (var adapter = new MySqlDataAdapter(cmd))
                {
                    dt = new DataTable();
                    adapter.Fill(dt);
                    grdThongTinKhachHang.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load dữ liệu: {ex.Message}");
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void btnNhapLaiTimKiem_Click(object sender, EventArgs e)
        {
            // Xóa các điều kiện tìm kiếm trên tab "tìm kiếm"
            txtBienSoXe.Clear();
            cbxLoaiVe.SelectedIndex = -1;
            txtSoVe.Clear();
            txtThoiGianVao.Clear();
            txtThoiGianRa.Clear();
            chkDaTra.Checked = chkChuaTra.Checked = false;
            // Hiển thị lại toàn bộ dữ liệu nếu đã load
            if (dt != null)
                dt.DefaultView.RowFilter = string.Empty;
        }

        private void btnThemDuLieu_Click(object sender, EventArgs e)
        {
            var bienSo = txtNhapBienSo.Text.Trim();
            var loaiVe = cbxNhapLoaiVe.Text;
            var soVe = txtNhapSoVe.Text.Trim();
            var tgVao = dtpNhapThoiGianVao.Value;
            var trangThai = cbxNhapTrangThaiVe.Text;
            DateTime? tgRa = trangThai == "DaTra" ? dtpNhapThoiGianRa.Value : (DateTime?)null;
            if (!decimal.TryParse(txtNhapTienPhat.Text.Trim(), out var tienPhat))
                tienPhat = 0;

            // Validate cơ bản
            if (string.IsNullOrWhiteSpace(bienSo) || !ValidateBienSo(bienSo))
            {
                MessageBox.Show(string.IsNullOrWhiteSpace(bienSo)
                    ? "Vui lòng nhập biển số xe." : $"Biển số '{bienSo}' không hợp lệ.",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(loaiVe) || string.IsNullOrWhiteSpace(soVe))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (trangThai == "DaTra" && tgRa == null)
            {
                MessageBox.Show("Nếu vé đã trả, phải nhập Thời gian Ra.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    // Kiểm tra trùng SoVe hoặc ID
                    using (var checkCmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM DuLieuXeVao WHERE SoVe=@sv OR ID=@id", conn))
                    {
                        checkCmd.Parameters.AddWithValue("@sv", soVe);
                        checkCmd.Parameters.AddWithValue("@id", txtNhapID.Text.Trim());
                        int exist = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (exist > 0)
                        {
                            MessageBox.Show("SoVe hoặc ID đã tồn tại, không thể thêm.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    // Kiểm tra BienSoXe đang gửi chưa trả
                    using (var checkBs = new MySqlCommand(
                        "SELECT COUNT(*) FROM DuLieuXeVao WHERE BienSoXe=@bs AND TrangThaiVe='ChuaTra'", conn))
                    {
                        checkBs.Parameters.AddWithValue("@bs", bienSo);
                        int cnt = Convert.ToInt32(checkBs.ExecuteScalar());
                        if (cnt > 0)
                        {
                            MessageBox.Show("Xe này đang gửi và chưa trả, không thể thêm vé mới.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    const string insert = @"
                        INSERT INTO DuLieuXeVao (BienSoXe, LoaiVe, SoVe, ThoiGianVao, TrangThaiVe, ThoiGianRa, TienPhat)
                        VALUES (@bs, @lv, @sv, @tgVao, @tt, @tgRa, @tp)";
                    using (var cmd = new MySqlCommand(insert, conn))
                    {
                        cmd.Parameters.AddWithValue("@bs", bienSo);
                        cmd.Parameters.AddWithValue("@lv", loaiVe);
                        cmd.Parameters.AddWithValue("@sv", soVe);
                        cmd.Parameters.AddWithValue("@tgVao", tgVao);
                        cmd.Parameters.AddWithValue("@tt", string.IsNullOrWhiteSpace(trangThai) ? "ChuaTra" : trangThai);
                        cmd.Parameters.AddWithValue("@tgRa", tgRa ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@tp", tienPhat);
                        int result = cmd.ExecuteNonQuery();
                        MessageBox.Show(result > 0 ? "Thêm dữ liệu thành công!" : "Thêm dữ liệu thất bại!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm dữ liệu: {ex.Message}");
            }
        }

        private bool ValidateBienSo(string bienSo)
        {
            const string pat = @"^\d{2}[A-Za-z]-[A-Za-z0-9]{1,2} \d{3,4}(\.\d{2})?$";
            return Regex.IsMatch(bienSo, pat);
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
                using (var conn = GetConnection())
                {
                    conn.Open();
                    // 1) Tìm số vé đã trả nhỏ nhất mà hiện không có bản ghi ChuaTra
                    const string sqlMinDaTra = @"
                        SELECT MIN(CAST(SoVe AS UNSIGNED))
                        FROM DuLieuXeVao
                        WHERE TrangThaiVe = 'DaTra'
                          AND SoVe NOT IN (
                              SELECT SoVe FROM DuLieuXeVao WHERE TrangThaiVe = 'ChuaTra'
                          )";
                    var cmdMin = new MySqlCommand(sqlMinDaTra, conn);
                    var kqMin = cmdMin.ExecuteScalar();

                    int soVeDefault = (kqMin != null && kqMin != DBNull.Value)
                        ? Convert.ToInt32(kqMin)
                        : Convert.ToInt32(
                            new MySqlCommand(
                                "SELECT COALESCE(MAX(CAST(SoVe AS UNSIGNED)), 0) + 1 FROM DuLieuXeVao", conn
                            ).ExecuteScalar()
                        );

                    txtNhapSoVe.Text = soVeDefault.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy số vé mặc định: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void XoaThongTinHienThi()
        {
            lblThonTinBienSo.Text = lblThongTinLoaiVe.Text =
            lblThongTinSoVe.Text = lblThongTinThoiGianVao.Text =
            lblThongTinTrangThaiVe.Text = lblThongTinThoiGianRa.Text =
            lblThongTinTienPhat.Text = string.Empty;

            txtChinhSuaBienSo.Clear();
            txtChinhSuaSoVe.Clear();
            cbxChinhSuaLoaiVe.SelectedIndex = -1;
            cbxChinhSuaTrangThaiVe.SelectedIndex = -1;
            dtpChinhSuaThoiGianVao.Value = DateTime.Now;
            dtpChinhSuaThoiGianRa.Value = DateTime.Now;
            txtChinhSuaTienPhat.Clear();
        }

        private void txtNhapID_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNhapID.Text))
            {
                XoaThongTinHienThi();
                return;
            }

            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    const string query = "SELECT * FROM DuLieuXeVao WHERE ID = @id";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtNhapID.Text.Trim());
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                XoaThongTinHienThi();
                                return;
                            }

                            lblThonTinBienSo.Text = reader["BienSoXe"].ToString();
                            lblThongTinLoaiVe.Text = reader["LoaiVe"].ToString();
                            lblThongTinSoVe.Text = reader["SoVe"].ToString();
                            lblThongTinThoiGianVao.Text = Convert.ToDateTime(reader["ThoiGianVao"]).ToString("G");
                            lblThongTinTrangThaiVe.Text = reader["TrangThaiVe"].ToString();
                            lblThongTinThoiGianRa.Text = reader["ThoiGianRa"] == DBNull.Value
                                ? string.Empty
                                : Convert.ToDateTime(reader["ThoiGianRa"]).ToString("G");
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
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy dữ liệu: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoaThongTin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNhapID.Text))
            {
                MessageBox.Show("Vui lòng nhập ID để xóa.", "Cảnh báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Bạn có chắc muốn xóa bản ghi này?", "Xác nhận",
                               MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        // Xóa bản ghi
                        using (var del = new MySqlCommand("DELETE FROM DuLieuXeVao WHERE ID = @id", conn, tran))
                        {
                            del.Parameters.AddWithValue("@id", txtNhapID.Text.Trim());
                            if (del.ExecuteNonQuery() == 0)
                                throw new Exception("Không tìm thấy bản ghi cần xóa.");
                        }
                        // (nếu có các bước tái đánh lại ID, đưa vào đây...)
                        tran.Commit();
                    }
                    MessageBox.Show("Xóa thành công và cập nhật ID.", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtNhapID.Clear();
                    XoaThongTinHienThi();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa dữ liệu: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLuuThonTin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNhapID.Text)
                || string.IsNullOrWhiteSpace(txtChinhSuaBienSo.Text)
                || string.IsNullOrWhiteSpace(txtChinhSuaSoVe.Text)
                || string.IsNullOrWhiteSpace(cbxChinhSuaLoaiVe.Text)
                || string.IsNullOrWhiteSpace(cbxChinhSuaTrangThaiVe.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin khi lưu.", "Cảnh báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    const string updateQuery = @"
                        UPDATE DuLieuXeVao SET
                            BienSoXe = @bs,
                            LoaiVe = @lv,
                            SoVe = @sv,
                            ThoiGianVao = @tgVao,
                            TrangThaiVe = @tt,
                            ThoiGianRa = @tgRa,
                            TienPhat = @tp
                        WHERE ID = @id";
                    using (var cmd = new MySqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@bs", txtChinhSuaBienSo.Text.Trim());
                        cmd.Parameters.AddWithValue("@lv", cbxChinhSuaLoaiVe.Text);
                        cmd.Parameters.AddWithValue("@sv", txtChinhSuaSoVe.Text.Trim());
                        cmd.Parameters.AddWithValue("@tgVao", dtpChinhSuaThoiGianVao.Value);
                        cmd.Parameters.AddWithValue("@tt", cbxChinhSuaTrangThaiVe.Text);

                        // Nếu trạng thái là ChuaTra => lưu ThoiGianRa = NULL
                        if (cbxChinhSuaTrangThaiVe.Text == "ChuaTra")
                            cmd.Parameters.AddWithValue("@tgRa", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@tgRa", dtpChinhSuaThoiGianRa.Value);

                        cmd.Parameters.AddWithValue("@tp", Convert.ToDouble(txtChinhSuaTienPhat.Text.Trim()));
                        cmd.Parameters.AddWithValue("@id", txtNhapID.Text.Trim());

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            MessageBox.Show("Cập nhật thành công.", "Thông báo",
                                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Giữ nguyên ở lại tab chỉnh sửa (tabtrang3)
                        }
                        else
                        {
                            MessageBox.Show("Cập nhật không thành công. Kiểm tra lại ID.", "Lỗi",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu dữ liệu: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbxChinhSuaLoaiVe_SelectedIndexChanged(object sender, EventArgs e) { }
        private void btnVietLai_Click(object sender, EventArgs e) { }
        private void gbxTrangThai_Enter(object sender, EventArgs e) { }
        private void lblThonTinBienSo_Click(object sender, EventArgs e) { }
        private void lblThongTinLoaiVe_Click(object sender, EventArgs e) { }
        private void lblThongTinSoVe_Click(object sender, EventArgs e) { }
        private void lblThongTinThoiGianVao_Click(object sender, EventArgs e) { }
        private void lblThongTinThoiGianRa_Click(object sender, EventArgs e) { }
        private void lblThongTinTienPhat_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void txtThoiGianVao_TextChanged(object sender, EventArgs e) { }
        private void txtThoiGianRa_TextChanged(object sender, EventArgs e) { }
        private void txtNhapBienSo_TextChanged(object sender, EventArgs e) { }
        private void txtNhapSoVe_TextChanged(object sender, EventArgs e) { }
        private void cbsNhapLoaiVe_SelectedIndexChanged(object sender, EventArgs e) { }
        private void cbxNhapTrangThaiVe_SelectedIndexChanged(object sender, EventArgs e) { }
        private void txtNhapTienPhat_TextChanged(object sender, EventArgs e) { }
        private void mySqlDataAdapter1_RowUpdated(object sender, MySqlRowUpdatedEventArgs e) { }
        private void mySqlDataAdapter1_RowUpdating(object sender, MySqlRowUpdatingEventArgs e) { }
        private void grd_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void txtBienSoXe_TextChanged(object sender, EventArgs e) { }
        private void cbxLoaiVe_SelectedIndexChanged(object sender, EventArgs e) { }
        private void txtSoVe_TextChanged(object sender, EventArgs e) { }
        private void dtpChinhSuaThoiGianRa_ValueChanged(object sender, EventArgs e) { }
        private void cbxChinhSuaTrangThaiVe_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}
