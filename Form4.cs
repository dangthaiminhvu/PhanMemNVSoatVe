using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using PhanMemNVSoatVe.DataAccess;
using PhanMemNVSoatVe.Models;

namespace PhanMemNVSoatVe
{
    public partial class frmPhanMemDanhChoNVQuanTriVien : Form
    {
        private readonly INhanVienRepository _repo;
        private DataTable dt;

        // hàm băm
        private string MaHoaSha256(string plain)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(plain);
                byte[] hash = sha.ComputeHash(bytes);
                var sb = new StringBuilder();
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        public frmPhanMemDanhChoNVQuanTriVien()
        {
            InitializeComponent();

            // Tải chuỗi kết nối từ app.config
            string cs = System.Configuration.ConfigurationManager
                            .ConnectionStrings["MyConnStr"]
                            .ConnectionString;
            _repo = new MySqlNhanVienRepository(cs);

            // Cấu hình chế độ cho DataGridView
            ConfigureGrid();

            // Nạp dữ liệu bất đồng bộ
            _ = LoadGridAsync();

            LoadGrid();
        }

        // Cấu hình lưới hiển thị
        private void ConfigureGrid()
        {
            dgvTimKiemNhanVien.ReadOnly = true;
            dgvTimKiemNhanVien.AllowUserToAddRows = false;
            dgvTimKiemNhanVien.AllowUserToDeleteRows = false;
            dgvTimKiemNhanVien.AllowUserToOrderColumns = false;
            dgvTimKiemNhanVien.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTimKiemNhanVien.MultiSelect = false;
            dgvTimKiemNhanVien.EditMode = DataGridViewEditMode.EditProgrammatically;
        }

        // Nạp dữ liệu bất đồng bộ
        private async Task LoadGridAsync()
        {
            dt = await Task.Run(() => _repo.GetAll());
            dgvTimKiemNhanVien.DataSource = dt;
        }

        private void LoadGrid()
        {
            dt = _repo.GetAll();
            dgvTimKiemNhanVien.DataSource = dt;
        }

        private async void btnThemNhanVien_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            var nv = BuildNhanVienFromForm();

            bool success = await Task.Run(() => _repo.Insert(nv));
            MessageBox.Show(success ? "Thêm nhân viên thành công!" : "Thêm thất bại!",
                            success ? "Thành công" : "Lỗi",
                            MessageBoxButtons.OK,
                            success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            if (success)
            {
                await LoadGridAsync();
                ResetInputs();
            }
        }

        private bool ValidateInput()
        {
            // Check bắt buộc và độ dài
            if (string.IsNullOrWhiteSpace(txtIDNhanVien.Text) || txtIDNhanVien.Text.Length > 10 ||
                string.IsNullOrWhiteSpace(txtTenNhanVien.Text) || txtTenNhanVien.Text.Length > 50 ||
                (!chkNam.Checked && !chkNu.Checked) ||
                string.IsNullOrWhiteSpace(txtNgaySinh.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) || txtEmail.Text.Length > 100 ||
                string.IsNullOrWhiteSpace(txtNgayVaoLam.Text) ||
                string.IsNullOrWhiteSpace(txtSDT.Text) || txtSDT.Text.Length > 15 ||
                string.IsNullOrWhiteSpace(txtDiaChi.Text) || txtDiaChi.Text.Length > 200 ||
                cbxChucVu.SelectedIndex < 0 ||
                cbxTrangThai.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ và đúng độ dài thông tin!", "Cảnh báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Email regex
            if (!Regex.IsMatch(txtEmail.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Email không hợp lệ!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Số điện thoại chỉ số
            if (!Regex.IsMatch(txtSDT.Text.Trim(), "^[0-9]{9,15}$"))
            {
                MessageBox.Show("Số điện thoại phải từ 9 đến 15 chữ số!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Ngày sinh và ngày vào làm định dạng dd/MM/yyyy
            if (!DateTime.TryParseExact(txtNgaySinh.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out _))
            {
                MessageBox.Show("Ngày sinh không hợp lệ! Vui lòng nhập dd/MM/yyyy.", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (!DateTime.TryParseExact(txtNgayVaoLam.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out _))
            {
                MessageBox.Show("Ngày vào làm không hợp lệ! Vui lòng nhập dd/MM/yyyy.", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Lương
            if (!decimal.TryParse(txtMucLuong.Text.Trim(), out _))
            {
                MessageBox.Show("Mức lương phải là số!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private NhanVien BuildNhanVienFromForm()
        {
            _ = DateTime.TryParseExact(txtNgaySinh.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture,
                                       DateTimeStyles.None, out DateTime ngaySinh);
            _ = DateTime.TryParseExact(txtNgayVaoLam.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture,
                                       DateTimeStyles.None, out DateTime ngayVaoLam);

            return new NhanVien
            {
                IDNhanVien = txtIDNhanVien.Text.Trim(),
                TenNhanVien = txtTenNhanVien.Text.Trim(),
                GioiTinh = chkNam.Checked ? "Nam" : "Nu",
                NgaySinh = ngaySinh,
                Email = txtEmail.Text.Trim(),
                NgayVaoLam = ngayVaoLam,
                SDT = txtSDT.Text.Trim(),
                DiaChi = txtDiaChi.Text.Trim(),
                ChucVu = cbxChucVu.SelectedItem.ToString(),
                MucLuong = Convert.ToDecimal(txtMucLuong.Text.Trim()),
                MatKhau = MaHoaSha256(txtMatKhau.Text.Trim()),
                TrangThai = cbxTrangThai.SelectedItem.ToString()
            };
        }

        private void ResetInputs()
        {
            dt.DefaultView.RowFilter = string.Empty;
            txtIDNhanVien.Clear();
            txtTenNhanVien.Clear();
            chkNam.Checked = chkNu.Checked = false;
            txtNgaySinh.Clear();
            txtEmail.Clear();
            txtNgayVaoLam.Clear();
            txtSDT.Clear();
            txtDiaChi.Clear();
            cbxChucVu.SelectedIndex = -1;
            cbxTrangThai.SelectedIndex = -1;
            txtMucLuong.Clear();
            txtMatKhau.Clear();
        }

        private void chkNam_CheckedChanged(object sender, EventArgs e)
        {
            if (chkNam.Checked)
                chkNu.Checked = false;
        }

        private void chkNu_CheckedChanged(object sender, EventArgs e)
        {
            if (chkNu.Checked)
                chkNam.Checked = false;
        }

        private void btnNhapLai_Click(object sender, EventArgs e)
        {
            dt.DefaultView.RowFilter = string.Empty;
            txtIDNhanVien.Clear();
            txtTenNhanVien.Clear();
            chkNam.Checked = chkNu.Checked = false;
            txtNgaySinh.Clear();
            txtEmail.Clear();
            txtNgayVaoLam.Clear();
            txtSDT.Clear();
            txtDiaChi.Clear();
            cbxChucVu.SelectedIndex = -1;
            cbxTrangThai.SelectedIndex = -1;
            txtMucLuong.Clear();
            txtMatKhau.Clear();
            LoadGrid();
        }

        private void gbxGioiTinh_Enter(object sender, EventArgs e)
        {

        }

        private void btnTimKiemNV_Click(object sender, EventArgs e)
        {
            if (dt == null) return;
            dt.DefaultView.RowFilter = BuildFilterString();
        }

        private string BuildFilterString()
        {
            var filters = new List<string>();
            // 1. Mã NV
            var id = txtIDNhanVien.Text.Trim().Replace("'", "''");
            if (!string.IsNullOrEmpty(id))
                filters.Add($"IDNhanVien LIKE '%{id}%' ");
            // 2. Tên NV
            var ten = txtTenNhanVien.Text.Trim().Replace("'", "''");
            if (!string.IsNullOrEmpty(ten))
                filters.Add($"TenNhanVien LIKE '%{ten}%' ");
            // 3. Giới tính
            if (chkNam.Checked && !chkNu.Checked)
                filters.Add("GioiTinh = 'Nam'");
            else if (chkNu.Checked && !chkNam.Checked)
                filters.Add("GioiTinh = 'Nu'");
            // 4. Ngày sinh
            if (DateTime.TryParseExact(txtNgaySinh.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var ns))
            {
                var next = ns.AddDays(1);
                filters.Add($"NgaySinh >= '{ns:yyyy-MM-dd}' AND NgaySinh < '{next:yyyy-MM-dd}'");
            }
            // 5. Email
            var email = txtEmail.Text.Trim().Replace("'", "''");
            if (!string.IsNullOrEmpty(email))
                filters.Add($"Email LIKE '%{email}%' ");
            // 6. SDT
            var sdt = txtSDT.Text.Trim().Replace("'", "''");
            if (!string.IsNullOrEmpty(sdt))
                filters.Add($"SDT LIKE '%{sdt}%' ");
            // 7. Địa chỉ
            var dc = txtDiaChi.Text.Trim().Replace("'", "''");
            if (!string.IsNullOrEmpty(dc))
                filters.Add($"DiaChi LIKE '%{dc}%' ");
            // 8. Chức vụ
            if (cbxChucVu.SelectedIndex >= 0)
            {
                var cv = cbxChucVu.SelectedItem.ToString().Replace("'", "''");
                filters.Add($"ChucVu = '{cv}'");
            }
            // 9. Mức lương
            if (decimal.TryParse(txtMucLuong.Text.Trim(), out var ml))
                filters.Add($"MucLuong = {ml}");
            // 10. Mật khẩu
            var mk = txtMatKhau.Text.Trim().Replace("'", "''");
            if (!string.IsNullOrEmpty(mk))
                filters.Add($"MatKhau LIKE '%{mk}%' ");
            // 11. Trạng thái
            if (cbxTrangThai.SelectedIndex >= 0)
            {
                var tt = cbxTrangThai.SelectedItem.ToString().Replace("'", "''");
                filters.Add($"TrangThai = '{tt}'");
            }
            return filters.Count > 0 ? string.Join(" AND ", filters) : string.Empty;
        }

        private void LoadNhanVien(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                ClearFields();
                return;
            }

            DataRow row = _repo.GetById(id);
            if (row == null)
            {
                ClearFields();
                return;
            }

            // Gán dữ liệu lên label
            lblHienThiTenNhanVien.Text = row["TenNhanVien"].ToString();
            lblHienThiGioiTinh.Text = row["GioiTinh"].ToString();
            lblHienThiNgaySinh.Text = Convert.ToDateTime(row["NgaySinh"]).ToShortDateString();
            lblHienThiEmail.Text = row["Email"].ToString();
            lblHienThiNgayVaoLam.Text = Convert.ToDateTime(row["NgayVaoLam"]).ToShortDateString();
            lblHienThiSDT.Text = row["SDT"].ToString();
            lblHienThiDiaChi.Text = row["DiaChi"].ToString();
            lblHienThiChucVu.Text = row["ChucVu"].ToString();
            lblHienThiTrangThai.Text = row["TrangThai"].ToString();
            lblHienThiMucLuong.Text = row["MucLuong"].ToString();
            lblHienThiMatKhau.Text = row["MatKhau"].ToString();

            // Gán dữ liệu lên control để chỉnh sửa
            txtNhapTenNV.Text = row["TenNhanVien"].ToString();
            cbxNhapGioiTinh.Text = row["GioiTinh"].ToString();
            datNhapNgaySinh.Value = Convert.ToDateTime(row["NgaySinh"]);
            txtNhapEmail.Text = row["Email"].ToString();
            datNhapNgayVaoLam.Value = Convert.ToDateTime(row["NgayVaoLam"]);
            txtNhapSDT.Text = row["SDT"].ToString();
            txtNhapDiaChi.Text = row["DiaChi"].ToString();
            cbxNhapChucVu.Text = row["ChucVu"].ToString();
            txtNhapMucLuong.Text = row["MucLuong"].ToString();
            txtNhapMatKhau.Text = row["MatKhau"].ToString();
            cbxNhapTrangThai.Text = row["TrangThai"].ToString();
        }

        private void ClearFields()
        {
            lblHienThiTenNhanVien.Text = string.Empty;
            lblHienThiGioiTinh.Text = string.Empty;
            lblHienThiNgaySinh.Text = string.Empty;
            lblHienThiEmail.Text = string.Empty;
            lblHienThiNgayVaoLam.Text = string.Empty;
            lblHienThiSDT.Text = string.Empty;
            lblHienThiDiaChi.Text = string.Empty;
            lblHienThiChucVu.Text = string.Empty;
            lblHienThiMucLuong.Text = string.Empty;
            lblHienThiMatKhau.Text = string.Empty;
            lblHienThiTrangThai.Text = string.Empty;

            txtNhapTenNV.Clear();
            cbxNhapGioiTinh.SelectedIndex = -1;
            datNhapNgaySinh.Value = DateTime.Today;
            txtNhapEmail.Clear();
            datNhapNgayVaoLam.Value = DateTime.Today;
            txtNhapSDT.Clear();
            txtNhapDiaChi.Clear();
            cbxNhapChucVu.SelectedIndex = -1;
            txtNhapMucLuong.Clear();
            txtNhapMatKhau.Clear();
            cbxNhapTrangThai.SelectedIndex = -1;
        }

        private void btnLuuThayDoi_Click(object sender, EventArgs e)
        {
            string id = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(id)) return;

            // Xác định mật khẩu lưu (hash hoặc raw)
            string raw = txtNhapMatKhau.Text.Trim();
            string matKhauToSave = Regex.IsMatch(raw, @"\A[a-f0-9]{64}\z", RegexOptions.IgnoreCase)
                ? raw
                : MaHoaSha256(raw);

            var nv = new NhanVien
            {
                IDNhanVien = id,
                TenNhanVien = txtNhapTenNV.Text,
                GioiTinh = cbxNhapGioiTinh.Text,
                NgaySinh = datNhapNgaySinh.Value.Date,
                Email = txtNhapEmail.Text,
                NgayVaoLam = datNhapNgayVaoLam.Value.Date,
                SDT = txtNhapSDT.Text,
                DiaChi = txtNhapDiaChi.Text,
                ChucVu = cbxNhapChucVu.Text,
                MucLuong = Convert.ToDecimal(txtNhapMucLuong.Text),
                MatKhau = matKhauToSave,
                TrangThai = cbxNhapTrangThai.Text
            };

            bool success = _repo.Update(nv);
            if (!success)
                MessageBox.Show("Cập nhật thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

            LoadNhanVien(id);
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            LoadNhanVien(textBox1.Text.Trim());
        }

        private void btnXoaNhanVien_Click(object sender, EventArgs e)
        {
            string id = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(id)) return;

            if (MessageBox.Show("Bạn có chắc muốn xóa nhân viên này?", "Xác nhận",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                bool success = _repo.Delete(id);
                if (success)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadGrid();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Xóa thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void cbxTrangThai_SelectedIndexChanged(object sender, EventArgs e) { }

        private void lblHienThiTrangThai_Click(object sender, EventArgs e) { }

        private void cbxNhapTrangThai_SelectedIndexChanged(object sender, EventArgs e) { }
        private void txtNgaySinh_TextChanged(object sender, EventArgs e) { }

        private void txtSDT_TextChanged(object sender, EventArgs e) { }

        private void txtDiaChi_TextChanged(object sender, EventArgs e) { }

        private void cbxChucVu_SelectedIndexChanged(object sender, EventArgs e) { }

        private void txtMucLuong_TextChanged(object sender, EventArgs e) { }

        private void txtMatKhau_TextChanged(object sender, EventArgs e) { }

        private void txtEmail_TextChanged(object sender, EventArgs e) { }

        private void txtIDNhanVien_TextChanged(object sender, EventArgs e) { }

        private void txtTenNhanVien_TextChanged(object sender, EventArgs e) { }

        private void txtNhapTenNV_TextChanged(object sender, EventArgs e) { }

        private void txtNhapEmail_TextChanged(object sender, EventArgs e) { }

        private void cbxNhapGioiTinh_SelectedIndexChanged(object sender, EventArgs e) { }

        private void txtNhapSDT_TextChanged(object sender, EventArgs e) { }

        private void datNhapNgaySinh_ValueChanged(object sender, EventArgs e) { }

        private void datNhapNgayVaoLam_ValueChanged(object sender, EventArgs e) { }

        private void txtNhapMucLuong_TextChanged(object sender, EventArgs e) { }

        private void txtNhapDiaChi_TextChanged(object sender, EventArgs e) { }

        private void txtNhapMatKhau_TextChanged(object sender, EventArgs e) { }

        private void lblHienThiTenNhanVien_Click(object sender, EventArgs e) { }

        private void lblHienThiEmail_Click(object sender, EventArgs e) { }

        private void lblHienThiGioiTinh_Click(object sender, EventArgs e) { }

        private void lblHienThiSDT_Click(object sender, EventArgs e) { }

        private void lblHienThiNgayVaoLam_Click(object sender, EventArgs e) { }

        private void lblHienThiNgaySinh_Click(object sender, EventArgs e) { }

        private void lblHienThiMucLuong_Click(object sender, EventArgs e) { }

        private void lblHienThiChucVu_Click(object sender, EventArgs e) { }

        private void lblHienThiDiaChi_Click(object sender, EventArgs e) { }

        private void lblHienThiMatKhau_Click(object sender, EventArgs e) { }
    }
}
