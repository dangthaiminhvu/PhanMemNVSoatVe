using System;
using System.Data;

namespace PhanMemNVSoatVe.Views
{
    public interface IQuanLyKhachHangView
    {
        DataTable DataSource { set; }

        // Các sự kiện do View phát ra, Presenter lắng nghe
        event EventHandler LoadData;
        event EventHandler GiaHanClicked;
        event EventHandler HuyGiaHanClicked;
        event EventHandler TimKiemClicked;
        event EventHandler NhapLaiClicked;
        event EventHandler LuuSuCoClicked;

        // Lấy thông tin input từ View, Presenter dùng để xử lý
        string TimKiemBienSo { get; }
        string TimKiemLoaiVe { get; }
        string TimKiemSoVe { get; }
        string TimKiemThoiGianVao { get; }

        int SelectedID { get; }

        // Thông tin sự cố (cho btnLuuSuCo)
        string SuCoTenKhachHang { get; }
        DateTime SuCoNgaySinh { get; }
        string SuCoGioiTinh { get; }
        string SuCoCCCD { get; }
        string SuCoSoDienThoai { get; }
        string SuCoLoaiXe { get; }
        string SuCoBienSo { get; }
        DateTime SuCoNgayGui { get; }
        DateTime SuCoNgayNhan { get; }
        string SuCoMoTa { get; }
        string SuCoYeuCauKhachHang { get; }

        // Hiển thị thông báo
        void ShowMessage(string message);
    }
}

