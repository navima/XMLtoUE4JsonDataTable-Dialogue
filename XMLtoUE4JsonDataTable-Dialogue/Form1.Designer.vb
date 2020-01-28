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
        Me.InputsFilesContainer = New System.Windows.Forms.FlowLayoutPanel()
        Me.ConvertButton = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'InputsFilesContainer
        '
        Me.InputsFilesContainer.AllowDrop = True
        Me.InputsFilesContainer.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.InputsFilesContainer.BackColor = System.Drawing.SystemColors.ControlLight
        Me.InputsFilesContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.InputsFilesContainer.Cursor = System.Windows.Forms.Cursors.Default
        Me.InputsFilesContainer.Location = New System.Drawing.Point(12, 12)
        Me.InputsFilesContainer.Name = "InputsFilesContainer"
        Me.InputsFilesContainer.Size = New System.Drawing.Size(317, 426)
        Me.InputsFilesContainer.TabIndex = 0
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
        Me.Controls.Add(Me.InputsFilesContainer)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents InputsFilesContainer As FlowLayoutPanel
    Friend WithEvents ConvertButton As Button
End Class
