using MySql.Data.MySqlClient;
using PhanMemNVSoatVe.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanMemNVSoatVe.DataAccess
{
    public class MySqlNhanVienRepository : INhanVienRepository
    {
        private readonly string _connectionString;
        public MySqlNhanVienRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable GetAll()
        {
            var dt = new DataTable();
            const string sql = "SELECT * FROM dulieunhanvien";
            using (var conn = new MySqlConnection(_connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            using (var adapter = new MySqlDataAdapter(cmd))
            {
                adapter.Fill(dt);
            }
            return dt;
        }

        public DataRow GetById(string id)
        {
            var dt = new DataTable();
            const string sql = "SELECT * FROM dulieunhanvien WHERE IDNhanVien = @id";
            using (var conn = new MySqlConnection(_connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (var adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public bool Insert(NhanVien nv)
        {
            const string sql = @"
                INSERT INTO dulieunhanvien
                (IDNhanVien, TenNhanVien, GioiTinh, NgaySinh, Email, NgayVaoLam, SDT, DiaChi, ChucVu, MucLuong, MatKhau, TrangThai)
                VALUES
                (@id, @ten, @gt, @ns, @email, @nvl, @sdt, @dc, @cv, @ml, @mk, @tt)";
            using (var conn = new MySqlConnection(_connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", nv.IDNhanVien);
                cmd.Parameters.AddWithValue("@ten", nv.TenNhanVien);
                cmd.Parameters.AddWithValue("@gt", nv.GioiTinh);
                cmd.Parameters.AddWithValue("@ns", nv.NgaySinh);
                cmd.Parameters.AddWithValue("@email", nv.Email);
                cmd.Parameters.AddWithValue("@nvl", nv.NgayVaoLam);
                cmd.Parameters.AddWithValue("@sdt", nv.SDT);
                cmd.Parameters.AddWithValue("@dc", nv.DiaChi);
                cmd.Parameters.AddWithValue("@cv", nv.ChucVu);
                cmd.Parameters.AddWithValue("@ml", nv.MucLuong);
                cmd.Parameters.AddWithValue("@mk", nv.MatKhau);
                cmd.Parameters.AddWithValue("@tt", nv.TrangThai);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(NhanVien nv)
        {
            const string sql = @"
                UPDATE dulieunhanvien SET
                    TenNhanVien = @ten,
                    GioiTinh    = @gt,
                    NgaySinh    = @ns,
                    Email       = @email,
                    NgayVaoLam  = @nvl,
                    SDT         = @sdt,
                    DiaChi      = @dc,
                    ChucVu      = @cv,
                    MucLuong    = @ml,
                    MatKhau     = @mk,
                    TrangThai   = @tt
                WHERE IDNhanVien = @id";
            using (var conn = new MySqlConnection(_connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", nv.IDNhanVien);
                cmd.Parameters.AddWithValue("@ten", nv.TenNhanVien);
                cmd.Parameters.AddWithValue("@gt", nv.GioiTinh);
                cmd.Parameters.AddWithValue("@ns", nv.NgaySinh);
                cmd.Parameters.AddWithValue("@email", nv.Email);
                cmd.Parameters.AddWithValue("@nvl", nv.NgayVaoLam);
                cmd.Parameters.AddWithValue("@sdt", nv.SDT);
                cmd.Parameters.AddWithValue("@dc", nv.DiaChi);
                cmd.Parameters.AddWithValue("@cv", nv.ChucVu);
                cmd.Parameters.AddWithValue("@ml", nv.MucLuong);
                cmd.Parameters.AddWithValue("@mk", nv.MatKhau);
                cmd.Parameters.AddWithValue("@tt", nv.TrangThai);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(string id)
        {
            const string sql = "DELETE FROM dulieunhanvien WHERE IDNhanVien = @id";
            using (var conn = new MySqlConnection(_connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
