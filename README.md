# README

## Mô tả dự án

Mô tả ngắn gọn về phần mềm quản lý vé và module quản lý nhân viên.

## Mục lục

1. [Giới thiệu chung](#giới-thiệu-chung)
2. [Yêu cầu môi trường](#yêu-cầu-môi-trường)
3. [Cấu trúc dự án](#cấu-trúc-dự-án)
4. [Hướng dẫn cài đặt](#hướng-dẫn-cài-đặt)
5. [Hướng dẫn sử dụng](#hướng-dẫn-sử-dụng)

   * Hiển thị danh sách nhân viên
   * Thêm nhân viên
   * Sửa thông tin nhân viên
   * Xóa nhân viên
   * Tìm kiếm nâng cao
6. [Kiến trúc MVP](#kiến-trúc-mvp)

   * View
   * Presenter
   * Model
7. [Chi tiết các lớp chính](#chi-tiết-các-lớp-chính)

   * Repository (DAL)
   * Model NhanVien
   * Presenter NhanVienPresenter
   * View frmPhanMemDanhChoNVQuanTriVien
8. [Validation và Hashing](#validation-và-hashing)
9. [Xử lý bất đồng bộ và UI phản hồi](#xử-lý-bất-đồng-bộ-và-ui-phản-hồi)
10. [Handling Exceptions và Logging](#handling-exceptions-và-logging)
11. [Unit Tests](#unit-tests)
12. [Mở rộng và cải tiến](#mở-rộng-và-cải-tiến)
13. [Tài liệu tham khảo](#tài-liệu-tham-khảo)

---

### 7. Chi tiết các lớp chính

#### 7.1 DataAccess

Thư mục `DataAccess` chứa các interface và lớp cài đặt kết nối CSDL cho các thực thể chính.

* **IDuLieuXeVaoRepository.cs**
  Giao diện CRUD cho bảng `DuLieuXeVao`:

  ```csharp
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
  ```

* **IKhachHangRepository.cs**
  Giao diện nội bộ (internal) xử lý các nghiệp vụ khách hàng chưa trả xe, gia hạn, và lưu sự cố:

  ```csharp
  using System;
  using System.Data;

  namespace PhanMemNVSoatVe.DataAccess
  {
      internal interface IKhachHangRepository
      {
          DataTable GetChuaTra();
          void GiaHan(int id);
          void HuyGiaHan(int id);
          int LuuSuCo(
              string ten, DateTime ngaySinh, string gioiTinh,
              string cccd, string sdt, string loaiXe, string bienSo,
              DateTime ngayGui, DateTime ngayNhan,
              string moTa, string yeuCau);
          bool KiemTraXeTonTai(string bienSo, DateTime ngayGui, string loaiVe);
      }
  }
  ```

* **INhanVienRepository.cs**
  CRUD và truy xuất dữ liệu kiểu `DataTable`/`DataRow` cho nhân viên:

  ```csharp
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
  ```

* **IXeVaoRepository.cs**
  Repository cho nghiệp vụ gửi/rời xe và tìm kiếm:

  ```csharp
  using System;
  using System.Collections.Generic;
  using PhanMemNVSoatVe.Models;

  namespace PhanMemNVSoatVe.DataAccess
  {
      public interface IXeVaoRepository
      {
          IEnumerable<XeVao> GetXeDangGui();
          XeVao GetBySoVe(string soVe);
          XeVao GetByBienSoChuaTra(string bienSo);
          int GetSoVeMoi();
          bool CheckSoVeDangSuDung(string soVe);
          bool Insert(XeVao xe);
          bool CapNhatRaVe(string soVe, DateTime thoiGianRa, double tienPhat);
          IEnumerable<XeVao> Search(string bienSo, string soVe, DateTime? from, DateTime? to);
          XeVao GetXeVaoByBienSoLoaiVeVaNgay(string bienSo, string loaiVe, DateTime ngayGui);
      }
  }
  ```

* **MySqlDuLieuXeVaoRepository.cs**
  Cài đặt `IDuLieuXeVaoRepository` sử dụng MySQL Connector/NET:

  ```csharp
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

          // Các phương thức GetById, Insert, Update, Delete tương tự, sử dụng parameterized queries

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
  ```

* **MySqlKhachHangRepository.cs**
  Triển khai `IKhachHangRepository` để xử lý nghiệp vụ khách hàng:

  ```csharp
  using System;
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

          public void GiaHan(int id) { /* ... */ }
          public void HuyGiaHan(int id) { /* ... */ }
          public int LuuSuCo(string ten, DateTime ngaySinh, string gioiTinh, string cccd, string sdt, string loaiXe, string bienSo, DateTime ngayGui, DateTime ngayNhan, string moTa, string yeuCau) { /* ... */ }
          public bool KiemTraXeTonTai(string bienSo, DateTime thoiGianVao, string loaiVe) { /* ... */ }
      }
  }
  ```

* **MySqlNhanVienRepository.cs**
  Cài đặt `INhanVienRepository` tương tác với bảng `dulieunhanvien`:

  ```csharp
  using MySql.Data.MySqlClient;
  using PhanMemNVSoatVe.Models;
  using System.Data;

  namespace PhanMemNVSoatVe.DataAccess
  {
      public class MySqlNhanVienRepository : INhanVienRepository
      {
          private readonly string _connectionString;
          public MySqlNhanVienRepository(string connectionString)
          {
              _connectionString = connectionString;
          }

          public DataTable GetAll() { /* ... */ }
          public DataRow GetById(string id) { /* ... */ }
          public bool Insert(NhanVien nv) { /* ... */ }
          public bool Update(NhanVien nv) { /* ... */ }
          public bool Delete(string id) { /* ... */ }
      }
  }
  ```

* **MySqlXeVaoRepository.cs**
  Triển khai `IXeVaoRepository` với các phương thức Get, Insert, Update, Search:

  ```csharp
  using System;
  using System.Collections.Generic;
  using System.Configuration;
  using MySql.Data.MySqlClient;
  using PhanMemNVSoatVe.Models;
  using System.Text;

  namespace PhanMemNVSoatVe.DataAccess
  {
      public class MySqlXeVaoRepository : IXeVaoRepository
      {
          private readonly string _connStr = ConfigurationManager.ConnectionStrings["MyConnStr"].ConnectionString;

          public IEnumerable<XeVao> GetXeDangGui() { /* ... */ }
          public XeVao GetBySoVe(string soVe) { /* ... */ }
          public XeVao GetByBienSoChuaTra(string bienSo) { /* ... */ }
          public int GetSoVeMoi() { /* ... */ }
          public bool CheckSoVeDangSuDung(string soVe) { /* ... */ }
          public bool Insert(XeVao xe) { /* ... */ }
          public bool CapNhatRaVe(string soVe, DateTime thoiGianRa, double tienPhat) { /* ... */ }
          public IEnumerable<XeVao> Search(string bienSo, string soVe, DateTime? from, DateTime? to) { /* ... */ }
          public XeVao GetXeVaoByBienSoLoaiVeVaNgay(string bienSo, string loaiVe, DateTime ngayGui) { /* ... */ }
          public bool KiemTraXeTonTai(string bienSo, DateTime ngayGui, string loaiVe) { /* ... */ }

          private XeVao MapReader(MySqlDataReader reader) { /* ... */ }
      }
  }
  ```

#### 7.2 Models

Thư mục `Models` chứa các lớp định nghĩa thực thể ánh xạ dữ liệu CSDL.

* **NhanVien.cs**

  ```csharp
  using System;

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
  ```

* **SuCo.cs**

  ```csharp
  using System;

  namespace PhanMemNVSoatVe.Models
  {
      public class SuCo
      {
          public string TenKhachHang { get; set; }
          public DateTime NgaySinh { get; set; }
          public string GioiTinh { get; set; }
          public string CCCD { get; set; }
          public string SDT { get; set; }
          public string LoaiXe { get; set; }
          public string BienSoXe { get; set; }
          public DateTime NgayGui { get; set; }
          public DateTime NgayNhan { get; set; }
          public string MoTaSuCo { get; set; }
          public string YeuCauKhachHang { get; set; }
      }
  }
  ```

* **XeVao.cs**

  ```csharp
  using System;

  namespace PhanMemNVSoatVe.Models
  {
      public class XeVao
      {
          public int ID { get; set; }
          public string BienSoXe { get; set; } = string.Empty;
          public string LoaiVe { get; set; } = string.Empty;
          public string SoVe { get; set; } = string.Empty;
          public DateTime ThoiGianVao { get; set; }
          public bool GiaHan { get; set; }
          public string TrangThaiVe { get; set; } = "ChuaTra";
          public DateTime? ThoiGianRa { get; set; }
          public double TienPhat { get; set; } = 0;
      }
  }
  ```

### 7.3 Presenters

Thư mục `Presenters` chứa các lớp xử lý logic giữa View và Repository theo mô hình MVP.

NhanVienPresenter.cs

  ```csharp
public class NhanVienPresenter
{
    private readonly INhanVienView _view;
    private readonly INhanVienRepository _repository;

    public NhanVienPresenter(INhanVienView view, INhanVienRepository repository)
    {
        _view = view;
        _repository = repository;
    }

    public void LoadAll() { /* Load tất cả nhân viên và gọi _view.SetNhanVienList(...) */ }
    public void Insert() { /* Tạo NhanVien từ View và gọi _repository.Insert(...) */ }
    public void Update() { /* Tạo NhanVien từ View và gọi _repository.Update(...) */ }
    public void Delete() { /* Xóa nhân viên dựa trên _view.IDNhanVien */ }

    private NhanVien CreateNhanVienFromView() { /* Map dữ liệu từ _view vào model */ }
}
```

```csharp
QuanLyKhachHangPresenter.cs

public class QuanLyKhachHangPresenter
{
    private readonly IQuanLyKhachHangView _view;
    private readonly IKhachHangRepository _repo;
    private DataTable _dt;

    public QuanLyKhachHangPresenter(IQuanLyKhachHangView view)
    {
        _view = view;
        _repo = new MySqlKhachHangRepository();
        // Đăng ký event và Initialize
    }

    private void LoadTatCaDuLieu() { /* GetChuaTra và gán vào _view.DataSource */ }
    private void View_GiaHanClicked(...) { /* Gọi _repo.GiaHan và reload */ }
    private void View_HuyGiaHanClicked(...) { /* Gọi _repo.HuyGiaHan và reload */ }
    private void View_TimKiemClicked(...) { /* Build filter trên _dt.DefaultView.RowFilter */ }
    private void OnLuuSuCo(...) { /* Kiểm tra, gọi _repo.LuuSuCo và ShowMessage */ }
}
```

QuanLyThongTinPresenter.cs

```csharp
public class QuanLyThongTinPresenter
{
    private readonly IQuanLyThongTinView _view;
    private readonly IDuLieuXeVaoRepository _repo;
    private BindingList<XeVao> _allData;

    public QuanLyThongTinPresenter(IQuanLyThongTinView view, IDuLieuXeVaoRepository repo)
    {
        _view = view;
        _repo = repo;
        // Đăng ký event và LoadAll
    }

    private void LoadAll() { /* Gọi _repo.GetAll và bind _view.GridData */ }
    private void ApplyFilter() { /* Áp dụng filter dựa trên các property của view */ }
    private void AddRecord() { /* Tạo XeVao từ view và _repo.Insert */ }
    private void UpdateRecord() { /* Lấy XeVao từ _repo và update */ }
    private void DeleteRecord() { /* Xóa theo EditID */ }
}
```

QuanLyXePresenter.cs

public class QuanLyXePresenter
{
    private readonly IQuanLyXeView _view;
    private readonly IXeVaoRepository _repo;
    private static readonly Regex _regexBienSo = /* regex kiểm tra biển số */;

    public QuanLyXePresenter(IQuanLyXeView view, IXeVaoRepository repo)
    {
        _view = view;
        _repo = repo;
        // Đăng ký sự kiện
    }

    private void OnMoBarrierVao(...) { /* Kiểm tra regex, Insert vé và ToggleBarrier */ }
    private void OnSoVeRaChanged(...) { /* Lấy thông tin xe và DisplayXeInfo */ }
    private void OnTraXe(...) { /* Tính phí, CapNhatRaVe và ShowInfo */ }
}

### 7.4 Views

Thư mục Views định nghĩa các interface cho UI layer, tương tác với Presenter.

INhanVienView.cs

using System;
using System.Data;

namespace PhanMemNVSoatVe.Views
{
    public interface INhanVienView
    {
        void SetNhanVienList(DataTable dt);
        void ShowMessage(string message);
        string IDNhanVien { get; }
        string TenNhanVien { get; }
        string GioiTinh { get; }
        DateTime NgaySinh { get; }
        string Email { get; }
        DateTime NgayVaoLam { get; }
        string SDT { get; }
        string DiaChi { get; }
        string ChucVu { get; }
        decimal MucLuong { get; }
        string MatKhau { get; }
        string TrangThai { get; }

        void ClearForm();
    }
}

IQuanLyKhachHangView.cs

using System;
using System.Data;

namespace PhanMemNVSoatVe.Views
{
    public interface IQuanLyKhachHangView
    {
        DataTable DataSource { set; }

        // Các sự kiện do View phát ra, Presenter lắng nghe
        event EventHandler LoadData;
        event EventHandler GiaHanClicked;
        event EventHandler HuyGiaHanClicked;
        event EventHandler TimKiemClicked;
        event EventHandler NhapLaiClicked;
        event EventHandler LuuSuCoClicked;

        // Lấy thông tin input từ View, Presenter dùng để xử lý
        string TimKiemBienSo { get; }
        string TimKiemLoaiVe { get; }
        string TimKiemSoVe { get; }
        string TimKiemThoiGianVao { get; }

        int SelectedID { get; }

        // Thông tin sự cố (cho btnLuuSuCo)
        string SuCoTenKhachHang { get; }
        DateTime SuCoNgaySinh { get; }
        string SuCoGioiTinh { get; }
        string SuCoCCCD { get; }
        string SuCoSoDienThoai { get; }
        string SuCoLoaiXe { get; }
        string SuCoBienSo { get; }
        DateTime SuCoNgayGui { get; }
        DateTime SuCoNgayNhan { get; }
        string SuCoMoTa { get; }
        string SuCoYeuCauKhachHang { get; }

        // Hiển thị thông báo
        void ShowMessage(string message);
    }
}

IQuanLyThongTinView.cs

using System;
using System.ComponentModel;
using PhanMemNVSoatVe.Models;

namespace PhanMemNVSoatVe.Views
{
    public interface IQuanLyThongTinView
    {
        // Input/filter từ UI
        string FilterBienSo { get; }
        string FilterLoaiVe { get; }
        string FilterSoVe { get; }
        DateTime? FilterVaoDate { get; }
        DateTime? FilterRaDate { get; }
        bool FilterDaTra { get; }
        bool FilterChuaTra { get; }

        // Dữ liệu hiển thị
        BindingList<XeVao> GridData { set; }

        // Thêm mới
        string NewBienSo { get; }
        string NewLoaiVe { get; }
        string NewSoVe { get; }
        DateTime NewThoiGianVao { get; }
        string NewTrangThaiVe { get; }
        DateTime? NewThoiGianRa { get; }
        double NewTienPhat { get; }

        // Sửa
        string EditID { get; }
        void ShowEditSection(XeVao xe);

        // Message
        void ShowError(string msg);
        void ShowInfo(string msg);

        // Events
        event EventHandler LoadData;
        event EventHandler FilterChanged;
        event EventHandler ResetFilterClicked;
        event EventHandler AddClicked;
        event EventHandler UpdateClicked;
        event EventHandler DeleteClicked;
        event EventHandler ResetNewClicked;
        event EventHandler EditIDChanged;
    }
}

IQuanLyXeView.cs

using PhanMemNVSoatVe.Models;
using System;

namespace PhanMemNVSoatVe.Views
{
    public interface IQuanLyXeView
    {
        // Dữ liệu từ UI
        string BienSo { get; }
        string LoaiVe { get; }
        string SoVe { get; }

        // Hiển thị lên UI
        void ShowError(string message);
        void ShowInfo(string message);
        void DisplayXeInfo(XeVao xe);
        void ToggleBarrierVao(bool isOpen);
        void ToggleBarrierRa(bool isOpen);

        // Sự kiện do View phát ra
        event EventHandler MoBarrierVaoClicked;
        event EventHandler DongBarrierVaoClicked;
        event EventHandler MoBarrierRaClicked;
        event EventHandler DongBarrierRaClicked;
        event EventHandler SoVeRaTextChanged;
        event EventHandler TimerTick;
    }
}


