using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace PhanMemNVSoatVe
{
    public partial class frmPhanMemDanhChoNVQuanLyKhachHang : Form
    {
        private DataTable dt = new DataTable();
        private string chuoiKetNoi =
            @"Server=localhost;
              Database=phanmemquanlybaiguixe;
              Uid=root;
              Pwd=3010D@ngth@im1nhvu2005;
              SslMode=None;
              AllowPublicKeyRetrieval=True;";
        public frmPhanMemDanhChoNVQuanLyKhachHang()
        {
            InitializeComponent();
            KhoiTaoGrid();
            LoadTatCaDuLieu();

            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToOrderColumns = false;  // không cho kèm cặp cột
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;             // chỉ cho phép chọn 1 dòng
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;
        }

        private void KhoiTaoGrid()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            // Cot ID
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ID",
                HeaderText = "ID",
                Name = "colID"
            });
            // Cot BienSoXe
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "BienSoXe",
                HeaderText = "Bien so xe",
                Name = "colBienSo"
            });
            // Cot LoaiVe
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "LoaiVe",
                HeaderText = "Loai ve",
                Name = "colLoaiVe"
            });
            // Cot SoVe
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SoVe",
                HeaderText = "So ve",
                Name = "colSoVe"
            });
            // Cot ThoiGianVao
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ThoiGianVao",
                HeaderText = "Thoi gian vao",
                Name = "colVao"
            });
            // Cot GiaHan
            dataGridView1.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "GiaHan",
                HeaderText = "Gia hạn",
                Name = "colGiaHan",
                ReadOnly = true,
                TrueValue = 1,
                FalseValue = 0
            });
        }

        private void LoadTatCaDuLieu()
        {
            dt.Clear();
            // chuoi ket noi moi gom AllowPublicKeyRetrieval va SslMode=None
            string chuoiKetNoi =
                @"Server=localhost;
                  Database=phanmemquanlybaiguixe;
                  Uid=root;
                  Pwd=3010D@ngth@im1nhvu2005;
                  AllowPublicKeyRetrieval=True;
                  SslMode=None;";

            using (var cn = new MySqlConnection(chuoiKetNoi))
            using (var da = new MySqlDataAdapter(
                "SELECT ID, BienSoXe, LoaiVe, SoVe, ThoiGianVao, GiaHan " +
                "FROM DuLieuXeVao WHERE TrangThaiVe='ChuaTra'", cn))
            {
                // mo ket noi truoc khi fill
                cn.Open();
                da.Fill(dt);
            }
            dataGridView1.DataSource = dt.DefaultView;
        }

        private void txtTimKiemBienSo_TextChanged(object sender, EventArgs e)
        {

        }

        private void cbxTimKiemLoaiVe_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtTimKiemSoVe_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTimKiemThoiGianVao_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnGiaHan_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;
            var id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["colID"].Value);

            var dr = MessageBox.Show(
                "Ban chac chan muon gia han cho ban ghi nay?",
                "Xac nhan gia han",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                using (var cn = new MySqlConnection(chuoiKetNoi))
                using (var cmd = new MySqlCommand(
                    "UPDATE DuLieuXeVao SET GiaHan=1 WHERE ID=@id", cn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadTatCaDuLieu();
            }
            // neu chon No thi khong lam gi ca
        }

        private void btbHuyGiaHan_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;
            var id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["colID"].Value);

            var dr = MessageBox.Show(
                "Ban chac chan muon huy gia han cho ban ghi nay?",
                "Xac nhan huy gia han",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                using (var cn = new MySqlConnection(chuoiKetNoi))
                using (var cmd = new MySqlCommand(
                    "UPDATE DuLieuXeVao SET GiaHan=0 WHERE ID=@id", cn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadTatCaDuLieu();
            }
            // neu chon No thi khong lam gi ca
        }

        private void bthNhapLai_Click(object sender, EventArgs e)
        {
            // 1. Xóa hết các ô nhập
            txtTimKiemBienSo.Clear();
            cbxTimKiemLoaiVe.SelectedIndex = -1;
            txtTimKiemSoVe.Clear();
            txtTimKiemThoiGianVao.Clear();

            // 2. Xóa filter trên DataView
            if (dt != null)
                dt.DefaultView.RowFilter = string.Empty;

            // 3. (Tùy chọn) Reload dữ liệu gốc từ CSDL
            LoadTatCaDuLieu();

            // 4. Refresh hiển thị
            dataGridView1.Refresh();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            if (dt == null) return;
            var filters = new List<string>();

            // 1. Biển số xe
            var bs = txtTimKiemBienSo.Text.Trim().Replace("'", "''");
            if (!string.IsNullOrEmpty(bs))
                filters.Add($"BienSoXe LIKE '%{bs}%'");

            // 2. Loại vé
            if (cbxTimKiemLoaiVe.SelectedIndex >= 0)
            {
                var lv = cbxTimKiemLoaiVe.SelectedItem.ToString().Replace("'", "''");
                filters.Add($"LoaiVe = '{lv}'");
            }

            // 3. Số vé (tìm chính xác)
            var sv = txtTimKiemSoVe.Text.Trim().Replace("'", "''");
            if (!string.IsNullOrEmpty(sv))
                filters.Add($"SoVe = '{sv}'");

            // 4. Thời gian vào
            var vaoText = txtTimKiemThoiGianVao.Text.Trim();
            if (DateTime.TryParse(vaoText, out var vaoDt))
            {
                var d = vaoDt.Date;
                var d2 = d.AddDays(1);
                filters.Add($"ThoiGianVao >= '{d:yyyy-MM-dd}' AND ThoiGianVao < '{d2:yyyy-MM-dd}'");
            }

            // Áp dụng
            dt.DefaultView.RowFilter = filters.Count > 0
                ? string.Join(" AND ", filters)
                : string.Empty;
        }

        private void txtSuCoTenKhachHang_TextChanged(object sender, EventArgs e)
        {

        }

        private void datNgaySinh_ValueChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void cbxSuCoLoaiXe_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void chkGTNam_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkGTNu_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void txtSuCoBienSo_TextChanged(object sender, EventArgs e)
        {

        }

        private void datNgayGui_ValueChanged(object sender, EventArgs e)
        {

        }

        private void datNgayNhan_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txtMoTaSuCo_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtYeuCauKhachHang_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnLuuSuCo_Click(object sender, EventArgs e)
        {
            // 1. Đọc giá trị từ control
            string ten = txtSuCoTenKhachHang.Text.Trim();
            DateTime ns = datNgaySinh.Value.Date;
            string gt = chkGTNam.Checked ? "Nam" : "Nu";
            string cccd = txtCCCD.Text.Trim();
            string sdt = txtSuCoSoDienThoai.Text.Trim();
            string loaiXe = cbxSuCoLoaiXe.SelectedItem?.ToString();
            string bienSo = txtSuCoBienSo.Text.Trim();
            DateTime gui = datNgayGui.Value;
            DateTime nhan = datNgayNhan.Value;
            string moTa = txtMoTaSuCo.Text.Trim();
            string yeuCau = txtYeuCauKhachHang.Text.Trim();

            // 2. Kiểm tra dữ liệu bắt buộc
            if (string.IsNullOrEmpty(ten) ||
                string.IsNullOrEmpty(cccd) ||
                string.IsNullOrEmpty(sdt) ||
                string.IsNullOrEmpty(loaiXe) ||
                string.IsNullOrEmpty(bienSo))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên, CCCD, SDT, Loại xe, Biển số.", "Thiếu thông tin",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Thực hiện INSERT
            try
            {
                using (var conn = new MySqlConnection(chuoiKetNoi))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = @"
                        INSERT INTO dulieusuco
                        (TenKhachHang, NgaySinh, GioiTinh, CCCD, SDT,
                         LoaiXe, BienSoXe, NgayGui, NgayNhan, MoTaSuCo, YeuCauKhachHang)
                        VALUES
                        (@ten, @ns, @gt, @cccd, @sdt,
                         @loai, @bien, @ngui, @nnhan, @mota, @yc)";

                    cmd.Parameters.AddWithValue("@ten", ten);
                    cmd.Parameters.AddWithValue("@ns", ns);
                    cmd.Parameters.AddWithValue("@gt", gt);
                    cmd.Parameters.AddWithValue("@cccd", cccd);
                    cmd.Parameters.AddWithValue("@sdt", sdt);
                    cmd.Parameters.AddWithValue("@loai", loaiXe);
                    cmd.Parameters.AddWithValue("@bien", bienSo);
                    cmd.Parameters.AddWithValue("@ngui", gui);
                    cmd.Parameters.AddWithValue("@nnhan", nhan);
                    cmd.Parameters.AddWithValue("@mota", moTa);
                    cmd.Parameters.AddWithValue("@yc", yeuCau);

                    int row = cmd.ExecuteNonQuery();
                    MessageBox.Show(row > 0
                        ? "Lưu sự cố thành công!"
                        : "Lưu thất bại, vui lòng thử lại.",
                        "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtCCCD_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
