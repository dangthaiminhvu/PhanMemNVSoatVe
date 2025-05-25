using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using PhanMemNVSoatVe.DataAccess;
using PhanMemNVSoatVe.Views;

namespace PhanMemNVSoatVe.Presenters
{
    public class QuanLyKhachHangPresenter
    {
        private readonly IQuanLyKhachHangView _view;
        private readonly IKhachHangRepository _repo;
        private DataTable _dt;

        public QuanLyKhachHangPresenter(IQuanLyKhachHangView view)
        {
            _view = view;
            _repo = new MySqlKhachHangRepository();

            // Đăng ký sự kiện từ View
            _view.LoadData += View_LoadData;
            _view.GiaHanClicked += View_GiaHanClicked;
            _view.HuyGiaHanClicked += View_HuyGiaHanClicked;
            _view.TimKiemClicked += View_TimKiemClicked;
            _view.NhapLaiClicked += View_NhapLaiClicked;
            _view.LuuSuCoClicked += View_LuuSuCoClicked;
        }

        private void View_LoadData(object sender, EventArgs e)
        {
            LoadTatCaDuLieu();
        }

        private void LoadTatCaDuLieu()
        {
            _dt = _repo.GetChuaTra();
            _view.DataSource = _dt;
        }

        private void View_GiaHanClicked(object sender, EventArgs e)
        {
            int id = _view.SelectedID;
            if (id <= 0) return;

            // Giả sử View đã xác nhận Yes/No rồi
            _repo.GiaHan(id);
            LoadTatCaDuLieu();
        }

        private void View_HuyGiaHanClicked(object sender, EventArgs e)
        {
            int id = _view.SelectedID;
            if (id <= 0) return;

            _repo.HuyGiaHan(id);
            LoadTatCaDuLieu();
        }

        private void View_TimKiemClicked(object sender, EventArgs e)
        {
            if (_dt == null) return;

            var filters = new System.Collections.Generic.List<string>();

            var bs = _view.TimKiemBienSo.Replace("'", "''");
            if (!string.IsNullOrEmpty(bs))
                filters.Add($"BienSoXe LIKE '%{bs}%'");

            var lv = _view.TimKiemLoaiVe?.Replace("'", "''");
            if (!string.IsNullOrEmpty(lv))
                filters.Add($"LoaiVe = '{lv}'");

            var sv = _view.TimKiemSoVe.Replace("'", "''");
            if (!string.IsNullOrEmpty(sv))
                filters.Add($"SoVe = '{sv}'");

            if (DateTime.TryParse(_view.TimKiemThoiGianVao, out var vaoDt))
            {
                var d = vaoDt.Date;
                var d2 = d.AddDays(1);
                filters.Add($"ThoiGianVao >= '{d:yyyy-MM-dd}' AND ThoiGianVao < '{d2:yyyy-MM-dd}'");
            }

            _dt.DefaultView.RowFilter = filters.Count > 0
                ? string.Join(" AND ", filters)
                : string.Empty;
        }

        private void View_NhapLaiClicked(object sender, EventArgs e)
        {
            _dt.DefaultView.RowFilter = string.Empty;
            LoadTatCaDuLieu();
        }

        private void View_LuuSuCoClicked(object sender, EventArgs e)
        {
            var row = _repo.LuuSuCo(
                _view.SuCoTenKhachHang,
                _view.SuCoNgaySinh,
                _view.SuCoGioiTinh,
                _view.SuCoCCCD,
                _view.SuCoSoDienThoai,
                _view.SuCoLoaiXe,
                _view.SuCoBienSo,
                _view.SuCoNgayGui,
                _view.SuCoNgayNhan,
                _view.SuCoMoTa,
                _view.SuCoYeuCauKhachHang
            );
            if (row > 0)
                _view.ShowMessage("Lưu thành công!");
            else
                _view.ShowMessage("Lưu thất bại");
        }
    }
}

