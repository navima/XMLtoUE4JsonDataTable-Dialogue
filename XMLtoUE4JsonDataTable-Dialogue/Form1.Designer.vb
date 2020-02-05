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
        Me.logTextBox = New System.Windows.Forms.TextBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
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
        'ConvertButton
        '
        Me.ConvertButton.Location = New System.Drawing.Point(335, 12)
        Me.ConvertButton.Name = "ConvertButton"
        Me.ConvertButton.Size = New System.Drawing.Size(453, 191)
        Me.ConvertButton.TabIndex = 1
        Me.ConvertButton.Text = "Convert"
        Me.ConvertButton.UseVisualStyleBackColor = True
        '
        'logTextBox
        '
        Me.logTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.logTextBox.Location = New System.Drawing.Point(3, 16)
        Me.logTextBox.Multiline = True
        Me.logTextBox.Name = "logTextBox"
        Me.logTextBox.ReadOnly = True
        Me.logTextBox.Size = New System.Drawing.Size(447, 210)
        Me.logTextBox.TabIndex = 2
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.logTextBox)
        Me.GroupBox1.Location = New System.Drawing.Point(335, 209)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(453, 229)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Log"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.ConvertButton)
        Me.Controls.Add(Me.InputFileContainer)
        Me.Name = "Form1"
        Me.Text = "graphml XML to UE4 JSON converter"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ConvertButton As Button
    Friend WithEvents InputFileContainer As FlowLayoutPanel
    Friend WithEvents logTextBox As TextBox
    Friend WithEvents GroupBox1 As GroupBox
End Class
