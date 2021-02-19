Imports System.IO
Imports System.Text.Json
Imports System.Xml.Serialization

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
	Sub addFile(container As Control, path As String)

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

			'Deserialize XML
			Dim InGraph As Graphml.graphml = deserializeXML(filelabel.Text)

			'Processing
			For Each GroupNode In InGraph.graph.node
				If GroupNode.graph IsNot Nothing Then
					Dim resultDict As New Dictionary(Of String, sDialogue)


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

						If foundExplicitStart Then Exit For
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
								Case Else

							End Select

							Select Case edge.target
								Case myGroupId + "::" + startOrigId
									edge.target = myGroupId + "n0"
								Case myGroupId + "::n0"
									edge.target = myGroupId + "::" + startOrigId
								Case Else

							End Select

						Next

					End If


					'Nodes
					For Each smallNode In GroupNode.graph.node

						Dim currDialogue = New sDialogue()
						currDialogue.Name = smallNode.id

						For Each adat In smallNode.data
							If adat.ShapeNode IsNot Nothing Then
								currDialogue.Statement = adat.ShapeNode.NodeLabel.Text(0)
							End If
						Next

						resultDict.Add(smallNode.id, currDialogue)
					Next


					'Edges
					For Each edge In InGraph.graph.edge
						If resultDict.ContainsKey(edge.source) AndAlso resultDict.ContainsKey(edge.target) Then
							For Each adat In edge.data
								If adat.PolyLineEdge IsNot Nothing Then
									If adat.PolyLineEdge.EdgeLabel IsNot Nothing Then
										resultDict(edge.source).addResponse(adat.PolyLineEdge.EdgeLabel.Text(0), Array.IndexOf(resultDict.Keys.ToArray(), edge.target))
									Else
										log("Edge has no label (from " + edge.source +
													" (" + resultDict(edge.source).Statement + ")" +
													" to " + edge.target +
													" (" + resultDict(edge.target).Statement + "))",
											logtype.logWarning)

										resultDict(edge.source).addResponse("", Array.IndexOf(resultDict.Keys.ToArray(), edge.target))
									End If
								End If
							Next
						End If
					Next


					'Serialize to JSON

					'Try to find Groupnode Label - I have no idea what decides where it is but it seems to randomly change index...
					Dim groupNodeName As String = ""
					For Each elem In GroupNode.data
						Try
							groupNodeName = elem.ProxyAutoBoundsNode.Realizers.GroupNode(0).NodeLabel.Value
							Exit For 'exit loop if we successfully found the label text
						Catch ex As Exception

						End Try
					Next


					If foundExplicitStart Then
						'Switch order of dialogues if found explicit start
						Dim tempkeyvaluepair = resultDict(myGroupId + "::" + startOrigId)
						resultDict(myGroupId + "::" + startOrigId) = resultDict(myGroupId + "::" + "n0")
						resultDict(myGroupId + "::" + "n0") = tempkeyvaluepair
					End If



					'Write

					'Dim writer As New StreamWriter(filelabel.Text + "-" + groupNodeName + ".json")
					'Dim jWriter As New Newtonsoft.Json.JsonTextWriter(writer)

					'Dim ser2 As New Newtonsoft.Json.JsonSerializer()
					'ser2.Serialize(jWriter, resultDict.Values)
					'writer.Close()
					'jWriter.Close()

					File.WriteAllText(filelabel.Text + "-" + groupNodeName + ".json", JsonSerializer.Serialize(resultDict.Values))

				End If
			Next

			log("finished " + filelabel.Text)
		Next

		log("finished " + Now.ToString())
		ConvertButton.Enabled = True
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

	Function deserializeXML(path As String)
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

