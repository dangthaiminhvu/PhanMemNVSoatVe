using System;
using System.Collections.Generic;
using PhanMemNVSoatVe.Models;

namespace PhanMemNVSoatVe.DataAccess
{
    public interface IXeVaoRepository
    {
        IEnumerable<XeVao> GetXeDangGui();
        XeVao GetBySoVe(string soVe);
        XeVao GetByBienSoChuaTra(string bienSo);
        int GetSoVeMoi();
        bool CheckSoVeDangSuDung(string soVe);
        bool Insert(XeVao xe);
        bool CapNhatRaVe(string soVe, DateTime thoiGianRa, double tienPhat);
    }
}