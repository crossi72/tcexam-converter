﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class convertCSV2XML
	Inherits System.Windows.Forms.Form

	'Form esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
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

	'Richiesto da Progettazione Windows Form
	Private components As System.ComponentModel.IContainer

	'NOTA: la procedura che segue è richiesta da Progettazione Windows Form
	'Può essere modificata in Progettazione Windows Form.  
	'Non modificarla mediante l'editor del codice.
	<System.Diagnostics.DebuggerStepThrough()> _
	Private Sub InitializeComponent()
		Me.OpenFileDialog = New System.Windows.Forms.OpenFileDialog()
		Me.Button1 = New System.Windows.Forms.Button()
		Me.SuspendLayout()
		'
		'OpenFileDialog
		'
		Me.OpenFileDialog.FileName = "OpenFileDialog"
		Me.OpenFileDialog.Filter = "csv files|*.csv"
		'
		'Button1
		'
		Me.Button1.Location = New System.Drawing.Point(116, 70)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New System.Drawing.Size(75, 23)
		Me.Button1.TabIndex = 0
		Me.Button1.Text = "Button1"
		Me.Button1.UseVisualStyleBackColor = True
		'
		'convertCSV2XML
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(800, 450)
		Me.Controls.Add(Me.Button1)
		Me.Name = "convertCSV2XML"
		Me.Text = "Form1"
		Me.ResumeLayout(False)

	End Sub

	Friend WithEvents OpenFileDialog As OpenFileDialog
	Friend WithEvents Button1 As Button
End Class
