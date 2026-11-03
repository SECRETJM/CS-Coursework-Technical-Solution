Public Class Form1
    Private WithEvents GameTimer As New Timer

    Dim MyPlayer As New Player

    Dim Enemy1 As New GroundEnemy
    Dim Enemy2 As New AirEnemy
    Dim Enemies As New List(Of Enemy)

    Dim Platforms As New List(Of Platform)
    Dim Floor As New Platform(30, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height - 100, 0)
    Dim Platform1 As New Platform(30, 200, 800, 300)

    Dim NavigationGraph As New NavGraph
    Dim PathLabel As New Label
    Dim ShowNavGraph As Boolean = True

    Dim MovingLeft As Boolean = False
    Dim MovingRight As Boolean = False
    Dim JumpsRemaining As Integer = 2


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load 'form loads, start timer at 16ms intervals, load all of the game objects (player,platforms)
        GameTimer.Interval = 16
        GameTimer.Start()
        WindowState = FormWindowState.Maximized
        KeyPreview = True

        MyPlayer.BackColor = Color.Black
        Me.Controls.Add(MyPlayer)

        Floor.BackColor = Color.Green
        Me.Controls.Add(Floor)
        Platforms.Add(Floor)

        Platform1.BackColor = Color.Blue
        Me.Controls.Add(Platform1)
        Platforms.Add(Platform1)

        NavigationGraph.Build(Platforms, Enemy1.Width, Enemy1.Height)
        If ShowNavGraph Then
            DrawNavNodes()
        End If

        Me.Controls.Add(Enemy1)
        Enemy1.SetNavigationGraph(NavigationGraph)
        Enemies.Add(Enemy1)

        Me.Controls.Add(Enemy2)
        Enemies.Add(Enemy2)

        PathLabel.AutoSize = True
        PathLabel.Left = 12
        PathLabel.Top = 12
        PathLabel.Font = New Font("Consolas", 10.0!)
        PathLabel.Text = "A* path: building..."
        Me.Controls.Add(PathLabel)
        PathLabel.BringToFront()
    End Sub

    Private Sub DrawNavNodes()
        For Each node As GraphNode In NavigationGraph.Nodes
            Dim marker As New PictureBox
            marker.Width = 8
            marker.Height = 8
            marker.BackColor = Color.Gold
            marker.Left = node.X - 4
            marker.Top = node.PlatformTop - 8
            Me.Controls.Add(marker)
        Next
    End Sub

    Private Sub GameTimer_Tick(sender As Object, e As EventArgs) Handles GameTimer.Tick 'each timer interval of 16ms, checks player movement(if A/D are pressed), checks if jumps have been used/if jumps need to be reset.

        If MovingLeft Then
            MyPlayer.MoveLeft()
        End If
        If MovingRight Then
            MyPlayer.MoveRight()
        End If
        If JumpsRemaining < 2 Then
            For Each platform As Platform In Platforms  'checks each platform for collisions.
                If platform.IsTouchingGround(MyPlayer.Top, MyPlayer.Left, MyPlayer.VelocityY, MyPlayer.Height) Then
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
        For Each platform As Platform In Platforms
            If platform.IsTouchingCeiling(MyPlayer.Top, MyPlayer.Left, MyPlayer.VelocityY, MyPlayer.Height) Then
                MyPlayer.VelocityY += 2
            End If
        Next

        MyPlayer.UpdatePosition(Platforms)
        For Each Enemy In Enemies
            Enemy.UpdatePosition(Platforms)
            Enemy.UpdateAI(MyPlayer, Platforms, NavigationGraph)
        Next

        PathLabel.Text = "A* path: " & Enemy1.DebugPathText
        PathLabel.BringToFront()
    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.Space
                If JumpsRemaining > 0 Then
                    MyPlayer.Jump()
                    JumpsRemaining -= 1
                End If
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
