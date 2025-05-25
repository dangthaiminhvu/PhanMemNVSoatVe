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
using PhanMemNVSoatVe.Views;
using PhanMemNVSoatVe.Presenters;

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
    public partial class frmPhanMemChoNVSoatVe : Form, IQuanLyXeView
    {
        private readonly QuanLyXePresenter _presenter;

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

            // KHỞI TẠO PRESENTER vói this và repository
            _presenter = new QuanLyXePresenter(this, _repo);

            // WIRE UI events thành các event của IQuanLyXeView
            btnMoBarrier.Click += (s, e) => MoBarrierVaoClicked.Invoke(s, e);
            btnDongBarrier.Click += (s, e) => DongBarrierVaoClicked.Invoke(s, e);
            button1.Click += (s, e) => DongBarrierRaClicked.Invoke(s, e);
            button2.Click += (s, e) => MoBarrierRaClicked.Invoke(s, e);
            txtSoVeRa.TextChanged += (s, e) => SoVeRaTextChanged.Invoke(s, e);
            tmrThoiGianVao.Tick += (s, e) => TimerTick.Invoke(s, e);
        }

        #region IQuanLyXeView Implementation

        // Properties lấy từ UI
        public string BienSo   => txtBienSoVao.Text.Trim();
        public string LoaiVe   => cbxLoaiVeVao.SelectedItem?.ToString() ?? "";
        public string SoVe     => txtSoVeVao.Text.Trim();

        // Events để Presenter đăng ký
        public event EventHandler MoBarrierVaoClicked;
        public event EventHandler DongBarrierVaoClicked;
        public event EventHandler MoBarrierRaClicked;
        public event EventHandler DongBarrierRaClicked;
        public event EventHandler SoVeRaTextChanged;
        public event EventHandler TimerTick;

        // Các phương thức Presenter gọi vào
        public void ShowError(string message)
            => MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

        public void ShowInfo(string message)
            => MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

        public void DisplayXeInfo(XeVao xe)
        {
            if (xe == null)
            {
                lblBienSoRa.Text =
                lblLoaiVeRa.Text =
                lblThoiGianRa.Text =
                lblPhatMuon.Text = "";
                return;
            }
            lblBienSoRa.Text = xe.BienSoXe;
            lblLoaiVeRa.Text = xe.LoaiVe;
            lblThoiGianRa.Text = xe.ThoiGianVao.ToString("dd/MM/yyyy | H:mm:ss");
            lblPhatMuon.Text = xe.TienPhat > 0
                                  ? $"{xe.TienPhat:N0} VND | muộn {Math.Floor((DateTime.Now - xe.ThoiGianVao).TotalHours)} giờ"
                                  : "0 VND";
        }

        public void ToggleBarrierVao(bool isOpen)
            => lblTrangThaiVao.BackColor = isOpen ? Color.LimeGreen : Color.Red;

        public void ToggleBarrierRa(bool isOpen)
            => lblTrangThaiRa.BackColor = isOpen ? Color.LimeGreen : Color.Red;

        #endregion

        #region
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

        private void btnMoBarrier_Click_1(object sender, EventArgs e){ }
        private void txtSoVeRa_TextChanged(object sender, EventArgs e) { }
        private void button1_Click(object sender, EventArgs e) { }
        private void btnDongBarrier_Click(object sender, EventArgs e) { }
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

        #endregion
    }
}
