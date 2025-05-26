using PhanMemNVSoatVe.Models;
using System.Data;

namespace PhanMemNVSoatVe.DataAccess
{
    public interface INhanVienRepository
    {
        DataTable GetAll();

        DataRow GetById(string id);

        bool Insert(NhanVien nv);

        bool Update(NhanVien nv);

        bool Delete(string id);
    }
}
