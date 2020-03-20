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
		Me.GroupBox1 = New System.Windows.Forms.GroupBox()
		Me.logTextBox = New System.Windows.Forms.TextBox()
		Me.ConvertButton = New System.Windows.Forms.Button()
		Me.InputFileContainer = New System.Windows.Forms.FlowLayoutPanel()
		Me.GroupBox1.SuspendLayout()
		Me.SuspendLayout()
		'
		'GroupBox1
		'
		Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.GroupBox1.Controls.Add(Me.logTextBox)
		Me.GroupBox1.Location = New System.Drawing.Point(335, 66)
		Me.GroupBox1.Name = "GroupBox1"
		Me.GroupBox1.Size = New System.Drawing.Size(453, 372)
		Me.GroupBox1.TabIndex = 3
		Me.GroupBox1.TabStop = False
		Me.GroupBox1.Text = "Log"
		'
		'logTextBox
		'
		Me.logTextBox.Dock = System.Windows.Forms.DockStyle.Fill
		Me.logTextBox.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.logTextBox.Location = New System.Drawing.Point(3, 16)
		Me.logTextBox.Multiline = True
		Me.logTextBox.Name = "logTextBox"
		Me.logTextBox.ReadOnly = True
		Me.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both
		Me.logTextBox.Size = New System.Drawing.Size(447, 353)
		Me.logTextBox.TabIndex = 2
		Me.logTextBox.WordWrap = False
		'
		'ConvertButton
		'
		Me.ConvertButton.Location = New System.Drawing.Point(335, 12)
		Me.ConvertButton.Name = "ConvertButton"
		Me.ConvertButton.Size = New System.Drawing.Size(130, 48)
		Me.ConvertButton.TabIndex = 1
		Me.ConvertButton.Text = "Convert"
		Me.ConvertButton.UseVisualStyleBackColor = True
		'
		'InputFileContainer
		'
		Me.InputFileContainer.AllowDrop = True
		Me.InputFileContainer.BackColor = System.Drawing.SystemColors.ControlLight
		Me.InputFileContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
		Me.InputFileContainer.Cursor = System.Windows.Forms.Cursors.Default
		Me.InputFileContainer.Location = New System.Drawing.Point(12, 12)
		Me.InputFileContainer.Name = "InputFileContainer"
		Me.InputFileContainer.Size = New System.Drawing.Size(317, 426)
		Me.InputFileContainer.TabIndex = 0
		'
		'Form1
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(800, 450)
		Me.Controls.Add(Me.ConvertButton)
		Me.Controls.Add(Me.InputFileContainer)
		Me.Controls.Add(Me.GroupBox1)
		Me.Name = "Form1"
		Me.Text = "graphml XML to UE4 JSON converter"
		Me.GroupBox1.ResumeLayout(False)
		Me.GroupBox1.PerformLayout()
		Me.ResumeLayout(False)

	End Sub

	Friend WithEvents GroupBox1 As GroupBox
	Friend WithEvents logTextBox As TextBox
	Friend WithEvents ConvertButton As Button
	Friend WithEvents InputFileContainer As FlowLayoutPanel
End Class
