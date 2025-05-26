using System;
using System.ComponentModel;
using PhanMemNVSoatVe.Models;

namespace PhanMemNVSoatVe.Views
{
    public interface IQuanLyThongTinView
    {
        // Input/filter từ UI
        string FilterBienSo { get; }
        string FilterLoaiVe { get; }
        string FilterSoVe { get; }
        DateTime? FilterVaoDate { get; }
        DateTime? FilterRaDate { get; }
        bool FilterDaTra { get; }
        bool FilterChuaTra { get; }
        bool NewGiaHan { get; }


        // Dữ liệu hiển thị
        BindingList<XeVao> GridData { set; }

        // Thêm mới
        string NewBienSo { get; }
        string NewLoaiVe { get; }
        string NewSoVe { get; }
        DateTime NewThoiGianVao { get; }
        string NewTrangThaiVe { get; }
        DateTime? NewThoiGianRa { get; }
        double NewTienPhat { get; }

        // Sửa
        string EditID { get; }
        void ShowEditSection(XeVao xe);

        void ClearFilterInputs();

        // Message
        void ShowError(string msg);
        void ShowInfo(string msg);

        // Events
        event EventHandler LoadData;
        event EventHandler FilterChanged;
        event EventHandler ResetFilterClicked;
        event EventHandler AddClicked;
        event EventHandler UpdateClicked;
        event EventHandler DeleteClicked;
        event EventHandler ResetNewClicked;
        event EventHandler EditIDChanged;
    }
}
