' A* pathfinding on the navigation graph.
' f(n) = g(n) + h(n)
' g(n) = cost so far from the start
' h(n) = straight-line estimate to the goal 

Public Class PathResult
    Public Property Found As Boolean
    Public Property Nodes As List(Of GraphNode)
    Public Property Actions As List(Of String)

    Public Sub New()
        Found = False
        Nodes = New List(Of GraphNode)
        Actions = New List(Of String)
    End Sub

    Public Function ToDebugString() As String
        If Not Found OrElse Nodes.Count = 0 Then
            Return "No path"
        End If

        Dim text As String = "N" & Nodes(0).NodeId.ToString()
        For i As Integer = 0 To Actions.Count - 1
            text &= " " & Actions(i) & " N" & Nodes(i + 1).NodeId.ToString()
        Next
        Return text
    End Function
End Class

Public Class AStarPathfinder
    Public Function FindPath(ByVal graph As NavGraph, ByVal startNode As GraphNode, ByVal goalNode As GraphNode) As PathResult
        Dim result As New PathResult

        If graph Is Nothing OrElse startNode Is Nothing OrElse goalNode Is Nothing Then
            Return result
        End If

        If startNode.NodeId = goalNode.NodeId Then
            result.Found = True
            result.Nodes.Add(startNode)
            Return result
        End If

        Dim nodeCount As Integer = graph.Nodes.Count
        Dim gScore(nodeCount - 1) As Decimal
        Dim fScore(nodeCount - 1) As Decimal
        Dim parentId(nodeCount - 1) As Integer
        Dim parentAction(nodeCount - 1) As String
        Dim closed(nodeCount - 1) As Boolean
        Dim inOpen(nodeCount - 1) As Boolean

        For i As Integer = 0 To nodeCount - 1
            gScore(i) = Decimal.MaxValue
            fScore(i) = Decimal.MaxValue
            parentId(i) = -1
            parentAction(i) = ""
        Next

        Dim openList As New List(Of GraphNode)
        gScore(startNode.NodeId) = 0
        fScore(startNode.NodeId) = Heuristic(startNode, goalNode)
        openList.Add(startNode)
        inOpen(startNode.NodeId) = True

        While openList.Count > 0
            Dim current As GraphNode = NodeWithLowestF(openList, fScore)
            If current.NodeId = goalNode.NodeId Then
                Return ReconstructPath(graph, startNode, goalNode, parentId, parentAction)
            End If

            openList.Remove(current)
            inOpen(current.NodeId) = False
            closed(current.NodeId) = True

            For Each edge As GraphEdge In current.Edges
                Dim neighbour As GraphNode = edge.ToNode
                If closed(neighbour.NodeId) Then
                    Continue For
                End If

                Dim tentativeG As Decimal = gScore(current.NodeId) + edge.Cost
                If tentativeG < gScore(neighbour.NodeId) Then
                    parentId(neighbour.NodeId) = current.NodeId
                    parentAction(neighbour.NodeId) = edge.Action
                    gScore(neighbour.NodeId) = tentativeG
                    fScore(neighbour.NodeId) = tentativeG + Heuristic(neighbour, goalNode)

                    If Not inOpen(neighbour.NodeId) Then
                        openList.Add(neighbour)
                        inOpen(neighbour.NodeId) = True
                    End If
                End If
            Next
        End While

        Return result
    End Function

    ' Euclidean distance. This is admissible because every edge costs at least the straight-line length.
    Private Function Heuristic(ByVal a As GraphNode, ByVal b As GraphNode) As Decimal
        Dim dx As Decimal = a.PositionX - b.PositionX
        Dim dy As Decimal = a.PlatformTop - b.PlatformTop
        Return CDec(Math.Sqrt(CDbl((dx * dx) + (dy * dy))))
    End Function

    ' Linear scan of the open list. Fine here because the graph only has a few dozen nodes.
    Private Function NodeWithLowestF(ByVal openList As List(Of GraphNode), ByVal fScore() As Decimal) As GraphNode
        Dim best As GraphNode = openList(0)
        Dim bestF As Decimal = fScore(best.NodeId)

        For i As Integer = 1 To openList.Count - 1
            Dim candidate As GraphNode = openList(i)
            If fScore(candidate.NodeId) < bestF Then
                best = candidate
                bestF = fScore(candidate.NodeId)
            End If
        Next

        Return best
    End Function

    Private Function ReconstructPath(ByVal graph As NavGraph, ByVal startNode As GraphNode, ByVal goalNode As GraphNode, ByVal parentId() As Integer, ByVal parentAction() As String) As PathResult
        Dim result As New PathResult
        result.Found = True

        Dim reverseNodes As New List(Of GraphNode)
        Dim reverseActions As New List(Of String)
        Dim currentId As Integer = goalNode.NodeId

        While currentId <> startNode.NodeId
            reverseNodes.Add(graph.Nodes(currentId))
            reverseActions.Add(parentAction(currentId))
            currentId = parentId(currentId)
            If currentId < 0 Then
                result.Found = False
                Return New PathResult()
            End If
        End While

        reverseNodes.Add(startNode)
        reverseNodes.Reverse()
        reverseActions.Reverse()

        result.Nodes = reverseNodes
        result.Actions = reverseActions
        Return result
    End Function
End Class
