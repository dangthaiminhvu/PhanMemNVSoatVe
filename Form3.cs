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
using PhanMemNVSoatVe.DataAccess;

namespace PhanMemNVSoatVe
{
    public partial class frmPhanMemDanhChoNVQuanLyKhachHang : Form
    {

        private readonly IKhachHangRepository _repo;
        private DataTable _dt;

        public frmPhanMemDanhChoNVQuanLyKhachHang()
        {
            InitializeComponent();
            _repo = new MySqlKhachHangRepository();
            KhoiTaoGrid();
            LoadTatCaDuLieu();

            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToOrderColumns = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;
        }

        private void KhoiTaoGrid()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ID",
                HeaderText = "ID",
                Name = "colID"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "BienSoXe",
                HeaderText = "Bien so xe",
                Name = "colBienSo"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "LoaiVe",
                HeaderText = "Loai ve",
                Name = "colLoaiVe"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SoVe",
                HeaderText = "So ve",
                Name = "colSoVe"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ThoiGianVao",
                HeaderText = "Thoi gian vao",
                Name = "colVao"
            });
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
            _dt = _repo.GetChuaTra();
            dataGridView1.DataSource = _dt.DefaultView;
        }

        private void btnGiaHan_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;
            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["colID"].Value);
            if (MessageBox.Show("Xác nhận gia hạn?", "Gia hạn",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _repo.GiaHan(id);
                LoadTatCaDuLieu();
            }
        }

        private void btbHuyGiaHan_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;
            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["colID"].Value);
            if (MessageBox.Show("Xác nhận hủy gia hạn?", "Hủy gia hạn",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _repo.HuyGiaHan(id);
                LoadTatCaDuLieu();
            }
        }

        private void bthNhapLai_Click(object sender, EventArgs e)
        {
            _dt.DefaultView.RowFilter = string.Empty;
            LoadTatCaDuLieu();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            if (_dt == null) return;
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

            _dt.DefaultView.RowFilter = filters.Count > 0
                ? string.Join(" AND ", filters)
                : string.Empty;
        }

        private void btnLuuSuCo_Click(object sender, EventArgs e)
        {
            var row = _repo.LuuSuCo(
                txtSuCoTenKhachHang.Text.Trim(),
                datNgaySinh.Value.Date,
                chkGTNam.Checked ? "Nam" : "Nu",
                txtCCCD.Text.Trim(),
                txtSuCoSoDienThoai.Text.Trim(),
                cbxSuCoLoaiXe.SelectedItem?.ToString(),
                txtSuCoBienSo.Text.Trim(),
                datNgayGui.Value,
                datNgayNhan.Value,
                txtMoTaSuCo.Text.Trim(),
                txtYeuCauKhachHang.Text.Trim()
            );
            MessageBox.Show(row > 0
                ? "Lưu thành công!"
                : "Lưu thất bại");
        }

        private void txtCCCD_TextChanged(object sender, EventArgs e) { }
        private void txtSuCoTenKhachHang_TextChanged(object sender, EventArgs e) { }
        private void datNgaySinh_ValueChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void cbxSuCoLoaiXe_SelectedIndexChanged(object sender, EventArgs e) { }
        private void chkGTNam_CheckedChanged(object sender, EventArgs e) { }
        private void chkGTNu_CheckedChanged(object sender, EventArgs e) { }
        private void txtSuCoBienSo_TextChanged(object sender, EventArgs e) { }
        private void datNgayGui_ValueChanged(object sender, EventArgs e) { }
        private void datNgayNhan_ValueChanged(object sender, EventArgs e) { }
        private void txtMoTaSuCo_TextChanged(object sender, EventArgs e) { }
        private void txtYeuCauKhachHang_TextChanged(object sender, EventArgs e) { }
        private void txtTimKiemBienSo_TextChanged(object sender, EventArgs e) { }
        private void cbxTimKiemLoaiVe_SelectedIndexChanged(object sender, EventArgs e) { }
        private void txtTimKiemSoVe_TextChanged(object sender, EventArgs e) { }
        private void txtTimKiemThoiGianVao_TextChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }
}
