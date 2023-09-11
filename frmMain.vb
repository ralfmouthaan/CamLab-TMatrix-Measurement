' Ralf Mouthaan
' University of Cambridge
' March 2020
'
' T-matrix measurements in the presence of drift.
' This is form plumbing code. Main code resides in modMeasurementRoutines.

Option Explicit On
Option Strict On

Public Class frmMain

    Private Sub DisableControls()

        cmdRandomTest.Enabled = False
        cmdFourierBasis.Enabled = False
        cmdMacropixelBasis.Enabled = False
        cmdHadamardBasis.Enabled = False
        cmdRandomBasis.Enabled = False
        cmdHeartDiamondClubSpade.Enabled = False
        cmbNoMacropixels.Enabled = False
        cmdSmiley.Enabled = False
        Application.DoEvents()

    End Sub
    Private Sub EnableControls()

        cmdRandomTest.Enabled = True
        cmdFourierBasis.Enabled = True
        cmdMacropixelBasis.Enabled = True
        cmdHadamardBasis.Enabled = True
        cmdRandomBasis.Enabled = True
        cmdHeartDiamondClubSpade.Enabled = True
        cmbNoMacropixels.Enabled = True
        cmdSmiley.Enabled = True
        Application.DoEvents()

    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        For i = 1 To 100
            cmbNoMacropixels.Items.Add((i * i).ToString)
        Next
        cmbNoMacropixels.SelectedIndex = 5

    End Sub

    Private Sub cmdMacropixelBasis_Click(sender As Object, e As EventArgs) Handles cmdMacropixelBasis.Click

        Dim lbl As New ToolStripStatusLabel

        'Prep work
        DisableControls()
        modTMatrixMeasurement.NoModes = CInt(cmbNoMacropixels.SelectedItem.ToString)
        modTMatrixMeasurement.NoMeasurements = CInt(nudNoMeasurements.Value)
        statusbar.Items.Add(lbl)
        lbl.Text = "T-Matrix Measurement, Macropixel Basis"
        Application.DoEvents()

        'Set up background worker
        modTMatrixMeasurement.bw = New System.ComponentModel.BackgroundWorker
        modTMatrixMeasurement.bw.WorkerReportsProgress = True
        AddHandler bw.DoWork, AddressOf modTMatrixMeasurement.TMatrixMeasurement_MacropixelBasis
        AddHandler bw.ProgressChanged, Sub(_sender As System.Object, _e As System.ComponentModel.ProgressChangedEventArgs)
                                           lbl.Text = "T-Matrix Measurement, Macropixel Basis - " + (_e.ProgressPercentage / 10).ToString + "%"
                                       End Sub
        AddHandler bw.RunWorkerCompleted, Sub()
                                              EnableControls()
                                              statusbar.Items.Clear()
                                          End Sub

        'Run background worker
        bw.RunWorkerAsync()

    End Sub
    Private Sub cmdHadamardBasis_Click(sender As Object, e As EventArgs) Handles cmdHadamardBasis.Click

        Dim lbl As New ToolStripStatusLabel

        'Prep work
        DisableControls()
        modTMatrixMeasurement.NoModes = CInt(cmbNoMacropixels.SelectedItem.ToString)
        modTMatrixMeasurement.NoMeasurements = CInt(nudNoMeasurements.Value)
        statusbar.Items.Add(lbl)
        lbl.Text = "T-Matrix Measurement, Hadamard Basis"
        Application.DoEvents()

        'Set up background worker
        modTMatrixMeasurement.bw = New System.ComponentModel.BackgroundWorker
        modTMatrixMeasurement.bw.WorkerReportsProgress = True
        AddHandler bw.DoWork, AddressOf modTMatrixMeasurement.TMatrixMeasurement_HadamardBasis
        AddHandler bw.ProgressChanged, Sub(_sender As System.Object, _e As System.ComponentModel.ProgressChangedEventArgs)
                                           lbl.Text = "T-Matrix Measurement, Hadamard Basis - " + (_e.ProgressPercentage / 10).ToString + "%"
                                       End Sub
        AddHandler bw.RunWorkerCompleted, Sub()
                                              EnableControls()
                                              statusbar.Items.Clear()
                                          End Sub

        'Run background worker
        bw.RunWorkerAsync()

    End Sub
    Private Sub cmdFourierBasis_Click(sender As Object, e As EventArgs) Handles cmdFourierBasis.Click

        Dim lbl As New ToolStripStatusLabel

        'Prep work
        DisableControls()
        modTMatrixMeasurement.NoModes = CInt(cmbNoMacropixels.SelectedItem.ToString)
        modTMatrixMeasurement.NoMeasurements = CInt(nudNoMeasurements.Value)
        statusbar.Items.Add(lbl)
        lbl.Text = "T-Matrix Measurement, Fourier Basis"
        Application.DoEvents()

        'Set up background worker
        modTMatrixMeasurement.bw = New System.ComponentModel.BackgroundWorker
        modTMatrixMeasurement.bw.WorkerReportsProgress = True
        AddHandler bw.DoWork, AddressOf modTMatrixMeasurement.TMatrixMeasurement_FourierBasis
        AddHandler bw.ProgressChanged, Sub(_sender As System.Object, _e As System.ComponentModel.ProgressChangedEventArgs)
                                           lbl.Text = "T-Matrix Measurement, Fourier Basis - " + (_e.ProgressPercentage / 10).ToString + "%"
                                       End Sub
        AddHandler bw.RunWorkerCompleted, Sub()
                                              EnableControls()
                                              statusbar.Items.Clear()
                                          End Sub

        'Run background worker
        bw.RunWorkerAsync()

    End Sub
    Private Sub cmdRandomBasis_Click(sender As Object, e As EventArgs) Handles cmdRandomBasis.Click

        Dim lbl As New ToolStripStatusLabel

        'Prep work
        DisableControls()
        modTMatrixMeasurement.NoModes = CInt(cmbNoMacropixels.SelectedItem.ToString)
        modTMatrixMeasurement.NoMeasurements = CInt(nudNoMeasurements.Value)
        statusbar.Items.Add(lbl)
        lbl.Text = "T-Matrix Measurement, Random Basis"
        Application.DoEvents()

        'Set up background worker
        modTMatrixMeasurement.bw = New System.ComponentModel.BackgroundWorker
        modTMatrixMeasurement.bw.WorkerReportsProgress = True
        AddHandler bw.DoWork, AddressOf modTMatrixMeasurement.TMatrixMeasurement_RandomBasis
        AddHandler bw.ProgressChanged, Sub(_sender As System.Object, _e As System.ComponentModel.ProgressChangedEventArgs)
                                           lbl.Text = "T-Matrix Measurement, Random Basis - " + (_e.ProgressPercentage / 10).ToString + "%"
                                       End Sub
        AddHandler bw.RunWorkerCompleted, Sub()
                                              EnableControls()
                                              statusbar.Items.Clear()
                                          End Sub

        'Run background worker
        bw.RunWorkerAsync()

    End Sub

    Private Sub cmdLoadHeartDiamondClubSpade_Click(sender As Object, e As EventArgs) Handles cmdHeartDiamondClubSpade.Click

        Dim Path As String

        DisableControls()
        Path = "C:\Users\ipas.labpcs\Desktop\CamLab-TMatrix-Measurement\32x32 Targets\"
        Call TestMeasurements.DisplayHologramRecordImage(Path + "Club Target - Amplitude.txt", Path + "Club Result - Amplitude.txt")
        Call TestMeasurements.DisplayHologramRecordImage(Path + "Diamond Target - Amplitude.txt", Path + "Diamond Result - Amplitude.txt")
        Call TestMeasurements.DisplayHologramRecordImage(Path + "Heart Target - Amplitude.txt", Path + "Heart Result - Amplitude.txt")
        Call TestMeasurements.DisplayHologramRecordImage(Path + "Spade Target - Amplitude.txt", Path + "Spade Result - Amplitude.txt")
        Call TestMeasurements.DisplayHologramRecordImage(Path + "Club Target - Phase.txt", Path + "Club Result - Phase.txt")
        Call TestMeasurements.DisplayHologramRecordImage(Path + "Diamond Target - Phase.txt", Path + "Diamond Result - Phase.txt")
        Call TestMeasurements.DisplayHologramRecordImage(Path + "Heart Target - Phase.txt", Path + "Heart Result - Phase.txt")
        Call TestMeasurements.DisplayHologramRecordImage(Path + "Spade Target - Phase.txt", Path + "Spade Result - Phase.txt")
        EnableControls()

    End Sub
    Private Sub cmdLoadSmiley_Click(sender As Object, e As EventArgs) Handles cmdSmiley.Click

        Dim dlg As New OpenFileDialog

        dlg.Filter = "Text File|*.txt"
        dlg.CheckFileExists = True
        dlg.InitialDirectory = "C:\Users\Ralf\Desktop\T-Matrix Measurement\bin\Debug\"
        If dlg.ShowDialog = vbCancel Or dlg.FileName = "" Then Exit Sub

        DisableControls()
        Call TestMeasurements.DisplayHologram(dlg.FileName)
        EnableControls()

    End Sub

    Private Sub cmdRandomTest_Click(sender As Object, e As EventArgs) Handles cmdRandomTest.Click

        DisableControls()
        Call TestMeasurements.DisplayRandomHologram(CInt(cmbNoMacropixels.SelectedItem), "Random Hologram.txt", "Random Result.txt")
        EnableControls()

    End Sub
End Class
