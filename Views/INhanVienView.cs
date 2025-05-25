using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace PhanMemNVSoatVe.Views
{
    public interface INhanVienView
    {
        void SetNhanVienList(DataTable dt);
        void ShowMessage(string message);
        string IDNhanVien { get; }
        string TenNhanVien { get; }
        string GioiTinh { get; }
        DateTime NgaySinh { get; }
        string Email { get; }
        DateTime NgayVaoLam { get; }
        string SDT { get; }
        string DiaChi { get; }
        string ChucVu { get; }
        decimal MucLuong { get; }
        string MatKhau { get; }
        string TrangThai { get; }

        void ClearForm();
    }
}

