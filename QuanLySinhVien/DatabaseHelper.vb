Imports Microsoft.Data.SqlClient
Imports System.Data
Imports System.Windows.Forms

' Lớp DatabaseHelper: quản lý các thao tác với cơ sở dữ liệu SQL Server
Public Class TroGiupCoSoDuLieu
    ' Chuỗi kết nối tới SQL Server
    Private Shared ReadOnly ChuoiKetNoi As String = "Data Source=localhost;Initial Catalog=StudentManagement;Integrated Security=True;TrustServerCertificate=True"

    ' Hàm TaoKetNoi: tạo và trả về một SqlConnection mới
    Public Shared Function TaoKetNoi() As SqlConnection
        Return New SqlConnection(ChuoiKetNoi)
    End Function

    ' Hàm KiemTraKetNoi: kiểm tra kết nối tới cơ sở dữ liệu
    Public Shared Function KiemTraKetNoi() As Boolean
        Try
            Using ketNoi As SqlConnection = TaoKetNoi()
                ketNoi.Open() ' Thử mở kết nối
                MessageBox.Show("Kết nối Database thành công", "Test Connection", MessageBoxButtons.OK)
                Return True
            End Using
        Catch ex As Exception
            MessageBox.Show("Lỗi kết nối database: " & ex.Message, "Test Connection", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    ' Hàm ChenDuLieuMau: thêm dữ liệu mẫu vào bảng Departments, Classes, Students
    Public Shared Function ChenDuLieuMau() As Boolean
        Try
            ' Thêm khoa nếu chưa có
            ThucThiLenh("IF NOT EXISTS (SELECT 1 FROM Departments) BEGIN INSERT INTO Departments (DepartmentName, Description) VALUES (N'Công nghệ thông tin', N'Khoa chuyên về lập trình và công nghệ'), (N'Kinh tế', N'Khoa chuyên về kinh tế và quản trị'), (N'Ngoại ngữ', N'Khoa chuyên về ngôn ngữ và văn hóa') END")

            ' Thêm lớp nếu chưa có
            ThucThiLenh("IF NOT EXISTS (SELECT 1 FROM Classes) BEGIN INSERT INTO Classes (ClassName, DepartmentID, AcademicYear) VALUES (N'CNTT01', 1, '2024-2025'), (N'CNTT02', 1, '2024-2025'), (N'KT01', 2, '2024-2025'), (N'NN01', 3, '2024-2025') END")

            ' Thêm sinh viên nếu chưa có
            ThucThiLenh("IF NOT EXISTS (SELECT 1 FROM Students) BEGIN INSERT INTO Students (StudentCode, FullName, DateOfBirth, Gender, Email, Phone, Address, ClassID) VALUES (N'SV001', N'Nguyễn Văn An', '2000-05-15', N'Nam', 'an.nguyen@email.com', '0123456789', N'Hà Nội', 1), (N'SV002', N'Trần Thị Bình', '2001-03-20', N'Nữ', 'binh.tran@email.com', '0987654321', N'TP.HCM', 1), (N'SV003', N'Lê Văn Cường', '2000-12-10', N'Nam', 'cuong.le@email.com', '0369258147', N'Đà Nẵng', 2), (N'SV004', N'Phạm Thị Dung', '2001-08-25', N'Nữ', 'dung.pham@email.com', '0741852963', N'Hải Phòng', 3), (N'SV005', N'Hoàng Văn Em', '2000-11-30', N'Nam', 'em.hoang@email.com', '0159753248', N'Cần Thơ', 4) END")

            MessageBox.Show("Đã thêm dữ liệu mẫu thành công!", "Insert Sample Data", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return True
        Catch ex As Exception
            MessageBox.Show("Lỗi khi thêm dữ liệu mẫu: " & ex.Message, "Insert Sample Data", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    ' Hàm ThucThiTruyVan: chạy câu lệnh SELECT và trả về DataTable
    Public Shared Function ThucThiTruyVan(cauLenh As String, Optional thamSo As List(Of SqlParameter) = Nothing) As DataTable
        Dim bang As New DataTable()
        Try
            Using ketNoi As SqlConnection = TaoKetNoi()
                ketNoi.Open()
                Using lenh As New SqlCommand(cauLenh, ketNoi)
                    If thamSo IsNot Nothing Then
                        lenh.Parameters.AddRange(thamSo.ToArray())
                    End If
                    Using boDem As New SqlDataAdapter(lenh)
                        boDem.Fill(bang)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Lỗi trong ThucThiTruyVan: " & ex.Message, "Debug", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return bang
    End Function

    ' Hàm ThucThiLenh: chạy INSERT/UPDATE/DELETE trả về số dòng ảnh hưởng
    Public Shared Function ThucThiLenh(cauLenh As String, Optional thamSo As List(Of SqlParameter) = Nothing) As Integer
        Using ketNoi As SqlConnection = TaoKetNoi()
            ketNoi.Open()
            Using lenh As New SqlCommand(cauLenh, ketNoi)
                If thamSo IsNot Nothing Then
                    lenh.Parameters.AddRange(thamSo.ToArray())
                End If
                Return lenh.ExecuteNonQuery()
            End Using
        End Using
    End Function

    ' Hàm ThucThiGiaTriDon: chạy câu lệnh trả về 1 giá trị duy nhất (Scalar)
    Public Shared Function ThucThiGiaTriDon(cauLenh As String, Optional thamSo As List(Of SqlParameter) = Nothing) As Object
        Using ketNoi As SqlConnection = TaoKetNoi()
            ketNoi.Open()
            Using lenh As New SqlCommand(cauLenh, ketNoi)
                If thamSo IsNot Nothing Then
                    lenh.Parameters.AddRange(thamSo.ToArray())
                End If
                Return lenh.ExecuteScalar()
            End Using
        End Using
    End Function

    ' Hàm LaySinhVienTheoID: lấy thông tin chi tiết 1 sinh viên
    Public Shared Function LaySinhVienTheoID(maSV As Integer) As DataTable
        Dim thamSo As New List(Of SqlParameter) From {
            New SqlParameter("@StudentID", maSV)
        }
        Return ThucThiTruyVan("SELECT s.StudentID, s.StudentCode, s.FullName, s.DateOfBirth, s.Gender, s.Email, s.Phone, s.Address, s.Status, s.ClassID FROM Students s WHERE s.StudentID = @StudentID", thamSo)
    End Function

    ' Hàm LayDanhSachSinhVien: trả về toàn bộ danh sách sinh viên, có filter theo search, class, department
    Public Shared Function LayDanhSachSinhVien(Optional tuKhoa As String = Nothing, Optional maLop As Integer? = Nothing, Optional maKhoa As Integer? = Nothing) As DataTable
        Dim sql As String =
        "SELECT s.StudentID, s.StudentCode, s.FullName, s.DateOfBirth, s.Gender, s.Email, s.Phone, s.Address, s.Status, s.EnrollmentDate, " &
        "       s.ClassID, c.ClassName, d.DepartmentName " &
        "FROM Students s " &
        "INNER JOIN Classes c ON s.ClassID = c.ClassID " &
        "INNER JOIN Departments d ON c.DepartmentID = d.DepartmentID " &
        "WHERE (COALESCE(@SearchTerm,'') = '' " &
        "       OR s.StudentCode LIKE '%' + @SearchTerm + '%' " &
        "       OR s.FullName  LIKE '%' + @SearchTerm + '%' " &
        "       OR s.Email     LIKE '%' + @SearchTerm + '%') " &
        "  AND (COALESCE(@ClassID,0) = 0 OR s.ClassID = @ClassID) " &
        "  AND (COALESCE(@DepartmentID,0) = 0 OR d.DepartmentID = @DepartmentID) " &
        "ORDER BY s.StudentCode"

        Dim thamSo As New List(Of SqlParameter) From {
            New SqlParameter("@SearchTerm", If(String.IsNullOrWhiteSpace(tuKhoa), "", tuKhoa)),
            New SqlParameter("@ClassID", If(maLop.HasValue, maLop.Value, 0)),
            New SqlParameter("@DepartmentID", If(maKhoa.HasValue, maKhoa.Value, 0))
        }

        Return ThucThiTruyVan(sql, thamSo)
    End Function

    ' Hàm LayDanhSachLop: trả về toàn bộ danh sách lớp
    Public Shared Function LayDanhSachLop() As DataTable
        Return ThucThiTruyVan("SELECT ClassID, ClassName, DepartmentID, AcademicYear FROM Classes ORDER BY ClassName")
    End Function

    ' Hàm LayDanhSachKhoa: trả về toàn bộ danh sách khoa
    Public Shared Function LayDanhSachKhoa() As DataTable
        Return ThucThiTruyVan("SELECT DepartmentID, DepartmentName FROM Departments ORDER BY DepartmentName")
    End Function

    ' Hàm ThemSinhVien: thêm mới 1 sinh viên
    Public Shared Function ThemSinhVien(maSV As String, hoTen As String, ngaySinh As Date,
                                       gioiTinh As String, email As String, sdt As String,
                                       diaChi As String, maLop As Integer) As Integer
        Dim thamSo As New List(Of SqlParameter) From {
            New SqlParameter("@StudentCode", maSV),
            New SqlParameter("@FullName", hoTen),
            New SqlParameter("@DateOfBirth", ngaySinh),
            New SqlParameter("@Gender", gioiTinh),
            New SqlParameter("@Email", email),
            New SqlParameter("@Phone", sdt),
            New SqlParameter("@Address", diaChi),
            New SqlParameter("@ClassID", maLop)
        }

        Return Convert.ToInt32(ThucThiGiaTriDon("EXEC sp_InsertStudent @StudentCode, @FullName, @DateOfBirth, @Gender, @Email, @Phone, @Address, @ClassID", thamSo))
    End Function

    ' Hàm CapNhatSinhVien: cập nhật thông tin sinh viên
    Public Shared Function CapNhatSinhVien(maSV As Integer, maCode As String, hoTen As String,
                                           ngaySinh As Date, gioiTinh As String, email As String,
                                           sdt As String, diaChi As String, maLop As Integer, trangThai As String) As Integer
        Dim thamSo As New List(Of SqlParameter) From {
            New SqlParameter("@StudentID", maSV),
            New SqlParameter("@StudentCode", maCode),
            New SqlParameter("@FullName", hoTen),
            New SqlParameter("@DateOfBirth", ngaySinh),
            New SqlParameter("@Gender", gioiTinh),
            New SqlParameter("@Email", email),
            New SqlParameter("@Phone", sdt),
            New SqlParameter("@Address", diaChi),
            New SqlParameter("@ClassID", maLop),
            New SqlParameter("@Status", trangThai)
        }

        Return ThucThiLenh("EXEC sp_UpdateStudent @StudentID, @StudentCode, @FullName, @DateOfBirth, @Gender, @Email, @Phone, @Address, @ClassID, @Status", thamSo)
    End Function

    ' Hàm XoaSinhVien: xóa sinh viên theo ID
    Public Shared Function XoaSinhVien(maSV As Integer) As Integer
        Dim thamSo As New List(Of SqlParameter) From {
            New SqlParameter("@StudentID", maSV)
        }
        Return ThucThiLenh("EXEC sp_DeleteStudent @StudentID", thamSo)
    End Function
End Class
