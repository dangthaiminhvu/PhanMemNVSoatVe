using PhanMemNVSoatVe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanMemNVSoatVe.Views
{
    public interface IQuanLyXeView
    {
        // Dữ liệu từ UI
        string BienSo { get; }
        string LoaiVe { get; }
        string SoVe { get; }

        // Hiển thị lên UI
        void ShowError(string message);
        void ShowInfo(string message);
        void DisplayXeInfo(XeVao xe);
        void ToggleBarrierVao(bool isOpen);
        void ToggleBarrierRa(bool isOpen);

        // Sự kiện do View phát ra
        event EventHandler MoBarrierVaoClicked;
        event EventHandler DongBarrierVaoClicked;
        event EventHandler MoBarrierRaClicked;
        event EventHandler DongBarrierRaClicked;
        event EventHandler SoVeRaTextChanged;
        event EventHandler TimerTick;
    }

}
