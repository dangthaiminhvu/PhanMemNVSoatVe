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
using PhanMemNVSoatVe.Views;
using PhanMemNVSoatVe.Presenters;
using PhanMemNVSoatVe.Models;
using System.Globalization;


namespace PhanMemNVSoatVe
{
    public partial class frmPhanMemNVQuanLyThongTin : Form, IQuanLyThongTinView
    {
        private readonly QuanLyThongTinPresenter _presenter;

        public void ClearFilterInputs()
        {
            txtBienSoXe.Text = "";
            cbxLoaiVe.SelectedIndex = -1;
            txtSoVe.Text = "";
            txtThoiGianVao.Text = "";
            txtThoiGianRa.Text = "";
            chkDaTra.Checked = false;
            chkChuaTra.Checked = false;
        }

        public void ClearNewInputs()
        {
            txtNhapBienSo.Text = "";
            cbxNhapLoaiVe.SelectedIndex = -1;
            txtNhapSoVe.Text = "";
            cbxNhapTrangThaiVe.SelectedIndex = -1;
            dtpNhapThoiGianVao.Value = DateTime.Now;
            dtpNhapThoiGianRa.Value = DateTime.Now;
            txtNhapTienPhat.Text = "";
            cbxGiaHan.Checked = false;
        }

        public void ClearEditSectionInputs()
        {
            txtNhapID.Text = "";
            lblThonTinBienSo.Text = "";
            lblThongTinLoaiVe.Text = "";
            lblThongTinSoVe.Text = "";
            lblThongTinThoiGianVao.Text = "";
            lblThongTinTrangThaiVe.Text = "";
            lblThongTinThoiGianRa.Text = "";
            lblThongTinTienPhat.Text = "";

            txtChinhSuaBienSo.Text = "";
            cbxChinhSuaLoaiVe.Text = "";
            txtChinhSuaSoVe.Text = "";
            txtChinhSuaThoiGianVao.Text = "";
            cbxChinhSuaTrangThaiVe.Text = "";
            txtChinhSuaThoiGianRa.Text = "";
            txtChinhSuaTienPhat.Text = "";
        }


        public frmPhanMemNVQuanLyThongTin()
        {
            InitializeComponent();
            _presenter = new QuanLyThongTinPresenter(this, new MySqlDuLieuXeVaoRepository());

            this.Load += (s, e) => LoadData?.Invoke(s, e);
            btnTimKiem.Click += (s, e) => FilterChanged?.Invoke(s, e);
            btnNhapLaiTimKiem.Click += (s, e) => ResetFilterClicked?.Invoke(s, e);
            btnThemDuLieu.Click += (s, e) => AddClicked?.Invoke(s, e);
            btnLuuThonTin.Click += (s, e) => UpdateClicked?.Invoke(s, e);
            btnXoaThongTin.Click += (s, e) => DeleteClicked?.Invoke(s, e);
            btnReset.Click += (s, e) => ResetNewClicked?.Invoke(s, e);
            txtNhapID.TextChanged += (s, e) => EditIDChanged?.Invoke(s, e);
            btnTimKiem.Click += (s, e) => FilterChanged?.Invoke(s, e);
            btnNhapLaiTimKiem.Click += (s, e) => ResetFilterClicked?.Invoke(s, e);
            txtNhapID.TextChanged += (s, e) => EditIDChanged?.Invoke(s, e);
            btnVietLai.Click += btnVietLai_Click;

            grdThongTinKhachHang.ReadOnly = true;
            grdThongTinKhachHang.AllowUserToAddRows = false;
            grdThongTinKhachHang.AllowUserToDeleteRows = false;
            grdThongTinKhachHang.AllowUserToOrderColumns = false;
            grdThongTinKhachHang.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdThongTinKhachHang.MultiSelect = false;
            grdThongTinKhachHang.EditMode = DataGridViewEditMode.EditProgrammatically;
        }

        #region IQuanLyThongTinView Members
        public string FilterBienSo => txtBienSoXe.Text.Trim();
        public string FilterLoaiVe => cbxLoaiVe.SelectedIndex >= 0 ? cbxLoaiVe.Text : string.Empty;
        public string FilterSoVe => txtSoVe.Text.Trim();
        public DateTime? FilterVaoDate => DateTime.TryParse(txtThoiGianVao.Text, out var d1) ? d1 : (DateTime?)null;
        public DateTime? FilterRaDate => DateTime.TryParse(txtThoiGianRa.Text, out var d2) ? d2 : (DateTime?)null;
        public bool FilterDaTra => chkDaTra.Checked;
        public bool FilterChuaTra => chkChuaTra.Checked;
        public BindingList<XeVao> GridData
        {
            set => grdThongTinKhachHang.DataSource = value;
        }
        public bool NewGiaHan => cbxGiaHan.Checked;
        public string NewBienSo => txtNhapBienSo.Text.Trim();
        public string NewLoaiVe => cbxNhapLoaiVe.SelectedIndex >= 0 ? cbxNhapLoaiVe.Text : string.Empty;
        public string NewSoVe => txtNhapSoVe.Text.Trim();
        public DateTime NewThoiGianVao => dtpNhapThoiGianVao.Value;
        public string NewTrangThaiVe => cbxNhapTrangThaiVe.SelectedIndex >= 0 ? cbxNhapTrangThaiVe.Text : string.Empty;
        public DateTime? NewThoiGianRa => cbxNhapTrangThaiVe.Text == "DaTra" ? (DateTime?)dtpNhapThoiGianRa.Value : null;
        public double NewTienPhat => double.TryParse(txtNhapTienPhat.Text, out var tp) ? tp : 0;
        public string EditID => txtNhapID.Text.Trim();
        public string EditBienSo => txtChinhSuaBienSo.Text.Trim();
        public string EditLoaiVe => cbxChinhSuaLoaiVe.Text.Trim();
        public string EditSoVe => txtChinhSuaSoVe.Text.Trim();
        public DateTime EditThoiGianVao =>
            DateTime.ParseExact(
                txtChinhSuaThoiGianVao.Text,
                "dd/MM/yyyy HH:mm:ss",
                CultureInfo.InvariantCulture); 
        public string EditTrangThaiVe => cbxChinhSuaTrangThaiVe.SelectedIndex >= 0 ? cbxChinhSuaTrangThaiVe.Text : string.Empty;
        public DateTime? EditThoiGianRa
        {
            get
            {
                if (string.IsNullOrWhiteSpace(txtChinhSuaThoiGianRa.Text))
                    return null;
                return DateTime.ParseExact(
                    txtChinhSuaThoiGianRa.Text,
                    "dd/MM/yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture);
            }
        }
        public double EditTienPhat => double.TryParse(txtChinhSuaTienPhat.Text, out var v) ? v : 0;


        public void ShowEditSection(XeVao xe)
        {
            if (xe == null) return;
            lblThonTinBienSo.Text = xe.BienSoXe;
            lblThongTinLoaiVe.Text = xe.LoaiVe;
            lblThongTinSoVe.Text = xe.SoVe;
            lblThongTinThoiGianVao.Text = xe.ThoiGianVao.ToString("dd/MM/yyyy HH:mm:ss");
            lblThongTinTrangThaiVe.Text = xe.TrangThaiVe;
            lblThongTinThoiGianRa.Text = xe.ThoiGianRa?.ToString("dd/MM/yyyy HH:mm:ss");
            lblThongTinTienPhat.Text = xe.TienPhat.ToString("N0");

            txtChinhSuaBienSo.Text = xe.BienSoXe;
            cbxChinhSuaLoaiVe.Text = xe.LoaiVe;
            txtChinhSuaSoVe.Text = xe.SoVe;
            txtChinhSuaThoiGianVao.Text = xe.ThoiGianVao.ToString("dd/MM/yyyy HH:mm:ss");
            cbxChinhSuaTrangThaiVe.Text = xe.TrangThaiVe;
            txtChinhSuaThoiGianRa.Text = xe.ThoiGianRa?.ToString("dd/MM/yyyy HH:mm:ss");
            txtChinhSuaTienPhat.Text = xe.TienPhat.ToString();
        }
        public void ShowError(string msg)
            => MessageBox.Show(msg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        public void ShowInfo(string msg)
            => MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

        public event EventHandler LoadData;
        public event EventHandler FilterChanged;
        public event EventHandler ResetFilterClicked;
        public event EventHandler AddClicked;
        public event EventHandler UpdateClicked;
        public event EventHandler DeleteClicked;
        public event EventHandler ResetNewClicked;
        public event EventHandler EditIDChanged;
        public event EventHandler ResetEditClicked;
        #endregion

        #region
        private void txtNhapSoVe_Enter_1(object sender, EventArgs e) { }

        private void cbxChinhSuaLoaiVe_SelectedIndexChanged(object sender, EventArgs e) { }
        private void btnVietLai_Click(object sender, EventArgs e) {
        
        }
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
        #endregion

        private void btnTimKiem_Click(object sender, EventArgs e)
        {

        }

        private void btnNhapLaiTimKiem_Click(object sender, EventArgs e)
        {

        }

        private void btnReset_Click(object sender, EventArgs e)
        {

        }

        private void btnThemDuLieu_Click(object sender, EventArgs e)
        {

        }

        private void cbxGiaHan_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnLuuThonTin_Click(object sender, EventArgs e)
        {

        }
    }
}
