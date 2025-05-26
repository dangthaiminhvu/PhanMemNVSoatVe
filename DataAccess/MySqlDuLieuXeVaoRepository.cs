using MySql.Data.MySqlClient;
using PhanMemNVSoatVe.Models;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace PhanMemNVSoatVe.DataAccess
{
    public class MySqlDuLieuXeVaoRepository : IDuLieuXeVaoRepository
    {
        private readonly string _connStr = ConfigurationManager.ConnectionStrings["MyConnStr"].ConnectionString;

        public IEnumerable<XeVao> GetAll()
        {
            var list = new List<XeVao>();
            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT * FROM DuLieuXeVao", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(Map(reader));
                }
            }
            return list;
        }

        public XeVao GetById(int id)
        {
            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT * FROM DuLieuXeVao WHERE ID=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        return reader.Read() ? Map(reader) : null;
                    }
                }
            }
        }

        public bool Insert(XeVao xe)
        {
            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                const string sql = @"INSERT INTO DuLieuXeVao (BienSoXe, LoaiVe, SoVe, ThoiGianVao, GiaHan, TrangThaiVe, ThoiGianRa, TienPhat)
                                    VALUES (@bs,@lv,@sv,@tgVao,@giaHan,@tt,@tgRa,@tp)";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@bs", xe.BienSoXe);
                    cmd.Parameters.AddWithValue("@lv", xe.LoaiVe);
                    cmd.Parameters.AddWithValue("@sv", xe.SoVe);
                    cmd.Parameters.AddWithValue("@tgVao", xe.ThoiGianVao);
                    cmd.Parameters.AddWithValue("@giaHan", xe.GiaHan);
                    cmd.Parameters.AddWithValue("@tt", xe.TrangThaiVe);
                    cmd.Parameters.AddWithValue("@tgRa", xe.ThoiGianRa ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@tp", xe.TienPhat);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Update(XeVao xe)
        {
            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                const string sql = @"UPDATE DuLieuXeVao SET BienSoXe=@bs, LoaiVe=@lv, SoVe=@sv,
                                    ThoiGianVao=@tgVao, GiaHan=@giaHan, TrangThaiVe=@tt,
                                    ThoiGianRa=@tgRa, TienPhat=@tp WHERE ID=@id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@bs", xe.BienSoXe);
                    cmd.Parameters.AddWithValue("@lv", xe.LoaiVe);
                    cmd.Parameters.AddWithValue("@sv", xe.SoVe);
                    cmd.Parameters.AddWithValue("@tgVao", xe.ThoiGianVao);
                    cmd.Parameters.AddWithValue("@giaHan", xe.GiaHan);
                    cmd.Parameters.AddWithValue("@tt", xe.TrangThaiVe);
                    cmd.Parameters.AddWithValue("@tgRa", xe.ThoiGianRa ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@tp", xe.TienPhat);
                    cmd.Parameters.AddWithValue("@id", xe.ID);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Delete(int id)
        {
            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("DELETE FROM DuLieuXeVao WHERE ID=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        private XeVao Map(MySqlDataReader reader)
        {
            int ordRa = reader.GetOrdinal("ThoiGianRa");
            return new XeVao
            {
                ID = reader.GetInt32("ID"),
                BienSoXe = reader.GetString("BienSoXe"),
                LoaiVe = reader.GetString("LoaiVe"),
                SoVe = reader.GetString("SoVe"),
                ThoiGianVao = reader.GetDateTime("ThoiGianVao"),
                GiaHan = reader.GetBoolean("GiaHan"),
                TrangThaiVe = reader.GetString("TrangThaiVe"),
                ThoiGianRa = reader.IsDBNull(ordRa) ? (DateTime?)null : reader.GetDateTime(ordRa),
                TienPhat = reader.GetDouble("TienPhat")
            };
        }
    }
}
