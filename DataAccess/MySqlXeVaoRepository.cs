using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using PhanMemNVSoatVe.Models;
using PhanMemNVSoatVe.DataAccess;
using System.Collections;
using System.Text;

namespace PhanMemNVSoatVe.DataAccess
{
    public class MySqlXeVaoRepository : IXeVaoRepository
    {
        private readonly string _connStr = ConfigurationManager.ConnectionStrings["MyConnStr"].ConnectionString;

        public IEnumerable<XeVao> GetXeDangGui()
        {
            var list = new List<XeVao>();
            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT * FROM DuLieuXeVao WHERE TrangThaiVe='ChuaTra';", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(MapReader(reader));
                        }
                    }
                }
            }
            return list;
        }

        public XeVao GetBySoVe(string soVe)
        {
            XeVao result = null;
            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT * FROM DuLieuXeVao WHERE SoVe=@soVe LIMIT 1;", conn))
                {
                    cmd.Parameters.AddWithValue("@soVe", soVe);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = MapReader(reader);
                        }
                    }
                }
            }
            return result;
        }

        public XeVao GetByBienSoChuaTra(string bienSo)
        {
            XeVao result = null;
            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT * FROM DuLieuXeVao WHERE BienSoXe=@bienSo AND TrangThaiVe='ChuaTra' LIMIT 1;", conn))
                {
                    cmd.Parameters.AddWithValue("@bienSo", bienSo);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = MapReader(reader);
                        }
                    }
                }
            }
            return result;
        }

        public int GetSoVeMoi()
        {
            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                using (var cmdMin = new MySqlCommand(
                    @"SELECT MIN(CAST(SoVe AS UNSIGNED))
                      FROM DuLieuXeVao
                      WHERE TrangThaiVe='DaTra'
                        AND SoVe NOT IN (
                            SELECT SoVe FROM DuLieuXeVao WHERE TrangThaiVe='ChuaTra'
                        );", conn))
                {
                    var resMin = cmdMin.ExecuteScalar();
                    if (resMin != DBNull.Value && resMin != null)
                    {
                        return Convert.ToInt32(resMin);
                    }
                }
                using (var cmdMax = new MySqlCommand("SELECT COALESCE(MAX(CAST(SoVe AS UNSIGNED)),0)+1 FROM DuLieuXeVao;", conn))
                {
                    return Convert.ToInt32(cmdMax.ExecuteScalar());
                }
            }
        }

        public bool CheckSoVeDangSuDung(string soVe)
        {
            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM DuLieuXeVao WHERE SoVe=@soVe AND TrangThaiVe='ChuaTra';", conn))
                {
                    cmd.Parameters.AddWithValue("@soVe", soVe);
                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public bool Insert(XeVao xe)
        {
            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(
                    @"INSERT INTO DuLieuXeVao (BienSoXe, LoaiVe, SoVe, ThoiGianVao, GiaHan)
                       VALUES (@bienSo, @loaiVe, @soVe, @thoiGianVao, @giaHan);", conn))
                {
                    cmd.Parameters.AddWithValue("@bienSo", xe.BienSoXe);
                    cmd.Parameters.AddWithValue("@loaiVe", xe.LoaiVe);
                    cmd.Parameters.AddWithValue("@soVe", xe.SoVe);
                    cmd.Parameters.AddWithValue("@thoiGianVao", xe.ThoiGianVao);
                    cmd.Parameters.AddWithValue("@giaHan", xe.GiaHan);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool CapNhatRaVe(string soVe, DateTime thoiGianRa, double tienPhat)
        {
            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(
                    @"UPDATE DuLieuXeVao
                       SET TrangThaiVe='DaTra', ThoiGianRa=@thoiGianRa, TienPhat=@tienPhat
                       WHERE SoVe=@soVe AND TrangThaiVe='ChuaTra';", conn))
                {
                    cmd.Parameters.AddWithValue("@soVe", soVe);
                    cmd.Parameters.AddWithValue("@thoiGianRa", thoiGianRa);
                    cmd.Parameters.AddWithValue("@tienPhat", tienPhat);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        private XeVao MapReader(MySqlDataReader reader)
        {
            int ordinalRa = reader.GetOrdinal("ThoiGianRa");
            var xe = new XeVao
            {
                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                BienSoXe = reader.GetString(reader.GetOrdinal("BienSoXe")),
                LoaiVe = reader.GetString(reader.GetOrdinal("LoaiVe")),
                SoVe = reader.GetString(reader.GetOrdinal("SoVe")),
                ThoiGianVao = reader.GetDateTime(reader.GetOrdinal("ThoiGianVao")),
                GiaHan = reader.GetBoolean(reader.GetOrdinal("GiaHan")),
                TrangThaiVe = reader.GetString(reader.GetOrdinal("TrangThaiVe")),
                ThoiGianRa = reader.IsDBNull(ordinalRa) ? (DateTime?)null : reader.GetDateTime(ordinalRa),
                TienPhat = reader.GetDouble(reader.GetOrdinal("TienPhat"))
            };
            return xe;
        }

        public IEnumerable<XeVao> Search(string bienSo, string soVe, DateTime? from, DateTime? to)
        {
            var list = new List<XeVao>();
            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                var sql = new StringBuilder("SELECT * FROM DuLieuXeVao WHERE 1=1");

                if (!string.IsNullOrWhiteSpace(bienSo))
                    sql.Append(" AND BienSoXe LIKE @bienSo");
                if (!string.IsNullOrWhiteSpace(soVe))
                    sql.Append(" AND SoVe = @soVe");
                if (from.HasValue)
                    sql.Append(" AND ThoiGianVao >= @from");
                if (to.HasValue)
                    sql.Append(" AND ThoiGianVao < @to");

                using (var cmd = new MySqlCommand(sql.ToString(), conn))
                {
                    if (!string.IsNullOrWhiteSpace(bienSo))
                        cmd.Parameters.AddWithValue("@bienSo", $"%{bienSo}%");
                    if (!string.IsNullOrWhiteSpace(soVe))
                        cmd.Parameters.AddWithValue("@soVe", soVe);
                    if (from.HasValue)
                        cmd.Parameters.AddWithValue("@from", from.Value);
                    if (to.HasValue)
                        cmd.Parameters.AddWithValue("@to", to.Value);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var xeVao = new XeVao
                            {
                                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                                BienSoXe = reader.GetString(reader.GetOrdinal("BienSoXe")),
                                LoaiVe = reader.GetString(reader.GetOrdinal("LoaiVe")),
                                SoVe = reader.GetString(reader.GetOrdinal("SoVe")),
                                ThoiGianVao = reader.GetDateTime(reader.GetOrdinal("ThoiGianVao")),
                                GiaHan = reader.GetBoolean(reader.GetOrdinal("GiaHan")),
                                TrangThaiVe = reader.GetString(reader.GetOrdinal("TrangThaiVe")),
                                ThoiGianRa = reader.IsDBNull(reader.GetOrdinal("ThoiGianRa")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ThoiGianRa")),
                                TienPhat = reader.GetDouble(reader.GetOrdinal("TienPhat"))
                            };

                            list.Add(xeVao);
                        }
                    }
                }
            }

            return list;
        }

        public XeVao GetXeVaoByBienSoLoaiVeVaNgay(string bienSo, string loaiVe, DateTime ngayGui)
        {
            using (var connection = new MySqlConnection(_connStr))
            {
                connection.Open();
                var query = @"SELECT * FROM XeVao 
                      WHERE BienSoXe = @BienSoXe 
                        AND LoaiVe = @LoaiVe 
                        AND DATE(ThoiGianVao) = @NgayGui
                        AND TrangThaiVe = 'ChuaTra'
                      LIMIT 1";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BienSoXe", bienSo);
                    command.Parameters.AddWithValue("@LoaiVe", loaiVe);
                    command.Parameters.AddWithValue("@NgayGui", ngayGui.Date);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new XeVao
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                BienSoXe = reader["BienSoXe"].ToString(),
                                LoaiVe = reader["LoaiVe"].ToString(),
                                SoVe = reader["SoVe"].ToString(),
                                ThoiGianVao = Convert.ToDateTime(reader["ThoiGianVao"]),
                                GiaHan = Convert.ToBoolean(reader["GiaHan"]),
                                TrangThaiVe = reader["TrangThaiVe"].ToString(),
                                ThoiGianRa = reader["ThoiGianRa"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["ThoiGianRa"]),
                                TienPhat = Convert.ToDouble(reader["TienPhat"])
                            };
                        }
                    }
                }
            }
            return null;
        }
        public bool KiemTraXeTonTai(string bienSo, DateTime ngayGui, string loaiVe)
        {
            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    @"SELECT COUNT(*) FROM XeVao 
              WHERE BienSoXe = @bienSo 
                AND DATE(ThoiGianVao) = @ngayGui 
                AND LoaiVe = @loaiVe
                AND TrangThaiVe = 'ChuaTra'", conn);

                cmd.Parameters.AddWithValue("@bienSo", bienSo);
                cmd.Parameters.AddWithValue("@ngayGui", ngayGui.Date);
                cmd.Parameters.AddWithValue("@loaiVe", loaiVe);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }
    }
}