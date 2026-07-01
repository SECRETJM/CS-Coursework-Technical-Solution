Public Class Platform
    Inherits PictureBox 'Inheriting from picturebox gives built in properties like width, height, top, left and allows the platform to be drawn on the form.

    Public Sub New()
        Me.Height = 30
        Me.Width = Screen.PrimaryScreen.Bounds.Width
        Me.Top = Screen.PrimaryScreen.Bounds.Height - 100
        Me.Left = 0
        Me.BackColor = Color.Green
    End Sub

    Public Function IsTouching(ByVal PlayerY As Integer, ByVal PlayerVelocityY As Decimal, ByVal PlayerHeight As Integer) As Boolean
        If PlayerY + PlayerVelocityY >= Me.Top - PlayerHeight Then
            Return True
        Else
            Return False
        End If
    End Function

End Class