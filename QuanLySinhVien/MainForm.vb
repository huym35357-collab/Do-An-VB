Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Reflection

' Form chính để quản lý sinh viên
Public Class MainForm
    Inherits Form

    ' Khai báo các control
    Private WithEvents dgvSinhVien As DataGridView
    Private WithEvents txtTimKiem As TextBox
    Private WithEvents btnTimKiem As Button
    Private WithEvents btnThem As Button
    Private WithEvents btnXoa As Button
    Private WithEvents btnLamMoi As Button
    Private WithEvents btnKiemTraKetNoi As Button
    Private WithEvents btnChenDuLieuMau As Button
    Private WithEvents cmbLop As ComboBox
    Private WithEvents cmbKhoa As ComboBox
    Private WithEvents lblTimKiem As Label
    Private WithEvents lblLop As Label
    Friend WithEvents pnlTimKiem As Panel
    Friend WithEvents pnlNut As Panel
    Private WithEvents lblKhoa As Label

    ' Hàm khởi tạo Form
    Public Sub New()
        InitializeComponent()
    End Sub


    Private Sub KhiFormTaiLen(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Tránh lỗi khi mở trong Designer
        If DesignMode OrElse LicenseManager.UsageMode = LicenseUsageMode.Designtime Then Return
        NapDuLieuComboBox()
        NapDuLieuBang()
    End Sub



    Private Sub NapDuLieuComboBox()
        Try
            ' Lấy danh sách khoa từ database
            Dim bangKhoa As DataTable = TroGiupCoSoDuLieu.LayDanhSachKhoa()
            cmbKhoa.Items.Clear()
            cmbKhoa.Items.Add(New ComboBoxItem("Tất cả", 0))

            ' Thêm từng khoa vào combobox
            For Each dong As DataRow In bangKhoa.Rows
                cmbKhoa.Items.Add(New ComboBoxItem(dong("DepartmentName").ToString(), Convert.ToInt32(dong("DepartmentID"))))
            Next
            cmbKhoa.SelectedIndex = 0

            ' Nạp danh sách lớp tương ứng
            NapDanhSachLop()
        Catch ex As Exception
            MessageBox.Show("Lỗi khi tải dữ liệu khoa: " & ex.Message)
        End Try
    End Sub


    ' Nạp dữ liệu cho combobox Lớp từ cơ sở dữ liệu>
    Private Sub NapDanhSachLop()
        Try
            ' Lấy danh sách lớp từ database
            Dim bangLop As DataTable = TroGiupCoSoDuLieu.LayDanhSachLop()
            cmbLop.Items.Clear()
            cmbLop.Items.Add(New ComboBoxItem("Tất cả", 0))

            ' Thêm từng lớp vào combobox
            For Each dong As DataRow In bangLop.Rows
                cmbLop.Items.Add(New ComboBoxItem(dong("ClassName").ToString(), Convert.ToInt32(dong("ClassID"))))
            Next
            cmbLop.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show("Lỗi khi tải danh sách lớp: " & ex.Message)
        End Try
    End Sub


    ' Nạp dữ liệu sinh viên vào DataGridView dựa trên điều kiện tìm kiếm
    Private Sub NapDuLieuBang()
        Try
            ' Lấy điều kiện tìm kiếm từ các control
            Dim tuKhoa As String = If(String.IsNullOrEmpty(txtTimKiem.Text), Nothing, txtTimKiem.Text)
            Dim maLop As Integer? = If(cmbLop.SelectedItem IsNot Nothing AndAlso CType(cmbLop.SelectedItem, ComboBoxItem).Value <> 0, CType(cmbLop.SelectedItem, ComboBoxItem).Value, Nothing)
            Dim maKhoa As Integer? = If(cmbKhoa.SelectedItem IsNot Nothing AndAlso CType(cmbKhoa.SelectedItem, ComboBoxItem).Value <> 0, CType(cmbKhoa.SelectedItem, ComboBoxItem).Value, Nothing)

            ' Lấy dữ liệu sinh viên từ database
            Dim bangSV As DataTable = TroGiupCoSoDuLieu.LayDanhSachSinhVien(tuKhoa, maLop, maKhoa)
            dgvSinhVien.DataSource = bangSV

            ' Ẩn cột ID để người dùng không thấy
            If dgvSinhVien.Columns.Contains("StudentID") Then
                dgvSinhVien.Columns("StudentID").Visible = False
            End If

            ' Cập nhật tên cột và trạng thái nút
            DatLaiTenCot()
            CapNhatTrangThaiNut()
        Catch ex As Exception
            MessageBox.Show("Lỗi khi tải dữ liệu sinh viên: " & ex.Message)
        End Try
    End Sub

    ' Đặt lại tiêu đề cột trong DataGridView sang tiếng Việt
    Private Sub DatLaiTenCot()
        ' Đổi tên các cột từ tiếng Anh sang tiếng Việt
        If dgvSinhVien.Columns.Contains("StudentCode") Then dgvSinhVien.Columns("StudentCode").HeaderText = "Mã SV"
        If dgvSinhVien.Columns.Contains("FullName") Then dgvSinhVien.Columns("FullName").HeaderText = "Họ và tên"
        If dgvSinhVien.Columns.Contains("DateOfBirth") Then dgvSinhVien.Columns("DateOfBirth").HeaderText = "Ngày sinh"
        If dgvSinhVien.Columns.Contains("Gender") Then dgvSinhVien.Columns("Gender").HeaderText = "Giới tính"
        If dgvSinhVien.Columns.Contains("Email") Then dgvSinhVien.Columns("Email").HeaderText = "Email"
        If dgvSinhVien.Columns.Contains("Phone") Then dgvSinhVien.Columns("Phone").HeaderText = "SĐT"
        If dgvSinhVien.Columns.Contains("Address") Then dgvSinhVien.Columns("Address").HeaderText = "Địa chỉ"
        If dgvSinhVien.Columns.Contains("ClassName") Then dgvSinhVien.Columns("ClassName").HeaderText = "Lớp"
        If dgvSinhVien.Columns.Contains("DepartmentName") Then dgvSinhVien.Columns("DepartmentName").HeaderText = "Khoa"
        If dgvSinhVien.Columns.Contains("Status") Then dgvSinhVien.Columns("Status").HeaderText = "Trạng thái"
        If dgvSinhVien.Columns.Contains("EnrollmentDate") Then dgvSinhVien.Columns("EnrollmentDate").HeaderText = "Ngày nhập học"
    End Sub


    ' Cập nhật trạng thái các nút dựa trên dòng được chọn trong DataGridView
    Private Sub CapNhatTrangThaiNut()
        ' Chỉ cho phép xóa khi có dòng được chọn
        btnXoa.Enabled = dgvSinhVien.SelectedRows.Count > 0
    End Sub

    ' Sự kiện khi click nút tìm kiếm - thực hiện tìm kiếm sinh viên
    Private Sub KhiNhanNutTimKiem(sender As Object, e As EventArgs) Handles btnTimKiem.Click
        NapDuLieuBang()
    End Sub

    'Sự kiện khi click nút thêm mới - tạo dòng mới để nhập thông tin sinh viên
    Private Sub KhiNhanNutThemMoi(sender As Object, e As EventArgs) Handles btnThem.Click
        ' Lấy bảng dữ liệu hiện tại
        Dim bang As DataTable = CType(dgvSinhVien.DataSource, DataTable)
        ' Tạo dòng mới
        Dim dongMoi As DataRow = bang.NewRow()
        ' Thiết lập giá trị mặc định
        dongMoi("Status") = "Đang học"
        dongMoi("EnrollmentDate") = DateTime.Today
        ' Thêm dòng mới vào đầu bảng (vị trí 0)
        bang.Rows.InsertAt(dongMoi, 0)
    End Sub

    ' Sự kiện khi click nút xóa - xóa sinh viên được chọn sau khi xác nhận
    Private Sub KhiNhanNutXoa(sender As Object, e As EventArgs) Handles btnXoa.Click
        ' Kiểm tra xem có dòng nào được chọn không
        If dgvSinhVien.SelectedRows.Count > 0 Then
            ' Lấy thông tin dòng được chọn
            Dim dong As DataGridViewRow = dgvSinhVien.SelectedRows(0)
            Dim maSV As Integer = Convert.ToInt32(dong.Cells("StudentID").Value)
            Dim hoTen As String = dong.Cells("FullName").Value.ToString()

            ' Hiển thị hộp thoại xác nhận
            Dim ketQua As DialogResult = MessageBox.Show($"Bạn có chắc chắn muốn xóa sinh viên {hoTen}?", "Xác nhận", MessageBoxButtons.YesNo)
            If ketQua = DialogResult.Yes Then
                ' Thực hiện xóa từ database
                TroGiupCoSoDuLieu.XoaSinhVien(maSV)
                ' Cập nhật lại bảng dữ liệu
                NapDuLieuBang()
            End If
        End If
    End Sub

    ' Sự kiện khi người dùng hoàn thành chỉnh sửa dòng trong DataGridView
    ' Tự động lưu dữ liệu vào cơ sở dữ liệu
    Private Sub KhiHoanThanhChinhSuaDong(sender As Object, e As DataGridViewCellCancelEventArgs) Handles dgvSinhVien.RowValidating
        ' Kiểm tra điều kiện cần thiết
        If dgvSinhVien.CurrentRow Is Nothing OrElse dgvSinhVien.IsCurrentRowDirty = False Then Return
        Dim dong As DataGridViewRow = dgvSinhVien.Rows(e.RowIndex)
        If dong.IsNewRow Then Return

        ' Lấy dữ liệu từ các ô trong dòng
        Dim maSV As Object = dong.Cells("StudentID").Value
        Dim maCode As String = CStr(dong.Cells("StudentCode").Value)
        Dim hoTen As String = CStr(dong.Cells("FullName").Value)
        Dim ngaySinh As Date = If(dong.Cells("DateOfBirth").Value Is Nothing, Date.Today.AddYears(-18), CDate(dong.Cells("DateOfBirth").Value))
        Dim gioiTinh As String = If(dong.Cells("Gender").Value Is Nothing, "Nam", CStr(dong.Cells("Gender").Value))
        Dim email As String = If(dong.Cells("Email").Value, String.Empty)
        Dim sdt As String = If(dong.Cells("Phone").Value, String.Empty)
        Dim diaChi As String = If(dong.Cells("Address").Value, String.Empty)
        Dim maLop As Integer = Convert.ToInt32(dong.Cells("ClassID").Value)
        Dim trangThai As String = If(dong.Cells("Status").Value, "Đang học")

        ' Kiểm tra xem là thêm mới hay cập nhật
        If maSV Is DBNull.Value OrElse Convert.ToInt32(maSV) = 0 Then
            ' Thêm sinh viên mới vào database
            Dim idMoi As Integer = TroGiupCoSoDuLieu.ThemSinhVien(maCode, hoTen, ngaySinh, gioiTinh, email, sdt, diaChi, maLop)
            dong.Cells("StudentID").Value = idMoi
        Else
            ' Cập nhật thông tin sinh viên hiện có
            TroGiupCoSoDuLieu.CapNhatSinhVien(Convert.ToInt32(maSV), maCode, hoTen, ngaySinh, gioiTinh, email, sdt, diaChi, maLop, trangThai)
        End If
    End Sub

    'Sự kiện khi click nút làm mới - reset tất cả điều kiện tìm kiếm và tải lại dữ liệu
    Private Sub KhiNhanNutLamMoi(sender As Object, e As EventArgs) Handles btnLamMoi.Click
        ' Xóa nội dung tìm kiếm
        txtTimKiem.Clear()
        ' Reset combobox về "Tất cả"
        cmbLop.SelectedIndex = 0
        cmbKhoa.SelectedIndex = 0
        ' Tải lại toàn bộ dữ liệu
        NapDuLieuBang()
    End Sub

    ' Sự kiện khi thay đổi lựa chọn khoa - cập nhật danh sách lớp tương ứng
    Private Sub KhiThayDoiKhoa(sender As Object, e As EventArgs) Handles cmbKhoa.SelectedIndexChanged
        ' Nạp lại danh sách lớp theo khoa được chọn
        NapDanhSachLop()
    End Sub

    '' Sự kiện khi click nút kiểm tra kết nối - test kết nối cơ sở dữ liệu
    Private Sub KhiNhanNutKiemTraKetNoi(sender As Object, e As EventArgs) Handles btnKiemTraKetNoi.Click
        TroGiupCoSoDuLieu.KiemTraKetNoi()
    End Sub

    '' Khởi tạo giao diện form - thiết lập tất cả các control và thuộc tính
    Private Sub InitializeComponent()
        Me.pnlTimKiem = New System.Windows.Forms.Panel()
        Me.lblKhoa = New System.Windows.Forms.Label()
        Me.cmbKhoa = New System.Windows.Forms.ComboBox()
        Me.lblLop = New System.Windows.Forms.Label()
        Me.cmbLop = New System.Windows.Forms.ComboBox()
        Me.btnTimKiem = New System.Windows.Forms.Button()
        Me.txtTimKiem = New System.Windows.Forms.TextBox()
        Me.lblTimKiem = New System.Windows.Forms.Label()
        Me.pnlNut = New System.Windows.Forms.Panel()
        Me.btnChenDuLieuMau = New System.Windows.Forms.Button()
        Me.btnKiemTraKetNoi = New System.Windows.Forms.Button()
        Me.btnLamMoi = New System.Windows.Forms.Button()
        Me.btnXoa = New System.Windows.Forms.Button()
        Me.btnThem = New System.Windows.Forms.Button()
        Me.dgvSinhVien = New System.Windows.Forms.DataGridView()
        Me.pnlTimKiem.SuspendLayout()
        Me.pnlNut.SuspendLayout()
        CType(Me.dgvSinhVien, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pnlTimKiem
        '
        Me.pnlTimKiem.BackColor = System.Drawing.Color.LightGray
        Me.pnlTimKiem.Controls.Add(Me.lblKhoa)
        Me.pnlTimKiem.Controls.Add(Me.cmbKhoa)
        Me.pnlTimKiem.Controls.Add(Me.lblLop)
        Me.pnlTimKiem.Controls.Add(Me.cmbLop)
        Me.pnlTimKiem.Controls.Add(Me.btnTimKiem)
        Me.pnlTimKiem.Controls.Add(Me.txtTimKiem)
        Me.pnlTimKiem.Controls.Add(Me.lblTimKiem)
        Me.pnlTimKiem.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlTimKiem.Location = New System.Drawing.Point(0, 0)
        Me.pnlTimKiem.Name = "pnlTimKiem"
        Me.pnlTimKiem.Size = New System.Drawing.Size(984, 80)
        Me.pnlTimKiem.TabIndex = 0
        '
        'lblKhoa
        '
        Me.lblKhoa.AutoSize = True
        Me.lblKhoa.Location = New System.Drawing.Point(580, 15)
        Me.lblKhoa.Name = "lblKhoa"
        Me.lblKhoa.Size = New System.Drawing.Size(35, 15)
        Me.lblKhoa.TabIndex = 6
        Me.lblKhoa.Text = "Khoa:"
        '
        'cmbKhoa
        '
        Me.cmbKhoa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKhoa.FormattingEnabled = True
        Me.cmbKhoa.Location = New System.Drawing.Point(620, 12)
        Me.cmbKhoa.Name = "cmbKhoa"
        Me.cmbKhoa.Size = New System.Drawing.Size(120, 23)
        Me.cmbKhoa.TabIndex = 5
        '
        'lblLop
        '
        Me.lblLop.AutoSize = True
        Me.lblLop.Location = New System.Drawing.Point(400, 15)
        Me.lblLop.Name = "lblLop"
        Me.lblLop.Size = New System.Drawing.Size(28, 15)
        Me.lblLop.TabIndex = 4
        Me.lblLop.Text = "Lớp:"
        '
        'cmbLop
        '
        Me.cmbLop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbLop.FormattingEnabled = True
        Me.cmbLop.Location = New System.Drawing.Point(440, 12)
        Me.cmbLop.Name = "cmbLop"
        Me.cmbLop.Size = New System.Drawing.Size(120, 23)
        Me.cmbLop.TabIndex = 3
        '
        'btnTimKiem
        '
        Me.btnTimKiem.BackColor = System.Drawing.Color.LightBlue
        Me.btnTimKiem.Location = New System.Drawing.Point(290, 10)
        Me.btnTimKiem.Name = "btnTimKiem"
        Me.btnTimKiem.Size = New System.Drawing.Size(75, 23)
        Me.btnTimKiem.TabIndex = 2
        Me.btnTimKiem.Text = "Tìm kiếm"
        Me.btnTimKiem.UseVisualStyleBackColor = False
        '
        'txtTimKiem
        '
        Me.txtTimKiem.Location = New System.Drawing.Point(80, 12)
        Me.txtTimKiem.Name = "txtTimKiem"
        Me.txtTimKiem.Size = New System.Drawing.Size(200, 23)
        Me.txtTimKiem.TabIndex = 1
        '
        'lblTimKiem
        '
        Me.lblTimKiem.AutoSize = True
        Me.lblTimKiem.Location = New System.Drawing.Point(10, 15)
        Me.lblTimKiem.Name = "lblTimKiem"
        Me.lblTimKiem.Size = New System.Drawing.Size(60, 15)
        Me.lblTimKiem.TabIndex = 0
        Me.lblTimKiem.Text = "Tìm kiếm:"
        '
        'pnlNut
        '
        Me.pnlNut.BackColor = System.Drawing.Color.LightBlue
        Me.pnlNut.Controls.Add(Me.btnChenDuLieuMau)
        Me.pnlNut.Controls.Add(Me.btnKiemTraKetNoi)
        Me.pnlNut.Controls.Add(Me.btnLamMoi)
        Me.pnlNut.Controls.Add(Me.btnXoa)
        Me.pnlNut.Controls.Add(Me.btnThem)
        Me.pnlNut.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlNut.Location = New System.Drawing.Point(0, 80)
        Me.pnlNut.Name = "pnlNut"
        Me.pnlNut.Size = New System.Drawing.Size(984, 50)
        Me.pnlNut.TabIndex = 1
        '
        'btnChenDuLieuMau
        '
        Me.btnChenDuLieuMau.BackColor = System.Drawing.Color.LightGreen
        Me.btnChenDuLieuMau.Location = New System.Drawing.Point(290, 10)
        Me.btnChenDuLieuMau.Name = "btnChenDuLieuMau"
        Me.btnChenDuLieuMau.Size = New System.Drawing.Size(100, 23)
        Me.btnChenDuLieuMau.TabIndex = 4
        Me.btnChenDuLieuMau.Text = "Thêm dữ liệu"
        Me.btnChenDuLieuMau.UseVisualStyleBackColor = False
        '
        'btnKiemTraKetNoi
        '
        Me.btnKiemTraKetNoi.BackColor = System.Drawing.Color.LightPink
        Me.btnKiemTraKetNoi.Location = New System.Drawing.Point(400, 10)
        Me.btnKiemTraKetNoi.Name = "btnKiemTraKetNoi"
        Me.btnKiemTraKetNoi.Size = New System.Drawing.Size(75, 23)
        Me.btnKiemTraKetNoi.TabIndex = 3
        Me.btnKiemTraKetNoi.Text = "Test DB"
        Me.btnKiemTraKetNoi.UseVisualStyleBackColor = False
        '
        'btnLamMoi
        '
        Me.btnLamMoi.BackColor = System.Drawing.Color.LightYellow
        Me.btnLamMoi.Location = New System.Drawing.Point(200, 10)
        Me.btnLamMoi.Name = "btnLamMoi"
        Me.btnLamMoi.Size = New System.Drawing.Size(75, 23)
        Me.btnLamMoi.TabIndex = 2
        Me.btnLamMoi.Text = "Làm mới"
        Me.btnLamMoi.UseVisualStyleBackColor = False
        '
        'btnXoa
        '
        Me.btnXoa.BackColor = System.Drawing.Color.LightCoral
        Me.btnXoa.Enabled = False
        Me.btnXoa.Location = New System.Drawing.Point(100, 10)
        Me.btnXoa.Name = "btnXoa"
        Me.btnXoa.Size = New System.Drawing.Size(75, 23)
        Me.btnXoa.TabIndex = 1
        Me.btnXoa.Text = "Xóa"
        Me.btnXoa.UseVisualStyleBackColor = False
        '
        'btnThem
        '
        Me.btnThem.BackColor = System.Drawing.Color.LightGreen
        Me.btnThem.Location = New System.Drawing.Point(10, 10)
        Me.btnThem.Name = "btnThem"
        Me.btnThem.Size = New System.Drawing.Size(75, 23)
        Me.btnThem.TabIndex = 0
        Me.btnThem.Text = "Thêm mới"
        Me.btnThem.UseVisualStyleBackColor = False
        '
        'dgvSinhVien
        '
        Me.dgvSinhVien.AllowUserToAddRows = True
        Me.dgvSinhVien.AllowUserToDeleteRows = False
        Me.dgvSinhVien.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightCyan
        Me.dgvSinhVien.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvSinhVien.BackgroundColor = System.Drawing.Color.White
        Me.dgvSinhVien.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvSinhVien.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvSinhVien.Location = New System.Drawing.Point(0, 130)
        Me.dgvSinhVien.MultiSelect = False
        Me.dgvSinhVien.Name = "dgvSinhVien"
        Me.dgvSinhVien.ReadOnly = False
        Me.dgvSinhVien.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvSinhVien.Size = New System.Drawing.Size(984, 431)
        Me.dgvSinhVien.TabIndex = 2
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(984, 561)
        Me.Controls.Add(Me.dgvSinhVien)
        Me.Controls.Add(Me.pnlNut)
        Me.Controls.Add(Me.pnlTimKiem)
        Me.MinimumSize = New System.Drawing.Size(800, 500)
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Quản lý sinh viên"
        Me.pnlTimKiem.ResumeLayout(False)
        Me.pnlTimKiem.PerformLayout()
        Me.pnlNut.ResumeLayout(False)
        CType(Me.dgvSinhVien, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    ' Sự kiện khi click nút chèn dữ liệu mẫu - thêm dữ liệu test vào database
    Private Sub KhiNhanNutChenDuLieuMau(sender As Object, e As EventArgs) Handles btnChenDuLieuMau.Click
        ' Thực hiện chèn dữ liệu mẫu
        If TroGiupCoSoDuLieu.ChenDuLieuMau() Then
            ' Nếu thành công thì tải lại dữ liệu
            NapDuLieuBang()
        End If
    End Sub
End Class

' Lớp ComboBoxItem: lưu trữ Text và Value cho combobox
Public Class ComboBoxItem
    Public Property Text As String
    Public Property Value As Integer
    Public Sub New(text As String, value As Integer)
        Me.Text = text
        Me.Value = value
    End Sub
    Public Overrides Function ToString() As String
        Return Text
    End Function
End Class
