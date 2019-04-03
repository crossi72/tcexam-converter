<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class convertCSV2XML
	Inherits System.Windows.Forms.Form

	'Form esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
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

	'Richiesto da Progettazione Windows Form
	Private components As System.ComponentModel.IContainer

	'NOTA: la procedura che segue è richiesta da Progettazione Windows Form
	'Può essere modificata in Progettazione Windows Form.  
	'Non modificarla mediante l'editor del codice.
	<System.Diagnostics.DebuggerStepThrough()>
	Private Sub InitializeComponent()
		Me.OpenFileDialog = New System.Windows.Forms.OpenFileDialog()
		Me.btnFileSelection = New System.Windows.Forms.Button()
		Me.SuspendLayout()
		'
		'OpenFileDialog
		'
		Me.OpenFileDialog.FileName = "OpenFileDialog"
		Me.OpenFileDialog.Filter = "csv files|*.csv| xml files|*.xml"
		'
		'btnFileSelection
		'
		Me.btnFileSelection.Location = New System.Drawing.Point(116, 70)
		Me.btnFileSelection.Name = "btnFileSelection"
		Me.btnFileSelection.Size = New System.Drawing.Size(127, 23)
		Me.btnFileSelection.TabIndex = 0
		Me.btnFileSelection.Text = "select file"
		Me.btnFileSelection.UseVisualStyleBackColor = True
		'
		'convertCSV2XML
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(800, 450)
		Me.Controls.Add(Me.btnFileSelection)
		Me.Name = "convertCSV2XML"
		Me.Text = "TCExam converter"
		Me.ResumeLayout(False)

	End Sub

	Friend WithEvents OpenFileDialog As OpenFileDialog
	Friend WithEvents btnFileSelection As Button
End Class
