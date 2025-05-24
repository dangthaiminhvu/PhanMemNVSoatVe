using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanMemNVSoatVe.Models
{
    public class NhanVien
    {
        public string IDNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string GioiTinh { get; set; }
        public DateTime NgaySinh { get; set; }
        public string Email { get; set; }
        public DateTime NgayVaoLam { get; set; }
        public string SDT { get; set; }
        public string DiaChi { get; set; }
        public string ChucVu { get; set; }
        public decimal MucLuong { get; set; }
        public string MatKhau { get; set; }
        public string TrangThai { get; set; }
    }
}
