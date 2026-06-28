Public Class Player
    Inherits PictureBox 'since player is most likely going to be a drawing a picturebox fits the most
    Private _PositionX As Integer
    Const _Gravity As Decimal = 0.5 'Constant value of acceleration.
    Public Property PlayerPositionX As Integer 'Player has properties of Position (both X and Y) and Size which are public to be accessible to other parts of the code
        Get
            Return _PositionX
        End Get
        Set(ByVal value As Integer)
            _PositionX = value
        End Set
    End Property
    Private _PositionY As Integer
    Public Property PlayerPositionY As Integer
        Get
            Return _PositionY
        End Get
        Set(ByVal value As Integer)
            _PositionY = value
        End Set
    End Property

    Private _Height As Integer
    Public Property PlayerHeight As Integer 'Height is needed in order to recognise if the character has hit the platform/floor
        Get
            Return _Height
        End Get
        Set(ByVal value As Integer)
            _Height = value
        End Set
    End Property

    Private _Width As Integer
    Public Property PlayerWidth As Integer
        Get
            Return _Width
        End Get
        Set(ByVal value As Integer)
            _Width = value
        End Set
    End Property

    Private _VelocityY As Decimal 'Player will move up/down, gravity accelerates this movement, speed is not a constant, velocity is needed as can be negative/positive.
    Public Property VelocityY As Decimal
        Get
            Return _VelocityY
        End Get
        Set(ByVal value As Decimal)
            _VelocityY = value
        End Set
    End Property
    Private _VelocityX As Decimal
    Public Property VelocityX() As Decimal
        Get
            Return _VelocityX
        End Get
        Set(ByVal value As Decimal)
            _VelocityX = value
        End Set
    End Property

    Sub New()

        _PositionX = 200
        _PositionY = 200
        _Width = 50
        _Height = 75
        _VelocityY = 0
        _VelocityX = 0
    End Sub


    Sub Jump() 'Sets the VelocityY to a negative value which makes the character move up.

        _VelocityY = -10

    End Sub
    Sub UpdateVelocityY()
        _VelocityY += _Gravity
    End Sub
    Sub UpdatePosition() 'Adds gravity to Y Velocity (acceleration downwards) and changes position according to the velocity.

        _PositionY += VelocityY
        _PositionX += VelocityX

    End Sub
    Sub MoveLeft() 'Decreases the X velocity to move the character left.
        If _VelocityX > -10 Then
            _VelocityX -= 5
        End If
    End Sub

    Sub MoveRight() 'Increases the X velocity to move the character right.
        If _VelocityX < 10 Then
            _VelocityX += 5
        End If
    End Sub

End Class
