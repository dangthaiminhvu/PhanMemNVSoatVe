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
using PhanMemNVSoatVe.Presenters;
using PhanMemNVSoatVe.Views;

namespace PhanMemNVSoatVe
{
    public partial class frmPhanMemDanhChoNVQuanLyKhachHang : Form, IQuanLyKhachHangView
    {
        private QuanLyKhachHangPresenter _presenter;
        private readonly IKhachHangRepository _repo;
        private DataTable _dt;
        public DataTable DataSource
        {
            get => _dt;
            set
            {
                _dt = value;
                dataGridView1.DataSource = _dt?.DefaultView;
            }
        }


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

            _presenter = new QuanLyKhachHangPresenter(this);

            LoadData?.Invoke(this, EventArgs.Empty);
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

        public event EventHandler LoadData;
        public event EventHandler GiaHanClicked;
        public event EventHandler HuyGiaHanClicked;
        public event EventHandler TimKiemClicked;
        public event EventHandler NhapLaiClicked;
        public event EventHandler LuuSuCoClicked;

        public string TimKiemBienSo => txtTimKiemBienSo.Text.Trim();
        public string TimKiemLoaiVe => cbxTimKiemLoaiVe.SelectedIndex >= 0 ? cbxTimKiemLoaiVe.SelectedItem.ToString() : null;
        public string TimKiemSoVe => txtTimKiemSoVe.Text.Trim();
        public string TimKiemThoiGianVao => txtTimKiemThoiGianVao.Text.Trim();

        public int SelectedID
        {
            get
            {
                if (dataGridView1.CurrentRow == null) return -1;
                return Convert.ToInt32(dataGridView1.CurrentRow.Cells["colID"].Value);
            }
        }

        public string SuCoTenKhachHang => txtSuCoTenKhachHang.Text.Trim();
        public DateTime SuCoNgaySinh => datNgaySinh.Value.Date;
        public string SuCoGioiTinh => chkGTNam.Checked ? "Nam" : "Nu";
        public string SuCoCCCD => txtCCCD.Text.Trim();
        public string SuCoSoDienThoai => txtSuCoSoDienThoai.Text.Trim();
        public string SuCoLoaiXe => cbxSuCoLoaiXe.SelectedItem?.ToString();
        public string SuCoBienSo => txtSuCoBienSo.Text.Trim();
        public DateTime SuCoNgayGui => datNgayGui.Value;
        public DateTime SuCoNgayNhan => datNgayNhan.Value;
        public string SuCoMoTa => txtMoTaSuCo.Text.Trim();
        public string SuCoYeuCauKhachHang => txtYeuCauKhachHang.Text.Trim();

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        private void LoadTatCaDuLieu()
        {
            _dt = _repo.GetChuaTra();
            dataGridView1.DataSource = _dt.DefaultView;
        }

        private void btnGiaHan_Click(object sender, EventArgs e)
        {
            if (SelectedID < 0) return;
            if (MessageBox.Show("Xác nhận gia hạn?", "Gia hạn", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                GiaHanClicked?.Invoke(this, EventArgs.Empty);
            }
        }

        private void btbHuyGiaHan_Click(object sender, EventArgs e)
        {
            if (SelectedID < 0) return;
            if (MessageBox.Show("Xác nhận hủy gia hạn?", "Hủy gia hạn", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                HuyGiaHanClicked?.Invoke(this, EventArgs.Empty);
            }
        }

        private void bthNhapLai_Click(object sender, EventArgs e)
        {
            NhapLaiClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            TimKiemClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnLuuSuCo_Click(object sender, EventArgs e)
        {
            LuuSuCoClicked?.Invoke(this, EventArgs.Empty);
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
