Imports System.Security.Cryptography
Imports System.Data.SqlClient
Imports System.Text
Imports System.IO
Imports System.Text.RegularExpressions

Public Class ForgotPassForm

    Private Sub ForgotPassForm_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        End
    End Sub

    Private Sub SubmitBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SubmitBtn.Click
        ' Get the entered username and password
        Dim username As String = txtUsername.Text
        Dim secretword As String = txtSecretWord.Text
        Dim email As String = txtEmail.Text

        If UserExists(username) Then
            'Valid Username
        Else
            txtError.Text = "Username Doesn't exists."
            txtError.Visible = True
            Return
        End If

        If Not Regex.IsMatch(txtEmail.Text, "^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$") Then
            txtError.Text = "Invalid email address."
            txtError.Visible = True
            Return
        End If

        ' Create a new instance of the SHA256CryptoServiceProvider
        Dim sha256 As New SHA256CryptoServiceProvider()

        ' Retrieve the user's salt and hashed password from the database
        Dim salt As String = GetSaltFromDb(username)
        Dim hasedsecretkey As String = GethasedsecretkeyFromDb(username)
        Dim emailDB As String = GetEmailFromDb(username)

        ' Combine the salt and entered password
        Dim saltedSecretKey As String = salt & secretword

        ' Hash the combined salt and password
        Dim hashedSaltedSecretKey As String = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedSecretKey)))

        ' Check if the hashed and salted password matches the one in the database
        If hashedSaltedSecretKey = hasedsecretkey And email = emailDB Then
            ' successful
            ResetPass.Show()
            txtError.Visible = False
            Me.Visible = False
            txtEmail.Text = ""
            txtSecretWord.Text = ""
        Else
            ' failed
            txtEmail.Text = ""
            txtSecretWord.Text = ""
            txtUsername.Text = ""
            txtError.Text = "Invalid details."
            txtError.Visible = True
        End If
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
    Private Function GethasedsecretkeyFromDb(ByVal username As String) As String
        'Create Connection
        Dim connection As New SqlConnection("Data Source=DESKTOP-116TR10\SQLEXPRESS01;Initial Catalog=Finance;Integrated Security=True")

        'Open Connection
        Try
            connection.Open()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try

        'Create Command
        Dim command As New SqlCommand("SELECT SecretWord FROM UsersData WHERE username = @username", connection)
        command.Parameters.AddWithValue("@username", username)

        'Execute Command
        Dim reader As SqlDataReader = command.ExecuteReader()

        'get hashed password from database
        reader.Read()
        Dim hasedsecretkey As String = reader("SecretWord").ToString()

        'Close Connection
        connection.Close()

        'return the hashed password
        Return hasedsecretkey
    End Function
    Private Function GetEmailFromDb(ByVal username As String) As String
        'Create Connection
        Dim connection As New SqlConnection("Data Source=DESKTOP-116TR10\SQLEXPRESS01;Initial Catalog=Finance;Integrated Security=True")

        'Open Connection
        Try
            connection.Open()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try

        'Create Command
        Dim command As New SqlCommand("SELECT Email FROM UsersData WHERE username = @username", connection)
        command.Parameters.AddWithValue("@username", username)

        'Execute Command
        Dim reader As SqlDataReader = command.ExecuteReader()

        'get hashed password from database
        reader.Read()
        Dim hasedsecretkey As String = reader("Email").ToString()

        'Close Connection
        connection.Close()

        'return the hashed password
        Return hasedsecretkey
    End Function
    Private Function UserExists(ByVal username As String) As Boolean
        ' Create a new connection to the database
        Dim connection As New SqlConnection("Data Source=DESKTOP-116TR10\SQLEXPRESS01;Initial Catalog=Finance;Integrated Security=True")

        ' Open the connection
        Try
            connection.Open()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try

        ' Create a new command to check if the user already exists
        Dim command As New SqlCommand("SELECT COUNT(*) FROM UsersData WHERE Username = @username", connection)
        command.Parameters.AddWithValue("@username", username)

        ' Execute the command and check the result
        Dim count As Integer = CInt(command.ExecuteScalar())
        connection.Close()

        If count = 0 Then
            Return False
        Else
            Return True
        End If
    End Function

    Private Sub txtUsername_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtUsername.KeyDown
        ' If Enter key is pressed inside textbox press LoginBtn
        If e.KeyCode = Keys.Enter Then
            SubmitBtn.PerformClick()
        End If
    End Sub

    Private Sub txtEmail_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtEmail.KeyDown
        If e.KeyCode = Keys.Enter Then
            SubmitBtn.PerformClick()
        End If
    End Sub

    Private Sub txtSecretWord_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtSecretWord.KeyDown
        If e.KeyCode = Keys.Enter Then
            SubmitBtn.PerformClick()
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        LoginForm.Show()
        Me.Visible = False
        txtError.Visible = False
        txtEmail.Text = ""
        txtSecretWord.Text = ""
        txtUsername.Text = ""
    End Sub
End Class