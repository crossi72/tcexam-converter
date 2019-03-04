
#Region " Data types "

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

#Region " Contants "

	Private Const _customHeader As String = "_eslHeader_"
	Private Const _customTrailer As String = "_eslHeader_"

#End Region

#Region " Events management "

	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnFileSelection.Click
		Me.Openfile()
	End Sub

#End Region

#Region " Private methods "

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
		Dim subject, question, answer As String
		Dim xmlDocument As New Xml.XmlDocument
		Dim xmlDeclaration As Xml.XmlDeclaration
		Dim root, body, tcexamquestions, header, subjectElement, questionElement, answerElement, moduleElement, element As Xml.XmlElement

		tmpDT = Me.ImportCSV(filename, enumSeparatore.semicolon, True)

		If tmpDT IsNot Nothing Then
			subject = ""
			question = ""

			'init XML structure
			xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", "")
			root = xmlDocument.DocumentElement
			xmlDocument.InsertBefore(xmlDeclaration, root)

			'add tcexamquestions element
			tcexamquestions = xmlDocument.CreateElement(String.Empty, "tcexamquestions", String.Empty)
			tcexamquestions.SetAttribute("Version", "12.2.1")
			xmlDocument.AppendChild(tcexamquestions)

			'add header element
			header = xmlDocument.CreateElement(String.Empty, "header", String.Empty)
			header.SetAttribute("lang", "it")
			header.SetAttribute("date", Now.Year & "-" & Now.Month & Now.Day & " " & Now.ToLongTimeString)
			tcexamquestions.AppendChild(header)

			'add body element
			body = xmlDocument.CreateElement(String.Empty, "body", String.Empty)
			tcexamquestions.AppendChild(body)

			'add module element
			moduleElement = xmlDocument.CreateElement(String.Empty, "module", String.Empty)
			body.AppendChild(moduleElement)

			Me.AddTextNode(xmlDocument, moduleElement, "name", "Modulo per esame")

			Me.AddTextNode(xmlDocument, moduleElement, "enabled", "true")

			For i = 0 To tmpDT.Rows.Count - 1
				'for each row in table 

				If subject = tmpDT.Rows(i).Item("subject_name").ToString.Replace("""", "") Then
					'same subject of previous rows
				Else
					'subject has changed
					subject = tmpDT.Rows(i).Item("subject_name").ToString.Replace("""", "")

					'add subject element
					subjectElement = xmlDocument.CreateElement(String.Empty, "subject", String.Empty)
					moduleElement.AppendChild(subjectElement)

					Me.AddTextNode(xmlDocument, subjectElement, "name", subject)
					Me.AddTextNode(xmlDocument, subjectElement, "enabled", "true")
				End If

				If question = tmpDT.Rows(i).Item("question_description").ToString.Replace("""", "") Then
					'same question of previous rows
				Else
					'question has changed
					question = tmpDT.Rows(i).Item("question_description").ToString.Replace("""", "")

					'add question element
					questionElement = xmlDocument.CreateElement(String.Empty, "question", String.Empty)
					moduleElement.AppendChild(questionElement)

					Me.AddTextNode(xmlDocument, questionElement, "description", question)
					Me.AddTextNode(xmlDocument, questionElement, "enabled", "true")
					Me.AddTextNode(xmlDocument, questionElement, "type", "single")
					Me.AddTextNode(xmlDocument, questionElement, "difficulty", "1")
					Me.AddTextNode(xmlDocument, questionElement, "position", "1")
					Me.AddTextNode(xmlDocument, questionElement, "timer", "0")
					Me.AddTextNode(xmlDocument, questionElement, "fullscreen", "False")
					Me.AddTextNode(xmlDocument, questionElement, "inline_answers", "false")
					Me.AddTextNode(xmlDocument, questionElement, "auto_next", "false")
					Me.AddTextNode(xmlDocument, questionElement, "explanation", "")
				End If

				answer = tmpDT.Rows(i).Item("answer_description").ToString.Replace("""", "")

				'add answer element
				answerElement = xmlDocument.CreateElement(String.Empty, "answer", String.Empty)
				moduleElement.AppendChild(answerElement)

				Me.AddTextNode(xmlDocument, answerElement, "enabled", "true")
				Me.AddTextNode(xmlDocument, answerElement, "isright", tmpDT.Rows(i).Item("answer_isright").ToString.Replace("""", ""))
				Me.AddTextNode(xmlDocument, answerElement, "position", "1")
				Me.AddTextNode(xmlDocument, answerElement, "keyboard_key", "")
				Me.AddTextNode(xmlDocument, answerElement, "description", answer)
				Me.AddTextNode(xmlDocument, answerElement, "explanation", "")
			Next

			xmlDocument.Save("test.xml")
		End If
	End Sub

	''' <summary>
	''' Add a text node to an xml element
	''' </summary>
	''' <param name="document">xml document</param>
	''' <param name="parent">xml node to add element to</param>
	''' <param name="name">name of new xml node</param>
	''' <param name="value">value of new xml node</param>
	''' <returns></returns>
	Private Function AddTextNode(document As Xml.XmlDocument, parent As Xml.XmlElement, name As String, value As String) As Xml.XmlText
		Dim element As Xml.XmlElement
		Dim textNode As Xml.XmlText

		element = document.CreateElement(String.Empty, name, String.Empty)
		textNode = document.CreateTextNode(value)
		element.AppendChild(textNode)
		parent.AppendChild(element)

		Return textNode
	End Function

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
		'Dim RegexObj As New Text.RegularExpressions.Regex("&.*?;")

		'remove unnecessary "s
		result = value.Replace("""", "")

		'result = RegexObj.Replace(result, "test")

		result = result.Replace("&quot;", "'")
		result = result.Replace("&rsquo;", "'")
		result = result.Replace("&lsquo;", "'")
		result = result.Replace("&tilde;", "~")
		result = result.Replace("&agrave;", "à")
		result = result.Replace("&egrave;", "è")
		result = result.Replace("&egrave;", "è")
		result = result.Replace("&Egrave;", "È")
		result = result.Replace("&eacute;", "é")
		result = result.Replace("&ograve;", "ò")
		result = result.Replace("&ugrave;", "ù")
		result = result.Replace("&igrave;", "ì")
		result = result.Replace("&lt;&amp;;&gt;", "<&>") 'due to a strange response to HTML5 question
		result = result.Replace("$amp;", _customHeader & "semicolon" & _customTrailer) 'due to a strange response to HTML5 question
		result = result.Replace("&amp;amp;", _customHeader & "&" & _customTrailer)
		result = result.Replace("&larr;", _customHeader & "larr" & _customTrailer)
		result = result.Replace("&times;", _customHeader & "times" & _customTrailer)
		result = result.Replace("&amp;gt;", _customHeader & "gt" & _customTrailer)
		result = result.Replace("&gt;", ">")
		result = result.Replace("&lt;&amp;", _customHeader & "lt" & _customTrailer)
		result = result.Replace("&lt;", "<")
		result = result.Replace("&pi;", _customHeader & "pigreco" & _customTrailer)
		result = result.Replace("&frac12;", _customHeader & "frac12" & _customTrailer)
		result = result.Replace("&frac14;", _customHeader & "frac14" & _customTrailer)
		result = result.Replace("&frac34;", _customHeader & "frac34" & _customTrailer)
		result = result.Replace("&sup2;", _customHeader & "sup2" & _customTrailer)
		result = result.Replace("&sup3;", _customHeader & "sup3" & _customTrailer)
		result = result.Replace("&amp;", "&")

		Return result
	End Function

#End Region

End Class
