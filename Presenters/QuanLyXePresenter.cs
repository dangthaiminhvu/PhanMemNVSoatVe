using PhanMemNVSoatVe.DataAccess;
using PhanMemNVSoatVe.Models;
using PhanMemNVSoatVe.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhanMemNVSoatVe.Presenters
{
    public class QuanLyXePresenter
    {
        private readonly IQuanLyXeView _view;
        private readonly IXeVaoRepository _repo;
        private static readonly Regex _regexBienSo = new Regex(
            @"^[0-9]{2}[A-Z]-[A-Z0-9]{1,2}\s?[0-9]{3,4}$",
            RegexOptions.Compiled);

        public QuanLyXePresenter(IQuanLyXeView view, IXeVaoRepository repo)
        {
            _view = view;
            _repo = repo;

            // Đăng ký sự kiện
            _view.MoBarrierVaoClicked += OnMoBarrierVao;
            _view.DongBarrierVaoClicked += (s, e) => _view.ToggleBarrierVao(false);
            _view.MoBarrierRaClicked += OnTraXe;
            _view.DongBarrierRaClicked += (s, e) => _view.ToggleBarrierRa(false);
            _view.SoVeRaTextChanged += OnSoVeRaChanged;
            _view.TimerTick += (s, e) => { };
        }

        private void OnMoBarrierVao(object sender, EventArgs e)
        {
            if (!_regexBienSo.IsMatch(_view.BienSo))
            {
                _view.ShowError("Biển số không hợp lệ.");
                return;
            }

            if (_repo.GetByBienSoChuaTra(_view.BienSo) != null)
            {
                _view.ShowError($"Xe {_view.BienSo} đang còn trong bãi.");
                return;
            }

            var now = DateTime.Now;
            var xe = new XeVao
            {
                BienSoXe = _view.BienSo,
                LoaiVe = _view.LoaiVe,
                SoVe = _view.SoVe,
                ThoiGianVao = now,
                GiaHan = false
            };

            if (_repo.Insert(xe))
            {
                _view.ToggleBarrierVao(true);
                _view.ShowInfo("Cấp vé thành công.");
            }
            else
            {
                _view.ShowError("Lỗi khi cấp vé.");
            }
        }

        private void OnSoVeRaChanged(object sender, EventArgs e)
        {
            var xe = _repo.GetBySoVe(_view.SoVe);
            _view.DisplayXeInfo(xe);
        }

        private void OnTraXe(object sender, EventArgs e)
        {
            var soVe = _view.SoVe;
            var xe = _repo.GetBySoVe(soVe);
            if (xe == null)
            {
                _view.ShowError("Số vé không tồn tại.");
                return;
            }

            var now = DateTime.Now;
            var delta = now - xe.ThoiGianVao;
            double phat = 0;

            if (delta.TotalHours > (xe.GiaHan ? 1 : 0))
            {
                phat = Math.Floor(delta.TotalHours) * 10000;
                if (!xe.GiaHan && delta.TotalHours < 1)
                    phat = 5000;
            }

            if (_repo.CapNhatRaVe(soVe, now, phat))
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
