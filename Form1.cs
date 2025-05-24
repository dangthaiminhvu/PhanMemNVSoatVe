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
using PhanMemNVSoatVe.DataAccess;
using PhanMemNVSoatVe.Models;

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
        private readonly IXeVaoRepository _repo;
        private bool barrierVaoDangMo = false;
        private bool barrierRaDangMo = false;
        private static readonly Regex _regexBienSo = new Regex(@"^[0-9]{2}[A-Z]-[A-Z0-9]{1,2}\s?[0-9]{3,4}$", RegexOptions.Compiled);


        public frmPhanMemChoNVSoatVe()
        {
            InitializeComponent();
            _repo = new MySqlXeVaoRepository();
            CapNhatTrangThaiBarrier();
            tmrThoiGianVao.Start();
        }

        private void CapNhatTrangThaiBarrier()
        {
            lblTrangThaiVao.BackColor = barrierVaoDangMo ? Color.LimeGreen : Color.Red;
            lblTrangThaiRa.BackColor = barrierRaDangMo ? Color.LimeGreen : Color.Red;
        }

        private void txtSoVeVao_Enter_1(object sender, EventArgs e)
        {
            if (DesignMode || !string.IsNullOrWhiteSpace(txtSoVeVao.Text)) return;
            try
            {
                var used = _repo.GetXeDangGui()
                                .Select(x => int.TryParse(x.SoVe, out var n) ? n : 0)
                                .Where(n => n > 0)
                                .ToHashSet();
                int next = 1;
                while (used.Contains(next)) next++;
                txtSoVeVao.Text = next.ToString();
                txtSoVeVao.SelectionStart = txtSoVeVao.Text.Length;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy số vé: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tmrThoiGianVao_Tick(object sender, EventArgs e)
        {
            lblThoiGianVao.Text = DateTime.Now.ToString("dd/MM/yyyy | H:mm:ss");
        }

        private void btnMoBarrier_Click_1(object sender, EventArgs e)
        {
            var bienSo = txtBienSoVao.Text.Trim();
            var loaiVe = cbxLoaiVeVao.SelectedItem?.ToString();
            var soVe = txtSoVeVao.Text.Trim();
            var now = DateTime.Now;

            if (string.IsNullOrWhiteSpace(bienSo) || string.IsNullOrWhiteSpace(loaiVe) || string.IsNullOrWhiteSpace(soVe))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Biển số, Loại vé và Số vé.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_regexBienSo.IsMatch(bienSo))
            {
                MessageBox.Show($"Biển số '{bienSo}' không hợp lệ.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (barrierVaoDangMo)
            {
                MessageBox.Show("Đóng barrier trước khi cấp vé mới.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_repo.GetByBienSoChuaTra(bienSo) != null)
            {
                MessageBox.Show($"Xe {bienSo} đang còn trong bãi.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (now.TimeOfDay >= TimeSpan.FromHours(22))
            {
                MessageBox.Show("Quá giờ nhận xe (sau 22:00).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_repo.CheckSoVeDangSuDung(soVe))
            {
                MessageBox.Show($"Vé {soVe} chưa trả.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var xe = new XeVao
            {
                BienSoXe = bienSo,
                LoaiVe = loaiVe,
                SoVe = soVe,
                ThoiGianVao = now,
                GiaHan = false
            };

            if (_repo.Insert(xe))
            {
                barrierVaoDangMo = true;
                CapNhatTrangThaiBarrier();
                txtBienSoVao.Clear();
                cbxLoaiVeVao.SelectedIndex = -1;
                txtSoVeVao.Clear();
            }
            else
            {
                MessageBox.Show("Lỗi khi cấp vé.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSoVeRa_TextChanged(object sender, EventArgs e)
        {
            var soVe = txtSoVeRa.Text.Trim();
            if (string.IsNullOrEmpty(soVe))
            {
                lblBienSoRa.Text = lblLoaiVeRa.Text = lblThoiGianRa.Text = lblPhatMuon.Text = string.Empty;
                return;
            }

            var xe = _repo.GetBySoVe(soVe);
            if (xe == null)
            {
                lblBienSoRa.Text = lblLoaiVeRa.Text = lblThoiGianRa.Text = lblPhatMuon.Text = string.Empty;
                return;
            }

            lblBienSoRa.Text = xe.BienSoXe;
            lblLoaiVeRa.Text = xe.LoaiVe;
            lblThoiGianRa.Text = xe.ThoiGianVao.ToString("dd/MM/yyyy | H:mm:ss");

            var delta = DateTime.Now - xe.ThoiGianVao;
            double phat = 0;
            if (delta.TotalHours > (xe.GiaHan ? 1 : 0))
            {
                phat = Math.Floor(delta.TotalHours) * 10000;
                if (!xe.GiaHan && delta.TotalHours < 1)
                    phat = 5000;
            }
            lblPhatMuon.Text = phat > 0 ? $"{phat:N0} VND | muộn {Math.Floor(delta.TotalHours)} giờ" : "0 VND";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            barrierRaDangMo = false;
            CapNhatTrangThaiBarrier();
        }

        private void btnDongBarrier_Click(object sender, EventArgs e)
        {
            barrierVaoDangMo = false;
            CapNhatTrangThaiBarrier();
        }

        private void lblLanVao_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void cbxLoaiXe_SelectedIndexChanged(object sender, EventArgs e) { }
        private void lblThoiGian_Click(object sender, EventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void lblBienSoRa_Click(object sender, EventArgs e) { }
        private void txtBienSoVao_TextChanged(object sender, EventArgs e) { }
        private void lblLoaiVeRa_Click(object sender, EventArgs e) { }
        private void lblThoiGianRa_Click(object sender, EventArgs e) { }
        private void button2_Click(object sender, EventArgs e) { }
        private void lblTrangThaiVao_Click(object sender, EventArgs e) { }
        private void lblTrangThaiRa_Click(object sender, EventArgs e) { }
        private void lblPhatMuon_Click(object sender, EventArgs e) { }
    }
}
