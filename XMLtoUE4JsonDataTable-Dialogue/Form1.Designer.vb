<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.InputFileContainer = New System.Windows.Forms.FlowLayoutPanel()
        Me.ConvertButton = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'InputFileContainer
        '
        Me.InputFileContainer.AllowDrop = True
        Me.InputFileContainer.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.InputFileContainer.BackColor = System.Drawing.SystemColors.ControlLight
        Me.InputFileContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.InputFileContainer.Cursor = System.Windows.Forms.Cursors.Default
        Me.InputFileContainer.Location = New System.Drawing.Point(12, 12)
        Me.InputFileContainer.Name = "InputFileContainer"
        Me.InputFileContainer.Size = New System.Drawing.Size(317, 426)
        Me.InputFileContainer.TabIndex = 0
        '
        'ConvertButton
        '
        Me.ConvertButton.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ConvertButton.Location = New System.Drawing.Point(335, 12)
        Me.ConvertButton.Name = "ConvertButton"
        Me.ConvertButton.Size = New System.Drawing.Size(453, 426)
        Me.ConvertButton.TabIndex = 1
        Me.ConvertButton.Text = "Convert"
        Me.ConvertButton.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.ConvertButton)
        Me.Controls.Add(Me.InputFileContainer)
        Me.Name = "Form1"
        Me.Text = "graphml XML to UE4 JSON converter"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ConvertButton As Button
    Friend WithEvents InputFileContainer As FlowLayoutPanel
End Class
