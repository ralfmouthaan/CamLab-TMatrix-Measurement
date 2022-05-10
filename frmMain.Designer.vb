<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.lblNoMacropixels = New System.Windows.Forms.Label()
        Me.cmbNoMacropixels = New System.Windows.Forms.ComboBox()
        Me.cmdMacropixelBasis = New System.Windows.Forms.Button()
        Me.cmdHadamardBasis = New System.Windows.Forms.Button()
        Me.statusbar = New System.Windows.Forms.StatusStrip()
        Me.cmdHeartDiamondClubSpade = New System.Windows.Forms.Button()
        Me.cmdFourierBasis = New System.Windows.Forms.Button()
        Me.cmdRandomBasis = New System.Windows.Forms.Button()
        Me.lblNoMeasurements = New System.Windows.Forms.Label()
        Me.nudNoMeasurements = New System.Windows.Forms.NumericUpDown()
        Me.cmdSmiley = New System.Windows.Forms.Button()
        Me.cmdRandomTest = New System.Windows.Forms.Button()
        CType(Me.nudNoMeasurements, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblNoMacropixels
        '
        Me.lblNoMacropixels.AutoSize = True
        Me.lblNoMacropixels.Location = New System.Drawing.Point(25, 26)
        Me.lblNoMacropixels.Name = "lblNoMacropixels"
        Me.lblNoMacropixels.Size = New System.Drawing.Size(83, 13)
        Me.lblNoMacropixels.TabIndex = 1
        Me.lblNoMacropixels.Text = "No Macropixels:"
        '
        'cmbNoMacropixels
        '
        Me.cmbNoMacropixels.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmbNoMacropixels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbNoMacropixels.FormattingEnabled = True
        Me.cmbNoMacropixels.Location = New System.Drawing.Point(127, 24)
        Me.cmbNoMacropixels.Name = "cmbNoMacropixels"
        Me.cmbNoMacropixels.Size = New System.Drawing.Size(216, 21)
        Me.cmbNoMacropixels.TabIndex = 2
        '
        'cmdMacropixelBasis
        '
        Me.cmdMacropixelBasis.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdMacropixelBasis.Location = New System.Drawing.Point(28, 77)
        Me.cmdMacropixelBasis.Name = "cmdMacropixelBasis"
        Me.cmdMacropixelBasis.Size = New System.Drawing.Size(315, 30)
        Me.cmdMacropixelBasis.TabIndex = 3
        Me.cmdMacropixelBasis.Text = "Macropixel Basis"
        Me.cmdMacropixelBasis.UseVisualStyleBackColor = True
        '
        'cmdHadamardBasis
        '
        Me.cmdHadamardBasis.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdHadamardBasis.Location = New System.Drawing.Point(28, 149)
        Me.cmdHadamardBasis.Name = "cmdHadamardBasis"
        Me.cmdHadamardBasis.Size = New System.Drawing.Size(315, 30)
        Me.cmdHadamardBasis.TabIndex = 4
        Me.cmdHadamardBasis.Text = "Hadamard Basis"
        Me.cmdHadamardBasis.UseVisualStyleBackColor = True
        '
        'statusbar
        '
        Me.statusbar.Location = New System.Drawing.Point(0, 385)
        Me.statusbar.Name = "statusbar"
        Me.statusbar.Size = New System.Drawing.Size(367, 22)
        Me.statusbar.TabIndex = 5
        Me.statusbar.Text = "StatusStrip1"
        '
        'cmdHeartDiamondClubSpade
        '
        Me.cmdHeartDiamondClubSpade.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdHeartDiamondClubSpade.Location = New System.Drawing.Point(28, 292)
        Me.cmdHeartDiamondClubSpade.Name = "cmdHeartDiamondClubSpade"
        Me.cmdHeartDiamondClubSpade.Size = New System.Drawing.Size(315, 30)
        Me.cmdHeartDiamondClubSpade.TabIndex = 6
        Me.cmdHeartDiamondClubSpade.Text = "Load Heart / Diamond / Club / Spade Imaging Tests"
        Me.cmdHeartDiamondClubSpade.UseVisualStyleBackColor = True
        '
        'cmdFourierBasis
        '
        Me.cmdFourierBasis.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdFourierBasis.Location = New System.Drawing.Point(28, 113)
        Me.cmdFourierBasis.Name = "cmdFourierBasis"
        Me.cmdFourierBasis.Size = New System.Drawing.Size(315, 30)
        Me.cmdFourierBasis.TabIndex = 8
        Me.cmdFourierBasis.Text = "Fourier Basis"
        Me.cmdFourierBasis.UseVisualStyleBackColor = True
        '
        'cmdRandomBasis
        '
        Me.cmdRandomBasis.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdRandomBasis.Location = New System.Drawing.Point(28, 185)
        Me.cmdRandomBasis.Name = "cmdRandomBasis"
        Me.cmdRandomBasis.Size = New System.Drawing.Size(315, 30)
        Me.cmdRandomBasis.TabIndex = 9
        Me.cmdRandomBasis.Text = "Random Basis"
        Me.cmdRandomBasis.UseVisualStyleBackColor = True
        '
        'lblNoMeasurements
        '
        Me.lblNoMeasurements.AutoSize = True
        Me.lblNoMeasurements.Location = New System.Drawing.Point(25, 53)
        Me.lblNoMeasurements.Name = "lblNoMeasurements"
        Me.lblNoMeasurements.Size = New System.Drawing.Size(96, 13)
        Me.lblNoMeasurements.TabIndex = 10
        Me.lblNoMeasurements.Text = "No Measurements:"
        '
        'nudNoMeasurements
        '
        Me.nudNoMeasurements.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.nudNoMeasurements.Location = New System.Drawing.Point(127, 51)
        Me.nudNoMeasurements.Maximum = New Decimal(New Integer() {50000, 0, 0, 0})
        Me.nudNoMeasurements.Name = "nudNoMeasurements"
        Me.nudNoMeasurements.Size = New System.Drawing.Size(216, 20)
        Me.nudNoMeasurements.TabIndex = 11
        Me.nudNoMeasurements.Value = New Decimal(New Integer() {400, 0, 0, 0})
        '
        'cmdSmiley
        '
        Me.cmdSmiley.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdSmiley.Location = New System.Drawing.Point(28, 328)
        Me.cmdSmiley.Name = "cmdSmiley"
        Me.cmdSmiley.Size = New System.Drawing.Size(315, 30)
        Me.cmdSmiley.TabIndex = 12
        Me.cmdSmiley.Text = "Load Projection Test"
        Me.cmdSmiley.UseVisualStyleBackColor = True
        '
        'cmdRandomTest
        '
        Me.cmdRandomTest.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdRandomTest.Location = New System.Drawing.Point(28, 259)
        Me.cmdRandomTest.Name = "cmdRandomTest"
        Me.cmdRandomTest.Size = New System.Drawing.Size(315, 30)
        Me.cmdRandomTest.TabIndex = 13
        Me.cmdRandomTest.Text = "Load Random Imaging Test"
        Me.cmdRandomTest.UseVisualStyleBackColor = True
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(367, 407)
        Me.Controls.Add(Me.cmdRandomTest)
        Me.Controls.Add(Me.cmdSmiley)
        Me.Controls.Add(Me.nudNoMeasurements)
        Me.Controls.Add(Me.lblNoMeasurements)
        Me.Controls.Add(Me.cmdRandomBasis)
        Me.Controls.Add(Me.cmdFourierBasis)
        Me.Controls.Add(Me.cmdHeartDiamondClubSpade)
        Me.Controls.Add(Me.statusbar)
        Me.Controls.Add(Me.cmdHadamardBasis)
        Me.Controls.Add(Me.cmdMacropixelBasis)
        Me.Controls.Add(Me.cmbNoMacropixels)
        Me.Controls.Add(Me.lblNoMacropixels)
        Me.Name = "frmMain"
        Me.Text = "RPM Transmission Matrix Measurement"
        CType(Me.nudNoMeasurements, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblNoMacropixels As Label
    Friend WithEvents cmbNoMacropixels As ComboBox
    Friend WithEvents cmdMacropixelBasis As Button
    Friend WithEvents cmdHadamardBasis As Button
    Friend WithEvents statusbar As StatusStrip
    Friend WithEvents cmdHeartDiamondClubSpade As Button
    Friend WithEvents cmdFourierBasis As Button
    Friend WithEvents cmdRandomBasis As Button
    Friend WithEvents lblNoMeasurements As Label
    Friend WithEvents nudNoMeasurements As NumericUpDown
    Friend WithEvents cmdSmiley As Button
    Friend WithEvents cmdRandomTest As Button
End Class
