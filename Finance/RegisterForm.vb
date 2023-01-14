Imports System.Security.Cryptography
Imports System.Data.SqlClient
Imports System.Text
Imports System.Text.RegularExpressions

Public Class RegisterForm

    Private Sub Label3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub RegisterBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RegisterBtn.Click
        ' Get the entered username and password
        Dim username As String = txtUsername.Text
        Dim password As String = txtPassword.Text
        Dim email As String = txtEmail.Text
        Dim secretword As String = txtSecretWord.Text

        ' Create a new instance of the RNGCryptoServiceProvider
        Dim rng As New RNGCryptoServiceProvider()

        ' Create a new byte array to hold the salt
        Dim salt(15) As Byte

        ' Fill the array with a cryptographically secure random value
        rng.GetBytes(salt)

        ' Convert the salt to a string
        Dim saltString As String = BitConverter.ToString(salt).Replace("-", String.Empty)

        ' Combine the salt and entered password
        Dim saltedPassword As String = saltString & password

        ' Combine the salt and entered secretkey
        Dim saltedSecretWord As String = saltString & secretword

        ' Create a new instance of the SHA256CryptoServiceProvider
        Dim sha256 As New SHA256CryptoServiceProvider()

        ' Hash the combined salt and password
        Dim hashedSaltedPassword As String = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword)))

        ' Hash the combined salt and password
        Dim hashedSaltedSecretWord As String = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedSecretWord)))

        ' Insert the new user into the database
        InsertUserIntoDb(username, hashedSaltedPassword, saltString, email, hashedSaltedSecretWord)
    End Sub

    Private Sub InsertUserIntoDb(ByVal username As String, ByVal hashedSaltedPassword As String, ByVal salt As String, ByVal email As String, ByVal hashedSaltedSecretWord As String)
        ' Create a new connection to the database
        Dim connection As New SqlConnection("Data Source=DESKTOP-116TR10\SQLEXPRESS01;Initial Catalog=Finance;Integrated Security=True")

        ' Open the connection
        Try
            connection.Open()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try

        ' Run Function Checks(username) to check all kind of registration cases
        If Checks(username, email) = False Then
            txtEmail.Text = ""
            txtPassword.Text = ""
            txtSecretWord.Text = ""
            txtUsername.Text = ""
            Return
        End If

        Try
            ' Create a new command to insert the new user into the database
            Dim command As New SqlCommand("INSERT INTO UsersData (Username, HashedPassword, Salt, Email, SecretWord) VALUES (@username, @hashedpassword, @salt, @email, @secretword)", connection)
            command.Parameters.AddWithValue("@username", username)
            command.Parameters.AddWithValue("@hashedpassword", hashedSaltedPassword)
            command.Parameters.AddWithValue("@salt", salt)
            command.Parameters.AddWithValue("@email", email)
            command.Parameters.AddWithValue("@secretword", hashedSaltedSecretWord)

            ' Execute the command
            command.ExecuteNonQuery()
            LoginForm.Show()
            Me.Visible = False
            txtEmail.Text = ""
            txtPassword.Text = ""
            txtSecretWord.Text = ""
            txtUsername.Text = ""
            txtError.Visible = False
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try

        ' Close the connection
        connection.Close()
    End Sub
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
    Private Function EmailExists(ByVal email As String) As Boolean
        ' Create a new connection to the database
        Dim connection As New SqlConnection("Data Source=DESKTOP-116TR10\SQLEXPRESS01;Initial Catalog=Finance;Integrated Security=True")

        ' Open the connection
        Try
            connection.Open()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try

        ' Create a new command to check if the user already exists
        Dim command As New SqlCommand("SELECT COUNT(*) FROM UsersData WHERE Email = @email", connection)
        command.Parameters.AddWithValue("@email", email)

        ' Execute the command and check the result
        Dim count As Integer = CInt(command.ExecuteScalar())
        connection.Close()

        If count = 0 Then
            Return False
        Else
            Return True
        End If
    End Function
    Private Sub RegisterForm_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        End
    End Sub
    Private Function Checks(ByVal username As String, ByVal email As String) As Boolean
        If UserExists(username) Then
            ' Show an error message
            txtError.Text = "Username already exists."
            txtError.Visible = True
            Return False
        End If
        If EmailExists(email) Then
            ' Show an error message
            txtError.Text = "Email already exists."
            txtError.Visible = True
            Return False
        End If

        If txtPassword.Text.Length < 6 Then
            txtError.Text = "Password must be at least 6 characters."
            txtError.Visible = True
            Return False
        ElseIf Not Regex.IsMatch(txtPassword.Text, "[a-z]") Then
            txtError.Text = "Password must contain at least one lowercase letter."
            txtError.Visible = True
            Return False
        ElseIf Not Regex.IsMatch(txtPassword.Text, "[A-Z]") Then
            txtError.Text = "Password must contain at least one uppercase letter."
            txtError.Visible = True
            Return False
        ElseIf Not Regex.IsMatch(txtPassword.Text, "[!@#\$%^&*()]") Then
            txtError.Text = "Password must contain at least one symbol."
            txtError.Visible = True
            Return False
        ElseIf Not Regex.IsMatch(txtEmail.Text, "^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$") Then
            txtError.Text = "Invalid email address."
            txtError.Visible = True
            Return False
        End If

        If txtUsername.Text.Length < 6 Then
            txtError.Text = "Username must be at least 6 characters."
            txtError.Visible = True
            Return False
        End If



        ' if all checks are good then
        Return True

    End Function

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        LoginForm.Show()
        Me.Visible = False
        txtEmail.Text = ""
        txtError.Visible = False
        txtPassword.Text = ""
        txtSecretWord.Text = ""
        txtUsername.Text = ""
    End Sub

    Private Sub txtUsername_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtUsername.KeyDown
        If e.KeyCode = Keys.Enter Then
            RegisterBtn.PerformClick()
        End If
    End Sub

    Private Sub txtPassword_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtPassword.KeyDown
        If e.KeyCode = Keys.Enter Then
            RegisterBtn.PerformClick()
        End If
    End Sub

    Private Sub txtEmail_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtEmail.KeyDown
        If e.KeyCode = Keys.Enter Then
            RegisterBtn.PerformClick()
        End If
    End Sub

    Private Sub txtSecretWord_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtSecretWord.KeyDown
        If e.KeyCode = Keys.Enter Then
            RegisterBtn.PerformClick()
        End If
    End Sub

    Private Sub RegisterForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
End Class