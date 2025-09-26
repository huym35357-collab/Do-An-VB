
USE master;
GO
IF DB_ID(N'StudentManagement') IS NOT NULL
BEGIN
    ALTER DATABASE StudentManagement SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE StudentManagement;
END
GO
CREATE DATABASE StudentManagement;
GO
USE StudentManagement;
GO


-- Tạo bảng Khoa (Departments)
CREATE TABLE Departments (
    DepartmentID INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500),
    CreatedDate DATETIME DEFAULT GETDATE()
)
GO

-- Tạo bảng Lớp (Classes)
CREATE TABLE Classes (
    ClassID INT IDENTITY(1,1) PRIMARY KEY,
    ClassName NVARCHAR(50) NOT NULL,
    DepartmentID INT NOT NULL,
    AcademicYear NVARCHAR(10) NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Classes_Departments FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID) ON DELETE CASCADE
)
GO

-- Tạo bảng Sinh viên (Students)
CREATE TABLE Students (
    StudentID INT IDENTITY(1,1) PRIMARY KEY,
    StudentCode NVARCHAR(20) NOT NULL UNIQUE,
    FullName NVARCHAR(100) NOT NULL,
    DateOfBirth DATE NOT NULL,
    Gender NVARCHAR(10) NOT NULL,
    Email NVARCHAR(100) UNIQUE,
    Phone NVARCHAR(15),
    Address NVARCHAR(200),
    ClassID INT NOT NULL,
    EnrollmentDate DATE DEFAULT GETDATE(),
    Status NVARCHAR(20) DEFAULT 'Đang học' CHECK (Status IN ('Đang học', 'Tốt nghiệp', 'Nghỉ học')),
    CreatedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Students_Classes FOREIGN KEY (ClassID) REFERENCES Classes(ClassID) ON DELETE CASCADE,
    CONSTRAINT CK_Students_Age CHECK (DATEDIFF(YEAR, DateOfBirth, GETDATE()) >= 16 AND DATEDIFF(YEAR, DateOfBirth, GETDATE()) <= 50)
)
GO

-- Tạo index để tối ưu hóa tìm kiếm
CREATE INDEX IX_Students_StudentCode ON Students(StudentCode)
CREATE INDEX IX_Students_FullName ON Students(FullName)
CREATE INDEX IX_Students_ClassID ON Students(ClassID)
CREATE INDEX IX_Classes_DepartmentID ON Classes(DepartmentID)

-- Insert dữ liệu mẫu
INSERT INTO Departments (DepartmentName, Description) VALUES
(N'Công nghệ thông tin', N'Khoa chuyên về lập trình và công nghệ'),
(N'Kinh tế', N'Khoa chuyên về kinh tế và quản trị'),
(N'Ngoại ngữ', N'Khoa chuyên về ngôn ngữ và văn hóa')

INSERT INTO Classes (ClassName, DepartmentID, AcademicYear) VALUES
(N'CNTT01', 1, '2024-2025'),
(N'CNTT02', 1, '2024-2025'),
(N'KT01', 2, '2024-2025'),
(N'NN01', 3, '2024-2025')

INSERT INTO Students (StudentCode, FullName, DateOfBirth, Gender, Email, Phone, Address, ClassID) VALUES
(N'SV001', N'Nguyễn Văn An', '2000-05-15', N'Nam', 'an.nguyen@email.com', '0123456789', N'Hà Nội', 1),
(N'SV002', N'Trần Thị Bình', '2001-03-20', N'Nữ', 'binh.tran@email.com', '0987654321', N'TP.HCM', 1),
(N'SV003', N'Lê Văn Cường', '2000-12-10', N'Nam', 'cuong.le@email.com', '0369258147', N'Đà Nẵng', 2),
(N'SV004', N'Phạm Thị Dung', '2001-08-25', N'Nữ', 'dung.pham@email.com', '0741852963', N'Hải Phòng', 3),
(N'SV005', N'Hoàng Văn Em', '2000-11-30', N'Nam', 'em.hoang@email.com', '0159753248', N'Cần Thơ', 4)

-- Tạo stored procedures
GO

-- Stored procedure để tìm kiếm sinh viên
CREATE PROCEDURE sp_SearchStudents
    @SearchTerm NVARCHAR(100) = NULL,
    @ClassID INT = NULL,
    @DepartmentID INT = NULL
AS
BEGIN
    SELECT s.StudentID, s.StudentCode, s.FullName, s.DateOfBirth, s.Gender, 
           s.Email, s.Phone, s.Address, s.Status, s.EnrollmentDate,
           c.ClassName, d.DepartmentName
    FROM Students s
    INNER JOIN Classes c ON s.ClassID = c.ClassID
    INNER JOIN Departments d ON c.DepartmentID = d.DepartmentID
    WHERE (@SearchTerm IS NULL OR 
           s.StudentCode LIKE '%' + @SearchTerm + '%' OR 
           s.FullName LIKE '%' + @SearchTerm + '%' OR
           s.Email LIKE '%' + @SearchTerm + '%')
    AND (@ClassID IS NULL OR s.ClassID = @ClassID)
    AND (@DepartmentID IS NULL OR c.DepartmentID = @DepartmentID)
    ORDER BY s.StudentCode
END
GO

-- Stored procedure để thêm sinh viên
CREATE PROCEDURE sp_InsertStudent
    @StudentCode NVARCHAR(20),
    @FullName NVARCHAR(100),
    @DateOfBirth DATE,
    @Gender NVARCHAR(10),
    @Email NVARCHAR(100),
    @Phone NVARCHAR(15),
    @Address NVARCHAR(200),
    @ClassID INT
AS
BEGIN
    INSERT INTO Students (StudentCode, FullName, DateOfBirth, Gender, Email, Phone, Address, ClassID)
    VALUES (@StudentCode, @FullName, @DateOfBirth, @Gender, @Email, @Phone, @Address, @ClassID)
    
    SELECT SCOPE_IDENTITY() AS NewStudentID
END
GO

-- Stored procedure để cập nhật sinh viên
CREATE PROCEDURE sp_UpdateStudent
    @StudentID INT,
    @StudentCode NVARCHAR(20),
    @FullName NVARCHAR(100),
    @DateOfBirth DATE,
    @Gender NVARCHAR(10),
    @Email NVARCHAR(100),
    @Phone NVARCHAR(15),
    @Address NVARCHAR(200),
    @ClassID INT,
    @Status NVARCHAR(20)
AS
BEGIN
    UPDATE Students 
    SET StudentCode = @StudentCode,
        FullName = @FullName,
        DateOfBirth = @DateOfBirth,
        Gender = @Gender,
        Email = @Email,
        Phone = @Phone,
        Address = @Address,
        ClassID = @ClassID,
        Status = @Status
    WHERE StudentID = @StudentID
END
GO

-- Stored procedure để xóa sinh viên
CREATE PROCEDURE sp_DeleteStudent
    @StudentID INT
AS
BEGIN
    DELETE FROM Students WHERE StudentID = @StudentID
END
GO

USE StudentManagement
GO


-- Thêm dữ liệu mẫu
IF NOT EXISTS (SELECT 1 FROM Departments)
BEGIN
    INSERT INTO Departments (DepartmentName, Description) VALUES
    (N'Công nghệ thông tin', N'Khoa chuyên về lập trình và công nghệ'),
    (N'Kinh tế', N'Khoa chuyên về kinh tế và quản trị'),
    (N'Ngoại ngữ', N'Khoa chuyên về ngôn ngữ và văn hóa')
    PRINT 'Đã thêm dữ liệu Departments'
END

IF NOT EXISTS (SELECT 1 FROM Classes)
BEGIN
    INSERT INTO Classes (ClassName, DepartmentID, AcademicYear) VALUES
    (N'CNTT01', 1, '2024-2025'),
    (N'CNTT02', 1, '2024-2025'),
    (N'KT01', 2, '2024-2025'),
    (N'NN01', 3, '2024-2025')
    PRINT 'Đã thêm dữ liệu Classes'
END


INSERT INTO Students (StudentCode, FullName, DateOfBirth, Gender, Email, Phone, Address, ClassID) VALUES
(N'SV006', N'Nguyễn Thị Hoa', '2001-01-15', N'Nữ', 'hoa.nguyen@email.com', '0911111111', N'Hà Nội', 1),
(N'SV007', N'Lê Văn Long', '2000-02-20', N'Nam', 'long.le@email.com', '0911111112', N'TP.HCM', 1),
(N'SV008', N'Trần Thị Mai', '2001-03-25', N'Nữ', 'mai.tran@email.com', '0911111113', N'Đà Nẵng', 1),
(N'SV009', N'Phạm Văn Hải', '2000-04-10', N'Nam', 'hai.pham@email.com', '0911111114', N'Hải Phòng', 1),
(N'SV010', N'Hoàng Thị Lan', '2001-05-05', N'Nữ', 'lan.hoang@email.com', '0911111115', N'Cần Thơ', 1),

(N'SV011', N'Ngô Văn Bình', '2000-06-18', N'Nam', 'binh.ngo@email.com', '0911111116', N'Hà Nội', 2),
(N'SV012', N'Đỗ Thị Cúc', '2001-07-22', N'Nữ', 'cuc.do@email.com', '0911111117', N'TP.HCM', 2),
(N'SV013', N'Vũ Văn Minh', '2000-08-14', N'Nam', 'minh.vu@email.com', '0911111118', N'Đà Nẵng', 2),
(N'SV014', N'Bùi Thị Hằng', '2001-09-09', N'Nữ', 'hang.bui@email.com', '0911111119', N'Hải Phòng', 2),
(N'SV015', N'Nguyễn Văn Tuấn', '2000-10-30', N'Nam', 'tuan.nguyen@email.com', '0911111120', N'Cần Thơ', 2),

(N'SV016', N'Phan Thị Yến', '2001-01-10', N'Nữ', 'yen.phan@email.com', '0911111121', N'Hà Nội', 3),
(N'SV017', N'Trịnh Văn Nam', '2000-02-28', N'Nam', 'nam.trinh@email.com', '0911111122', N'TP.HCM', 3),
(N'SV018', N'Lương Thị Hạnh', '2001-03-17', N'Nữ', 'hanh.luong@email.com', '0911111123', N'Đà Nẵng', 3),
(N'SV019', N'Cao Văn Lực', '2000-04-08', N'Nam', 'luc.cao@email.com', '0911111124', N'Hải Phòng', 3),
(N'SV020', N'Tạ Thị Vân', '2001-05-19', N'Nữ', 'van.ta@email.com', '0911111125', N'Cần Thơ', 3),

(N'SV021', N'Nguyễn Văn Khánh', '2000-06-22', N'Nam', 'khanh.nguyen@email.com', '0911111126', N'Hà Nội', 4),
(N'SV022', N'Trần Thị Nhung', '2001-07-27', N'Nữ', 'nhung.tran@email.com', '0911111127', N'TP.HCM', 4),
(N'SV023', N'Lê Văn Thành', '2000-08-11', N'Nam', 'thanh.le@email.com', '0911111128', N'Đà Nẵng', 4),
(N'SV024', N'Phạm Thị Kim', '2001-09-21', N'Nữ', 'kim.pham@email.com', '0911111129', N'Hải Phòng', 4),
(N'SV025', N'Hoàng Văn Dũng', '2000-10-13', N'Nam', 'dung.hoang@email.com', '0911111130', N'Cần Thơ', 4),

(N'SV026', N'Nguyễn Thị Liên', '2001-11-02', N'Nữ', 'lien.nguyen@email.com', '0911111131', N'Hà Nội', 1),
(N'SV027', N'Lê Văn Hòa', '2000-12-12', N'Nam', 'hoa.le@email.com', '0911111132', N'TP.HCM', 1),
(N'SV028', N'Trần Thị Ngọc', '2001-01-29', N'Nữ', 'ngoc.tran@email.com', '0911111133', N'Đà Nẵng', 1),
(N'SV029', N'Phạm Văn Thắng', '2000-02-18', N'Nam', 'thang.pham@email.com', '0911111134', N'Hải Phòng', 1),
(N'SV030', N'Hoàng Thị Quỳnh', '2001-03-08', N'Nữ', 'quynh.hoang@email.com', '0911111135', N'Cần Thơ', 1),

(N'SV031', N'Ngô Văn Hùng', '2000-04-14', N'Nam', 'hung.ngo@email.com', '0911111136', N'Hà Nội', 2),
(N'SV032', N'Đỗ Thị Thu', '2001-05-25', N'Nữ', 'thu.do@email.com', '0911111137', N'TP.HCM', 2),
(N'SV033', N'Vũ Văn Khải', '2000-06-19', N'Nam', 'khai.vu@email.com', '0911111138', N'Đà Nẵng', 2),
(N'SV034', N'Bùi Thị Lan', '2001-07-23', N'Nữ', 'lan.bui@email.com', '0911111139', N'Hải Phòng', 2),
(N'SV035', N'Nguyễn Văn Tài', '2000-08-04', N'Nam', 'tai.nguyen@email.com', '0911111140', N'Cần Thơ', 2),

(N'SV036', N'Phan Thị Hòa', '2001-09-15', N'Nữ', 'hoa.phan@email.com', '0911111141', N'Hà Nội', 3),
(N'SV037', N'Trịnh Văn Hậu', '2000-10-26', N'Nam', 'hau.trinh@email.com', '0911111142', N'TP.HCM', 3),
(N'SV038', N'Lương Thị Phượng', '2001-11-30', N'Nữ', 'phuong.luong@email.com', '0911111143', N'Đà Nẵng', 3),
(N'SV039', N'Cao Văn Sơn', '2000-12-09', N'Nam', 'son.cao@email.com', '0911111144', N'Hải Phòng', 3),
(N'SV040', N'Tạ Thị Minh', '2001-01-19', N'Nữ', 'minh.ta@email.com', '0911111145', N'Cần Thơ', 3),

(N'SV041', N'Nguyễn Văn Quân', '2000-02-25', N'Nam', 'quan.nguyen@email.com', '0911111146', N'Hà Nội', 4),
(N'SV042', N'Trần Thị Oanh', '2001-03-12', N'Nữ', 'oanh.tran@email.com', '0911111147', N'TP.HCM', 4),
(N'SV043', N'Lê Văn Đức', '2000-04-07', N'Nam', 'duc.le@email.com', '0911111148', N'Đà Nẵng', 4),
(N'SV044', N'Phạm Thị Thu', '2001-05-16', N'Nữ', 'thu.pham@email.com', '0911111149', N'Hải Phòng', 4),
(N'SV045', N'Hoàng Văn Nam', '2000-06-28', N'Nam', 'nam.hoang@email.com', '0911111150', N'Cần Thơ', 4);

PRINT 'Đã thêm dữ liệu Students'



-- Test query giống như trong ứng dụng
SELECT s.StudentID, s.StudentCode, s.FullName, s.DateOfBirth, s.Gender, s.Email, s.Phone, s.Address, s.Status, s.EnrollmentDate, s.ClassID, c.ClassName, d.DepartmentName 
FROM Students s 
INNER JOIN Classes c ON s.ClassID = c.ClassID 
INNER JOIN Departments d ON c.DepartmentID = d.DepartmentID 
ORDER BY s.StudentCode

