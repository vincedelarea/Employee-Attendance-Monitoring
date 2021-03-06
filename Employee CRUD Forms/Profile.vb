Imports System.Data.OleDb

Public Class Profile

    Dim ConnString As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=EAM.mdb"
    Dim conn As New OleDbConnection(ConnString)
    Dim userID
    Dim name
    Dim EmpStat
    Dim StatTag
    Dim EmpID
    Dim Department
    Dim JobTitle
    Dim Position

    Public Function GetUserID(ByVal ID)
        Me.userID = ID
        Return 0
    End Function

    Private Sub Profile_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Chart1.ChartAreas(0).AxisY.LabelStyle.Enabled = False

        Dim cmd As New OleDbCommand("SELECT * FROM EmployeeRoster WHERE ID=@id", conn)
        cmd.Parameters.AddWithValue("@id", userID)
        conn.Open()

        Dim fetch As OleDbDataReader = cmd.ExecuteReader()
        If (fetch.Read = True) Then
            name = fetch("EmployeeFName") & " " & fetch("EmployeeLName")
            EmpStat = fetch("EmpStatus")
            StatTag = fetch("EmpStatusTag")
            EmpID = fetch("EmployeeID")
            Department = fetch("Department")
            JobTitle = fetch("JobTitle")
            Position = fetch("Position")
        End If
        conn.Close()
        Try
            name_txt.Text = name
            empStat_txt.Text = EmpStat
            dep_txt.Text = Department
            title_txt.Text = JobTitle
            pos_txt.Text = Position
            emp_tag.Text = StatTag
        Catch ex As Exception

        End Try
        Try
            Using dmc As New OleDbCommand("select profile_img from EmployeeRoster where ID=@id", conn)

                dmc.Parameters.AddWithValue("@id", userID)
                Dim stream As New IO.MemoryStream()
                conn.Open()
                Dim image As Byte() = DirectCast(dmc.ExecuteScalar(), Byte())
                stream.Write(image, 0, image.Length)
                Dim bitmap As New Bitmap(stream)
                PictureBox1.Image = bitmap
                stream.Close()
                conn.Close()
            End Using
        Catch ex As Exception

        End Try
    End Sub

    Public Function GetAttendance()
        Dim dateToday As DateTime = Date.Today()
        dateToday = dateToday.AddDays(-7)
        For a = 0 To 6
            Dim cmd As New OleDbCommand("SELECT COUNT(*) FROM Employees WHERE WorkDate=@date AND Attendance=Yes AND EmployeeID=@id", conn)
            cmd.Parameters.AddWithValue("@date", dateToday.AddDays(a))
            cmd.Parameters.AddWithValue("@id", EmpID)
            conn.Open()
            'MsgBox(dateToday.AddDays(a).ToString("dddd") & " " & cmd.ExecuteScalar())
            Me.Chart1.Series("Attendance").Points.AddXY(dateToday.AddDays(a).ToString("ddd"), cmd.ExecuteScalar())
            'Me.Chart1.Series("Absent").Points.AddXY(dateToday.AddDays(a).ToString("dddd"), EmployeeCount)
            conn.Close()
        Next
        'MsgBox(dateToday)
        'MsgBox(dateToday.AddDays(-6))
        Return Nothing
    End Function
End Class