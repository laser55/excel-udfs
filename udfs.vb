'  Will Lee-Wagner
'  whentheresawill.net
'  2014-03-14
'
'  This is a set of public functions and subs for Excel
'  You can put these in the VBA module of a workbook or Excel add-in
'  to add their functionality, or go to whentheresawill.net/code
'  to download a pre-generated add-in.
'
'  Some of these require extra steps to use (such as adding fields or
'  shortcuts. See the REQUIRES field in the comments for more instructions.
'  ------------------------------------------------------------
'
'  This program is free software: you can redistribute it and/or modify
'  it under the terms of the GNU General Public License as published by
'  the Free Software Foundation, either version 3 of the License, or
'  (at your option) any later version.
'
'  This program is distributed in the hope that it will be useful,
'  but WITHOUT ANY WARRANTY; without even the implied warranty of
'  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'  GNU General Public License for more details.
'
'  You should have received a copy of the GNU General Public License
'  along with this program.  If not, see <http://www.gnu.org/licenses/>.

Public Function RandomName(nameType As Boolean) As String
    ' USE:              to place random first and last names in a cell
    '                   from a list of the top 100 American names
    ' nameType:         True for last names, false for first names
    ' OUTPUT:           The chosen name
    ' REQUIRES:         a worksheet called "Names" with name to choose
    '                   first names should be in col 1 and last names in 2
    Dim col As Integer
    Dim nameChooser As Integer
    Dim UPPER_BOUND_FIRST As Integer
    Dim UPPER_BOUND_LAST As Integer
    
    ' length of the name lists
    UPPER_BOUND_FIRST = 197
    UPPER_BOUND_LAST = 100
    
    ' Get the correct sheet to choose from
    If nameType Then
        col = 1 ' First Names
    Else
        col = 2 ' Last Names
    End If
    
    Randomize ' seeds the random # generator with the system time
    nameChooser = Int(upperBound * Rnd + 1) ' randomly choose a name
    
    ' return the name
    RandomName = ThisWorkbook.Sheets("Names").Cells(nameChooser, col).Value
End Function

Public Function RandomAssign(percentAssigned As Double, waysSplit As Integer) As Integer
    ' USE:              to randomly choose a percent of cells in a list
    '                   and randomly assign them to a number of people
    ' percentAssigned:  probabliltiy of being chosen
    ' waysSplit:        number of people
    ' OUTPUT:           0 if not chosen, number of person assigned otherwise
    rand_use = Int(Rnd * (1 / percentAssigned)) + 1 '1 to 10
    rand2_assign = Int(Rnd * waysSplit) + 1 ' 1 to 5
    
    If rand_use = 1 Then
        RandomAssign = rand2_assign
    Else
        RandomAssign = 0
    End If
End Function

Public Function FullName(lastFirst As Boolean, first, last, suffix, Optional mi = "") As String
    ' USE:              return a full name from a broken up one
    '
    ' lastFirst:        if true, return {Last, First Mi. Suffix}. Otherwise {First Mi. Last Suffix}.
    ' first:            first name
    ' last:             last name
    ' suffix:           suffix (III, Jr.)
    ' mi:               middle initial
    ' OUTPUT:           full name, formatted as requested
    
    Dim name As String
    
    'Set nulls to blanks (otherwise will cause errors)
    If IsNull(first) Then first = vbNullString
    If IsNull(last) Then last = vbNullString
    If IsNull(suffix) Then suffix = vbNullString
    If IsNull(mi) Then mi = vbNullString
    
    If lastFirst Then
        'Last Name First
        name = last & ", " & first
        If mi <> vbNullString Then
            name = name & " " & mi & "."
        End If
        If suffix <> vbNullString Then
            name = name & " " & suffix 'don't add dot to suffix (it could be III, etc.)
        End If
    Else
        'First Name First
        name = first
        If mi <> vbNullString Then
            name = name & " " & mi & "."
        End If
        name = name & " " & last
        If suffix <> vbNullString Then
            name = name & " " & suffix
        End If
    End If
    
    'return
    FullName = name
    
End Function

Public Function ColorIf(cell As Range, Optional reference_cell As Range, Optional reference_color As Long)
    ' USE:              Check if a cell is the same color as a reference cell
    ' cell:             cell to check
    ' reference_cell:   cell to check against
    ' OUTPUT:           True if they match
    
    Dim color_num As Long
    
    'get the color from the first cell in the range
    color_num = cell.Cells(1, 1).Interior.Color
    
    'check if the user included a reference cell
    If Not reference_cell Is Nothing Then
        'compare cell color to ref cell color
        If color_num = color_cell.Cells(1, 1).Interior.Color Then
            ColorIf = True
        Else
            ColorIf = False
        End If
    ElseIf reference_color <> 0 Then
        'compare cell color to the ref color
        'Use CellColor with show_index = false to get the color of a cell
        If color_num = reference_color Then
            ColorIf = True
        Else
            ColorIf = False
        End If
    Else
        'if neither, return a value error
        ColorIf = CVErr(xlErrValue)
    End If
End Function

Public Function CellColor(cell As Range, Optional show_index As Boolean = False) As Long
    ' USE:              Display the Excel color of a cell
    ' cell:             cell to check
    ' show_index:       if true return the ColorIndex. Otherwise return the color
    ' OUTPUT:           ColorIndex or Color of cell
    
    If show_index Then
        'color index is the nearest of the 57 base Excel colors
        CellColor = cell.Cells(1, 1).Interior.ColorIndex
    Else
        'color is a long that specifies the exact color in decimal (not hex)
        CellColor = cell.Cells(1, 1).Interior.Color
    End If
End Function

Public Sub PasteSpecialValues()
    ' USE:              to give paste special - values its own shortcut
    ' Shortcut:         Ctrl Shift + V
    ' REQUIRES:         set up a shortcut under Macros
    Dim DataObj As MsForms.DataObject
    Dim paste_text As String, cell_end As Long
    
    On Error GoTo Paste_One
    ' paste special values
    Selection.PasteSpecial Paste:=xlPasteValues, Operation:=xlNone, SkipBlanks _
            :=False, Transpose:=False
    On Error GoTo 0
    
    ' exit if no error
Resume_Exit:
    Exit Sub

    ' Second attempt, on error
    ' try pasting the text from the clipboard into the first selected cell
    
Paste_One_Err:
    ' close the DataObj on error
    Set DataObj = Nothing
    Resume Resume_Exit
    
Paste_One:
    Set DataObj = New MsForms.DataObject
    ' silently fail if this doesn't work either
    On Error GoTo Paste_One_Err
    ' get clipboard data
    DataObj.GetFromClipboard
    
    ' check if clipboard is text
    If DataObj.GetFormat(1) Then
        ' get text from clipboard
        paste_text = DataObj.GetText(1)
        ' get the location of the first line feed (probably end of first cell)
        cell_end = InStr(1, paste_text, vbCrLf, vbTextCompare)
        ' cut off later cells (if any)
        If cell_end > 0 Then
            paste_text = Left(paste_text, cell_end - 1)
        End If
        ' insert text into first selected cell (even if it's merged)
        Selection.Cells(1, 1).Value = paste_text
    End If
    On Error GoTo 0
    
    ' close the DataObj
    Set DataObj = Nothing
End Sub

Public Sub CopyTextOnly()
    ' USE:              to copy only the text, not the cell(s), from a selection
    ' Shortcut:         CTRL-SHIFT-C
    ' REQUIRES:         set up a shortcut under Macros
    ' REQUIRES:         Microsoft Forms 2.0 Library
    
    Dim DataObj As MsForms.DataObject
    Dim selectionStart As Range
    Dim copyText As String
    
    Set DataObj = New MsForms.DataObject
    
    On Error Resume Next ' on error, do nothing
    ' get only the first cell in a selection
    Set selectionStart = Selection.Cells(1, 1)
    ' get the text from the first cell
    copyText = selectionStart.Text
    ' put the text into the clipboard
    DataObj.SetText copyText
    DataObj.PutInClipboard
    On Error GoTo 0

End Sub

Public Sub ConvertToNumber()
    ' USE:              to convert any numbers stored as text
    '                   in a selection to numbers
    ' Shortcut:         CTRL-SHIFT-N
    ' REQUIRES:         set up a shortcut under Macros
    
    Dim c As Range
    Dim v As String
    
    ' loop through all of select
    ' limit to UsedRange, of converting a whole column takes a long time
    For Each c In Intersect(Selection, ActiveSheet.UsedRange)
        v = Replace(c.Value, ",", vbNullString)
        
        'get any numbers
        If IsNumeric(v) And Not IsDate(v) And v <> vbNullString Then
            ' convert to number
            c.Value = val(v)
        End If
    Next c
End Sub
