Imports System.Security.Cryptography
Imports System.Data.SqlClient
Imports System.Text
Imports System.IO
Imports System.Text.RegularExpressions
Public Class ResetPass

    Private Sub ResetPass_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        End
    End Sub

    Private Sub txtResetPass_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtResetPass.KeyDown
        If e.KeyCode = Keys.Enter Then
            SubmitBtn.PerformClick()
        End If
    End Sub

    Private Sub txtResetPassConfirm_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtResetPassConfirm.KeyDown
        If e.KeyCode = Keys.Enter Then
            SubmitBtn.PerformClick()
        End If
    End Sub

    Private Sub SubmitBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SubmitBtn.Click
        'Create Connection
        Dim connection As New SqlConnection("Data Source=DESKTOP-116TR10\SQLEXPRESS01;Initial Catalog=Finance;Integrated Security=True")
        Dim username As String = ForgotPassForm.txtUsername.Text
        Dim password As String = txtResetPass.Text
        Dim salt As String = GetSaltFromDb(username)
        Dim saltedPassword As String = salt & password
        ' Create a new instance of the SHA256CryptoServiceProvider
        Dim sha256 As New SHA256CryptoServiceProvider()
        ' Get Current Password to compare
        Dim hashedPassword As String = GetHashedPasswordFromDb(username)
        ' Hash the combined salt and password
        Dim hashedSaltedPassword As String = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword)))
        'IF

        If (txtResetPass.Text = txtResetPassConfirm.Text) Then
            ' Passwords Match
        Else
            ' Passwords Dont Match
            txtError.Text = "Passwords dont match"
            txtError.Visible = True
            txtResetPass.Text = ""
            txtResetPassConfirm.Text = ""
            Return
        End If

        If Checks(username) = False Then
            txtResetPass.Text = ""
            txtResetPassConfirm.Text = ""
            Return
        End If

        If hashedSaltedPassword = hashedPassword Then
            txtError.Text = "New Password can't be as the previous Password"
            txtError.Visible = True
            txtResetPass.Text = ""
            txtResetPassConfirm.Text = ""
            Return
        End If
            'Open Connection
            Try
                connection.Open()
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Critical)
            End Try
        Try
            'Create Command
            Dim command As New SqlCommand("Update UsersData SET HashedPassword = @hashsaltedpassword WHERE username = @username", connection)
            command.Parameters.AddWithValue("@username", username)
            command.Parameters.AddWithValue("@hashsaltedpassword", hashedSaltedPassword)
            'Execute Command
            Dim reader As SqlDataReader = command.ExecuteReader()
            ForgotPassForm.txtUsername.Text = ""
            ForgotPassForm.txtSecretWord.Text = ""
            ForgotPassForm.txtEmail.Text = ""
            txtResetPass.Text = ""
            txtResetPassConfirm.Text = ""
            txtError.Visible = False
            LoginForm.Show()
            Me.Visible = False
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        'Close Connection
        connection.Close()
    End Sub

    Private Function GetSaltFromDb(ByVal username As String) As String
        'Create Connection
        Dim connection As New SqlConnection("Data Source=DESKTOP-116TR10\SQLEXPRESS01;Initial Catalog=Finance;Integrated Security=True")

        'Open Connection
        Try
            connection.Open()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try

        'Create Command
        Dim command As New SqlCommand("SELECT salt FROM UsersData WHERE username = @username", connection)
        command.Parameters.AddWithValue("@username", username)

        'Execute Command
        Dim reader As SqlDataReader = command.ExecuteReader()

        'get salt from database
        reader.Read()
        Dim salt As String = reader("salt").ToString()

        'Close Connection
        connection.Close()

        'return the salt
        Return salt
    End Function
    Private Function Checks(ByVal username As String) As Boolean

        If txtResetPass.Text.Length < 6 Then
            txtError.Text = "Password must be at least 6 characters."
            txtError.Visible = True
            Return False
        ElseIf Not Regex.IsMatch(txtResetPass.Text, "[a-z]") Then
            txtError.Text = "Password must contain at least one lowercase letter."
            txtError.Visible = True
            Return False
        ElseIf Not Regex.IsMatch(txtResetPass.Text, "[A-Z]") Then
            txtError.Text = "Password must contain at least one uppercase letter."
            txtError.Visible = True
            Return False
        ElseIf Not Regex.IsMatch(txtResetPass.Text, "[!@#\$%^&*()]") Then
            txtError.Text = "Password must contain at least one symbol."
            txtError.Visible = True
            Return False
        End If

        ' If all checks are valid return true
        Return True
    End Function
    Private Function GetHashedPasswordFromDb(ByVal username As String) As String
        'Create Connection
        Dim connection As New SqlConnection("Data Source=DESKTOP-116TR10\SQLEXPRESS01;Initial Catalog=Finance;Integrated Security=True")

        'Open Connection
        Try
            connection.Open()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try

        'Create Command
        Dim command As New SqlCommand("SELECT HashedPassword FROM UsersData WHERE username = @username", connection)
        command.Parameters.AddWithValue("@username", username)

        'Execute Command
        Dim reader As SqlDataReader = command.ExecuteReader()

        'get hashed password from database
        reader.Read()
        Dim hashedPassword As String = reader("HashedPassword").ToString()

        'Close Connection
        connection.Close()

        'return the hashed password
        Return hashedPassword
    End Function

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        ForgotPassForm.txtUsername.Text = ""
        ForgotPassForm.txtSecretWord.Text = ""
        ForgotPassForm.txtEmail.Text = ""
        txtResetPass.Text = ""
        txtResetPassConfirm.Text = ""
        txtError.Visible = False
        LoginForm.Show()
        Me.Visible = False
    End Sub
End Class