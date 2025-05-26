using PhanMemNVSoatVe.Models;
using System.Collections.Generic;

namespace PhanMemNVSoatVe.DataAccess
{
    public interface IDuLieuXeVaoRepository
    {
        IEnumerable<XeVao> GetAll();
        XeVao GetById(int id);
        bool Insert(XeVao xe);
        bool Update(XeVao xe);
        bool Delete(int id);
    }
}
