
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



