# Hướng dẫn Cài đặt Nhanh - Ứng dụng Quản lý Sinh viên

## Bước 1: Chuẩn bị Database
1. Mở **SQL Server Management Studio** (SSMS)
2. Kết nối đến SQL Server instance của bạn
3. Mở file `DatabaseScript.sql` và chạy toàn bộ script
4. Kiểm tra database `StudentManagement` đã được tạo với 3 bảng và dữ liệu mẫu

## Bước 2: Cấu hình Connection String (nếu cần)
Mở file `DatabaseHelper.vb` và cập nhật connection string nếu cần:
```vb
Private Shared ReadOnly ConnectionString As String = "Server=YOUR_SERVER;Database=StudentManagement;Integrated Security=true;TrustServerCertificate=true;"
```

**Các trường hợp thường gặp:**
- **SQL Server Express**: `Server=.\SQLEXPRESS;Database=StudentManagement;Integrated Security=true;TrustServerCertificate=true;`
- **SQL Server với username/password**: `Server=localhost;Database=StudentManagement;User Id=sa;Password=yourpassword;TrustServerCertificate=true;`
- **SQL Server trên máy khác**: `Server=192.168.1.100;Database=StudentManagement;Integrated Security=true;TrustServerCertificate=true;`

## Bước 3: Mở và Chạy Project
### Cách 1: Sử dụng Visual Studio
1. Mở file `StudentManagement.sln` trong Visual Studio 2022
2. Nhấn **F5** hoặc **Ctrl+F5** để chạy

### Cách 2: Sử dụng Command Line
```bash
# Restore packages
dotnet restore

# Build project
dotnet build

# Chạy ứng dụng
dotnet run
```

## Bước 4: Test Chức năng
1. **Kiểm tra hiển thị dữ liệu**: Ứng dụng sẽ hiển thị 5 sinh viên mẫu
2. **Test tìm kiếm**: Nhập "Nguyễn" vào ô tìm kiếm
3. **Test lọc**: Chọn "CNTT01" từ dropdown Lớp
4. **Test thêm mới**: Nhấn "Thêm mới" và điền thông tin
5. **Test sửa**: Chọn một sinh viên và nhấn "Sửa"
6. **Test xóa**: Chọn một sinh viên và nhấn "Xóa"

## Troubleshooting

### Lỗi: "Cannot connect to database"
- Kiểm tra SQL Server đang chạy
- Kiểm tra connection string
- Thử kết nối bằng SSMS trước

### Lỗi: "Database does not exist"
- Chạy lại file `DatabaseScript.sql`
- Kiểm tra tên database trong connection string

### Lỗi: "Package restore failed"
```bash
dotnet clean
dotnet restore
dotnet build
```

### Lỗi: "Build failed"
- Đảm bảo .NET 8.0 SDK đã cài đặt
- Kiểm tra Visual Studio 2022 đã cài đặt

## Cấu trúc File Project
```
StudentManagement/
├── StudentManagement.sln          # Solution file
├── StudentManagement.vbproj       # Project file
├── Program.vb                     # Entry point
├── DatabaseHelper.vb              # Database operations
├── MainForm.vb                    # Main form
├── StudentForm.vb                 # Add/Edit form
├── DatabaseScript.sql             # Database schema
├── README.md                      # Full documentation
└── INSTALLATION.md                # This file
```

## Yêu cầu Hệ thống
- **OS**: Windows 10/11
- **.NET**: .NET 8.0 SDK
- **IDE**: Visual Studio 2022 (khuyến nghị)
- **Database**: SQL Server (LocalDB, Express, hoặc Full)

## Liên hệ Hỗ trợ
Nếu gặp vấn đề, hãy kiểm tra:
1. File `README.md` để biết thêm chi tiết
2. Log lỗi trong Visual Studio Output window
3. Kiểm tra SQL Server logs

