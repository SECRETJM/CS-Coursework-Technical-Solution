Public Class Player
    Inherits PictureBox

    Private Const _Gravity As Decimal = 0.5

    Private _VelocityY As Decimal
    Public Property VelocityY As Decimal
        Get
            Return _VelocityY
        End Get
        Set(ByVal value As Decimal)
            _VelocityY = value
        End Set
    End Property

    Private _VelocityX As Decimal
    Public Property VelocityX As Decimal
        Get
            Return _VelocityX
        End Get
        Set(ByVal value As Decimal)
            _VelocityX = value
        End Set
    End Property

    Sub New() 'When a new player is made it has 0 velocity so it is still, and the players dimensions are set along with starting position.
        Me.Left = 200
        Me.Top = 200
        Me.Width = 50
        Me.Height = 75
        _VelocityY = 0
        _VelocityX = 0
    End Sub

    Sub Jump() 'Changes velocity to a negative value to move the player upwards in order to jump.
        _VelocityY = -10
    End Sub

    Sub UpdateVelocityY()
        _VelocityY += _Gravity
    End Sub

    Sub UpdatePosition(ByVal platforms As List(Of Platform)) 'Updates the player's position using velocity and checks for a collision with a platform.

        UpdateVelocityY()

        For Each platform As Platform In platforms
            If platform.IsTouching(Me.Top, _VelocityY, Me.Height) Then
                _VelocityY = 0
                Me.Top = platform.Top - Me.Height
            End If
        Next

        Me.Top += _VelocityY
        Me.Left += _VelocityX

    End Sub

    Sub MoveLeft()
        If _VelocityX > -10 Then
            _VelocityX -= 5
        End If
    End Sub

    Sub MoveRight()
        If _VelocityX < 10 Then
            _VelocityX += 5
        End If
    End Sub

End Class