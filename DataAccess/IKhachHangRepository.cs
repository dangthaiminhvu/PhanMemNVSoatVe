using System;
using System.Data;

namespace PhanMemNVSoatVe.DataAccess
{
    internal interface IKhachHangRepository
    {
        DataTable GetChuaTra();
        void GiaHan(int id);
        void HuyGiaHan(int id);
        int LuuSuCo(
            string ten, DateTime ngaySinh, string gioiTinh,
            string cccd, string sdt, string loaiXe, string bienSo,
            DateTime ngayGui, DateTime ngayNhan,
            string moTa, string yeuCau);
        bool KiemTraXeTonTai(string bienSo, DateTime ngayGui, string loaiVe);
    }
}
