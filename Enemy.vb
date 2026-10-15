Public Class Enemy
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
    Sub New()
        Me.Width = 50
        Me.Height = 75
    End Sub
    Private Sub UpdateVelocityY()
        _VelocityY += _Gravity
    End Sub
    Public Sub UpdatePosition(ByVal platforms As List(Of Platform))
        UpdateVelocityY()
        For Each platform As Platform In platforms
            If platform.IsTouchingGround(Me.Top, Me.Left, _VelocityY, Me.Height) And _VelocityY > 0 Then
                _VelocityY = 0
                Me.Top = platform.Top - Me.Height
            End If
        Next
        Me.Top += _VelocityY
        Me.Left += _VelocityX * 0.8
    End Sub

    Public Overridable Sub CheckDirection(ByVal PlayerX As Integer)
        If Me.Left > PlayerX Then
            _VelocityX = -5
        ElseIf Me.Left < PlayerX Then
            _VelocityX = 5
        End If
    End Sub

    Public Overridable Function InAttackRange(ByVal PlayerX As Integer, ByVal PlayerY As Integer) As Boolean
        Return False
    End Function

End Class

Class GroundEnemy
    Inherits Enemy
    Private Const _Gravity As Decimal = 0.5
    Private Const _MaxGroundVelocityX As Decimal = 10
    Private Const _MaxAirVelocityX As Decimal = 5
    Private Const _FrictionConst As Decimal = 0.8
    Private _Range As Integer = 50
    Public Sub New()
        MyBase.New()
        Me.BackColor = Color.Red
        Me.Left = 400
        Me.Top = 200
    End Sub

    Public Overrides Function InAttackRange(PlayerX As Integer, PlayerY As Integer) As Boolean
        MyBase.InAttackRange(PlayerX, PlayerY)
        If Me.VelocityX > 0 AndAlso Me.Left + Me.Width + _Range >= PlayerX Then
            Return True
        ElseIf Me.VelocityX < 0 AndAlso Me.Left - _Range <= PlayerX Then
            Return True
        Else
            Return False
        End If

    End Function
End Class