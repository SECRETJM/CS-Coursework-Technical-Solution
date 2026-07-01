Public Class Player
    Inherits PictureBox

    Private Const _Gravity As Decimal = 0.5
    Private Const _MaxGroundVelocityX As Decimal = 10
    Private Const _MaxAirVelocityX As Decimal = 5

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

    Public Sub New() 'When a new player is made it has 0 velocity so it is still, and the players dimensions are set along with starting position.
        Me.Left = 200
        Me.Top = 200
        Me.Width = 50
        Me.Height = 75
        _VelocityY = 0
        _VelocityX = 0
    End Sub

    Public Sub Jump() 'Changes velocity to a negative value to move the player upwards in order to jump.
        _VelocityY = -10
    End Sub

    Private Sub UpdateVelocityY()
        _VelocityY += _Gravity
    End Sub

    Public Sub UpdatePosition(ByVal platforms As List(Of Platform)) 'Updates the player's position using velocity and checks for a collision with a platform.

        UpdateVelocityY()

        For Each platform As Platform In platforms
            If platform.IsTouching(Me.Top, _VelocityY, Me.Height) Then
                _VelocityY = 0
                Me.Top = platform.Top - Me.Height
            End If
        Next

        Me.Top += _VelocityY
        Me.Left += _VelocityX

        For Each platform In platforms
            If platform.IsTouching(Me.Top, _VelocityY, Me.Height) Then
                If _VelocityX > 0 Then
                    _VelocityX -= 1
                ElseIf _VelocityX < 0 Then
                    _VelocityX += 1
                End If
            End If
        Next

    End Sub

    Public Sub MoveLeft()
        If _VelocityX > -(_MaxGroundVelocityX) Then
            _VelocityX -= 5
        End If
    End Sub

    Public Sub MoveRight()
        If _VelocityX < _MaxGroundVelocityX Then
            _VelocityX += 5
        End If
    End Sub

End Class