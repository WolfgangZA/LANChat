Imports System.Threading
Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Text

Public Class Form1
    Private Const listenPort As Integer = 11000
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If TextBox1.Text = "" Then
            MsgBox("Your Name cannot be empty.")
        ElseIf Button1.Text = "Connect" Then
            IO.File.WriteAllText(Application.StartupPath + "\settings\name.txt", TextBox1.Text)
            Button1.Text = "Disconnect"
            TextBox1.Enabled = False
            Send.Enabled = True

            BackgroundWorker1.RunWorkerAsync()
            Label1.Hide()
            TextBox1.Hide()
            Button1.Hide()
            RichTextBox1.Show()
            Send.Show()
            TextBox2.Show()
            TextBox2.Select()
        ElseIf Button1.Text = "Disconnect" Then
            Button1.Text = "Connect"

            TextBox1.Enabled = True
            Send.Enabled = False
            BackgroundWorker1.CancelAsync()
        End If
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick

    End Sub

    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim done As Boolean = False
        Dim listener As New UdpClient(listenPort)
        Dim groupEP As New IPEndPoint(IPAddress.Any, listenPort)
        RichTextBox1.AppendText("Waiting for broadcast" & Environment.NewLine)

        'sends connection message
        Dim udpClient As New UdpClient()
        udpClient.Connect("255.255.255.255", 11000)
        Dim senddata As Byte()
        senddata = Encoding.ASCII.GetBytes("!" + TextBox1.Text + " has connected to the network!")
        udpClient.Send(senddata, senddata.Length)

        'continues to listen for packets
        Try

            While Not done
                'Handels Recieved packets
                '! = Server Messsage
                '* = Normal Chat Message
                '@ = How many chat clients are there on the network? (Updates Label)

                Dim bytes As Byte() = listener.Receive(groupEP)
                'RichTextBox1.AppendText(groupEP.ToString())
                RichTextBox1.AppendText(Environment.NewLine)

                Dim msg As String = Encoding.ASCII.GetString(bytes, 0, bytes.Length)
                Dim leftpart As String = Split(msg, ":")(0)
                Dim rightpart As String = Split(msg, ":")(0)

                Dim s As String = msg
                Dim answer As Char
                answer = s.Substring(0, 1)

                If answer = "!" Then
                    Dim Servermsg As String = Split(msg, "!")(1)
                    'RichTextBox1.AppendText(Servermsg)
                    RichTextBox1.SelectionColor = Color.Red
                    RichTextBox1.SelectedText = Servermsg

                ElseIf answer = "*" Then
                    Dim chatmsg As String = Split(msg, "*")(1)
                    RichTextBox1.AppendText(chatmsg)
                ElseIf answer = "@" Then


                Else
                    RichTextBox1.SelectionColor = Color.Red
                    RichTextBox1.SelectedText = "An unkown packet was recieved from: " + groupEP.ToString
                End If

               


                'RichTextBox1.AppendText(msg)
                'RichTextBox1.SelectionColor = Color.Red
                'RichTextBox1.SelectedText = "Hello "
                'RichTextBox1.SelectionColor = Color.Green
                'RichTextBox1.SelectedText = "World"

            End While
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            listener.Close()
        End Try
    End Sub


    Private Sub Send_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Send.Click
        If TextBox2.Text = "" Then
            MsgBox("Message cannot be empty")
        Else
            Dim udpClient As New UdpClient()
            udpClient.Connect("255.255.255.255", 11000)
            Dim senddata As Byte()

            senddata = Encoding.ASCII.GetBytes("*" + TextBox1.Text + ": " + TextBox2.Text)
            udpClient.Send(senddata, senddata.Length)
            TextBox2.Clear()
        End If

    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Dim errors As String = DateTime.Now
        Dim correction As String = errors.Replace(" ", "@")
        Dim coracao As String = correction.Replace("/", "-")
        Dim final As String = coracao.Replace(":", "-")
        'IO.File.Create(Application.StartupPath + "\logs\" + correction + ".txt")
        If RichTextBox1.Visible = False Then

        Else
            System.IO.File.AppendAllText(Application.StartupPath + "\logs\" + final + ".rtf", RichTextBox1.Text)
        End If

    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If IO.File.Exists(Application.StartupPath + "\settings\name.txt") Then
            TextBox1.Text = System.IO.File.ReadAllText(Application.StartupPath + "\settings\name.txt")
        Else
            IO.File.Create(Application.StartupPath + "\settings\name.txt")
        End If


        CheckForIllegalCrossThreadCalls = False
        Send.Enabled = False
        RichTextBox1.ReadOnly = True
        RichTextBox1.BackColor = Color.White
        RichTextBox1.Hide()
        TextBox2.Hide()
        Send.Hide()


        'testing
        Dim errors As String = DateTime.Now
        Dim correction As String = errors.Replace(" ", "@")
        errors.Replace(" ", "@")
        'MsgBox(Application.StartupPath + "\logs\" + correction + ".txt")
    End Sub

    Private Sub TextBox2_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox2.KeyPress

    End Sub

    Private Sub TextBox2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox2.TextChanged
        Me.AcceptButton = Send

    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
        Me.AcceptButton = Button1
    End Sub

End Class
