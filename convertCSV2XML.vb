
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
	Private Const _customTrailer As String = "_eslTrailer_"

#End Region

#Region " Events management "

	Private Sub btnFileSelection_Click(sender As Object, e As EventArgs) Handles btnFileSelection.Click
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

				Me.Cursor = Cursors.WaitCursor

				Select Case IO.Path.GetExtension(filename).ToLower
					Case ".csv"
						Me.ConvertCSVData(filename)
					Case ".xml"
						Me.ConvertXMLData(filename)
				End Select

				Me.Cursor = Cursors.Default
			End If
		End If
	End Sub

	''' <summary>
	''' Convert data from Moodle XML format to TCExam XML
	''' </summary>
	''' <param name="filename">name of the file to import</param>
	Private Sub ConvertXMLData(filename As String)
		Dim tmpDT As DataTable
		Dim i, importedSubjects, importedQuestions, importedAnswers As Integer
		Dim subject, question, answer As String
		Dim xmlDocument As New Xml.XmlDocument
		Dim xmlDeclaration As Xml.XmlDeclaration
		Dim root, body, tcexamquestions, header, subjectElement, questionElement, answerElement, moduleElement, element As Xml.XmlElement

		tmpDT = Me.ImportMXL(filename, True)

		If tmpDT IsNot Nothing Then
			subject = ""
			question = ""
			importedQuestions = 0
			importedSubjects = 0
			importedAnswers = 0

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

			questionElement = Nothing
			subjectElement = Nothing

			For i = 0 To tmpDT.Rows.Count - 1
				'for each row in table 

				'XML file contains only one subject
				subject = "moodle"
				importedSubjects = 1

				'add subject element
				subjectElement = xmlDocument.CreateElement(String.Empty, "subject", String.Empty)
				moduleElement.AppendChild(subjectElement)

				Me.AddTextNode(xmlDocument, subjectElement, "name", subject)
				Me.AddTextNode(xmlDocument, subjectElement, "enabled", "true")

				If question = tmpDT.Rows(i).Item("questionText").ToString.Replace("""", "") Then
					'same question of previous rows
				Else
					'question has changed
					importedQuestions += 1

					question = tmpDT.Rows(i).Item("questionText").ToString.Replace("""", "")
					'question = Me.AddHTMLEscapeSequences(question)

					'add question element
					questionElement = xmlDocument.CreateElement(String.Empty, "question", String.Empty)
					subjectElement.AppendChild(questionElement)

					Me.AddTextNode(xmlDocument, questionElement, "description", Me.AddHTMLEscapeSequences(question))
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

				answer = tmpDT.Rows(i).Item("answerText").ToString.Replace("""", "")
				answer = Me.AddHTMLEscapeSequences(answer)
				importedAnswers += 1

				'add answer element
				answerElement = xmlDocument.CreateElement(String.Empty, "answer", String.Empty)
				questionElement.AppendChild(answerElement)

				Me.AddTextNode(xmlDocument, answerElement, "enabled", "true")
				Me.AddTextNode(xmlDocument, answerElement, "isright", tmpDT.Rows(i).Item("isCorrect").ToString.Replace("""", ""))
				Me.AddTextNode(xmlDocument, answerElement, "position", "1")
				Me.AddTextNode(xmlDocument, answerElement, "keyboard_key", "")
				Me.AddTextNode(xmlDocument, answerElement, "description", answer)
				Me.AddTextNode(xmlDocument, answerElement, "explanation", "")
			Next

			xmlDocument.Save("TcExam.xml")

			MessageBox.Show(importedSubjects & " Subjects imported" & ControlChars.CrLf & importedQuestions & " questions imported" & ControlChars.CrLf & importedAnswers & " answers imported")
		End If
	End Sub

	''' <summary>
	''' Convert data from CSV format to XML
	''' </summary>
	''' <param name="filename">name of the file to import</param>
	Private Sub ConvertCSVData(filename As String)
		Dim tmpDT As DataTable
		Dim i, importedSubjects, importedQuestions, importedAnswers As Integer
		Dim subject, question, answer As String
		Dim xmlDocument As New Xml.XmlDocument
		Dim xmlDeclaration As Xml.XmlDeclaration
		Dim root, body, tcexamquestions, header, subjectElement, questionElement, answerElement, moduleElement, element As Xml.XmlElement

		tmpDT = Me.ImportCSV(filename, enumSeparatore.semicolon, True)

		If tmpDT IsNot Nothing Then
			subject = ""
			question = ""
			importedQuestions = 0
			importedSubjects = 0
			importedAnswers = 0

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

			questionElement = Nothing
			subjectElement = Nothing

			For i = 0 To tmpDT.Rows.Count - 1
				'for each row in table 

				If subject = tmpDT.Rows(i).Item("subject_name").ToString.Replace("""", "") Then
					'same subject of previous rows
				Else
					'subject has changed
					importedSubjects += 1
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
					importedQuestions += 1

					question = tmpDT.Rows(i).Item("question_description").ToString.Replace("""", "")
					'question = Me.AddHTMLEscapeSequences(question)

					'add question element
					questionElement = xmlDocument.CreateElement(String.Empty, "question", String.Empty)
					subjectElement.AppendChild(questionElement)

					Me.AddTextNode(xmlDocument, questionElement, "description", Me.AddHTMLEscapeSequences(question))
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
				answer = Me.AddHTMLEscapeSequences(answer)
				importedAnswers += 1

				'add answer element
				answerElement = xmlDocument.CreateElement(String.Empty, "answer", String.Empty)
				questionElement.AppendChild(answerElement)

				Me.AddTextNode(xmlDocument, answerElement, "enabled", "true")
				Me.AddTextNode(xmlDocument, answerElement, "isright", tmpDT.Rows(i).Item("answer_isright").ToString.Replace("""", ""))
				Me.AddTextNode(xmlDocument, answerElement, "position", "1")
				Me.AddTextNode(xmlDocument, answerElement, "keyboard_key", "")
				Me.AddTextNode(xmlDocument, answerElement, "description", answer)
				Me.AddTextNode(xmlDocument, answerElement, "explanation", "")
			Next

			xmlDocument.Save("TcExam.xml")

			MessageBox.Show(importedSubjects & " Subjects imported" & ControlChars.CrLf & importedQuestions & " questions imported" & ControlChars.CrLf & importedAnswers & " answers imported")
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
	''' read a XML file and put the content into a datatable
	''' </summary>
	''' <param name="fileName">name of the file to import</param>
	''' <returns>tabella contenente i dati importati</returns>
	''' <remarks></remarks>
	Private Function ImportMXL(ByVal fileName As String, ByVal dropEmptyRows As Boolean) As DataTable
		Dim myFile As XDocument
		Dim question, answer As XElement
		Dim questionText, answerText As String
		Dim isCorrect As Boolean
		Dim result As New DataTable

		Try
			myFile = XDocument.Load(fileName)
			result.Columns.Add("questionText")
			result.Columns.Add("answerText")
			result.Columns.Add("isCorrect")

			For Each question In myFile.Descendants("question")
				questionText = question.Descendants("questiontext").Descendants("text").Value
				If questionText <> "" Then
					For Each answer In question.Descendants("answer")
						answerText = answer.Descendants("text").Value
						If answer.Attribute("fraction").Value = "100" Then
							isCorrect = True
						Else
							isCorrect = False
						End If

						result.Rows.Add(questionText, answerText, isCorrect)
					Next
				End If
			Next

		Catch ex As IO.IOException
			Throw New Exception("error accessing file" & fileName)
		End Try

		Return result
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
	Private Function AddHTMLEscapeSequences(value As String) As String
		Dim result As String

		result = value

		result = result.Replace(_customHeader & "quot" & _customTrailer, "&quot;")
		result = result.Replace(_customHeader & "rsquo" & _customTrailer, "&rsquo;")
		result = result.Replace(_customHeader & "lsquo" & _customTrailer, "&lsquo;")
		result = result.Replace(_customHeader & "tilde" & _customTrailer, "&tilde;")
		result = result.Replace(_customHeader & "agrave" & _customTrailer, "&agrave;")
		result = result.Replace(_customHeader & "egrave" & _customTrailer, "&egrave;")
		result = result.Replace(_customHeader & "Egrave" & _customTrailer, "&Egrave;")
		result = result.Replace(_customHeader & "eacute" & _customTrailer, "&eacute;")
		result = result.Replace(_customHeader & "ograve" & _customTrailer, "&ograve;")
		result = result.Replace(_customHeader & "ugrave" & _customTrailer, "&ugrave;")
		result = result.Replace(_customHeader & "igrave" & _customTrailer, "&igrave;")
		result = result.Replace(_customHeader & "semicolon" & _customTrailer, "$amp;")
		result = result.Replace(_customHeader & "larr" & _customTrailer, "&larr;")
		result = result.Replace(_customHeader & "times" & _customTrailer, "&times;")
		result = result.Replace(_customHeader & "pigreco" & _customTrailer, "&pi;")
		result = result.Replace(_customHeader & "frac12" & _customTrailer, "&frac12;")
		result = result.Replace(_customHeader & "frac14" & _customTrailer, "&frac14;")
		result = result.Replace(_customHeader & "frac34" & _customTrailer, "&frac34;")
		result = result.Replace(_customHeader & "sup2" & _customTrailer, "&sup2;")
		result = result.Replace(_customHeader & "sup3" & _customTrailer, "&sup3;")
		result = result.Replace(_customHeader & "ampgt" & _customTrailer, "&amp;gt;")
		result = result.Replace(_customHeader & "ltamp" & _customTrailer, "&lt;&amp;")
		result = result.Replace(_customHeader & "gt" & _customTrailer, "&gt;")
		result = result.Replace(_customHeader & "lt" & _customTrailer, "&lt;")
		result = result.Replace(_customHeader & "ltampgt" & _customTrailer, "&lt;&amp;;&gt;")
		result = result.Replace(_customHeader & "doubleand" & _customTrailer, "&amp;amp;")
		result = result.Replace(_customHeader & "and" & _customTrailer, "&amp;")

		Return result
	End Function

	''' <summary>
	''' Replace HTML escape sequences to avoid problems with csv separator
	''' </summary>
	''' <param name="value">string to clean</param>
	''' <returns></returns>
	Private Function RemoveHTMLEscapeSequences(value As String) As String
		Dim result As String

		'remove unnecessary "s
		result = value.Replace("""", "")

		result = result.Replace("&quot;", _customHeader & "quot" & _customTrailer)
		result = result.Replace("&rsquo;", _customHeader & "rsquo" & _customTrailer)
		result = result.Replace("&lsquo;", _customHeader & "lsquo" & _customTrailer)
		result = result.Replace("&tilde;", _customHeader & "tilde" & _customTrailer)
		result = result.Replace("&agrave;", _customHeader & "agrave" & _customTrailer)
		result = result.Replace("&egrave;", _customHeader & "egrave" & _customTrailer)
		result = result.Replace("&Egrave;", _customHeader & "Egrave" & _customTrailer)
		result = result.Replace("&eacute;", _customHeader & "eacute" & _customTrailer)
		result = result.Replace("&ograve;", _customHeader & "ograve" & _customTrailer)
		result = result.Replace("&ugrave;", _customHeader & "ugrave" & _customTrailer)
		result = result.Replace("&igrave;", _customHeader & "igrave" & _customTrailer)
		result = result.Replace("&lt;&amp;;&gt;", _customHeader & "ltampgt" & _customTrailer) 'due to a strange response to HTML5 question
		result = result.Replace("$amp;", _customHeader & "semicolon" & _customTrailer)
		result = result.Replace("&larr;", _customHeader & "larr" & _customTrailer)
		result = result.Replace("&times;", _customHeader & "times" & _customTrailer)
		result = result.Replace("&amp;gt;", _customHeader & "ampgt" & _customTrailer)
		result = result.Replace("&gt;", _customHeader & "gt" & _customTrailer)
		result = result.Replace("&lt;&amp;", _customHeader & "ltamp" & _customTrailer)
		result = result.Replace("&lt;", _customHeader & "lt" & _customTrailer)
		result = result.Replace("&pi;", _customHeader & "pigreco" & _customTrailer)
		result = result.Replace("&frac12;", _customHeader & "frac12" & _customTrailer)
		result = result.Replace("&frac14;", _customHeader & "frac14" & _customTrailer)
		result = result.Replace("&frac34;", _customHeader & "frac34" & _customTrailer)
		result = result.Replace("&sup2;", _customHeader & "sup2" & _customTrailer)
		result = result.Replace("&sup3;", _customHeader & "sup3" & _customTrailer)
		result = result.Replace("&amp;amp;", _customHeader & "doubleand" & _customTrailer)
		result = result.Replace("&amp;", _customHeader & "and" & _customTrailer)

		Return result
	End Function


#End Region

End Class
