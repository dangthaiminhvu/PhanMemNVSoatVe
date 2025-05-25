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
            _view.ResetFilterClicked += (_, __) => ResetFilter();
            _view.AddClicked += (_, __) => AddRecord();
            _view.UpdateClicked += (_, __) => UpdateRecord();
            _view.DeleteClicked += (_, __) => DeleteRecord();
            _view.ResetNewClicked += (_, __) => {};
            _view.EditIDChanged += (_, __) => PopulateEdit();

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

        private void AddRecord()
        {
            var xe = new XeVao
            {
                BienSoXe = _view.NewBienSo,
                LoaiVe = _view.NewLoaiVe,
                SoVe = _view.NewSoVe,
                ThoiGianVao = _view.NewThoiGianVao,
                TrangThaiVe = _view.NewTrangThaiVe,
                ThoiGianRa = _view.NewTrangThaiVe == "DaTra" ? _view.NewThoiGianRa : null,
                TienPhat = _view.NewTienPhat
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
