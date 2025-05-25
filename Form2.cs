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
using PhanMemNVSoatVe.DataAccess;
using PhanMemNVSoatVe.Models;


namespace PhanMemNVSoatVe
{
    public partial class frmPhanMemNVQuanLyThongTin : Form
    {
        private readonly IDuLieuXeVaoRepository _repo;
        private BindingList<XeVao> _list;

        public frmPhanMemNVQuanLyThongTin()
        {
            InitializeComponent();
            _repo = new MySqlDuLieuXeVaoRepository();
            ConfigureGrid();
            LoadData();
            HookEvents();
        }

        private void HookEvents()
        {
            this.Load += (_, __) => LoadData();
            btnTimKiem.Click += (_, __) => ApplyFilter();
            btnNhapLaiTimKiem.Click += (_, __) => ResetFilter();
            btnThemDuLieu.Click += (_, __) => AddNewRecord();
            btnLuuThonTin.Click += (_, __) => UpdateRecord();
            btnXoaThongTin.Click += (_, __) => DeleteRecord();
            txtNhapID.TextChanged += (_, __) => PopulateEditSection();
            btnReset.Click += (_, __) => ResetNewForm();
        }

        private void btnTimKiem_Click(object sender, EventArgs e) => ApplyFilter();
        private void btnNhapLaiTimKiem_Click(object sender, EventArgs e) => ResetFilter();
        private void btnThemDuLieu_Click(object sender, EventArgs e) => AddNewRecord();
        private void btnLuuThonTin_Click(object sender, EventArgs e) => UpdateRecord();
        private void btnXoaThongTin_Click(object sender, EventArgs e) => DeleteRecord();
        private void txtNhapID_TextChanged(object sender, EventArgs e) => PopulateEditSection();
        private void btnReset_Click(object sender, EventArgs e) => ResetNewForm();
        private void txtNhapSoVe_Enter_1(object sender, EventArgs e) { }

        private void ConfigureGrid()
        {
            grdThongTinKhachHang.AutoGenerateColumns = true;
            grdThongTinKhachHang.ReadOnly = true;
            grdThongTinKhachHang.AllowUserToAddRows = false;
            grdThongTinKhachHang.AllowUserToDeleteRows = false;
            grdThongTinKhachHang.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdThongTinKhachHang.MultiSelect = false;
        }

        private void LoadData()
        {
            var data = _repo.GetAll().ToList();
            _list = new BindingList<XeVao>(data);
            grdThongTinKhachHang.DataSource = _list;
        }

        private void ApplyFilter()
        {
            var filtered = _list.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(txtBienSoXe.Text))
                filtered = filtered.Where(x => x.BienSoXe.IndexOf(txtBienSoXe.Text.Trim(), StringComparison.OrdinalIgnoreCase)>=0);

            if (cbxLoaiVe.SelectedIndex >= 0)
                filtered = filtered.Where(x => x.LoaiVe == cbxLoaiVe.Text);

            if (int.TryParse(txtSoVe.Text.Trim(), out var soVe))
                filtered = filtered.Where(x => int.TryParse(x.SoVe, out var v) && v == soVe);

            if (DateTime.TryParse(txtThoiGianVao.Text, out var d1))
                filtered = filtered.Where(x => x.ThoiGianVao.Date == d1.Date);

            if (DateTime.TryParse(txtThoiGianRa.Text, out var d2))
                filtered = filtered.Where(x => x.ThoiGianRa?.Date == d2.Date);

            if (chkDaTra.Checked ^ chkChuaTra.Checked)
                filtered = filtered.Where(x => x.TrangThaiVe == (chkDaTra.Checked ? "DaTra" : "ChuaTra"));

            grdThongTinKhachHang.DataSource = new BindingList<XeVao>(filtered.ToList());
        }

        private void ResetFilter()
        {
            txtBienSoXe.Clear();
            cbxLoaiVe.SelectedIndex = -1;
            txtSoVe.Clear();
            txtThoiGianVao.Clear();
            txtThoiGianRa.Clear();
            chkDaTra.Checked = chkChuaTra.Checked = false;
            LoadData();
        }

        private void AddNewRecord()
        {
            var xe = new XeVao
            {
                BienSoXe = txtNhapBienSo.Text.Trim(),
                LoaiVe = cbxNhapLoaiVe.Text,
                SoVe = txtNhapSoVe.Text.Trim(),
                ThoiGianVao = dtpNhapThoiGianVao.Value,
                TrangThaiVe = string.IsNullOrWhiteSpace(cbxNhapTrangThaiVe.Text) ? "ChuaTra" : cbxNhapTrangThaiVe.Text,
                ThoiGianRa = cbxNhapTrangThaiVe.Text == "DaTra" ? dtpNhapThoiGianRa.Value : (DateTime?)null,
                TienPhat = double.TryParse(txtNhapTienPhat.Text.Trim(), out var tp) ? tp : 0
            };
            if (_repo.Insert(xe))
            {
                MessageBox.Show("Thêm dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetNewForm();
                LoadData();
            }
            else
                MessageBox.Show("Thêm dữ liệu thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void PopulateEditSection()
        {
            if (!int.TryParse(txtNhapID.Text.Trim(), out var id))
            {
                ClearEditSection();
                return;
            }

            var xe = _repo.GetById(id);
            if (xe == null)
            {
                ClearEditSection();
                return;
            }

            // Hiển thị thông tin hiện tại
            lblThonTinBienSo.Text = xe.BienSoXe;
            lblThongTinLoaiVe.Text = xe.LoaiVe;
            lblThongTinSoVe.Text = xe.SoVe;
            lblThongTinThoiGianVao.Text = xe.ThoiGianVao.ToString("dd/MM/yyyy HH:mm:ss");
            lblThongTinTrangThaiVe.Text = xe.TrangThaiVe;
            lblThongTinThoiGianRa.Text = xe.ThoiGianRa?.ToString("dd/MM/yyyy HH:mm:ss") ?? string.Empty;
            lblThongTinTienPhat.Text = xe.TienPhat.ToString("N0");

            // Gán giá trị vào các textbox chỉnh sửa
            txtChinhSuaBienSo.Text = xe.BienSoXe;
            cbxChinhSuaLoaiVe.Text = xe.LoaiVe;
            txtChinhSuaSoVe.Text = xe.SoVe;
            txtChinhSuaThoiGianVao.Text = xe.ThoiGianVao.ToString("dd/MM/yyyy HH:mm:ss");
            cbxChinhSuaTrangThaiVe.Text = xe.TrangThaiVe;
            txtChinhSuaThoiGianRa.Text = xe.ThoiGianRa?.ToString("dd/MM/yyyy HH:mm:ss") ?? string.Empty;
            txtChinhSuaTienPhat.Text = xe.TienPhat.ToString();
        }

    private void ClearEditSection()
        {
            lblThonTinBienSo.Text = lblThongTinLoaiVe.Text = lblThongTinSoVe.Text =
            lblThongTinThoiGianVao.Text = lblThongTinTrangThaiVe.Text =
            lblThongTinThoiGianRa.Text = lblThongTinTienPhat.Text = string.Empty;

            txtChinhSuaBienSo.Clear();
            cbxChinhSuaLoaiVe.SelectedIndex = -1;
            txtChinhSuaSoVe.Clear();
            txtChinhSuaThoiGianVao.Clear();
            cbxChinhSuaTrangThaiVe.SelectedIndex = -1;
            txtChinhSuaThoiGianRa.Clear();
            txtChinhSuaTienPhat.Clear();
        }

        private void UpdateRecord()
        {
            if (!int.TryParse(txtNhapID.Text.Trim(), out var id)) return;
            var xe = _repo.GetById(id);
            if (xe == null) return;

            xe.BienSoXe = txtChinhSuaBienSo.Text.Trim();
            xe.LoaiVe = cbxChinhSuaLoaiVe.Text;
            xe.SoVe = txtChinhSuaSoVe.Text.Trim();

            // Parse ThoiGianVao
            if (DateTime.TryParseExact(txtChinhSuaThoiGianVao.Text.Trim(),
                "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out var vao))
                xe.ThoiGianVao = vao;

            xe.TrangThaiVe = cbxChinhSuaTrangThaiVe.Text;

            // Parse ThoiGianRa nếu có
            if (xe.TrangThaiVe == "DaTra" &&
                DateTime.TryParseExact(txtChinhSuaThoiGianRa.Text.Trim(),
                "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out var ra))
            {
                xe.ThoiGianRa = ra;
            }
            else
            {
                xe.ThoiGianRa = null;
            }

            xe.TienPhat = double.TryParse(txtChinhSuaTienPhat.Text.Trim(), out var tp2) ? tp2 : 0;

            if (_repo.Update(xe))
            {
                MessageBox.Show("Cập nhật thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            else
            {
                MessageBox.Show("Cập nhật không thành công.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteRecord()
        {
            if (!int.TryParse(txtNhapID.Text.Trim(), out var id)) return;
            if (MessageBox.Show("Bạn có chắc muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            if (_repo.Delete(id))
            {
                MessageBox.Show("Xóa thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData(); ClearEditSection(); txtNhapID.Clear();
            }
            else
                MessageBox.Show("Xóa không thành công.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ResetNewForm()
        {
            txtNhapBienSo.Clear(); cbxNhapLoaiVe.SelectedIndex = -1;
            txtNhapSoVe.Clear(); dtpNhapThoiGianVao.Value = DateTime.Now;
            cbxNhapTrangThaiVe.SelectedIndex = -1; dtpNhapThoiGianRa.Value = DateTime.Now;
            txtNhapTienPhat.Clear();
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
        private void cbxChinhSuaTrangThaiVe_SelectedIndexChanged(object sender, EventArgs e) { }
        private void txtChinhSuaThoiGianVao_TextChanged(object sender, EventArgs e) { }
        private void txtChinhSuaThoiGianRa_TextChanged(object sender, EventArgs e) { }
    }
}
