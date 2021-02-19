Imports System.IO
Imports System.Text.Json
Imports System.Xml.Serialization

#Disable Warning IDE1006 ' Naming Styles

Public Class Form1

	''' <summary>
	''' Handles files dropped onto the form
	''' </summary>
	Sub OnDropped(sender As Object, e As DragEventArgs) Handles InputFileContainer.DragDrop

		For Each file In e.Data.GetData(DataFormats.FileDrop)

			addFile(InputFileContainer, file.ToString)

		Next

	End Sub

	''' <summary>
	''' Adds a label with text 'path' to 'container's controls
	''' Also adds DoubleClick handler
	''' </summary>
	Shared Sub addFile(container As Control, path As String)

		Dim label1 = New Label With {.Text = path,
									 .AutoSize = True}


		container.Controls.Add(label1)

		AddHandler label1.DoubleClick, Sub(sender, e)
										   sender.Parent.Controls.Remove(sender)
									   End Sub
	End Sub

	''' <summary>
	''' Handles DragEnter of InputFilesContainer
	''' </summary>
	Private Sub InputsFilesContainer_DragEnter(sender As Object, e As DragEventArgs) Handles InputFileContainer.DragEnter
		e.Effect = DragDropEffects.Link
	End Sub

	''' <summary>
	''' Handles Convert-button click
	''' </summary>
	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles ConvertButton.Click

		log("started " + Now.ToString())
		ConvertButton.Enabled = False

		For Each filelabel As Label In InputFileContainer.Controls
			processFile(filelabel.Text)
		Next

		log("finished " + Now.ToString())
		ConvertButton.Enabled = True
	End Sub
	Structure Edge
		Public source As String
		Public target As String
		Public label As String
		Sub New(source As String, target As String, label As String)
			Me.source = source
			Me.target = target
			Me.label = label
		End Sub
	End Structure
	Structure DialogueGroup
		Dim dialogueData As Dictionary(Of String, sDialogue)
		Dim groupName As String
		Dim groupId As String
	End Structure
	Private Sub processFile(path As String)

		'Check if path is still valid
		Dim x As FileInfo
		Try
			x = New FileInfo(path)
		Catch ex As Exception
			log("file not found (" + ex.Message + ")", logtype.logError)
			Return
		End Try
		If Not x.Exists Then
			log("file not found (doesn't exist)", logtype.logError)
			Return
		End If

		'Deserialize XML
		Dim InGraph As Graphml.graphml = deserializeXML(path)

		Dim processedGroups As New Dictionary(Of String, DialogueGroup)

		'Processing

		'Cross Group Edges
		Dim crossGroupEdges = New List(Of Edge)
		For Each edge In InGraph.graph.edge
			Dim myEdge = New Edge(edge.source, edge.target, edge.getLabel())
			If Not myEdge.source.Split("::")(0) = myEdge.target.Split("::")(0) Then
				crossGroupEdges.Add(myEdge)
			End If
		Next

		For Each GroupNode In InGraph.graph.node
			If GroupNode.graph IsNot Nothing Then
				Dim processedGroup As New Dictionary(Of String, sDialogue)


				'Find node with geometry:triangle2 and switch its id with n*::n0

				Dim startOrigId = "n0"
				Dim myGroupId = "n0"

				Dim foundExplicitStart = False
				For Each smallNode In GroupNode.graph.node
					For Each smalldata In smallNode.data
						If smalldata.ShapeNode IsNot Nothing AndAlso smalldata.ShapeNode.Shape.type = "triangle2" Then
							foundExplicitStart = True
							startOrigId = smallNode.id.Split("::")(2)
							myGroupId = smallNode.id.Split("::")(0)
							smallNode.id = myGroupId + "::" + "n0"
							Exit For
						End If
					Next
					If foundExplicitStart Then
						'log
						log("found explicit start at " + myGroupId + "::" + startOrigId)

						'Switch node ids
						GroupNode.graph.node()(0).id = GroupNode.graph.node()(0).id.Split("::")(0) + "::" + startOrigId

						'Fix edges after switch
						For Each edge In InGraph.graph.edge
							Select Case edge.source
								Case myGroupId + "::" + startOrigId
									edge.source = myGroupId + "::n0"
								Case myGroupId + "::n0"
									edge.source = myGroupId + "::" + startOrigId
							End Select

							Select Case edge.target
								Case myGroupId + "::" + startOrigId
									edge.target = myGroupId + "n0"
								Case myGroupId + "::n0"
									edge.target = myGroupId + "::" + startOrigId
							End Select
						Next
						Exit For
					End If
				Next


				'Nodes
				For Each smallNode In GroupNode.graph.node
					processedGroup.Add(smallNode.id, New sDialogue With {
						.Name = smallNode.id,
						.Statement = smallNode.getLabel()
					})
				Next
				myGroupId = processedGroup.Keys(0).Split("::")(0)


				'Edges
				For Each edge In InGraph.graph.edge
					Dim myEdge = New Edge(edge.source, edge.target, edge.getLabel())
					If processedGroup.ContainsKey(myEdge.source) AndAlso processedGroup.ContainsKey(myEdge.target) Then
						If myEdge.label = "" Then log(String.Format("Edge has no label (from {0} {1} to {2} {3})",
																	myEdge.source, processedGroup(myEdge.source).Statement,
																	myEdge.target, processedGroup(myEdge.target).Statement),
														logtype.logWarning)
						processedGroup(myEdge.source).addResponse(myEdge.label, Array.IndexOf(processedGroup.Keys.ToArray(), myEdge.target))
					End If
				Next


				'Try to find Groupnode Label - I have no idea what decides where it is but it seems to randomly change index...
				Dim groupNodeName As String = ""
				For Each elem In GroupNode.data
					Try
						groupNodeName = elem.ProxyAutoBoundsNode.Realizers.GroupNode(0).NodeLabel.Value
						Exit For 'exit loop if we successfully found the label text
					Catch ex As Exception
					End Try
				Next

				'Switch order of dialogues if found explicit start
				If foundExplicitStart Then
					Dim tempkeyvaluepair = processedGroup(myGroupId + "::" + startOrigId)
					processedGroup(myGroupId + "::" + startOrigId) = processedGroup(myGroupId + "::" + "n0")
					processedGroup(myGroupId + "::" + "n0") = tempkeyvaluepair
				End If


				processedGroups.Add(myGroupId, New DialogueGroup With {.dialogueData = processedGroup, .groupId = myGroupId, .groupName = groupNodeName})

			End If
		Next



		For Each edge In crossGroupEdges
			Dim sourceGroup = processedGroups(edge.source.Split("::")(0))
			Dim sourceStatement = sourceGroup.dialogueData(edge.source)
			Dim targetGroup = processedGroups(edge.target.Split("::")(0))
			Dim targetStatement = targetGroup.dialogueData(edge.target)

			sourceStatement.Statement += "#activate " + targetGroup.groupName.Replace(" ", "_") + " " + edge.GetHashCode().ToString()
			targetStatement.Statement += "#name " + sourceGroup.groupName.Replace(" ", "_") + " " + edge.GetHashCode().ToString()
		Next



		'Write
		For Each processedGroup In processedGroups
			File.WriteAllText(IO.Path.Combine(IO.Path.GetDirectoryName(path), IO.Path.GetFileNameWithoutExtension(path)) + "-" + processedGroup.Value.groupName + ".json",
								  JsonSerializer.Serialize(processedGroup.Value.dialogueData.Values))
		Next

		log("finished " + path)
	End Sub

	Enum logtype
		logError
		logWarning
		logLog
	End Enum
	Private Sub log(s As String, Optional type As logtype = logtype.logLog)
		logTextBox.AppendText(type.ToString().PadRight(12) + ": " + s + System.Environment.NewLine)
	End Sub

	Private Sub logReset()
		logTextBox.ResetText()
	End Sub

	''' <summary>
	''' Handles loading
	''' Also loads last session's file entries
	''' </summary>
	Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
		Try
			Dim r As New StreamReader("PrevSession")
			Do Until r.EndOfStream
				Dim be = r.ReadLine
				addFile(InputFileContainer, be)
			Loop
			r.Close()
		Catch ex As Exception
		End Try
	End Sub

	''' <summary>
	''' Handles closing
	''' Also writes session info for next startup
	''' </summary>
	Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
		Dim w As New StreamWriter("PrevSession")
		For Each elem As Label In InputFileContainer.Controls
			w.WriteLine(elem.Text)
		Next
		w.Close()
	End Sub

	Shared Function deserializeXML(path As String)
		Dim r As New StreamReader(path)
		Dim x As New Graphml.graphml
		Dim ser = New Xml.Serialization.XmlSerializer(x.GetType)
		x = ser.Deserialize(r)
		r.Close()

		Return x
	End Function

	Public Class sDialogue
		Public Property Name As String = "name"
		Public Property Statement As String = "statement"
		Public Property Response As sResponse() = {New sResponse()}
		Public Property Timeout As Single = 4
		Public Property JumpTo As Integer = -1

		Function hasResponse() As Boolean
			Return Not (Response.Length() = 1 And Response(0) = New sResponse())
		End Function

		Sub addResponse(response As String, jumpToIndex As Integer)
			'Dim jumptoIndex = jumptoID.Split("::")(2).TrimStart("n")

			If Not Me.hasResponse() Then
				Me.Response = {New sResponse(response, jumpToIndex)}
			Else
				ReDim Preserve Me.Response(Me.Response.Length)
				Me.Response(Me.Response.Length - 1) = New sResponse(response, jumpToIndex)
			End If
		End Sub

		'Public Overrides Function ToString() As String
		'    Return Name + " " + Statement + " " + Response.Length()
		'End Function
	End Class

	Public Class sResponse
		Public Property ResponseText As String = "..."
		Public Property JumpTo As Integer = -1

		Sub New(response As String, Optional jump As Integer = -1)
			ResponseText = response
			JumpTo = jump
		End Sub

		Shared Operator <>(A As sResponse, B As sResponse) As Boolean
			Return Not (A = B)
		End Operator

		Shared Operator =(A As sResponse, B As sResponse) As Boolean
			Return A.JumpTo = B.JumpTo AndAlso A.ResponseText = B.ResponseText
		End Operator

		Sub New()

		End Sub
	End Class

End Class

