using System;
using System.ComponentModel;
using System.Linq;
using PhanMemNVSoatVe.DataAccess;
using PhanMemNVSoatVe.Models;
using PhanMemNVSoatVe.Views;

namespace PhanMemNVSoatVe.Presenters
{
    public class QuanLyThongTinPresenter
    {
        private readonly IQuanLyThongTinView _view;
        private readonly IDuLieuXeVaoRepository _repo;
        private BindingList<XeVao> _allData;

        public QuanLyThongTinPresenter(IQuanLyThongTinView view, IDuLieuXeVaoRepository repo)
        {
            _view = view;
            _repo = repo;

            // Đăng ký event
            _view.LoadData += (_, __) => LoadAll();
            _view.FilterChanged += (_, __) => ApplyFilter();
            _view.AddClicked += (_, __) => AddRecord();
            _view.UpdateClicked += (_, __) => UpdateRecord();
            _view.DeleteClicked += (_, __) => DeleteRecord();
            _view.ResetNewClicked += (_, __) => { };
            _view.EditIDChanged += (_, __) => PopulateEdit();
            _view.ResetFilterClicked += (_, __) =>
            {
                ResetFilter();
                _view.ClearFilterInputs();
            };

            // Load lần đầu
            LoadAll();
        }

        private void LoadAll()
        {
            var list = new BindingList<XeVao>(_repo.GetAll().ToList());
            _allData = list;
            _view.GridData = list;
        }

        private void ApplyFilter()
        {
            var q = _allData.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(_view.FilterBienSo))
                q = q.Where(x => x.BienSoXe.IndexOf(_view.FilterBienSo, StringComparison.OrdinalIgnoreCase) >= 0);
            if (!string.IsNullOrEmpty(_view.FilterLoaiVe))
                q = q.Where(x => x.LoaiVe == _view.FilterLoaiVe);
            if (!string.IsNullOrWhiteSpace(_view.FilterSoVe) && int.TryParse(_view.FilterSoVe, out var sv))
                q = q.Where(x => int.TryParse(x.SoVe, out var v) && v == sv);
            if (_view.FilterVaoDate.HasValue)
                q = q.Where(x => x.ThoiGianVao.Date == _view.FilterVaoDate.Value.Date);
            if (_view.FilterRaDate.HasValue)
                q = q.Where(x => x.ThoiGianRa?.Date == _view.FilterRaDate.Value.Date);
            if (_view.FilterDaTra ^ _view.FilterChuaTra)
                q = q.Where(x => x.TrangThaiVe == (_view.FilterDaTra ? "DaTra" : "ChuaTra"));

            _view.GridData = new BindingList<XeVao>(q.ToList());
        }

        private void ResetFilter()
        {
            _view.ShowInfo("Filter reset.");
            LoadAll();
        }

        private bool KiemTraBienSoHopLe(string bienSo, string loaiVe)
        {
            if (loaiVe != "vé xe máy" && loaiVe != "vé ô tô")
                return true;

            // Regex cơ bản: 2 số đầu + 1 hoặc 2 chữ cái + 4-5 số
            var regexXeMay = @"^\d{2}[A-Z]\d{4,5}$";     // VD: 29A12345
            var regexOTo = @"^\d{2}[A-Z]{1,2}\d{4,5}$";  // VD: 30AB12345

            return loaiVe == "vé xe máy"
                ? System.Text.RegularExpressions.Regex.IsMatch(bienSo, regexXeMay, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                : System.Text.RegularExpressions.Regex.IsMatch(bienSo, regexOTo, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        private void AddRecord()
        {
            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrWhiteSpace(_view.NewBienSo) ||
                string.IsNullOrWhiteSpace(_view.NewLoaiVe) ||
                string.IsNullOrWhiteSpace(_view.NewSoVe) ||
                string.IsNullOrWhiteSpace(_view.NewTrangThaiVe))
            {
                _view.ShowError("Vui lòng nhập đầy đủ thông tin bắt buộc.");
                return;
            }

            // Kiểm tra định dạng biển số nếu là xe máy hoặc ô tô
            if ((_view.NewLoaiVe == "Vé xe máy" || _view.NewLoaiVe == "Vé ô tô") &&
                !System.Text.RegularExpressions.Regex.IsMatch(_view.NewBienSo, @"^\d{2}[A-Z]-\d{3,5}$"))
            {
                _view.ShowError("Biển số xe không hợp lệ. Vui lòng nhập đúng định dạng.");
                return;
            }

            // Kiểm tra số vé chưa tồn tại với vé chưa trả
            var soVe = _view.NewSoVe;
            var trung = _repo.GetAll().Any(x => x.SoVe == soVe && x.TrangThaiVe == "ChuaTra");
            if (trung)
            {
                _view.ShowError($"Không thể cấp thẻ. Vé {soVe} vẫn chưa được trả.");
                return;
            }

            // Ánh xạ trạng thái
            string trangThaiDb;

            if (_view.NewTrangThaiVe == "Đã trả")
            {
                trangThaiDb = "DaTra";
            }
            else if (_view.NewTrangThaiVe == "Chưa trả")
            {
                trangThaiDb = "ChuaTra";
            }
            else
            {
                throw new Exception("Giá trị trạng thái vé không hợp lệ.");
            }


            // Lấy thời gian ra nếu là đã trả, nếu không thì để null
            DateTime? tgRa = trangThaiDb == "DaTra" ? _view.NewThoiGianRa : null;

            // Tạo đối tượng mới
            var xe = new XeVao
            {
                BienSoXe = _view.NewBienSo,
                LoaiVe = _view.NewLoaiVe,
                SoVe = soVe,
                ThoiGianVao = _view.NewThoiGianVao,
                TrangThaiVe = trangThaiDb,
                GiaHan = _view.NewGiaHan, // false nếu không tích
                ThoiGianRa = tgRa,
                TienPhat = _view.NewTienPhat // mặc định là 0 nếu không nhập
            };

            if (_repo.Insert(xe))
            {
                _view.ShowInfo("Thêm thành công.");
                LoadAll();
            }
            else
                _view.ShowError("Thêm thất bại.");
        }



        private void PopulateEdit()
        {
            if (int.TryParse(_view.EditID, out var id))
            {
                var xe = _repo.GetById(id);
                _view.ShowEditSection(xe);
            }
        }

        private void UpdateRecord()
        {
            if (!int.TryParse(_view.EditID, out var id)) return;
            var xe = _repo.GetById(id);
            if (xe == null) return;
            // Presenter không modify tất cả field, ShowEditSection đã gán UI->model
            if (_repo.Update(xe)) { _view.ShowInfo("Cập nhật thành công."); LoadAll(); }
            else _view.ShowError("Cập nhật thất bại.");
        }

        private void DeleteRecord()
        {
            if (int.TryParse(_view.EditID, out var id) && _repo.Delete(id))
            {
                _view.ShowInfo("Xóa thành công.");
                LoadAll();
            }
            else
                _view.ShowError("Xóa thất bại.");
        }
    }
}
