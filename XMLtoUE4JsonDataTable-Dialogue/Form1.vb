Imports System.IO
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
        log(Now.ToString())
        For Each filelabel As Label In InputFileContainer.Controls

            'Deserialize XML
            Dim InGraph As graphml = deserializeXML(filelabel.Text)

            'Processing
            For Each GroupNode In InGraph.graph.node
                Dim sf As New Dictionary(Of String, sDialogue)


                'Nodes
                For Each smallNode In GroupNode.graph.node
                    Dim currDialogue = New sDialogue()
                    currDialogue.Name = smallNode.id
                    For Each adat In smallNode.data
                        If adat.ShapeNode IsNot Nothing Then
                            currDialogue.Statement = adat.ShapeNode.NodeLabel.Text(0)
                        End If
                    Next
                    sf.Add(smallNode.id, currDialogue)
                Next


                'Edges
                For Each edge In InGraph.graph.edge
                    If sf.ContainsKey(edge.source) Then
                        For Each adat In edge.data
                            If adat.PolyLineEdge IsNot Nothing Then
                                If adat.PolyLineEdge.EdgeLabel IsNot Nothing Then
                                    sf(edge.source).addResponse(adat.PolyLineEdge.EdgeLabel.Text(0), edge.target)
                                Else
                                    log("Edge from " + edge.source +
                                                    " (" + sf(edge.source).Statement + ")" +
                                                    " to " + edge.target +
                                                    " (" + sf(edge.target).Statement + ")" +
                                                    " has no label")
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


                'Write
                Dim writer As New StreamWriter(filelabel.Text + "-" + groupNodeName + ".json")
                Dim jWriter As New Newtonsoft.Json.JsonTextWriter(writer)

                Dim ser2 As New Newtonsoft.Json.JsonSerializer()
                ser2.Serialize(jWriter, sf.Values)
                writer.Close()
                jWriter.Close()

            Next
        Next
    End Sub

    Private Sub log(s As String)
        logTextBox.AppendText(s + vbNewLine)
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
        Dim x As New graphml
        Dim ser = New Xml.Serialization.XmlSerializer(x.GetType)
        x = ser.Deserialize(r)
        Return x
    End Function

    Class sDialogue
        Public Name As String = "name"
        Public Statement As String = "statement"
        Public Response() As sResponse
        Public Timeout As Single = 4
        Public JumpTo As Integer = -1
        Sub addResponse(response As String, jumptoID As String)
            Dim jumptoIndex = jumptoID.Split("::")(2).TrimStart("n")

            If Me.Response Is Nothing Then
                Me.Response = {New sResponse(response, jumptoIndex)}
            Else
                ReDim Preserve Me.Response(Me.Response.Length)
                Me.Response(Me.Response.Length - 1) = New sResponse(response, jumptoIndex)
            End If
        End Sub
    End Class

    Class sResponse
        Public ResponseText As String = "..."
        Public JumpTo As Integer = -1
        Sub New(response As String, Optional jump As Integer = -1)
            ResponseText = response
            JumpTo = jump
        End Sub
        Sub New()

        End Sub
    End Class

End Class


'// NOTE: Generated code may require at least .NET Framework 4.5 Or .NET Core/Standard 2.0.
'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://graphml.graphdrawing.org/xmlns"),
 System.Xml.Serialization.XmlRootAttribute([Namespace]:="http://graphml.graphdrawing.org/xmlns", IsNullable:=False)>
Partial Public Class graphml

    Private keyField() As graphmlKey

    Private graphField As graphmlGraph

    Private dataField As graphmlData

    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute("key")>
    Public Property key() As graphmlKey()
        Get
            Return Me.keyField
        End Get
        Set
            Me.keyField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property graph() As graphmlGraph
        Get
            Return Me.graphField
        End Get
        Set
            Me.graphField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property data() As graphmlData
        Get
            Return Me.dataField
        End Get
        Set
            Me.dataField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://graphml.graphdrawing.org/xmlns")>
Partial Public Class graphmlKey

    Private attrnameField As String

    Private attrtypeField As String

    Private forField As String

    Private idField As String

    Private yfilestypeField As String

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute("attr.name")>
    Public Property attrname() As String
        Get
            Return Me.attrnameField
        End Get
        Set
            Me.attrnameField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute("attr.type")>
    Public Property attrtype() As String
        Get
            Return Me.attrtypeField
        End Get
        Set
            Me.attrtypeField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property [for]() As String
        Get
            Return Me.forField
        End Get
        Set
            Me.forField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property id() As String
        Get
            Return Me.idField
        End Get
        Set
            Me.idField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute("yfiles.type")>
    Public Property yfilestype() As String
        Get
            Return Me.yfilestypeField
        End Get
        Set
            Me.yfilestypeField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://graphml.graphdrawing.org/xmlns")>
Partial Public Class graphmlGraph

    Private dataField As graphmlGraphData

    Private nodeField() As graphmlGraphNode

    Private edgeField() As graphmlGraphEdge

    Private edgedefaultField As String

    Private idField As String

    '''<remarks/>
    Public Property data() As graphmlGraphData
        Get
            Return Me.dataField
        End Get
        Set
            Me.dataField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute("node")>
    Public Property node() As graphmlGraphNode()
        Get
            Return Me.nodeField
        End Get
        Set
            Me.nodeField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute("edge")>
    Public Property edge() As graphmlGraphEdge()
        Get
            Return Me.edgeField
        End Get
        Set
            Me.edgeField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property edgedefault() As String
        Get
            Return Me.edgedefaultField
        End Get
        Set
            Me.edgedefaultField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property id() As String
        Get
            Return Me.idField
        End Get
        Set
            Me.idField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://graphml.graphdrawing.org/xmlns")>
Partial Public Class graphmlGraphData

    Private keyField As String

    Private spaceField As String

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property key() As String
        Get
            Return Me.keyField
        End Get
        Set
            Me.keyField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Qualified, [Namespace]:="http://www.w3.org/XML/1998/namespace")>
    Public Property space() As String
        Get
            Return Me.spaceField
        End Get
        Set
            Me.spaceField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://graphml.graphdrawing.org/xmlns")>
Partial Public Class graphmlGraphNode

    Private dataField() As graphmlGraphNodeData

    Private graphField As graphmlGraphNodeGraph

    Private idField As String

    Private yfilesfoldertypeField As String

    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute("data")>
    Public Property data() As graphmlGraphNodeData()
        Get
            Return Me.dataField
        End Get
        Set
            Me.dataField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property graph() As graphmlGraphNodeGraph
        Get
            Return Me.graphField
        End Get
        Set
            Me.graphField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property id() As String
        Get
            Return Me.idField
        End Get
        Set
            Me.idField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute("yfiles.foldertype")>
    Public Property yfilesfoldertype() As String
        Get
            Return Me.yfilesfoldertypeField
        End Get
        Set
            Me.yfilesfoldertypeField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://graphml.graphdrawing.org/xmlns")>
Partial Public Class graphmlGraphNodeData

    Private proxyAutoBoundsNodeField As ProxyAutoBoundsNode

    Private keyField As String

    Private spaceField As String

    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute([Namespace]:="http://www.yworks.com/xml/graphml")>
    Public Property ProxyAutoBoundsNode() As ProxyAutoBoundsNode
        Get
            Return Me.proxyAutoBoundsNodeField
        End Get
        Set
            Me.proxyAutoBoundsNodeField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property key() As String
        Get
            Return Me.keyField
        End Get
        Set
            Me.keyField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Qualified, [Namespace]:="http://www.w3.org/XML/1998/namespace")>
    Public Property space() As String
        Get
            Return Me.spaceField
        End Get
        Set
            Me.spaceField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml"),
 System.Xml.Serialization.XmlRootAttribute([Namespace]:="http://www.yworks.com/xml/graphml", IsNullable:=False)>
Partial Public Class ProxyAutoBoundsNode

    Private realizersField As ProxyAutoBoundsNodeRealizers

    '''<remarks/>
    Public Property Realizers() As ProxyAutoBoundsNodeRealizers
        Get
            Return Me.realizersField
        End Get
        Set
            Me.realizersField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class ProxyAutoBoundsNodeRealizers

    Private groupNodeField() As ProxyAutoBoundsNodeRealizersGroupNode

    Private activeField As Byte

    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute("GroupNode")>
    Public Property GroupNode() As ProxyAutoBoundsNodeRealizersGroupNode()
        Get
            Return Me.groupNodeField
        End Get
        Set
            Me.groupNodeField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property active() As Byte
        Get
            Return Me.activeField
        End Get
        Set
            Me.activeField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class ProxyAutoBoundsNodeRealizersGroupNode

    Private geometryField As ProxyAutoBoundsNodeRealizersGroupNodeGeometry

    Private fillField As ProxyAutoBoundsNodeRealizersGroupNodeFill

    Private borderStyleField As ProxyAutoBoundsNodeRealizersGroupNodeBorderStyle

    Private nodeLabelField As ProxyAutoBoundsNodeRealizersGroupNodeNodeLabel

    Private shapeField As ProxyAutoBoundsNodeRealizersGroupNodeShape

    Private stateField As ProxyAutoBoundsNodeRealizersGroupNodeState

    Private insetsField As ProxyAutoBoundsNodeRealizersGroupNodeInsets

    Private borderInsetsField As ProxyAutoBoundsNodeRealizersGroupNodeBorderInsets

    '''<remarks/>
    Public Property Geometry() As ProxyAutoBoundsNodeRealizersGroupNodeGeometry
        Get
            Return Me.geometryField
        End Get
        Set
            Me.geometryField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property Fill() As ProxyAutoBoundsNodeRealizersGroupNodeFill
        Get
            Return Me.fillField
        End Get
        Set
            Me.fillField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property BorderStyle() As ProxyAutoBoundsNodeRealizersGroupNodeBorderStyle
        Get
            Return Me.borderStyleField
        End Get
        Set
            Me.borderStyleField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property NodeLabel() As ProxyAutoBoundsNodeRealizersGroupNodeNodeLabel
        Get
            Return Me.nodeLabelField
        End Get
        Set
            Me.nodeLabelField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property Shape() As ProxyAutoBoundsNodeRealizersGroupNodeShape
        Get
            Return Me.shapeField
        End Get
        Set
            Me.shapeField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property State() As ProxyAutoBoundsNodeRealizersGroupNodeState
        Get
            Return Me.stateField
        End Get
        Set
            Me.stateField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property Insets() As ProxyAutoBoundsNodeRealizersGroupNodeInsets
        Get
            Return Me.insetsField
        End Get
        Set
            Me.insetsField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property BorderInsets() As ProxyAutoBoundsNodeRealizersGroupNodeBorderInsets
        Get
            Return Me.borderInsetsField
        End Get
        Set
            Me.borderInsetsField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class ProxyAutoBoundsNodeRealizersGroupNodeGeometry

    Private heightField As Decimal

    Private widthField As Decimal

    Private xField As Decimal

    Private yField As Decimal

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property height() As Decimal
        Get
            Return Me.heightField
        End Get
        Set
            Me.heightField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property width() As Decimal
        Get
            Return Me.widthField
        End Get
        Set
            Me.widthField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property x() As Decimal
        Get
            Return Me.xField
        End Get
        Set
            Me.xField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property y() As Decimal
        Get
            Return Me.yField
        End Get
        Set
            Me.yField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class ProxyAutoBoundsNodeRealizersGroupNodeFill

    Private colorField As String

    Private transparentField As Boolean

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property color() As String
        Get
            Return Me.colorField
        End Get
        Set
            Me.colorField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property transparent() As Boolean
        Get
            Return Me.transparentField
        End Get
        Set
            Me.transparentField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class ProxyAutoBoundsNodeRealizersGroupNodeBorderStyle

    Private colorField As String

    Private typeField As String

    Private widthField As Decimal

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property color() As String
        Get
            Return Me.colorField
        End Get
        Set
            Me.colorField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property type() As String
        Get
            Return Me.typeField
        End Get
        Set
            Me.typeField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property width() As Decimal
        Get
            Return Me.widthField
        End Get
        Set
            Me.widthField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class ProxyAutoBoundsNodeRealizersGroupNodeNodeLabel

    Private alignmentField As String

    Private autoSizePolicyField As String

    Private backgroundColorField As String

    Private borderDistanceField As Decimal

    Private fontFamilyField As String

    Private fontSizeField As Byte

    Private fontStyleField As String

    Private hasLineColorField As Boolean

    Private heightField As Decimal

    Private horizontalTextPositionField As String

    Private iconTextGapField As Byte

    Private modelNameField As String

    Private modelPositionField As String

    Private textColorField As String

    Private verticalTextPositionField As String

    Private visibleField As Boolean

    Private widthField As Decimal

    Private xField As Decimal

    Private spaceField As String

    Private yField As Decimal

    Private valueField As String

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property alignment() As String
        Get
            Return Me.alignmentField
        End Get
        Set
            Me.alignmentField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property autoSizePolicy() As String
        Get
            Return Me.autoSizePolicyField
        End Get
        Set
            Me.autoSizePolicyField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property backgroundColor() As String
        Get
            Return Me.backgroundColorField
        End Get
        Set
            Me.backgroundColorField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property borderDistance() As Decimal
        Get
            Return Me.borderDistanceField
        End Get
        Set
            Me.borderDistanceField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property fontFamily() As String
        Get
            Return Me.fontFamilyField
        End Get
        Set
            Me.fontFamilyField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property fontSize() As Byte
        Get
            Return Me.fontSizeField
        End Get
        Set
            Me.fontSizeField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property fontStyle() As String
        Get
            Return Me.fontStyleField
        End Get
        Set
            Me.fontStyleField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property hasLineColor() As Boolean
        Get
            Return Me.hasLineColorField
        End Get
        Set
            Me.hasLineColorField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property height() As Decimal
        Get
            Return Me.heightField
        End Get
        Set
            Me.heightField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property horizontalTextPosition() As String
        Get
            Return Me.horizontalTextPositionField
        End Get
        Set
            Me.horizontalTextPositionField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property iconTextGap() As Byte
        Get
            Return Me.iconTextGapField
        End Get
        Set
            Me.iconTextGapField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property modelName() As String
        Get
            Return Me.modelNameField
        End Get
        Set
            Me.modelNameField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property modelPosition() As String
        Get
            Return Me.modelPositionField
        End Get
        Set
            Me.modelPositionField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property textColor() As String
        Get
            Return Me.textColorField
        End Get
        Set
            Me.textColorField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property verticalTextPosition() As String
        Get
            Return Me.verticalTextPositionField
        End Get
        Set
            Me.verticalTextPositionField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property visible() As Boolean
        Get
            Return Me.visibleField
        End Get
        Set
            Me.visibleField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property width() As Decimal
        Get
            Return Me.widthField
        End Get
        Set
            Me.widthField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property x() As Decimal
        Get
            Return Me.xField
        End Get
        Set
            Me.xField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Qualified, [Namespace]:="http://www.w3.org/XML/1998/namespace")>
    Public Property space() As String
        Get
            Return Me.spaceField
        End Get
        Set
            Me.spaceField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property y() As Decimal
        Get
            Return Me.yField
        End Get
        Set
            Me.yField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlTextAttribute()>
    Public Property Value() As String
        Get
            Return Me.valueField
        End Get
        Set
            Me.valueField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class ProxyAutoBoundsNodeRealizersGroupNodeShape

    Private typeField As String

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property type() As String
        Get
            Return Me.typeField
        End Get
        Set
            Me.typeField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class ProxyAutoBoundsNodeRealizersGroupNodeState

    Private closedField As Boolean

    Private closedHeightField As Decimal

    Private closedWidthField As Decimal

    Private innerGraphDisplayEnabledField As Boolean

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property closed() As Boolean
        Get
            Return Me.closedField
        End Get
        Set
            Me.closedField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property closedHeight() As Decimal
        Get
            Return Me.closedHeightField
        End Get
        Set
            Me.closedHeightField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property closedWidth() As Decimal
        Get
            Return Me.closedWidthField
        End Get
        Set
            Me.closedWidthField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property innerGraphDisplayEnabled() As Boolean
        Get
            Return Me.innerGraphDisplayEnabledField
        End Get
        Set
            Me.innerGraphDisplayEnabledField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class ProxyAutoBoundsNodeRealizersGroupNodeInsets

    Private bottomField As Byte

    Private bottomFField As Decimal

    Private leftField As Byte

    Private leftFField As Decimal

    Private rightField As Byte

    Private rightFField As Decimal

    Private topField As Byte

    Private topFField As Decimal

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property bottom() As Byte
        Get
            Return Me.bottomField
        End Get
        Set
            Me.bottomField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property bottomF() As Decimal
        Get
            Return Me.bottomFField
        End Get
        Set
            Me.bottomFField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property left() As Byte
        Get
            Return Me.leftField
        End Get
        Set
            Me.leftField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property leftF() As Decimal
        Get
            Return Me.leftFField
        End Get
        Set
            Me.leftFField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property right() As Byte
        Get
            Return Me.rightField
        End Get
        Set
            Me.rightField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property rightF() As Decimal
        Get
            Return Me.rightFField
        End Get
        Set
            Me.rightFField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property top() As Byte
        Get
            Return Me.topField
        End Get
        Set
            Me.topField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property topF() As Decimal
        Get
            Return Me.topFField
        End Get
        Set
            Me.topFField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class ProxyAutoBoundsNodeRealizersGroupNodeBorderInsets

    Private bottomField As Byte

    Private bottomFField As Decimal

    Private leftField As UShort

    Private leftFField As Decimal

    Private rightField As Byte

    Private rightFField As Decimal

    Private topField As Byte

    Private topFField As Decimal

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property bottom() As Byte
        Get
            Return Me.bottomField
        End Get
        Set
            Me.bottomField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property bottomF() As Decimal
        Get
            Return Me.bottomFField
        End Get
        Set
            Me.bottomFField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property left() As UShort
        Get
            Return Me.leftField
        End Get
        Set
            Me.leftField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property leftF() As Decimal
        Get
            Return Me.leftFField
        End Get
        Set
            Me.leftFField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property right() As Byte
        Get
            Return Me.rightField
        End Get
        Set
            Me.rightField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property rightF() As Decimal
        Get
            Return Me.rightFField
        End Get
        Set
            Me.rightFField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property top() As Byte
        Get
            Return Me.topField
        End Get
        Set
            Me.topField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property topF() As Decimal
        Get
            Return Me.topFField
        End Get
        Set
            Me.topFField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://graphml.graphdrawing.org/xmlns")>
Partial Public Class graphmlGraphNodeGraph

    Private nodeField() As graphmlGraphNodeGraphNode

    Private edgedefaultField As String

    Private idField As String

    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute("node")>
    Public Property node() As graphmlGraphNodeGraphNode()
        Get
            Return Me.nodeField
        End Get
        Set
            Me.nodeField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property edgedefault() As String
        Get
            Return Me.edgedefaultField
        End Get
        Set
            Me.edgedefaultField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property id() As String
        Get
            Return Me.idField
        End Get
        Set
            Me.idField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://graphml.graphdrawing.org/xmlns")>
Partial Public Class graphmlGraphNodeGraphNode

    Private dataField() As graphmlGraphNodeGraphNodeData

    Private idField As String

    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute("data")>
    Public Property data() As graphmlGraphNodeGraphNodeData()
        Get
            Return Me.dataField
        End Get
        Set
            Me.dataField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property id() As String
        Get
            Return Me.idField
        End Get
        Set
            Me.idField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://graphml.graphdrawing.org/xmlns")>
Partial Public Class graphmlGraphNodeGraphNodeData

    Private shapeNodeField As ShapeNode

    Private keyField As String

    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute([Namespace]:="http://www.yworks.com/xml/graphml")>
    Public Property ShapeNode() As ShapeNode
        Get
            Return Me.shapeNodeField
        End Get
        Set
            Me.shapeNodeField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property key() As String
        Get
            Return Me.keyField
        End Get
        Set
            Me.keyField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml"),
 System.Xml.Serialization.XmlRootAttribute([Namespace]:="http://www.yworks.com/xml/graphml", IsNullable:=False)>
Partial Public Class ShapeNode

    Private geometryField As ShapeNodeGeometry

    Private fillField As ShapeNodeFill

    Private borderStyleField As ShapeNodeBorderStyle

    Private nodeLabelField As ShapeNodeNodeLabel

    Private shapeField As ShapeNodeShape

    '''<remarks/>
    Public Property Geometry() As ShapeNodeGeometry
        Get
            Return Me.geometryField
        End Get
        Set
            Me.geometryField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property Fill() As ShapeNodeFill
        Get
            Return Me.fillField
        End Get
        Set
            Me.fillField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property BorderStyle() As ShapeNodeBorderStyle
        Get
            Return Me.borderStyleField
        End Get
        Set
            Me.borderStyleField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property NodeLabel() As ShapeNodeNodeLabel
        Get
            Return Me.nodeLabelField
        End Get
        Set
            Me.nodeLabelField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property Shape() As ShapeNodeShape
        Get
            Return Me.shapeField
        End Get
        Set
            Me.shapeField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class ShapeNodeGeometry

    Private heightField As Decimal

    Private widthField As Decimal

    Private xField As Decimal

    Private yField As Decimal

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property height() As Decimal
        Get
            Return Me.heightField
        End Get
        Set
            Me.heightField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property width() As Decimal
        Get
            Return Me.widthField
        End Get
        Set
            Me.widthField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property x() As Decimal
        Get
            Return Me.xField
        End Get
        Set
            Me.xField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property y() As Decimal
        Get
            Return Me.yField
        End Get
        Set
            Me.yField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class ShapeNodeFill

    Private colorField As String

    Private transparentField As Boolean

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property color() As String
        Get
            Return Me.colorField
        End Get
        Set
            Me.colorField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property transparent() As Boolean
        Get
            Return Me.transparentField
        End Get
        Set
            Me.transparentField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class ShapeNodeBorderStyle

    Private colorField As String

    Private raisedField As Boolean

    Private typeField As String

    Private widthField As Decimal

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property color() As String
        Get
            Return Me.colorField
        End Get
        Set
            Me.colorField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property raised() As Boolean
        Get
            Return Me.raisedField
        End Get
        Set
            Me.raisedField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property type() As String
        Get
            Return Me.typeField
        End Get
        Set
            Me.typeField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property width() As Decimal
        Get
            Return Me.widthField
        End Get
        Set
            Me.widthField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class ShapeNodeNodeLabel

    Private labelModelField As ShapeNodeNodeLabelLabelModel

    Private modelParameterField As ShapeNodeNodeLabelModelParameter

    Private textField() As String

    Private alignmentField As String

    Private autoSizePolicyField As String

    Private fontFamilyField As String

    Private fontSizeField As Byte

    Private fontStyleField As String

    Private hasBackgroundColorField As Boolean

    Private hasLineColorField As Boolean

    Private heightField As Decimal

    Private horizontalTextPositionField As String

    Private iconTextGapField As Byte

    Private modelNameField As String

    Private textColorField As String

    Private verticalTextPositionField As String

    Private visibleField As Boolean

    Private widthField As Decimal

    Private xField As Decimal

    Private spaceField As String

    Private yField As Decimal

    '''<remarks/>
    Public Property LabelModel() As ShapeNodeNodeLabelLabelModel
        Get
            Return Me.labelModelField
        End Get
        Set
            Me.labelModelField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property ModelParameter() As ShapeNodeNodeLabelModelParameter
        Get
            Return Me.modelParameterField
        End Get
        Set
            Me.modelParameterField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlTextAttribute()>
    Public Property Text() As String()
        Get
            Return Me.textField
        End Get
        Set
            Me.textField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property alignment() As String
        Get
            Return Me.alignmentField
        End Get
        Set
            Me.alignmentField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property autoSizePolicy() As String
        Get
            Return Me.autoSizePolicyField
        End Get
        Set
            Me.autoSizePolicyField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property fontFamily() As String
        Get
            Return Me.fontFamilyField
        End Get
        Set
            Me.fontFamilyField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property fontSize() As Byte
        Get
            Return Me.fontSizeField
        End Get
        Set
            Me.fontSizeField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property fontStyle() As String
        Get
            Return Me.fontStyleField
        End Get
        Set
            Me.fontStyleField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property hasBackgroundColor() As Boolean
        Get
            Return Me.hasBackgroundColorField
        End Get
        Set
            Me.hasBackgroundColorField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property hasLineColor() As Boolean
        Get
            Return Me.hasLineColorField
        End Get
        Set
            Me.hasLineColorField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property height() As Decimal
        Get
            Return Me.heightField
        End Get
        Set
            Me.heightField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property horizontalTextPosition() As String
        Get
            Return Me.horizontalTextPositionField
        End Get
        Set
            Me.horizontalTextPositionField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property iconTextGap() As Byte
        Get
            Return Me.iconTextGapField
        End Get
        Set
            Me.iconTextGapField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property modelName() As String
        Get
            Return Me.modelNameField
        End Get
        Set
            Me.modelNameField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property textColor() As String
        Get
            Return Me.textColorField
        End Get
        Set
            Me.textColorField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property verticalTextPosition() As String
        Get
            Return Me.verticalTextPositionField
        End Get
        Set
            Me.verticalTextPositionField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property visible() As Boolean
        Get
            Return Me.visibleField
        End Get
        Set
            Me.visibleField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property width() As Decimal
        Get
            Return Me.widthField
        End Get
        Set
            Me.widthField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property x() As Decimal
        Get
            Return Me.xField
        End Get
        Set
            Me.xField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Qualified, [Namespace]:="http://www.w3.org/XML/1998/namespace")>
    Public Property space() As String
        Get
            Return Me.spaceField
        End Get
        Set
            Me.spaceField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property y() As Decimal
        Get
            Return Me.yField
        End Get
        Set
            Me.yField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class ShapeNodeNodeLabelLabelModel

    Private smartNodeLabelModelField As ShapeNodeNodeLabelLabelModelSmartNodeLabelModel

    '''<remarks/>
    Public Property SmartNodeLabelModel() As ShapeNodeNodeLabelLabelModelSmartNodeLabelModel
        Get
            Return Me.smartNodeLabelModelField
        End Get
        Set
            Me.smartNodeLabelModelField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class ShapeNodeNodeLabelLabelModelSmartNodeLabelModel

    Private distanceField As Decimal

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property distance() As Decimal
        Get
            Return Me.distanceField
        End Get
        Set
            Me.distanceField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class ShapeNodeNodeLabelModelParameter

    Private smartNodeLabelModelParameterField As ShapeNodeNodeLabelModelParameterSmartNodeLabelModelParameter

    '''<remarks/>
    Public Property SmartNodeLabelModelParameter() As ShapeNodeNodeLabelModelParameterSmartNodeLabelModelParameter
        Get
            Return Me.smartNodeLabelModelParameterField
        End Get
        Set
            Me.smartNodeLabelModelParameterField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class ShapeNodeNodeLabelModelParameterSmartNodeLabelModelParameter

    Private labelRatioXField As Decimal

    Private labelRatioYField As Decimal

    Private nodeRatioXField As Decimal

    Private nodeRatioYField As Decimal

    Private offsetXField As Decimal

    Private offsetYField As Decimal

    Private upXField As Decimal

    Private upYField As Decimal

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property labelRatioX() As Decimal
        Get
            Return Me.labelRatioXField
        End Get
        Set
            Me.labelRatioXField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property labelRatioY() As Decimal
        Get
            Return Me.labelRatioYField
        End Get
        Set
            Me.labelRatioYField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property nodeRatioX() As Decimal
        Get
            Return Me.nodeRatioXField
        End Get
        Set
            Me.nodeRatioXField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property nodeRatioY() As Decimal
        Get
            Return Me.nodeRatioYField
        End Get
        Set
            Me.nodeRatioYField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property offsetX() As Decimal
        Get
            Return Me.offsetXField
        End Get
        Set
            Me.offsetXField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property offsetY() As Decimal
        Get
            Return Me.offsetYField
        End Get
        Set
            Me.offsetYField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property upX() As Decimal
        Get
            Return Me.upXField
        End Get
        Set
            Me.upXField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property upY() As Decimal
        Get
            Return Me.upYField
        End Get
        Set
            Me.upYField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class ShapeNodeShape

    Private typeField As String

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property type() As String
        Get
            Return Me.typeField
        End Get
        Set
            Me.typeField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://graphml.graphdrawing.org/xmlns")>
Partial Public Class graphmlGraphEdge

    Private dataField() As graphmlGraphEdgeData

    Private idField As String

    Private sourceField As String

    Private targetField As String

    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute("data")>
    Public Property data() As graphmlGraphEdgeData()
        Get
            Return Me.dataField
        End Get
        Set
            Me.dataField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property id() As String
        Get
            Return Me.idField
        End Get
        Set
            Me.idField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property source() As String
        Get
            Return Me.sourceField
        End Get
        Set
            Me.sourceField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property target() As String
        Get
            Return Me.targetField
        End Get
        Set
            Me.targetField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://graphml.graphdrawing.org/xmlns")>
Partial Public Class graphmlGraphEdgeData

    Private polyLineEdgeField As PolyLineEdge

    Private keyField As String

    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute([Namespace]:="http://www.yworks.com/xml/graphml")>
    Public Property PolyLineEdge() As PolyLineEdge
        Get
            Return Me.polyLineEdgeField
        End Get
        Set
            Me.polyLineEdgeField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property key() As String
        Get
            Return Me.keyField
        End Get
        Set
            Me.keyField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml"),
 System.Xml.Serialization.XmlRootAttribute([Namespace]:="http://www.yworks.com/xml/graphml", IsNullable:=False)>
Partial Public Class PolyLineEdge

    Private pathField As PolyLineEdgePath

    Private lineStyleField As PolyLineEdgeLineStyle

    Private arrowsField As PolyLineEdgeArrows

    Private edgeLabelField As PolyLineEdgeEdgeLabel

    Private bendStyleField As PolyLineEdgeBendStyle

    '''<remarks/>
    Public Property Path() As PolyLineEdgePath
        Get
            Return Me.pathField
        End Get
        Set
            Me.pathField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property LineStyle() As PolyLineEdgeLineStyle
        Get
            Return Me.lineStyleField
        End Get
        Set
            Me.lineStyleField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property Arrows() As PolyLineEdgeArrows
        Get
            Return Me.arrowsField
        End Get
        Set
            Me.arrowsField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property EdgeLabel() As PolyLineEdgeEdgeLabel
        Get
            Return Me.edgeLabelField
        End Get
        Set
            Me.edgeLabelField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property BendStyle() As PolyLineEdgeBendStyle
        Get
            Return Me.bendStyleField
        End Get
        Set
            Me.bendStyleField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class PolyLineEdgePath

    Private pointField() As PolyLineEdgePathPoint

    Private sxField As Decimal

    Private syField As Decimal

    Private txField As Decimal

    Private tyField As Decimal

    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute("Point")>
    Public Property Point() As PolyLineEdgePathPoint()
        Get
            Return Me.pointField
        End Get
        Set
            Me.pointField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property sx() As Decimal
        Get
            Return Me.sxField
        End Get
        Set
            Me.sxField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property sy() As Decimal
        Get
            Return Me.syField
        End Get
        Set
            Me.syField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property tx() As Decimal
        Get
            Return Me.txField
        End Get
        Set
            Me.txField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property ty() As Decimal
        Get
            Return Me.tyField
        End Get
        Set
            Me.tyField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class PolyLineEdgePathPoint

    Private xField As Decimal

    Private yField As Decimal

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property x() As Decimal
        Get
            Return Me.xField
        End Get
        Set
            Me.xField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property y() As Decimal
        Get
            Return Me.yField
        End Get
        Set
            Me.yField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class PolyLineEdgeLineStyle

    Private colorField As String

    Private typeField As String

    Private widthField As Decimal

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property color() As String
        Get
            Return Me.colorField
        End Get
        Set
            Me.colorField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property type() As String
        Get
            Return Me.typeField
        End Get
        Set
            Me.typeField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property width() As Decimal
        Get
            Return Me.widthField
        End Get
        Set
            Me.widthField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class PolyLineEdgeArrows

    Private sourceField As String

    Private targetField As String

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property source() As String
        Get
            Return Me.sourceField
        End Get
        Set
            Me.sourceField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property target() As String
        Get
            Return Me.targetField
        End Get
        Set
            Me.targetField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class PolyLineEdgeEdgeLabel

    Private labelModelField As PolyLineEdgeEdgeLabelLabelModel

    Private modelParameterField As PolyLineEdgeEdgeLabelModelParameter

    Private preferredPlacementDescriptorField As PolyLineEdgeEdgeLabelPreferredPlacementDescriptor

    Private textField() As String

    Private alignmentField As String

    Private configurationField As String

    Private distanceField As Decimal

    Private fontFamilyField As String

    Private fontSizeField As Byte

    Private fontStyleField As String

    Private hasBackgroundColorField As Boolean

    Private hasLineColorField As Boolean

    Private heightField As Decimal

    Private horizontalTextPositionField As String

    Private iconTextGapField As Byte

    Private modelNameField As String

    Private preferredPlacementField As String

    Private ratioField As Decimal

    Private textColorField As String

    Private verticalTextPositionField As String

    Private visibleField As Boolean

    Private widthField As Decimal

    Private xField As Double

    Private spaceField As String

    Private yField As Decimal

    '''<remarks/>
    Public Property LabelModel() As PolyLineEdgeEdgeLabelLabelModel
        Get
            Return Me.labelModelField
        End Get
        Set
            Me.labelModelField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property ModelParameter() As PolyLineEdgeEdgeLabelModelParameter
        Get
            Return Me.modelParameterField
        End Get
        Set
            Me.modelParameterField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property PreferredPlacementDescriptor() As PolyLineEdgeEdgeLabelPreferredPlacementDescriptor
        Get
            Return Me.preferredPlacementDescriptorField
        End Get
        Set
            Me.preferredPlacementDescriptorField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlTextAttribute()>
    Public Property Text() As String()
        Get
            Return Me.textField
        End Get
        Set
            Me.textField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property alignment() As String
        Get
            Return Me.alignmentField
        End Get
        Set
            Me.alignmentField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property configuration() As String
        Get
            Return Me.configurationField
        End Get
        Set
            Me.configurationField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property distance() As Decimal
        Get
            Return Me.distanceField
        End Get
        Set
            Me.distanceField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property fontFamily() As String
        Get
            Return Me.fontFamilyField
        End Get
        Set
            Me.fontFamilyField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property fontSize() As Byte
        Get
            Return Me.fontSizeField
        End Get
        Set
            Me.fontSizeField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property fontStyle() As String
        Get
            Return Me.fontStyleField
        End Get
        Set
            Me.fontStyleField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property hasBackgroundColor() As Boolean
        Get
            Return Me.hasBackgroundColorField
        End Get
        Set
            Me.hasBackgroundColorField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property hasLineColor() As Boolean
        Get
            Return Me.hasLineColorField
        End Get
        Set
            Me.hasLineColorField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property height() As Decimal
        Get
            Return Me.heightField
        End Get
        Set
            Me.heightField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property horizontalTextPosition() As String
        Get
            Return Me.horizontalTextPositionField
        End Get
        Set
            Me.horizontalTextPositionField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property iconTextGap() As Byte
        Get
            Return Me.iconTextGapField
        End Get
        Set
            Me.iconTextGapField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property modelName() As String
        Get
            Return Me.modelNameField
        End Get
        Set
            Me.modelNameField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property preferredPlacement() As String
        Get
            Return Me.preferredPlacementField
        End Get
        Set
            Me.preferredPlacementField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property ratio() As Decimal
        Get
            Return Me.ratioField
        End Get
        Set
            Me.ratioField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property textColor() As String
        Get
            Return Me.textColorField
        End Get
        Set
            Me.textColorField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property verticalTextPosition() As String
        Get
            Return Me.verticalTextPositionField
        End Get
        Set
            Me.verticalTextPositionField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property visible() As Boolean
        Get
            Return Me.visibleField
        End Get
        Set
            Me.visibleField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property width() As Decimal
        Get
            Return Me.widthField
        End Get
        Set
            Me.widthField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property x() As Double
        Get
            Return Me.xField
        End Get
        Set
            Me.xField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Qualified, [Namespace]:="http://www.w3.org/XML/1998/namespace")>
    Public Property space() As String
        Get
            Return Me.spaceField
        End Get
        Set
            Me.spaceField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property y() As Decimal
        Get
            Return Me.yField
        End Get
        Set
            Me.yField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class PolyLineEdgeEdgeLabelLabelModel

    Private smartEdgeLabelModelField As PolyLineEdgeEdgeLabelLabelModelSmartEdgeLabelModel

    '''<remarks/>
    Public Property SmartEdgeLabelModel() As PolyLineEdgeEdgeLabelLabelModelSmartEdgeLabelModel
        Get
            Return Me.smartEdgeLabelModelField
        End Get
        Set
            Me.smartEdgeLabelModelField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class PolyLineEdgeEdgeLabelLabelModelSmartEdgeLabelModel

    Private autoRotationEnabledField As Boolean

    Private defaultAngleField As Decimal

    Private defaultDistanceField As Decimal

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property autoRotationEnabled() As Boolean
        Get
            Return Me.autoRotationEnabledField
        End Get
        Set
            Me.autoRotationEnabledField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property defaultAngle() As Decimal
        Get
            Return Me.defaultAngleField
        End Get
        Set
            Me.defaultAngleField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property defaultDistance() As Decimal
        Get
            Return Me.defaultDistanceField
        End Get
        Set
            Me.defaultDistanceField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class PolyLineEdgeEdgeLabelModelParameter

    Private smartEdgeLabelModelParameterField As PolyLineEdgeEdgeLabelModelParameterSmartEdgeLabelModelParameter

    '''<remarks/>
    Public Property SmartEdgeLabelModelParameter() As PolyLineEdgeEdgeLabelModelParameterSmartEdgeLabelModelParameter
        Get
            Return Me.smartEdgeLabelModelParameterField
        End Get
        Set
            Me.smartEdgeLabelModelParameterField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class PolyLineEdgeEdgeLabelModelParameterSmartEdgeLabelModelParameter

    Private angleField As Decimal

    Private distanceField As Decimal

    Private distanceToCenterField As Boolean

    Private positionField As String

    Private ratioField As Decimal

    Private segmentField As SByte

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property angle() As Decimal
        Get
            Return Me.angleField
        End Get
        Set
            Me.angleField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property distance() As Decimal
        Get
            Return Me.distanceField
        End Get
        Set
            Me.distanceField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property distanceToCenter() As Boolean
        Get
            Return Me.distanceToCenterField
        End Get
        Set
            Me.distanceToCenterField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property position() As String
        Get
            Return Me.positionField
        End Get
        Set
            Me.positionField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property ratio() As Decimal
        Get
            Return Me.ratioField
        End Get
        Set
            Me.ratioField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property segment() As SByte
        Get
            Return Me.segmentField
        End Get
        Set
            Me.segmentField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class PolyLineEdgeEdgeLabelPreferredPlacementDescriptor

    Private angleField As Decimal

    Private angleOffsetOnRightSideField As Byte

    Private angleReferenceField As String

    Private angleRotationOnRightSideField As String

    Private distanceField As Decimal

    Private frozenField As Boolean

    Private placementField As String

    Private sideField As String

    Private sideReferenceField As String

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property angle() As Decimal
        Get
            Return Me.angleField
        End Get
        Set
            Me.angleField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property angleOffsetOnRightSide() As Byte
        Get
            Return Me.angleOffsetOnRightSideField
        End Get
        Set
            Me.angleOffsetOnRightSideField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property angleReference() As String
        Get
            Return Me.angleReferenceField
        End Get
        Set
            Me.angleReferenceField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property angleRotationOnRightSide() As String
        Get
            Return Me.angleRotationOnRightSideField
        End Get
        Set
            Me.angleRotationOnRightSideField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property distance() As Decimal
        Get
            Return Me.distanceField
        End Get
        Set
            Me.distanceField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property frozen() As Boolean
        Get
            Return Me.frozenField
        End Get
        Set
            Me.frozenField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property placement() As String
        Get
            Return Me.placementField
        End Get
        Set
            Me.placementField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property side() As String
        Get
            Return Me.sideField
        End Get
        Set
            Me.sideField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property sideReference() As String
        Get
            Return Me.sideReferenceField
        End Get
        Set
            Me.sideReferenceField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.yworks.com/xml/graphml")>
Partial Public Class PolyLineEdgeBendStyle

    Private smoothedField As Boolean

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property smoothed() As Boolean
        Get
            Return Me.smoothedField
        End Get
        Set
            Me.smoothedField = Value
        End Set
    End Property
End Class

'''<remarks/>
<System.SerializableAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code"),
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://graphml.graphdrawing.org/xmlns")>
Partial Public Class graphmlData

    Private resourcesField As Object

    Private keyField As String

    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute([Namespace]:="http://www.yworks.com/xml/graphml")>
    Public Property Resources() As Object
        Get
            Return Me.resourcesField
        End Get
        Set
            Me.resourcesField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property key() As String
        Get
            Return Me.keyField
        End Get
        Set
            Me.keyField = Value
        End Set
    End Property
End Class


