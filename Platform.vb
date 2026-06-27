Public Class Platform
    Inherits PictureBox

    Private _PositionX As Integer
    Public Property PlatformPositionX As Integer 'Platform position is used in recognising if anything is in contact with it.
        Get
            Return _PositionX
        End Get
        Set(ByVal value As Integer)
            _PositionX = value
        End Set
    End Property
    Private _PositionY As Integer
    Public Property PlatformPositionY As Integer
        Get
            Return _PositionY
        End Get
        Set(ByVal value As Integer)
            _PositionY = value
        End Set
    End Property

    Private _Height As Integer
    Public Property PlatformHeight As Integer 'Height is needed in order to recognise if the bottom of the player has hit the top of the platform/floor.
        Get
            Return _Height
        End Get
        Set(ByVal value As Integer)
            _Height = value
        End Set
    End Property

    Private _Width As Integer
    Public Property PlatformWidth As Integer
        Get
            Return _Width
        End Get
        Set(ByVal value As Integer)
            _Width = value
        End Set
    End Property

    Sub New()

        _Height = 30
        _Width = Screen.PrimaryScreen.Bounds.Width
        _PositionY = Screen.PrimaryScreen.Bounds.Height - 100
        _PositionX = 0

    End Sub

    Function IsTouching(ByRef PlayerY, ByRef PlayerVelocityY, ByVal PlayerHeight)

        If PlayerY + PlayerVelocityY >= _PositionY - PlayerHeight Then
            PlayerVelocityY = 0
            Return True
        Else
            Return False
        End If




    End Function



End Class
