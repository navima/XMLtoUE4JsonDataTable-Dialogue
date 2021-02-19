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
		Me.GroupBox1 = New System.Windows.Forms.GroupBox()
		Me.logTextBox = New System.Windows.Forms.TextBox()
		Me.GroupBox1.SuspendLayout()
		Me.SuspendLayout()
		'
		'InputFileContainer
		'
		Me.InputFileContainer.AllowDrop = True
		Me.InputFileContainer.BackColor = System.Drawing.SystemColors.ControlLight
		Me.InputFileContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.InputFileContainer.Location = New System.Drawing.Point(12, 12)
		Me.InputFileContainer.Name = "InputFileContainer"
		Me.InputFileContainer.Size = New System.Drawing.Size(342, 426)
		Me.InputFileContainer.TabIndex = 0
		'
		'ConvertButton
		'
		Me.ConvertButton.Location = New System.Drawing.Point(360, 12)
		Me.ConvertButton.Name = "ConvertButton"
		Me.ConvertButton.Size = New System.Drawing.Size(132, 44)
		Me.ConvertButton.TabIndex = 1
		Me.ConvertButton.Text = "Convert"
		Me.ConvertButton.UseVisualStyleBackColor = True
		'
		'GroupBox1
		'
		Me.GroupBox1.Controls.Add(Me.logTextBox)
		Me.GroupBox1.Location = New System.Drawing.Point(361, 63)
		Me.GroupBox1.Name = "GroupBox1"
		Me.GroupBox1.Size = New System.Drawing.Size(427, 375)
		Me.GroupBox1.TabIndex = 2
		Me.GroupBox1.TabStop = False
		Me.GroupBox1.Text = "Log"
		'
		'logTextBox
		'
		Me.logTextBox.Dock = System.Windows.Forms.DockStyle.Fill
		Me.logTextBox.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
		Me.logTextBox.Location = New System.Drawing.Point(3, 19)
		Me.logTextBox.Multiline = True
		Me.logTextBox.Name = "logTextBox"
		Me.logTextBox.ReadOnly = True
		Me.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
		Me.logTextBox.Size = New System.Drawing.Size(421, 353)
		Me.logTextBox.TabIndex = 0
		Me.logTextBox.WordWrap = False
		'
		'Form1
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(800, 450)
		Me.Controls.Add(Me.GroupBox1)
		Me.Controls.Add(Me.ConvertButton)
		Me.Controls.Add(Me.InputFileContainer)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
		Me.Name = "Form1"
		Me.Text = "Form1"
		Me.GroupBox1.ResumeLayout(False)
		Me.GroupBox1.PerformLayout()
		Me.ResumeLayout(False)

	End Sub

	Friend WithEvents InputFileContainer As FlowLayoutPanel
	Friend WithEvents ConvertButton As Button
	Friend WithEvents GroupBox1 As GroupBox
	Friend WithEvents logTextBox As TextBox
End Class
