Public Class Form1
    Private WithEvents Timer1 As New Timer
    Dim MyPlayer As New Player
    Dim Floor As New Platform


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load 'On load, timer starts, new player and platform are instantiated and added to the form.
        Timer1.Interval = 16
        Timer1.Start()

        WindowState = FormWindowState.Maximized

        With MyPlayer
            .BackColor = Color.Black
            .Width = MyPlayer.PlayerWidth()
            .Height = MyPlayer.PlayerHeight()
            .Top = MyPlayer.PlayerPositionY()
            .Left = MyPlayer.PlayerPositionX()
        End With
        Me.Controls.Add(MyPlayer)

        With Floor
            .BackColor = Color.Green
            .Width = Floor.PlatformWidth()
            .Height = Floor.PlatformHeight()
            .Top = Floor.PlatformPositionY
            .Left = Floor.PlatformPositionX
        End With
        Me.Controls.Add(Floor)

    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick 'Every tick the players position (Y) is updated when it isn't touching the floor.

        MyPlayer.UpdatePosition()

        With MyPlayer
            .BackColor = Color.Black
            .Width = MyPlayer.PlayerWidth()
            .Height = MyPlayer.PlayerHeight()
            .Top = MyPlayer.PlayerPositionY()
            .Left = MyPlayer.PlayerPositionX()
        End With
        If Not Floor.IsTouching(MyPlayer.PlayerPositionY, MyPlayer.VelocityY, MyPlayer.PlayerHeight) Then
            MyPlayer.UpdateVelocityY()
        Else
            MyPlayer.UpdatePosition()
            MyPlayer.VelocityY = 0
        End If
        If MyPlayer.VelocityX <> 0 Then
            MyPlayer.UpdatePosition()
        End If




    End Sub
    Private Sub Jump(sender As Object, e As KeyEventArgs) Handles Me.KeyDown 'Allows the player to jump when the space bar is pressed if the player is touching the floor.
        If Floor.IsTouching(MyPlayer.PlayerPositionY, MyPlayer.VelocityY, MyPlayer.PlayerHeight) And e.KeyCode = Keys.Space Then
            MyPlayer.Jump()
        End If
    End Sub

    Private Sub MoveLeft(sender As Object, e As KeyEventArgs) Handles Me.KeyDown 'Moves the player left when the A key is pressed.
        If e.KeyCode = Keys.A Then
            MyPlayer.MoveLeft()
        End If
    End Sub

    Private Sub MoveRight(sender As Object, e As KeyEventArgs) Handles Me.KeyDown 'Moves the player right when the D key is pressed.        
        If e.KeyCode = Keys.D Then
            MyPlayer.MoveRight()
        End If
    End Sub

End Class
