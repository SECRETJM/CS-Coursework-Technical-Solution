Public Class Enemy
    Inherits PictureBox
    Private Const _Gravity As Decimal = 0.5

    Protected _VelocityY As Decimal
    Public Property VelocityY As Decimal
        Get
            Return _VelocityY
        End Get
        Set(ByVal value As Decimal)
            _VelocityY = value
        End Set
    End Property

    Protected _VelocityX As Decimal
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
    Protected Overridable Sub UpdateVelocityY()
        _VelocityY += _Gravity
    End Sub
    Public Overridable Sub UpdatePosition(ByVal platforms As List(Of Platform))
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

    Public Overridable Sub Jump()
        _VelocityY = -10
    End Sub

    Public Overridable Sub CheckXDirection(ByVal PlayerX As Integer)
        If Me.Left > PlayerX Then
            While _VelocityX > -5
                _VelocityX -= 0.5
            End While
        ElseIf Me.Left < PlayerX Then
            While _VelocityX < 5
                _VelocityX += 0.5
            End While
        End If
    End Sub

    Public Overridable Function InAttackRange(ByVal PlayerX As Integer, ByVal PlayerY As Integer) As Boolean
        Return False
    End Function

    ' Default AI: move towards the player on the X axis.
    ' GroundEnemy overrides this to follow an A* path on the navigation graph.
    Public Overridable Sub UpdateAI(ByVal target As Player, ByVal platforms As List(Of Platform), ByVal graph As NavGraph)
        CheckXDirection(target.Left)
    End Sub
End Class

Class GroundEnemy
    Inherits Enemy
    Private Const _Gravity As Decimal = 0.5
    Private Const _MaxGroundVelocityX As Decimal = 10
    Private Const _MaxAirVelocityX As Decimal = 5
    Private Const _FrictionConst As Decimal = 0.8
    Private ReadOnly _Range As Integer = 50

    Private _Graph As NavGraph
    Private _Pathfinder As New AStarPathfinder
    Private _Path As PathResult
    Private _TargetIndex As Integer
    Private _HasJumped As Boolean
    Private _TicksUntilRecalc As Integer
    Private _LastGoalId As Integer = -1

    Public Property DebugPathText As String = "No path"

    Public Sub New()
        MyBase.New()
        Me.BackColor = Color.Red
        Me.Left = 400
        Me.Top = 200
        _Path = New PathResult
    End Sub

    Public Sub SetNavigationGraph(ByVal graph As NavGraph)
        _Graph = graph
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

    Public Overrides Sub UpdateAI(ByVal target As Player, ByVal platforms As List(Of Platform), ByVal graph As NavGraph)
        If graph IsNot Nothing Then
            _Graph = graph
        End If
        If _Graph Is Nothing OrElse _Graph.Nodes.Count = 0 Then
            CheckXDirection(target.Left)
            Return
        End If

        If IsStandingOnPlatform(platforms) Then
            _TicksUntilRecalc -= 1
            Dim goalNode As GraphNode = _Graph.FindNearestNode(target.Left + (target.Width \ 2), target.Top, target.Height)
            Dim goalChanged As Boolean = goalNode IsNot Nothing AndAlso goalNode.Id <> _LastGoalId
            If _TicksUntilRecalc <= 0 OrElse goalChanged OrElse Not _Path.Found Then
                RecalculatePath(target)
                _TicksUntilRecalc = 20
            End If
        End If

        FollowPath(platforms, target)
    End Sub

    Private Sub RecalculatePath(ByVal target As Player)
        Dim startNode As GraphNode = _Graph.FindNearestNode(Me.Left + (Me.Width \ 2), Me.Top, Me.Height)
        Dim goalNode As GraphNode = _Graph.FindNearestNode(target.Left + (target.Width \ 2), target.Top, target.Height)

        If startNode Is Nothing OrElse goalNode Is Nothing Then
            _Path = New PathResult
            DebugPathText = "No path"
            Return
        End If

        _Path = _Pathfinder.FindPath(_Graph, startNode, goalNode)
        _TargetIndex = 1
        _HasJumped = False
        If goalNode IsNot Nothing Then
            _LastGoalId = goalNode.Id
        End If
        DebugPathText = _Path.ToDebugString()
    End Sub

    Private Sub FollowPath(ByVal platforms As List(Of Platform), ByVal target As Player)
        If Not _Path.Found OrElse _Path.Nodes.Count <= 1 OrElse _TargetIndex >= _Path.Nodes.Count Then
            If Math.Abs((Me.Left + (Me.Width \ 2)) - (target.Left + (target.Width \ 2))) < 40 Then
                VelocityX = 0
            Else
                CheckXDirection(target.Left)
            End If
            Return
        End If

        Dim dest As GraphNode = _Path.Nodes(_TargetIndex)
        Dim action As String = _Path.Actions(_TargetIndex - 1)
        Dim origin As GraphNode = _Path.Nodes(_TargetIndex - 1)
        Dim centerX As Integer = Me.Left + (Me.Width \ 2)
        Dim grounded As Boolean = IsStandingOnPlatform(platforms)

        If action = "Jump" AndAlso Not _HasJumped AndAlso grounded Then
            If Math.Abs(centerX - origin.X) <= 45 Then
                Jump()
                _HasJumped = True
            End If
        End If

        ' CheckXDirection compares Me.Left (left edge) with the target X.
        ' Node X is a standing centre, so dest.X + 50 can still be to the right of
        ' Me.Left even when the next node is to the left. Pass a target that is
        ' definitely on the side we need to move.
        If centerX < dest.X - 6 Then
            CheckXDirection(Me.Left + 200)
        ElseIf centerX > dest.X + 6 Then
            CheckXDirection(Me.Left - 200)
        ElseIf action = "Walk" Then
            VelocityX = 0
        End If

        If Math.Abs(centerX - dest.X) <= 28 Then
            Dim jumpLanded As Boolean = (action <> "Jump" AndAlso action <> "Drop") OrElse grounded
            If jumpLanded Then
                _TargetIndex += 1
                _HasJumped = False
            End If
        End If
    End Sub

    Private Function IsStandingOnPlatform(ByVal platforms As List(Of Platform)) As Boolean
        For Each platform As Platform In platforms
            Dim feet As Integer = Me.Top + Me.Height
            If Math.Abs(feet - platform.Top) <= 4 AndAlso Me.Left + 25 > platform.Left AndAlso Me.Left + 25 < platform.Left + platform.Width Then
                Return True
            End If
        Next
        Return False
    End Function
End Class
Class AirEnemy
    Inherits Enemy
    Private Const _Gravity As Decimal = 0.5
    Private Const _MaxGroundVelocityX As Decimal = 10
    Private Const _MaxAirVelocityX As Decimal = 5
    Private Const _FrictionConst As Decimal = 0.8
    Private ReadOnly _Range As Integer = 5
    Public Sub New()
        MyBase.New()
        Me.BackColor = Color.Purple
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
    Public Overrides Sub CheckXDirection(PlayerX As Integer)
        MyBase.CheckXDirection(PlayerX)
        UpdateVelocityY()
    End Sub
    Protected Overrides Sub UpdateVelocityY()
        If _VelocityY > 5 Then
            _VelocityY += 2
        ElseIf _VelocityY < -5 Then
            _VelocityY -= 2
        End If
    End Sub
    Public Overrides Sub UpdatePosition(platforms As List(Of Platform))
        MyBase.UpdatePosition(platforms)
        Me.Top += _VelocityY
    End Sub
End Class
