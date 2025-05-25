using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace PhanMemNVSoatVe.DataAccess
{
    public class MySqlKhachHangRepository : IKhachHangRepository
    {
        private readonly string _conn = ConfigurationManager
            .ConnectionStrings["MyConnStr"].ConnectionString;

        public DataTable GetChuaTra()
        {
            var dt = new DataTable();
            using (var cn = new MySqlConnection(_conn))
            using (var da = new MySqlDataAdapter(
                @"SELECT ID, BienSoXe, LoaiVe, SoVe, ThoiGianVao, GiaHan
                  FROM DuLieuXeVao
                  WHERE TrangThaiVe='ChuaTra'", cn))
            {
                cn.Open();
                da.Fill(dt);
            }
            return dt;
        }

        public void GiaHan(int id)
        {
            using (var cn = new MySqlConnection(_conn))
            using (var cmd = new MySqlCommand(
                "UPDATE DuLieuXeVao SET GiaHan=1 WHERE ID=@id", cn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void HuyGiaHan(int id)
        {
            using (var cn = new MySqlConnection(_conn))
            using (var cmd = new MySqlCommand(
                "UPDATE DuLieuXeVao SET GiaHan=0 WHERE ID=@id", cn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public int LuuSuCo(
            string ten, DateTime ngaySinh, string gioiTinh,
            string cccd, string sdt, string loaiXe, string bienSo,
            DateTime ngayGui, DateTime ngayNhan,
            string moTa, string yeuCau)
        {
            using (var cn = new MySqlConnection(_conn))
            using (var cmd = cn.CreateCommand())
            {
                cn.Open();
                cmd.CommandText = @"
                    INSERT INTO dulieusuco
                    (TenKhachHang, NgaySinh, GioiTinh, CCCD, SDT,
                     LoaiXe, BienSoXe, NgayGui, NgayNhan, MoTaSuCo, YeuCauKhachHang)
                    VALUES
                    (@ten, @ns, @gt, @cccd, @sdt,
                     @loai, @bien, @ngui, @nnhan, @mota, @yc)";
                cmd.Parameters.AddWithValue("@ten", ten);
                cmd.Parameters.AddWithValue("@ns", ngaySinh);
                cmd.Parameters.AddWithValue("@gt", gioiTinh);
                cmd.Parameters.AddWithValue("@cccd", cccd);
                cmd.Parameters.AddWithValue("@sdt", sdt);
                cmd.Parameters.AddWithValue("@loai", loaiXe);
                cmd.Parameters.AddWithValue("@bien", bienSo);
                cmd.Parameters.AddWithValue("@ngui", ngayGui);
                cmd.Parameters.AddWithValue("@nnhan", ngayNhan);
                cmd.Parameters.AddWithValue("@mota", moTa);
                cmd.Parameters.AddWithValue("@yc", yeuCau);
                return cmd.ExecuteNonQuery();
            }
        }


        public bool KiemTraXeTonTai(string bienSo, DateTime thoiGianVao, string loaiVe)
        {
            using (var cn = new MySqlConnection(_conn))
            using (var cmd = new MySqlCommand(
            @"SELECT COUNT(*) FROM DuLieuXeVao 
          WHERE BienSoXe = @bienSo AND ThoiGianVao = @thoiGianVao AND LoaiVe = @loaiVe", cn))
            {
                cmd.Parameters.AddWithValue("@bienSo", bienSo);
                cmd.Parameters.AddWithValue("@thoiGianVao", thoiGianVao);
                cmd.Parameters.AddWithValue("@loaiVe", loaiVe);

                cn.Open();
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }
    }
}
