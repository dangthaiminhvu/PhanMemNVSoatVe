using System;

namespace PhanMemNVSoatVe.Models
{
    public class XeVao
    {
        public int ID { get; set; }
        public string BienSoXe { get; set; } = string.Empty;
        public string LoaiVe { get; set; } = string.Empty;
        public string SoVe { get; set; } = string.Empty;
        public DateTime ThoiGianVao { get; set; }
        public bool GiaHan { get; set; }
        public string TrangThaiVe { get; set; } = "ChuaTra";
        public DateTime? ThoiGianRa { get; set; }
        public double TienPhat { get; set; } = 0;
    }
}