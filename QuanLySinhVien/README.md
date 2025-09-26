# Ứng dụng Quản lý Sinh viên - Windows Forms VB.NET 8.0

## Mô tả
Ứng dụng quản lý sinh viên được phát triển bằng Visual Basic .NET 8.0 và Windows Forms, sử dụng SQL Server làm cơ sở dữ liệu.

## Tính năng chính
- ✅ Thêm sinh viên mới
- ✅ Sửa thông tin sinh viên
- ✅ Xóa sinh viên
- ✅ Tìm kiếm sinh viên theo nhiều tiêu chí
- ✅ Lọc theo lớp và khoa
- ✅ Hiển thị danh sách sinh viên với thông tin đầy đủ

## Cấu trúc Database
Database bao gồm 3 bảng chính với các ràng buộc:

### 1. Bảng Departments (Khoa)
- `DepartmentID` (PK, Identity)
- `DepartmentName` (Unique, Not Null)
- `Description`
- `CreatedDate`

### 2. Bảng Classes (Lớp)
- `ClassID` (PK, Identity)
- `ClassName` (Not Null)
- `DepartmentID` (FK → Departments)
- `AcademicYear` (Not Null)
- `CreatedDate`

### 3. Bảng Students (Sinh viên)
- `StudentID` (PK, Identity)
- `StudentCode` (Unique, Not Null)
- `FullName` (Not Null)
- `DateOfBirth` (Not Null, Check Age 16-50)
- `Gender` (Check: 'Nam' hoặc 'Nữ')
- `Email` (Unique)
- `Phone`
- `Address`
- `ClassID` (FK → Classes)
- `EnrollmentDate`
- `Status` (Check: 'Đang học', 'Tốt nghiệp', 'Nghỉ học')
- `CreatedDate`

## Ràng buộc Database
1. **Foreign Key Constraints**: Classes → Departments, Students → Classes
2. **Check Constraints**: 
   - Gender chỉ nhận 'Nam' hoặc 'Nữ'
   - Status chỉ nhận 'Đang học', 'Tốt nghiệp', 'Nghỉ học'
   - Tuổi sinh viên từ 16-50
3. **Unique Constraints**: StudentCode, Email, DepartmentName
4. **Cascade Delete**: Xóa khoa → xóa lớp → xóa sinh viên

## Cài đặt và Chạy

### Yêu cầu hệ thống
- Windows 10/11
- Visual Studio 2022 hoặc .NET 8.0 SDK
- SQL Server (LocalDB, Express, hoặc Full)

### Bước 1: Tạo Database
1. Mở SQL Server Management Studio
2. Chạy file `DatabaseScript.sql` để tạo database và dữ liệu mẫu

### Bước 2: Cấu hình Connection String
Mở file `DatabaseHelper.vb` và cập nhật connection string nếu cần:
```vb
Private Shared ReadOnly ConnectionString As String = "Server=localhost;Database=StudentManagement;Integrated Security=true;TrustServerCertificate=true;"
```

### Bước 3: Build và Chạy
1. Mở project trong Visual Studio
2. Restore NuGet packages (Microsoft.Data.SqlClient)
3. Build solution (Ctrl+Shift+B)
4. Chạy ứng dụng (F5)

## Hướng dẫn sử dụng

### Giao diện chính
- **Tìm kiếm**: Nhập từ khóa vào ô tìm kiếm và nhấn "Tìm kiếm" hoặc Enter
- **Lọc**: Chọn lớp hoặc khoa từ dropdown để lọc danh sách
- **Làm mới**: Nhấn "Làm mới" để reset tất cả bộ lọc

### Quản lý sinh viên
- **Thêm mới**: Nhấn "Thêm mới" → Điền thông tin → "Lưu"
- **Sửa**: Chọn sinh viên → Nhấn "Sửa" → Chỉnh sửa → "Lưu"
- **Xóa**: Chọn sinh viên → Nhấn "Xóa" → Xác nhận

### Validation
- Mã sinh viên và họ tên là bắt buộc
- Email phải đúng định dạng (nếu có)
- Số điện thoại từ 10-15 ký tự (nếu có)
- Giới tính, lớp, trạng thái phải được chọn

## Cấu trúc Project
```
StudentManagement/
├── StudentManagement.vbproj    # Project file
├── Program.vb                  # Entry point
├── DatabaseHelper.vb           # Database operations
├── MainForm.vb                 # Main form
├── StudentForm.vb              # Add/Edit form
├── DatabaseScript.sql          # Database schema
└── README.md                   # Documentation
```

## Stored Procedures
- `sp_SearchStudents`: Tìm kiếm sinh viên với nhiều tiêu chí
- `sp_InsertStudent`: Thêm sinh viên mới
- `sp_UpdateStudent`: Cập nhật thông tin sinh viên
- `sp_DeleteStudent`: Xóa sinh viên

## Troubleshooting

### Lỗi kết nối Database
- Kiểm tra SQL Server đang chạy
- Kiểm tra connection string
- Đảm bảo database đã được tạo

### Lỗi Build
- Đảm bảo .NET 8.0 SDK đã cài đặt
- Clean và Rebuild solution

## Phiên bản
- Version: 1.0.0
- .NET Framework: .NET 8.0
- Database: SQL Server
- UI Framework: Windows Forms

