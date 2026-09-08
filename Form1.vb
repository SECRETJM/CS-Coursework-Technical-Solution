Public Class Form1
    Private WithEvents Timer1 As New Timer
    Dim MyPlayer As New Player
    Dim Platforms As New List(Of Platform)
    Dim Floor As New Platform(30, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height - 100, 0)
    Dim Platform1 As New Platform(30, 200, 800, 300)
    Dim MovingLeft As Boolean = False
    Dim MovingRight As Boolean = False
    Dim JumpsRemaining As Integer = 2


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load 'form loads, start timer at 16ms intervals, load all of the game objects (player,platforms)
        Timer1.Interval = 16
        Timer1.Start()
        WindowState = FormWindowState.Maximized

        MyPlayer.BackColor = Color.Black
        Me.Controls.Add(MyPlayer)

        Floor.BackColor = Color.Green
        Me.Controls.Add(Floor)
        Platforms.Add(Floor)

        Platform1.BackColor = Color.Blue
        Me.Controls.Add(Platform1)
        Platforms.Add(Platform1)

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick 'each timer interval of 16ms, checks player movement(if A/D are pressed), checks if jumps have been used/if jumps need to be reset.

        If MovingLeft Then
            MyPlayer.MoveLeft()
        End If
        If MovingRight Then
            MyPlayer.MoveRight()
        End If
        If JumpsRemaining < 2 Then
            For Each platform As Platform In Platforms
                If platform.IsTouching(MyPlayer.Top, MyPlayer.Left, MyPlayer.VelocityY, MyPlayer.Height) Then
                    JumpsRemaining = 2
                End If
            Next
        End If
        If MyPlayer.Left + MyPlayer.Width > Me.ClientSize.Width Then
            MyPlayer.Left = Me.ClientSize.Width - MyPlayer.Width
        End If
        If MyPlayer.Left < 0 Then
            MyPlayer.Left = 0
        End If


        MyPlayer.UpdatePosition(Platforms)
    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.Space
                For Each platform As Platform In Platforms 'checks each platform for collisions.
                    If platform.IsTouching(MyPlayer.Top, MyPlayer.Left, MyPlayer.VelocityY, MyPlayer.Height) Or JumpsRemaining > 0 Then
                        MyPlayer.Jump()
                        JumpsRemaining -= 1
                        Exit For
                    End If
                Next
            Case Keys.A
                MovingLeft = True
            Case Keys.D
                MovingRight = True
        End Select
    End Sub
    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        Select Case e.KeyCode
            Case Keys.A
                MovingLeft = False
            Case Keys.D
                MovingRight = False
        End Select
    End Sub

End Class