' Navigation graph for ground enemies.
' Each node is a standing position on a platform. Edges are Walk, Jump or Drop
' routes that have been checked against the same gravity/jump values the game uses.

Public Class GraphNode
    Public Property Id As Integer
    Public Property X As Integer
    Public Property PlatformTop As Integer
    Public Property PlatformIndex As Integer
    Public Property Edges As List(Of GraphEdge)

    Public Sub New(ByVal id As Integer, ByVal x As Integer, ByVal platformTop As Integer, ByVal platformIndex As Integer)
        Me.Id = id
        Me.X = x
        Me.PlatformTop = platformTop
        Me.PlatformIndex = platformIndex
        Me.Edges = New List(Of GraphEdge)
    End Sub
End Class

Public Class GraphEdge
    Public Property ToNode As GraphNode
    Public Property Cost As Decimal
    Public Property Action As String ' "Walk", "Jump" or "Drop"

    Public Sub New(ByVal toNode As GraphNode, ByVal cost As Decimal, ByVal action As String)
        Me.ToNode = toNode
        Me.Cost = cost
        Me.Action = action
    End Sub
End Class

Public Class NavGraph
    Public Property Nodes As List(Of GraphNode)

    ' These match Enemy movement: VelocityX 5, friction 0.8, gravity 0.5, jump -10.
    Private Const Gravity As Decimal = 0.5D
    Private Const JumpSpeed As Decimal = 10D
    Private Const HorizPerTick As Decimal = 4D
    Private Const NodeSpacing As Integer = 60
    Private Const MaxJumpHeight As Integer = 100

    Public Sub New()
        Nodes = New List(Of GraphNode)
    End Sub

    ' Builds walkable nodes along every platform, then connects them with walk/jump/drop edges.
    Public Sub Build(ByVal platforms As List(Of Platform), ByVal entityWidth As Integer, ByVal entityHeight As Integer)
        Nodes.Clear()

        For platformIndex As Integer = 0 To platforms.Count - 1
            AddNodesForPlatform(platforms(platformIndex), platformIndex, entityWidth)
        Next

        ConnectWalkEdges()
        ConnectJumpAndDropEdges(entityHeight)
    End Sub

    Private Sub AddNodesForPlatform(ByVal platform As Platform, ByVal platformIndex As Integer, ByVal entityWidth As Integer)
        Dim inset As Integer = Math.Max(10, entityWidth \ 4)
        Dim startX As Integer = platform.Left + inset
        Dim endX As Integer = platform.Left + platform.Width - inset

        If endX <= startX Then
            Nodes.Add(New GraphNode(Nodes.Count, platform.Left + (platform.Width \ 2), platform.Top, platformIndex))
            Return
        End If

        Dim x As Integer = startX
        Do
            Nodes.Add(New GraphNode(Nodes.Count, x, platform.Top, platformIndex))
            x += NodeSpacing
        Loop While x < endX

        Dim lastX As Integer = Nodes(Nodes.Count - 1).X
        If endX - lastX > 15 Then
            Nodes.Add(New GraphNode(Nodes.Count, endX, platform.Top, platformIndex))
        End If
    End Sub

    ' Adjacent nodes on the same platform can always be walked.
    Private Sub ConnectWalkEdges()
        For i As Integer = 0 To Nodes.Count - 2
            Dim a As GraphNode = Nodes(i)
            Dim b As GraphNode = Nodes(i + 1)
            If a.PlatformIndex = b.PlatformIndex Then
                Dim cost As Decimal = Math.Abs(b.X - a.X)
                a.Edges.Add(New GraphEdge(b, cost, "Walk"))
                b.Edges.Add(New GraphEdge(a, cost, "Walk"))
            End If
        Next
    End Sub

    ' Different platforms are linked only if a simulated jump or fall can land on the destination node.
    Private Sub ConnectJumpAndDropEdges(ByVal entityHeight As Integer)
        For i As Integer = 0 To Nodes.Count - 1
            For j As Integer = 0 To Nodes.Count - 1
                If i = j Then
                    Continue For
                End If

                Dim fromNode As GraphNode = Nodes(i)
                Dim toNode As GraphNode = Nodes(j)
                If fromNode.PlatformIndex = toNode.PlatformIndex Then
                    Continue For
                End If

                Dim heightChange As Integer = fromNode.PlatformTop - toNode.PlatformTop
                If heightChange > MaxJumpHeight Then
                    Continue For ' destination is higher than a jump can reach
                End If

                Dim startTop As Integer = fromNode.PlatformTop - entityHeight
                Dim destTop As Integer = toNode.PlatformTop - entityHeight
                Dim dx As Integer = Math.Abs(toNode.X - fromNode.X)
                Dim dy As Integer = Math.Abs(toNode.PlatformTop - fromNode.PlatformTop)
                Dim baseCost As Decimal = CDec(Math.Sqrt((dx * dx) + (dy * dy)))

                If SimulateArc(fromNode.X, startTop, toNode.X, destTop, True, entityHeight) Then
                    fromNode.Edges.Add(New GraphEdge(toNode, baseCost + 25, "Jump"))
                ElseIf toNode.PlatformTop > fromNode.PlatformTop + 10 AndAlso SimulateArc(fromNode.X, startTop, toNode.X, destTop, False, entityHeight) Then
                    fromNode.Edges.Add(New GraphEdge(toNode, baseCost + 10, "Drop"))
                End If
            Next
        Next
    End Sub

    ' Steps through the same tick physics as Enemy.UpdatePosition to see if a landing is possible.
    Private Function SimulateArc(ByVal startX As Decimal, ByVal startTop As Decimal, ByVal destX As Decimal, ByVal destTop As Decimal, ByVal jumping As Boolean, ByVal entityHeight As Integer) As Boolean
        Dim x As Decimal = startX
        Dim y As Decimal = startTop
        Dim velocityY As Decimal = 0
        If jumping Then
            velocityY = -JumpSpeed
        End If

        Dim dir As Integer = 0
        If destX > startX Then
            dir = 1
        ElseIf destX < startX Then
            dir = -1
        End If

        Dim destPlatformTop As Decimal = destTop + entityHeight

        For tick As Integer = 1 To 150
            velocityY += Gravity
            y += velocityY
            x += dir * HorizPerTick

            If velocityY >= 0 Then
                Dim feet As Decimal = y + entityHeight
                If feet >= destPlatformTop AndAlso feet <= destPlatformTop + 55 Then
                    If Math.Abs(x - destX) <= 48 Then
                        Return True
                    End If
                End If
                If feet > destPlatformTop + 90 Then
                    Return False
                End If
            End If
        Next

        Return False
    End Function

    Public Function FindNearestNode(ByVal worldX As Integer, ByVal entityTop As Integer, ByVal entityHeight As Integer) As GraphNode
        Dim nearest As GraphNode = Nothing
        Dim bestDistance As Double = Double.MaxValue

        For Each node As GraphNode In Nodes
            Dim standingTop As Integer = node.PlatformTop - entityHeight
            Dim dx As Double = worldX - node.X
            Dim dy As Double = entityTop - standingTop
            Dim distance As Double = (dx * dx) + (dy * dy)
            If distance < bestDistance Then
                bestDistance = distance
                nearest = node
            End If
        Next

        Return nearest
    End Function
End Class
