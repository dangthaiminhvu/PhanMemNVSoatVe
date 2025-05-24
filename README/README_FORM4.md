1. Giới thiệu chung Giới thiệu chung

frmPhanMemDanhChoNVQuanTriVien là form quản lý dữ liệu nhân viên trong phần mềm kiểm soát vé. Form cho phép:

Hiển thị danh sách nhân viên.

Thêm, chỉnh sửa, xóa nhân viên.

Tìm kiếm theo nhiều tiêu chí.

Xử lý mã hóa mật khẩu, validate dữ liệu.

2. Cấu trúc chính

Repository (DAL)

INhanVienRepository: interface CRUD.

MySqlNhanVienRepository: cài đặt kết nối MySQL, thực thi SQL bất đồng bộ.

Model

NhanVien: lớp mô hình ánh xạ cột CSDL.

UI Layer:

frmPhanMemDanhChoNVQuanTriVien.cs: code-behind WinForms.

3. Các tính năng chi tiết

3.1 Hiển thị dữ liệu

Tự động nạp toàn bộ nhân viên khi Form khởi tạo (LoadGridAsync).

DataGridView chỉ đọc, không cho thêm xóa/cộng cột, cho phép chọn nguyên dòng.

3.2 Thêm nhân viên

Nút: btnThemNhanVien

Quy trình:

Gọi ValidateInput() kiểm tra:

Các trường bắt buộc không để trống.

Độ dài chuỗi (ID ≤10, Tên ≤50, Email ≤100, Địa chỉ ≤200, SDT 9–15 chữ số).

Email đúng định dạng regex.

Ngày sinh và ngày vào làm định dạng dd/MM/yyyy.

Lương là số.

Nếu hợp lệ, BuildNhanVienFromForm() tạo đối tượng NhanVien.

Mã hóa SHA-256 mật khẩu nếu chưa là hash.

Gọi bất đồng bộ _repo.Insert(nv).

Hiển thị thông báo thành công/thất bại.

Nếu thành công, nạp lại lưới và reset toàn bộ ô nhập.

3.3 Sửa thông tin

Nút: btnLuuThayDoi

Quy trình (mẫu tương tự thêm):

kiểm tra ID không rỗng.

Chuyển mật khẩu: nếu đã ở dạng 64 ký tự hex thì giữ, ngược lại băm mới.

Tạo đối tượng từ các control sửa.

Gọi _repo.Update(nv) bất đồng bộ.

Reload lại chi tiết.

3.4 Xóa nhân viên

Nút: btnXoaNhanVien

Hiển thị hộp xác nhận.

Gọi _repo.Delete(id) bất đồng bộ.

Nếu xóa thành công, nạp lại lưới và clear các ô.

3.5 Tìm kiếm nâng cao

Nút: btnTimKiemNV

Phương thức BuildFilterString(): gom các điều kiện sau:

ID, Tên (LIKE %...%).

Giới tính (Nam/Nu).

Ngày sinh (so sánh ngày ≥ … < ngày kế tiếp).

Email, SDT, Địa chỉ (LIKE).

Chức vụ, Trạng thái (bằng).

Mức lương (bằng).

Mật khẩu (LIKE)

Áp dụng lên dt.DefaultView.RowFilter.

4. Xử lý ngoại lệ & bảo mật

SQL Injection: sử dụng parameterized query trong DAL.

Exception Handling:

Trong DAL, catch ngoại lệ liên quan CSDL, báo lỗi hoặc throw tiếp.

Trong UI, thông báo lỗi chung khi thao tác DB thất bại.

Mã hóa mật khẩu: SHA-256, kiểm tra chuỗi đã là hash hay chưa.

5. Lưu ý & cải tiến

Connection string: lưu trong App.config <connectionStrings>.

Async/await: dùng Task.Run cho file I/O database, tránh block UI.

Giảm duplication: logic validate, build model, reset form đã gom vào helper.

Mở rộng:

Thêm tính năng export CSV/Excel.

Phân quyền người dùng (admin, user thường).

Giao diện báo cáo thống kê.