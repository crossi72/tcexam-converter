
#Region " Tipi "

''' <summary>
''' separatore da usare per i file CSV
''' </summary>
''' <remarks></remarks>
Public Enum enumSeparatore
	pipe = 1
	semicolon = 2
	comma = 3
	tab = 4
End Enum

#End Region

Public Class convertCSV2XML

#Region " Gestori di evento "

	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
		Me.Openfile()
	End Sub

#End Region

#Region " Metodi privati "

	''' <summary>
	''' Select CSV file
	''' </summary>
	Private Sub Openfile()
		Dim result As DialogResult
		Dim filename As String

		result = Me.OpenFileDialog.ShowDialog()

		If result = DialogResult.OK Then
			'user confirmed selection
			filename = Me.OpenFileDialog.FileName

			If filename <> "" Then
				'user selected a file

				Me.ConvertData(filename)
			End If
		End If
	End Sub

	''' <summary>
	''' Convert data from CSV format to XML
	''' </summary>
	''' <param name="filename">name of the file to import</param>
	Private Sub ConvertData(filename As String)
		Dim tmpDT As DataTable
		Dim i As Integer
		Dim subject As String
		Dim serializer As Xml.Serialization.XmlSerializer
		Dim xmlDocument As New Xml.XmlDocument
		Dim xmlDeclaration As Xml.XmlDeclaration
		Dim root, element As Xml.XmlElement

		tmpDT = Me.ImportCSV(filename, enumSeparatore.semicolon, True)

		If tmpDT IsNot Nothing Then
			subject = ""

			'init XML structure
			xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", "")
			root = xmlDocument.DocumentElement
			xmlDocument.InsertBefore(xmlDeclaration, root)

			element = xmlDocument.CreateElement(String.Empty, "body", String.Empty)
			xmlDocument.AppendChild(element)

			For i = 0 To tmpDT.Rows.Count - 1
				'for esch row in table 
				If subject = tmpDT.Rows(i).Item("subject_name").ToString Then
					'same subject of previous rows
				Else
					'subject has changed

				End If
			Next
		End If
	End Sub

	''' <summary>
	''' read a CSV file and put the content into a datatable
	''' </summary>
	''' <param name="fileName">name of the file to import</param>
	''' <param name="separator">separator</param>
	''' <returns>tabella contenente i dati importati</returns>
	''' <remarks></remarks>
	Private Function ImportCSV(ByVal fileName As String, ByVal separator As enumSeparatore, ByVal dropEmptyRows As Boolean) As DataTable
		Dim myFile As IO.StreamReader
		Dim fileContent, sep, columnName As String
		Dim result As New DataTable
		Dim row As String()

		sep = ""
		Select Case separator
			Case enumSeparatore.pipe
				sep = "|"
			Case enumSeparatore.semicolon
				sep = ";"
			Case enumSeparatore.comma
				sep = ","
		End Select

		Try
			myFile = New IO.StreamReader(fileName)
			fileContent = myFile.ReadLine

			row = fileContent.Split(sep(0))

			For Each columnName In row
				'removes unnecessary "
				columnName = columnName.Replace("""", "")

				'add column to table structure
				result.Columns.Add(columnName)
			Next

			Do
				fileContent = myFile.ReadLine
				If dropEmptyRows Then
					While fileContent IsNot Nothing AndAlso fileContent = ""
						fileContent = myFile.ReadLine
					End While
				End If
				If fileContent IsNot Nothing AndAlso fileContent <> "" Then

					fileContent = RemoveHTMLEscapeSequences(fileContent)

					row = fileContent.Split(sep(0))
					result.Rows.Add(row)
				End If
			Loop While fileContent <> Nothing

		Catch ex As IO.IOException
			Throw New Exception("error accessing file" & fileName)
		End Try

		Return result
	End Function

	''' <summary>
	''' Replace HTML escape sequences to avoid problems with csv separator
	''' </summary>
	''' <param name="value">string to clean</param>
	''' <returns></returns>
	Private Function RemoveHTMLEscapeSequences(value As String) As String
		Dim result As String

		result = value

		result = result.Replace("&quot;", "'")
		result = result.Replace("&rsquo;", "'")
		result = result.Replace("&lsquo;", "'")
		result = result.Replace("&tilde;", "~")
		result = result.Replace("&agrave;", "à")
		result = result.Replace("&egrave;", "è")
		result = result.Replace("&Egrave;", "È")
		result = result.Replace("&eacute;", "é")
		result = result.Replace("&ograve;", "ò")
		result = result.Replace("&ugrave;", "ù")
		result = result.Replace("&igrave;", "ì")
		result = result.Replace("&amp;", "&")
		result = result.Replace("&larr;", "<--")
		result = result.Replace("&times;", "x")
		result = result.Replace("&gt;", ">")
		result = result.Replace("&lt;", "<")
		result = result.Replace("&pi;", "pi greco")
		result = result.Replace("&frac12;", "1/2")
		result = result.Replace("&frac14;", "1/4")
		result = result.Replace("&frac34;", "3/4")

		Return result
	End Function

#End Region

End Class
