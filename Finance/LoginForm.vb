Imports System.Security.Cryptography
Imports System.Data.SqlClient
Imports System.Text
Imports System.IO
Public Class LoginForm

    Private Sub LoginBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoginBtn.Click
        ' Get the entered username and password
        Dim username As String = txtUsername.Text
        Dim password As String = txtPassword.Text

        ' Create a new instance of the SHA256CryptoServiceProvider
        Dim sha256 As New SHA256CryptoServiceProvider()

        ' Retrieve the user's salt and hashed password from the database
        Dim salt As String = GetSaltFromDb(username)
        Dim hashedPassword As String = GetHashedPasswordFromDb(username)

        ' Combine the salt and entered password
        Dim saltedPassword As String = salt & password

        ' Hash the combined salt and password
        Dim hashedSaltedPassword As String = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword)))

        ' Check if the hashed and salted password matches the one in the database
        If hashedSaltedPassword = hashedPassword Then
            ' Login successful
            MessageBox.Show("Login successful!")
            txtUsername.Text = ""
            txtPassword.Text = ""
        Else
            ' Login failed
            MessageBox.Show("Invalid username or password.")
            txtUsername.Text = ""
            txtPassword.Text = ""
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

    Private Sub Label1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        RegisterForm.Show()
        Me.Visible = False
    End Sub

    Private Sub LoginForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

    Private Sub LinkLabel2_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        ForgotPassForm.Show()
        Me.Visible = False
    End Sub

    Private Sub txtUsername_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtUsername.KeyDown
        ' If Enter key is pressed inside textbox press LoginBtn
        If e.KeyCode = Keys.Enter Then
            LoginBtn.PerformClick()
        End If
    End Sub

    Private Sub txtPassword_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtPassword.KeyDown
        ' If Enter key is pressed inside textbox press LoginBtn
        If e.KeyCode = Keys.Enter Then
            LoginBtn.PerformClick()
        End If
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub
End Class
