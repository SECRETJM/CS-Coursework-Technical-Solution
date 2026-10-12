Public Class Platform
    Inherits PictureBox 'Inheriting from picturebox gives built in properties like width, height, top, left and allows the platform to be drawn on the form.

    Public Sub New(ByVal height As Integer, ByVal width As Integer, ByVal top As Integer, ByVal left As Integer)
        Me.Height = height
        Me.Width = width
        Me.Top = top
        Me.Left = left
        Me.BackColor = Color.Green
    End Sub

    Public Function IsTouchingGround(ByVal PlayerY As Integer, ByVal PlayerX As Decimal, ByVal PlayerVelocityY As Decimal, ByVal PlayerHeight As Integer) As Boolean
        If PlayerY + PlayerVelocityY >= Me.Top - PlayerHeight And PlayerX + 25 > Me.Left And PlayerX + 25 < Me.Left + Me.Width And Me.Top - Me.Height > PlayerY + PlayerVelocityY Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function IsTouchingCeiling(ByVal PlayerY As Integer, ByVal PlayerX As Decimal, ByVal PlayerVelocityY As Decimal, ByVal PlayerHeight As Integer) As Boolean
        If PlayerY + PlayerHeight > Me.Top And PlayerY + PlayerHeight < Me.Top + Me.Height And PlayerX + 50 > Me.Left And PlayerX < Me.Left + Me.Width Then
            Return True
        Else
            Return False
        End If
    End Function
End Class