using PhanMemNVSoatVe.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanMemNVSoatVe.DataAccess
{
    public interface INhanVienRepository
    {
        /// <summary>Trả về tất cả nhân viên dưới dạng DataTable</summary>
        DataTable GetAll();

        /// <summary>Trả về một dòng DataRow ứng với ID nhân viên, hoặc null nếu không tìm thấy</summary>
        DataRow GetById(string id);

        /// <summary>Thêm mới một nhân viên, trả về true nếu thành công</summary>
        bool Insert(NhanVien nv);

        /// <summary>Cập nhật thông tin nhân viên, trả về true nếu thành công</summary>
        bool Update(NhanVien nv);

        /// <summary>Xóa nhân viên theo ID, trả về true nếu thành công</summary>
        bool Delete(string id);
    }
}
