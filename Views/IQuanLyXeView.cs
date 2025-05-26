using PhanMemNVSoatVe.Models;
using System;

public interface IQuanLyXeView
{
    // Cấp vé
    string BienSo { get; }
    string LoaiVe { get; }
    string SoVeVao { get; }

    // Trả vé
    string SoVeRa { get; }

    void ShowError(string message);
    void ShowInfo(string message);
    void DisplayXeInfo(XeVao xe);
    void ToggleBarrierVao(bool isOpen);
    void ToggleBarrierRa(bool isOpen);

    event EventHandler MoBarrierVaoClicked;
    event EventHandler DongBarrierVaoClicked;
    event EventHandler MoBarrierRaClicked;
    event EventHandler DongBarrierRaClicked;
    event EventHandler SoVeRaTextChanged;
    event EventHandler TimerTick;
}
