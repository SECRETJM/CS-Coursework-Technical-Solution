Public Class Form1
    Private WithEvents Timer1 As New Timer
    Dim MyPlayer As New Player
    Dim Platforms As New List(Of Platform)
    Dim Floor As New Platform
    Dim MoveLeft As Boolean = False
    Dim MoveRight As Boolean = False


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Timer1.Interval = 16
        Timer1.Start()

        WindowState = FormWindowState.Maximized

        MyPlayer.BackColor = Color.Black
        Me.Controls.Add(MyPlayer)

        Floor.BackColor = Color.Green
        Me.Controls.Add(Floor)
        Platforms.Add(Floor)


    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        If MoveLeft Then
            MyPlayer.MoveLeft()
        End If
        If MoveRight Then
            MyPlayer.MoveRight()
        End If

        MyPlayer.UpdatePosition(Platforms)
    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.Space
                For Each platform As Platform In Platforms
                    If platform.IsTouching(MyPlayer.Top, MyPlayer.VelocityY, MyPlayer.Height) Then
                        MyPlayer.Jump()
                        Exit For
                    End If
                Next
            Case Keys.A
                MoveLeft = True
            Case Keys.D
                MoveRight = True
        End Select
    End Sub
    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        Select Case e.KeyCode
            Case Keys.A
                MoveLeft = False
            Case Keys.D
                MoveRight = False
        End Select
    End Sub

End Class