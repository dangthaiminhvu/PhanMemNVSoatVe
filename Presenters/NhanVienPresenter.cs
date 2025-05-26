using PhanMemNVSoatVe.DataAccess;
using PhanMemNVSoatVe.Models;
using PhanMemNVSoatVe.Views;

namespace PhanMemNVSoatVe.Presenters
{
    public class NhanVienPresenter
    {
        private readonly INhanVienView _view;
        private readonly INhanVienRepository _repository;

        public NhanVienPresenter(INhanVienView view, INhanVienRepository repository)
        {
            _view = view;
            _repository = repository;
        }

        public void LoadAll()
        {
            var dt = _repository.GetAll();
            _view.SetNhanVienList(dt);
        }

        public void Insert()
        {
            var nv = CreateNhanVienFromView();
            if (_repository.Insert(nv))
                _view.ShowMessage("Thêm thành công!");
            else
                _view.ShowMessage("Thêm thất bại!");
        }

        public void Update()
        {
            var nv = CreateNhanVienFromView();
            if (_repository.Update(nv))
                _view.ShowMessage("Cập nhật thành công!");
            else
                _view.ShowMessage("Cập nhật thất bại!");
        }

        public void Delete()
        {
            if (_repository.Delete(_view.IDNhanVien))
                _view.ShowMessage("Xóa thành công!");
            else
                _view.ShowMessage("Xóa thất bại!");
        }

        private NhanVien CreateNhanVienFromView()
        {
            return new NhanVien
            {
                IDNhanVien = _view.IDNhanVien,
                TenNhanVien = _view.TenNhanVien,
                GioiTinh = _view.GioiTinh,
                NgaySinh = _view.NgaySinh,
                Email = _view.Email,
                NgayVaoLam = _view.NgayVaoLam,
                SDT = _view.SDT,
                DiaChi = _view.DiaChi,
                ChucVu = _view.ChucVu,
                MucLuong = _view.MucLuong,
                MatKhau = _view.MatKhau,
                TrangThai = _view.TrangThai
            };
        }
    }
}

