using PhanMemNVSoatVe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
