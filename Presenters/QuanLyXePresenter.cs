using PhanMemNVSoatVe.DataAccess;
using PhanMemNVSoatVe.Models;
using PhanMemNVSoatVe.Views;
using System;
using System.Text.RegularExpressions;

namespace PhanMemNVSoatVe.Presenters
{
    public class QuanLyXePresenter
    {
        private bool _barrierVaoIsOpen = false;
        private bool _barrierRaIsOpen = false;

        private readonly IQuanLyXeView _view;
        private readonly IXeVaoRepository _repo;
        private static readonly Regex _regexBienSo = new Regex(
            @"^[0-9]{2}[A-Z]-[A-Z0-9]{1,2}\s?[0-9]{3,4}$",
            RegexOptions.Compiled);

        public QuanLyXePresenter(IQuanLyXeView view, IXeVaoRepository repo)
        {
            _view = view;
            _repo = repo;
            _barrierVaoIsOpen = false;
            _barrierRaIsOpen = false;

            _view.MoBarrierVaoClicked += OnMoBarrierVao;
            _view.DongBarrierVaoClicked += (s, e) => {
                _barrierVaoIsOpen = false;
                _view.ToggleBarrierVao(false);
            };

            _view.MoBarrierRaClicked += OnTraXe;
            _view.DongBarrierRaClicked += (s, e) => {
                _barrierRaIsOpen = false;
                _view.ToggleBarrierRa(false);
            };

            _view.SoVeRaTextChanged += OnSoVeRaChanged;
            _view.TimerTick += (s, e) => { };
        }

        // Cấp vé: dùng SoVeVao
        private void OnMoBarrierVao(object sender, EventArgs e)
        {
            var now = DateTime.Now;

            // 0) Không cho cấp vé sau 22:00
            TimeSpan gioDongCua = new TimeSpan(22, 0, 0); // 22:00
            if (now.TimeOfDay >= gioDongCua)
            {
                _view.ShowError($"Hệ thống không nhận xe sau {gioDongCua:hh\\:mm} (hiện tại: {now:HH:mm}).");
                return;
            }

            // 1) Chặn re-entry nếu barrier vẫn đang mở hoặc xe có biển số nào đó vẫn chưa trả
            if (_barrierVaoIsOpen)
            {
                _view.ShowError("Barrier cấp vé vẫn đang mở, vui lòng đóng trước khi cấp vé mới.");
                return;
            }

            var existingByPlate = _repo.GetByBienSoChuaTra(_view.BienSo);
            if (existingByPlate != null)
            {
                _view.ShowError($"Xe biển số {_view.BienSo} đang còn chưa trả vé trước đó.");
                return;
            }

            // 2) Chỉ áp dụng regex với xe máy và ô tô
            var loai = _view.LoaiVe.Trim().ToLowerInvariant();
            bool laXeCoBienSo = loai.Contains("xe máy") || loai.Contains("ô tô");
            if (laXeCoBienSo && !_regexBienSo.IsMatch(_view.BienSo))
            {
                _view.ShowError("Biển số không hợp lệ.");
                return;
            }

            // 3) Chặn cấp vé nếu đã có vé cùng số chưa trả
            var existing = _repo.GetBySoVe(_view.SoVeVao);
            if (existing != null && existing.TrangThaiVe == "ChuaTra")
            {
                _view.ShowError($"Số vé {_view.SoVeVao} đang còn chưa trả, không thể cấp lại.");
                return;
            }

            // 4) Mọi check ok, insert vé mới
            var xe = new XeVao
            {
                BienSoXe = _view.BienSo,
                LoaiVe = _view.LoaiVe,
                SoVe = _view.SoVeVao,
                ThoiGianVao = now,
                GiaHan = false
            };

            if (_repo.Insert(xe))
            {
                // 5) Mở barrier và set flag chỉ khi insert thành công
                _barrierVaoIsOpen = true;
                _view.ToggleBarrierVao(true);
                _view.ShowInfo("Cấp vé thành công.");
            }
            else
            {
                _view.ShowError("Lỗi khi cấp vé.");
            }
        }

        private double TinhTienPhat(XeVao xe, DateTime referenceTime)
        {
            var gateCloseTime = xe.ThoiGianVao.Date.AddHours(22);
            if (referenceTime <= gateCloseTime)
                return 0;

            var lateSpan = referenceTime - gateCloseTime;
            // Nếu chưa gia hạn
            if (!xe.GiaHan)
            {
                if (lateSpan.TotalHours < 1)
                    return 5000;
                else
                    return Math.Floor(lateSpan.TotalHours) * 10000;
            }
            // Nếu đã gia hạn
            else
            {
                if (lateSpan.TotalHours <= 1)
                    return 0;
                else
                    return Math.Floor(lateSpan.TotalHours) * 10000;
            }
        }

        // Gõ số vé vào ô trả xe: lấy vé chưa trả
        private void OnSoVeRaChanged(object sender, EventArgs e)
        {
            var soVeRa = _view.SoVeRa;
            if (string.IsNullOrWhiteSpace(soVeRa))
            {
                _view.DisplayXeInfo(null);
                return;
            }

            var xe = _repo.GetBySoVe(soVeRa);
            if (xe == null || xe.TrangThaiVe != "ChuaTra")
            {
                _view.DisplayXeInfo(null);
                return;
            }

            // Tính phạt ngay khi nhập
            var now = DateTime.Now;
            xe.TienPhat = TinhTienPhat(xe, now);

            _view.DisplayXeInfo(xe);
        }


        // Khi bấm “Trả xe” (MoBarrierRaClicked)
        private void OnTraXe(object sender, EventArgs e)
        {
            if (_barrierRaIsOpen)
            {
                _view.ShowError("Barrier trả xe đang mở, vui lòng đóng trước khi trả xe tiếp.");
                return;
            }

            var soVeRa = _view.SoVeRa;
            var xe = _repo.GetBySoVe(soVeRa);

            if (xe == null)
            {
                _view.ShowError("Số vé không tồn tại.");
                return;
            }
            if (xe.TrangThaiVe != "ChuaTra")
            {
                _view.ShowError("Vé này đã được trả.");
                return;
            }

            var now = DateTime.Now;
            double phat = TinhTienPhat(xe, now);

            if (_repo.CapNhatRaVe(soVeRa, now, phat))
            {
                xe.TienPhat = phat;
                _view.ToggleBarrierRa(true);
                _view.DisplayXeInfo(xe);
                _view.ShowInfo($"Trả xe thành công. Phí: {phat:N0} VND");
            }
            else
            {
                _view.ShowError("Lỗi khi trả xe.");
            }


            xe.TienPhat = phat;
            _view.DisplayXeInfo(xe);


            if (_repo.CapNhatRaVe(soVeRa, now, phat))
            {
                _view.ToggleBarrierRa(true);
                xe.TienPhat = phat;
                _view.DisplayXeInfo(xe);
                _view.ShowInfo($"Trả xe thành công. Phí: {phat:N0} VND");
            }
            else
            {
                _view.ShowError("Lỗi khi trả xe.");
            }
        }

    }
}
