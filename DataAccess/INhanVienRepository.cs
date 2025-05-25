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
        DataTable GetAll();

        DataRow GetById(string id);

        bool Insert(NhanVien nv);

        bool Update(NhanVien nv);

        bool Delete(string id);
    }
}
