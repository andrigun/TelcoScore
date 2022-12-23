
Public Class clsCommon

    Public Function FormatDec(ByVal par_Text As String) As String
        Dim textCurr As String
        Dim smallPart As String, textCuted As String

        If IsNumeric(par_Text) = False Then
            textCurr = par_Text
        Else

            textCurr = FormatCurrency(par_Text)
            textCurr = Replace(Replace(CStr(textCurr), "$", ""), "Rp", "")
            If Left(textCurr, 1) = "" Then
                textCurr = Mid(textCurr, 2, Len(textCurr) - 1)

            ElseIf Mid(textCurr, 2, 1) = "$" Then
                textCurr = Replace(textCurr, "$", "")
            End If

            If Right(textCurr, 3) = ".00" Then
                textCurr = Left(textCurr, Len(textCurr) - 3)
            End If

            If Right(textCurr, 6) = ".000000" Then
                textCurr = Left(textCurr, Len(textCurr) - 6)
            End If
        End If

        Return textCurr
    End Function


    Public Function replaceDec(ByVal par_Text As String) As String
        Dim sDec As String
        Dim iDec As Decimal
        Dim aText As Array, sDecPoint As String
        aText = par_Text.Split(".")
        If UBound(aText) = 1 Then
            sDecPoint = aText(1)
        Else
            sDecPoint = "00"
        End If

        ' kalau ini tidak bisa, sepertinya data angka yg dikirim harus text base 
        ' karena kemungkinan oleh return type Decimal akan di convert sesuai dengan setting local
        sDec = Replace(aText(0), ",", "")
        If sDecPoint = "00" Then
            sDec = sDec & ""            ' seharusnya tidak perlu begini, ini unt antisipasi 
        Else
            sDec = sDec & "." & sDecPoint
        End If
        '        iDec = sDec * 1
        Return sDec
    End Function


    Public Function formatDate(ByVal par_Date As Date) As String
        Return Format(par_Date, "MM-dd-yyyy")
    End Function

    Public Function formatDateSql2(ByVal par_Date As String) As String
        Return Format(par_Date, "MM/dd/yyyy")
    End Function

    Public Function formatDateInd(ByVal par_Date As Date) As String
        Dim str As String
        str = Format(par_Date, "dd-MM-yyyy")
        If str = "01-01-1900" Then str = "-"
        Return str
    End Function
    Public Function formatDateSQl(ByVal par_Date As Date) As String
        Return Format(par_Date, "yyyy-MM-dd")
    End Function

    Public Function formatDtmSQl(ByVal par_Date As String) As String
        'dd-MM-yyyy
        Dim syear, smonth, sdate As String
        syear = Right(par_Date, 4) & "-"
        smonth = syear & Mid(par_Date, 4, 2) & "-"
        sdate = smonth & Left(par_Date, 2)

        'par_Date = sdate

        Return sdate
    End Function

    Public Function validQuote(ByVal par_Text As String) As String
        Return Replace(par_Text, "'", "''")
    End Function
    Public Function randomizeNumber(ByVal rNumber As String) As String
        Dim i As Integer
        Randomize()
        i = Int((100 * Rnd()) + 1)
        Return rNumber = i

    End Function

    Function Random(ByVal Lowerbound As Long, ByVal Upperbound As Long)
        Randomize()
        Random = Int(Rnd() * Upperbound) + Lowerbound

    End Function
    Public Function DesKutip(ByVal par_Text As String) As String
        Return Replace(par_Text, "'", "")
    End Function
End Class


